using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.Instruments.Client;
using Lanymy.Common.Instruments.Common;
using Lanymy.Common.Instruments.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class SocketNettyTests
    {
        private sealed class TestNettySession : BaseChannelSession
        {
            public override byte[] CacheHeartBytes => Array.Empty<byte>();
        }

        private sealed class TestNettyFilter : BaseChannelFixedHeaderPackageFilter<object, object, TestNettySession>
        {
            public override bool CheckPackage(ReadOnlySpan<byte> packageBytes)
            {
                return true;
            }

            public override byte[] EncodePackage(object sendPackage)
            {
                return Array.Empty<byte>();
            }

            public override object DecodePackage(ReadOnlySpan<byte> packageBytes)
            {
                return null;
            }
        }

        private sealed class TestNettyClientContext : BaseClientChannelContext<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions>
        {
            public TestNettyClientContext(ClientChannelOptions channelOptions)
                : base(channelOptions)
            {
            }
        }

        private sealed class TestNettyClientHandler : BaseClientChannelHandler<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext>
        {
            public TestNettyClientHandler(TestNettyClientContext channelContext)
                : base(channelContext)
            {
            }

            public void TriggerChannelInactiveForTest()
            {
                base.OnChannelInactive(null);
            }

            protected override void OnChannelActive(IChannelHandlerContext context)
            {
            }

            protected override void OnChannelReadBytes(IChannelHandlerContext context, ReadOnlySpan<byte> packageDataBytes)
            {
            }
        }

        private sealed class TestNettyClientInitializer : BaseClientChannelInitializer<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext, TestNettyClientHandler>
        {
            public TestNettyClientInitializer(TestNettyClientContext serverChannelContext)
                : base(serverChannelContext)
            {
            }
        }

        private sealed class TestNettyServerContext : BaseServerChannelContext<object, object, TestNettySession, TestNettyFilter, ServerChannelOptions>
        {
            public TestNettyServerContext(ServerChannelOptions channelOptions)
                : base(channelOptions)
            {
            }
        }

        private sealed class TestNettyServerHandler : BaseServerChannelHandler<object, object, TestNettySession, TestNettyFilter, ServerChannelOptions, TestNettyServerContext>
        {
            public TestNettyServerHandler(TestNettyServerContext channelContext)
                : base(channelContext)
            {
            }

            public void SetCurrentChannelSessionForTest(TestNettySession channelSession)
            {
                _CurrentChannelSession = channelSession;
            }

            public void TriggerServerChannelActiveForTest()
            {
                base.OnChannelActive(null);
            }

            public void TriggerServerChannelInactiveForTest()
            {
                base.OnChannelInactive(null);
            }

            protected override void OnChannelReadBytes(IChannelHandlerContext context, ReadOnlySpan<byte> packageDataBytes)
            {
            }
        }

        private sealed class TestNettyServerInitializer : BaseServerChannelInitializer<object, object, TestNettySession, TestNettyFilter, ServerChannelOptions, TestNettyServerContext, TestNettyServerHandler>
        {
            public TestNettyServerInitializer(TestNettyServerContext serverChannelContext)
                : base(serverChannelContext)
            {
            }
        }

        private sealed class TestFaultInjectingChannelInitializer : BaseChannelInitializer<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext, TestNettyClientHandler>
        {
            public Exception GetChannelHandlerException { get; set; }

            public bool ReturnNullChannelHandler { get; set; }

            public Exception AddTerminalChannelHandlersException { get; set; }

            public Exception CloseChannelAfterInitFailureException { get; set; }

            public bool ReturnFaultedCloseChannelAfterInitFailureTask { get; set; }

            public int CloseChannelAfterInitFailureCallCount { get; private set; }

            public List<Exception> InitChannelErrors { get; } = new List<Exception>();

            public TestFaultInjectingChannelInitializer(TestNettyClientContext channelContext)
                : base(channelContext)
            {
            }

            public void TriggerInitChannelForTest(ISocketChannel channel)
            {
                base.InitChannel(channel);
            }

            protected override TestNettyClientHandler GetChannelHandler()
            {
                if (GetChannelHandlerException != null)
                {
                    throw GetChannelHandlerException;
                }

                if (ReturnNullChannelHandler)
                {
                    return null;
                }

                if (_CurrentServerChannelContext.CurrentConnectToServerAction == null)
                {
                    _CurrentServerChannelContext.CurrentConnectToServerAction = _ => { };
                }

                return new TestNettyClientHandler(_CurrentServerChannelContext);
            }

            protected override void AddTerminalChannelHandlers(IChannelPipeline pipeline, TestNettyClientHandler channelHandler)
            {
                if (AddTerminalChannelHandlersException != null)
                {
                    throw AddTerminalChannelHandlersException;
                }

                base.AddTerminalChannelHandlers(pipeline, channelHandler);
            }

            protected override Task ExecuteCloseChannelAfterInitFailureAsync(ISocketChannel channel)
            {
                CloseChannelAfterInitFailureCallCount++;

                if (CloseChannelAfterInitFailureException != null)
                {
                    if (ReturnFaultedCloseChannelAfterInitFailureTask)
                    {
                        return Task.FromException(CloseChannelAfterInitFailureException);
                    }

                    throw CloseChannelAfterInitFailureException;
                }

                return Task.CompletedTask;
            }

            protected override void OnInitChannelError(Exception exception)
            {
                InitChannelErrors.Add(exception);
            }
        }

        private sealed class TestSocketHost : BaseSocketHost<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext, TestNettyClientHandler, TestNettyClientInitializer>
        {
            private readonly TaskCompletionSource<bool> _startEntered = CreateSignal();
            private readonly TaskCompletionSource<bool> _stopEntered = CreateSignal();
            private readonly TaskCompletionSource<bool> _startGate = CreateSignal();
            private readonly TaskCompletionSource<bool> _stopGate = CreateSignal();

            public bool BlockStartForTest { get; set; }

            public bool BlockStopForTest { get; set; }

            public Exception StartException { get; set; }

            public Exception StopException { get; set; }

            public Exception DisposeException { get; set; }

            public int StartCallCount { get; private set; }

            public int StopCallCount { get; private set; }

            public int DisposeCallCount { get; private set; }

            public Task StartEnteredTask => _startEntered.Task;

            public Task StopEnteredTask => _stopEntered.Task;

            public TestSocketHost(TestNettyClientContext channelContext)
                : base(channelContext)
            {
            }

            public void ReleaseStartForTest()
            {
                _startGate.TrySetResult(true);
            }

            public void ReleaseStopForTest()
            {
                _stopGate.TrySetResult(true);
            }

            public void SetIsRunningForTest(bool isRunning)
            {
                IsRunning = isRunning;
            }

            protected override async Task OnStartAsync()
            {
                StartCallCount++;
                _startEntered.TrySetResult(true);

                if (StartException != null)
                {
                    throw StartException;
                }

                if (BlockStartForTest)
                {
                    await _startGate.Task;
                }
            }

            protected override async Task OnStopAsync()
            {
                StopCallCount++;
                _stopEntered.TrySetResult(true);

                if (StopException != null)
                {
                    throw StopException;
                }

                if (BlockStopForTest)
                {
                    await _stopGate.Task;
                }
            }

            protected override Task OnDisposeAsync()
            {
                DisposeCallCount++;

                if (DisposeException != null)
                {
                    throw DisposeException;
                }

                return Task.CompletedTask;
            }

            private static TaskCompletionSource<bool> CreateSignal()
            {
                return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }

        private sealed class TestBaseChannelHandler : BaseChannelHandler<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext>
        {
            public Exception ChannelActiveException { get; set; }

            public Exception ChannelReadBytesException { get; set; }

            public Exception ExceptionCallbackException { get; set; }

            public Exception UserEventTriggeredException { get; set; }

            public Exception ChannelReadCompleteException { get; set; }

            public Exception ScheduleSendBytesException { get; set; }

            public bool ReturnFaultedScheduleSendTaskForTest { get; set; }

            public Exception WriteBytesException { get; set; }

            public Exception ScheduleCloseContextException { get; set; }

            public bool ReturnFaultedScheduleCloseTaskForTest { get; set; }

            public bool AllowNullContextCloseSchedulingForTest { get; set; }

            public Exception CloseContextException { get; set; }

            public bool ReturnFaultedCloseContextTaskForTest { get; set; }

            public bool AllowNullContextCloseForTest { get; set; }

            public List<Exception> HandlerErrors { get; } = new List<Exception>();
            public int ScheduleCloseContextCallCount { get; private set; }
            public int ExecuteCloseContextCallCount { get; private set; }

            public TestBaseChannelHandler(TestNettyClientContext channelContext)
                : base(channelContext)
            {
            }

            public void TriggerSafeChannelReadForTest(object message)
            {
                SafeHandleChannelRead(null, message);
            }

            public void TriggerSafeExceptionForTest(Exception exception)
            {
                SafeHandleException(null, exception);
            }

            public void TriggerSafeUserEventForTest(object evt)
            {
                SafeHandleUserEventTriggered(null, evt);
            }

            public void TriggerChannelReadCompleteForTest()
            {
                SafeHandleChannelReadComplete(null);
            }

            public void TriggerSendBytesForTest(byte[] bytes)
            {
                SendBytes(null, bytes);
            }

            public void PrepareActiveStateForTest(IPEndPoint remoteIpEndPoint, bool isLogin)
            {
                _CurrentChannelHandlerContext = null;
                _CurrentChannelSession.RemoteIpEndPoint = remoteIpEndPoint;
                _CurrentChannelSession.IsLogin = isLogin;
            }

            public void TriggerSafeChannelActiveForTest()
            {
                SafeHandleChannelActive(null);
            }

            public void TriggerDelayedContextCloseForTest(int delayMilliseconds = 1)
            {
                OnContextClose(null, delayMilliseconds);
            }

            public void TriggerContextCloseForTest()
            {
                OnContextClose(null);
            }

            public void TriggerChannelInactiveForTest()
            {
                ResetCurrentChannelState();
                SafeHandleChannelInactive(null);
            }

            public void TriggerResetCurrentChannelStateForTest()
            {
                ResetCurrentChannelState();
            }

            public bool IsCurrentContextAssignedForTest => _CurrentChannelHandlerContext != null;

            public bool IsCurrentSessionLoginForTest => _CurrentChannelSession.IsLogin;

            public IPEndPoint CurrentRemoteIpEndPointForTest => _CurrentChannelSession.RemoteIpEndPoint;
            public int CurrentCloseRequestStateForTest => _CurrentCloseRequestState;

            public int WriterIdleTriggeredCount { get; private set; }

            public int ReaderIdleTriggeredCount { get; private set; }

            protected override void OnChannelActive(IChannelHandlerContext context)
            {
                if (ChannelActiveException != null)
                {
                    throw ChannelActiveException;
                }
            }

            protected override void OnChannelInactive(IChannelHandlerContext context)
            {
            }

            protected override void OnChannelReadBytes(IChannelHandlerContext context, ReadOnlySpan<byte> packageDataBytes)
            {
                if (ChannelReadBytesException != null)
                {
                    throw ChannelReadBytesException;
                }
            }

            protected override void OnUserEventTriggered(IChannelHandlerContext context, object evt)
            {
                if (UserEventTriggeredException != null)
                {
                    throw UserEventTriggeredException;
                }

                base.OnUserEventTriggered(context, evt);
            }

            protected override void OnWriterTimeOutEventTrigger(IChannelHandlerContext context)
            {
                WriterIdleTriggeredCount++;
            }

            protected override void OnReaderTimeOutEventTrigger(IChannelHandlerContext context)
            {
                ReaderIdleTriggeredCount++;
                base.OnReaderTimeOutEventTrigger(context);
            }

            protected override void OnException(IChannelHandlerContext context, Exception exception)
            {
                if (ExceptionCallbackException != null)
                {
                    throw ExceptionCallbackException;
                }

                base.OnException(context, exception);
            }

            protected override void ExecuteChannelReadComplete(IChannelHandlerContext context)
            {
                if (ChannelReadCompleteException != null)
                {
                    throw ChannelReadCompleteException;
                }
            }

            protected override void OnHandlerError(Exception exception)
            {
                HandlerErrors.Add(exception);
            }

            protected override bool CanScheduleSend(IChannelHandlerContext context, byte[] bytes)
            {
                return bytes != null && bytes.Length > 0;
            }

            protected override Task ScheduleSendBytesAsync(IChannelHandlerContext context, byte[] bytes)
            {
                if (ScheduleSendBytesException != null)
                {
                    if (ReturnFaultedScheduleSendTaskForTest)
                    {
                        return Task.FromException(ScheduleSendBytesException);
                    }

                    throw ScheduleSendBytesException;
                }

                return WriteBytesAsync(context, bytes);
            }

            protected override bool CanScheduleContextClose(IChannelHandlerContext context)
            {
                return AllowNullContextCloseSchedulingForTest || base.CanScheduleContextClose(context);
            }

            protected override bool CanCloseContext(IChannelHandlerContext context)
            {
                return AllowNullContextCloseForTest || base.CanCloseContext(context);
            }

            protected override Task ScheduleCloseContextAsync(IChannelHandlerContext context, int delayMilliseconds)
            {
                ScheduleCloseContextCallCount++;

                if (ScheduleCloseContextException != null)
                {
                    if (ReturnFaultedScheduleCloseTaskForTest)
                    {
                        return Task.FromException(ScheduleCloseContextException);
                    }

                    throw ScheduleCloseContextException;
                }

                return Task.CompletedTask;
            }

            protected override Task ExecuteCloseContextAsync(IChannelHandlerContext context)
            {
                ExecuteCloseContextCallCount++;

                if (CloseContextException != null)
                {
                    if (ReturnFaultedCloseContextTaskForTest)
                    {
                        return Task.FromException(CloseContextException);
                    }

                    throw CloseContextException;
                }

                return Task.CompletedTask;
            }

            protected override Task ExecuteWriteAndFlushAsync(IChannelHandlerContext context, byte[] bytes)
            {
                if (WriteBytesException != null)
                {
                    return Task.FromException(WriteBytesException);
                }

                return Task.CompletedTask;
            }
        }

        private sealed class TestThrowingReconnectClientChannelHandler : BaseClientChannelHandler<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext>
        {
            public List<Exception> HandlerErrors { get; } = new List<Exception>();

            public TestThrowingReconnectClientChannelHandler(TestNettyClientContext channelContext)
                : base(channelContext)
            {
            }

            public void TriggerSafeChannelInactiveForTest()
            {
                SafeHandleChannelInactive(null);
            }

            protected override void OnChannelActive(IChannelHandlerContext context)
            {
            }

            protected override void OnChannelReadBytes(IChannelHandlerContext context, ReadOnlySpan<byte> packageDataBytes)
            {
            }

            protected override void OnHandlerError(Exception exception)
            {
                HandlerErrors.Add(exception);
            }
        }

        private sealed class TestNettyClient : BaseNettySocketClient<object, object, TestNettySession, TestNettyFilter, ClientChannelOptions, TestNettyClientContext, TestNettyClientHandler, TestNettyClientInitializer>
        {
            private TaskCompletionSource<bool> _connectGate = CreateConnectGate();
            private TaskCompletionSource<bool> _awaitReconnectEntered = CreateConnectGate();
            private TaskCompletionSource<bool> _awaitReconnectGate = CreateConnectGate();

            public bool ForceHasActiveBoundChannel { get; set; }
            public bool BlockAwaitReconnectTaskForTest { get; set; }

            public Exception CloseChannelException { get; set; }

            public Exception ShutdownBossGroupException { get; set; }

            public Exception AwaitReconnectTaskException { get; set; }

            public Exception DisposeReconnectCancellationTokenSourceException { get; set; }

            public Exception CreateBossGroupException { get; set; }

            public Exception CreateBootstrapException { get; set; }

            public Exception CreateReconnectCancellationTokenSourceException { get; set; }

            public Exception EnsureReconnectLoopStartedException { get; set; }

            public Exception ConnectChannelException { get; set; }

            public bool CancelReconnectTokenBeforeThrowingConnectChannelException { get; set; }

            public bool StopAfterDelayForTest { get; set; }

            public int ConnectAttemptCount { get; private set; }

            public int CloseChannelCallCount { get; private set; }

            public int ShutdownBossGroupCallCount { get; private set; }

            public int AwaitReconnectTaskCallCount { get; private set; }

            public int DisposeReconnectCancellationTokenSourceCallCount { get; private set; }

            public int CreateBossGroupCallCount { get; private set; }

            public int CreateBootstrapCallCount { get; private set; }

            public int CreateReconnectCancellationTokenSourceCallCount { get; private set; }

            public int EnsureReconnectLoopStartedCallCount { get; private set; }

            public int ConnectChannelCallCount { get; private set; }

            public int DelayBeforeReconnectCallCount { get; private set; }

            public List<long> ConnectAttemptGenerations { get; } = new List<long>();

            public List<Exception> StopErrors { get; } = new List<Exception>();

            public List<Exception> StartErrors { get; } = new List<Exception>();

            public List<Exception> ConnectErrors { get; } = new List<Exception>();

            public TestNettyClient(TestNettyClientContext serverChannelContext)
                : base(serverChannelContext)
            {
            }

            public TestNettyClientContext CurrentContext => _CurrentChannelContext;
            public Task AwaitReconnectTaskEnteredTask => _awaitReconnectEntered.Task;

            public Task CurrentReconnectTaskForTest => _CurrentReconnectTask;

            public IChannel CurrentChannelHostForTest => _CurrentChannelHost;

            public IEventLoopGroup CurrentBossGroupForTest => _CurrentBossGroup;

            public CancellationTokenSource CurrentReconnectCancellationTokenSourceForTest => _CurrentReconnectCancellationTokenSource;

            public Bootstrap CurrentBootstrapForTest => _CurrentBootstrap;

            public void PrepareReconnectStateForTest(long reconnectGeneration, bool isRunning = true)
            {
                IsRunning = isRunning;
                _CurrentReconnectGeneration = reconnectGeneration;
                _CurrentChannelContext.CurrentReconnectGeneration = reconnectGeneration;
                _CurrentChannelContext.CurrentConnectToServerAction = EnsureReconnectLoopStarted;
                _CurrentReconnectCancellationTokenSource = new CancellationTokenSource();
                _CurrentReconnectTask = null;
                ConnectAttemptCount = 0;
                ConnectAttemptGenerations.Clear();
                ResetConnectGateForTest();
            }

            public void SetCurrentBootstrapForTest(Bootstrap currentBootstrap)
            {
                _CurrentBootstrap = currentBootstrap;
            }

            public void PrepareStopStateForTest(IChannel currentChannelHost = null, IEventLoopGroup currentBossGroup = null, Task currentReconnectTask = null, CancellationTokenSource currentReconnectCancellationTokenSource = null, Bootstrap currentBootstrap = null)
            {
                _CurrentChannelHost = currentChannelHost;
                _CurrentBossGroup = currentBossGroup;
                _CurrentReconnectTask = currentReconnectTask;
                _CurrentReconnectCancellationTokenSource = currentReconnectCancellationTokenSource;
                _CurrentBootstrap = currentBootstrap;
            }

            public void SetReconnectGenerationForTest(long reconnectGeneration)
            {
                _CurrentReconnectGeneration = reconnectGeneration;
                _CurrentChannelContext.CurrentReconnectGeneration = reconnectGeneration;
            }

            public void SetIsRunningForTest(bool isRunning)
            {
                IsRunning = isRunning;
            }

            public Task StopCoreAsyncForTest()
            {
                return base.OnStopAsync();
            }

            public Task RunBaseConnectToServerAsyncForTest(long reconnectGeneration)
            {
                return base.ConnectToServerAsync(reconnectGeneration);
            }

            public void TriggerReconnectForTest(long reconnectGeneration)
            {
                EnsureReconnectLoopStarted(reconnectGeneration);
            }

            public bool TryBindConnectedChannelForTest(long reconnectGeneration, CancellationToken cancellationToken, IChannel channel)
            {
                return TryBindConnectedChannel(reconnectGeneration, cancellationToken, channel);
            }

            public void ReleaseReconnectLoopForTest()
            {
                _connectGate.TrySetResult(true);
            }

            public void ResetConnectGateForTest()
            {
                _connectGate = CreateConnectGate();
            }

            public void ReleaseAwaitReconnectTaskForTest()
            {
                _awaitReconnectGate.TrySetResult(true);
            }

            public void ResetAwaitReconnectTaskForTest()
            {
                _awaitReconnectEntered = CreateConnectGate();
                _awaitReconnectGate = CreateConnectGate();
            }

            protected override async Task ConnectToServerAsync(long reconnectGeneration)
            {
                ConnectAttemptCount++;
                ConnectAttemptGenerations.Add(reconnectGeneration);
                await _connectGate.Task;
            }

            protected override Task<IChannel> ConnectChannelAsync()
            {
                ConnectChannelCallCount++;
                if (ConnectChannelException != null)
                {
                    if (CancelReconnectTokenBeforeThrowingConnectChannelException)
                    {
                        _CurrentReconnectCancellationTokenSource?.Cancel();
                    }

                    return Task.FromException<IChannel>(ConnectChannelException);
                }

                return Task.FromResult<IChannel>(null);
            }

            protected override IEventLoopGroup CreateBossGroup()
            {
                CreateBossGroupCallCount++;
                if (CreateBossGroupException != null)
                {
                    throw CreateBossGroupException;
                }

                return base.CreateBossGroup();
            }

            protected override Bootstrap CreateBootstrap(IEventLoopGroup currentBossGroup)
            {
                CreateBootstrapCallCount++;
                if (CreateBootstrapException != null)
                {
                    throw CreateBootstrapException;
                }

                return new Bootstrap();
            }

            protected override CancellationTokenSource CreateReconnectCancellationTokenSource()
            {
                CreateReconnectCancellationTokenSourceCallCount++;
                if (CreateReconnectCancellationTokenSourceException != null)
                {
                    throw CreateReconnectCancellationTokenSourceException;
                }

                return base.CreateReconnectCancellationTokenSource();
            }

            protected override void EnsureReconnectLoopStarted(long reconnectGeneration)
            {
                EnsureReconnectLoopStartedCallCount++;
                if (EnsureReconnectLoopStartedException != null)
                {
                    throw EnsureReconnectLoopStartedException;
                }

                base.EnsureReconnectLoopStarted(reconnectGeneration);
            }

            protected override async Task CloseChannelAsync(IChannel channel)
            {
                CloseChannelCallCount++;
                if (CloseChannelException != null)
                {
                    throw CloseChannelException;
                }

                await base.CloseChannelAsync(channel);
            }

            protected override Task ShutdownBossGroupAsync(IEventLoopGroup bossGroup)
            {
                ShutdownBossGroupCallCount++;
                if (ShutdownBossGroupException != null)
                {
                    throw ShutdownBossGroupException;
                }

                return Task.CompletedTask;
            }

            protected override Task AwaitReconnectTaskAsync(Task reconnectTask)
            {
                AwaitReconnectTaskCallCount++;
                if (AwaitReconnectTaskException != null)
                {
                    return Task.FromException(AwaitReconnectTaskException);
                }

                if (BlockAwaitReconnectTaskForTest)
                {
                    _awaitReconnectEntered.TrySetResult(true);
                    return _awaitReconnectGate.Task;
                }

                return Task.CompletedTask;
            }

            protected override void DisposeReconnectCancellationTokenSource(CancellationTokenSource reconnectCancellationTokenSource)
            {
                DisposeReconnectCancellationTokenSourceCallCount++;
                if (DisposeReconnectCancellationTokenSourceException != null)
                {
                    throw DisposeReconnectCancellationTokenSourceException;
                }

                base.DisposeReconnectCancellationTokenSource(reconnectCancellationTokenSource);
            }

            protected override bool HasActiveBoundChannel()
            {
                return ForceHasActiveBoundChannel || base.HasActiveBoundChannel();
            }

            protected override void OnStopError(Exception ex)
            {
                StopErrors.Add(ex);
            }

            protected override void OnStartError(Exception ex)
            {
                StartErrors.Add(ex);
            }

            protected override void OnConnectError(Exception ex)
            {
                ConnectErrors.Add(ex);
            }

            protected override Task DelayBeforeReconnectAsync(CancellationToken cancellationToken)
            {
                DelayBeforeReconnectCallCount++;
                if (StopAfterDelayForTest)
                {
                    SetIsRunningForTest(false);
                }

                return Task.CompletedTask;
            }

            protected override Task OnStartAsync()
            {
                return base.OnStartAsync();
            }

            protected override Task OnDisposeAsync()
            {
                return Task.CompletedTask;
            }

            private static TaskCompletionSource<bool> CreateConnectGate()
            {
                return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }

        private sealed class TestNettyServer : BaseNettySocketServer<object, object, TestNettySession, TestNettyFilter, ServerChannelOptions, TestNettyServerContext, TestNettyServerHandler, TestNettyServerInitializer>
        {
            public Exception BindServerException { get; set; }

            public Exception CloseChannelException { get; set; }

            public Exception ShutdownBossGroupException { get; set; }

            public Exception ShutdownWorkerGroupException { get; set; }

            public Exception CreateBossGroupException { get; set; }

            public Exception CreateWorkerGroupException { get; set; }

            public Exception CreateBootstrapException { get; set; }

            public Exception CleanupTrackedChannelHandlersException { get; set; }

            public int BindServerCallCount { get; private set; }

            public int CloseChannelCallCount { get; private set; }

            public int ShutdownBossGroupCallCount { get; private set; }

            public int ShutdownWorkerGroupCallCount { get; private set; }

            public int CreateBossGroupCallCount { get; private set; }

            public int CreateWorkerGroupCallCount { get; private set; }

            public int CreateBootstrapCallCount { get; private set; }

            public List<Exception> StartErrors { get; } = new List<Exception>();

            public List<Exception> StopErrors { get; } = new List<Exception>();

            public List<IEventLoopGroup> CreatedBossGroups { get; } = new List<IEventLoopGroup>();

            public List<IEventLoopGroup> CreatedWorkerGroups { get; } = new List<IEventLoopGroup>();

            public TestNettyServer(TestNettyServerContext serverChannelContext)
                : base(serverChannelContext)
            {
            }

            public IChannel CurrentChannelHostForTest => _CurrentChannelHost;

            public IEventLoopGroup CurrentBossGroupForTest => _CurrentBossGroup;

            public IEventLoopGroup CurrentWorkerGroupForTest => _CurrentWorkerGroup;

            public ServerBootstrap CurrentBootstrapForTest => _CurrentBootstrap;

            public TestNettyServerContext CurrentContext => _CurrentChannelContext;

            public void PrepareStopStateForTest(IChannel currentChannelHost = null, IEventLoopGroup currentBossGroup = null, IEventLoopGroup currentWorkerGroup = null, ServerBootstrap currentBootstrap = null)
            {
                _CurrentChannelHost = currentChannelHost;
                _CurrentBossGroup = currentBossGroup;
                _CurrentWorkerGroup = currentWorkerGroup;
                _CurrentBootstrap = currentBootstrap;
                IsRunning = true;
            }

            protected override IEventLoopGroup CreateBossGroup()
            {
                CreateBossGroupCallCount++;
                if (CreateBossGroupException != null)
                {
                    throw CreateBossGroupException;
                }

                var currentBossGroup = base.CreateBossGroup();
                CreatedBossGroups.Add(currentBossGroup);
                return currentBossGroup;
            }

            protected override IEventLoopGroup CreateWorkerGroup()
            {
                CreateWorkerGroupCallCount++;
                if (CreateWorkerGroupException != null)
                {
                    throw CreateWorkerGroupException;
                }

                var currentWorkerGroup = base.CreateWorkerGroup();
                CreatedWorkerGroups.Add(currentWorkerGroup);
                return currentWorkerGroup;
            }

            protected override ServerBootstrap CreateBootstrap(IEventLoopGroup currentBossGroup, IEventLoopGroup currentWorkerGroup)
            {
                CreateBootstrapCallCount++;
                if (CreateBootstrapException != null)
                {
                    throw CreateBootstrapException;
                }

                return new ServerBootstrap();
            }

            protected override Task<IChannel> BindServerAsync(ServerBootstrap bootstrap)
            {
                BindServerCallCount++;
                if (BindServerException != null)
                {
                    return Task.FromException<IChannel>(BindServerException);
                }

                return Task.FromResult<IChannel>(null);
            }

            protected override Task CloseChannelAsync(IChannel currentChannelHost)
            {
                CloseChannelCallCount++;
                if (CloseChannelException != null)
                {
                    return Task.FromException(CloseChannelException);
                }

                return base.CloseChannelAsync(currentChannelHost);
            }

            protected override Task ShutdownGroupAsync(IEventLoopGroup currentEventLoopGroup)
            {
                if (ReferenceEquals(currentEventLoopGroup, _CurrentBossGroup))
                {
                    ShutdownBossGroupCallCount++;
                    if (ShutdownBossGroupException != null)
                    {
                        return Task.FromException(ShutdownBossGroupException);
                    }
                }

                if (ReferenceEquals(currentEventLoopGroup, _CurrentWorkerGroup))
                {
                    ShutdownWorkerGroupCallCount++;
                    if (ShutdownWorkerGroupException != null)
                    {
                        return Task.FromException(ShutdownWorkerGroupException);
                    }
                }

                return base.ShutdownGroupAsync(currentEventLoopGroup);
            }

            protected override void OnStartError(Exception ex)
            {
                StartErrors.Add(ex);
            }

            protected override void OnStopError(Exception ex)
            {
                StopErrors.Add(ex);
            }

            protected override void CleanupTrackedChannelHandlers()
            {
                if (CleanupTrackedChannelHandlersException != null)
                {
                    throw CleanupTrackedChannelHandlersException;
                }

                base.CleanupTrackedChannelHandlers();
            }

            protected override Task OnDisposeAsync()
            {
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public void BaseClientChannelHandler_OnChannelInactive_ShouldUseCapturedReconnectGeneration()
        {
            long? requestedGeneration = null;
            var context = CreateClientContext();
            context.CurrentReconnectGeneration = 1;
            context.CurrentConnectToServerAction = generation => requestedGeneration = generation;

            var handler = new TestNettyClientHandler(context);
            context.CurrentReconnectGeneration = 2;

            handler.TriggerChannelInactiveForTest();

            Assert.AreEqual(1L, requestedGeneration);
        }

        [TestMethod]
        public void BaseClientChannelHandler_OnChannelInactive_WhenReconnectActionOnlyStoredOnContext_ShouldStillInvokeCallbackAfterGc()
        {
            long? requestedGeneration = null;
            var context = CreateClientContext();
            context.CurrentReconnectGeneration = 9;

            AssignReconnectActionForGcTest(context, generation => requestedGeneration = generation);
            ForceFullGcForTest();

            var handler = new TestNettyClientHandler(context);
            handler.TriggerChannelInactiveForTest();

            Assert.AreEqual(9L, requestedGeneration);
        }

        [TestMethod]
        public void BaseChannelHandler_ChannelRead_WhenReadCallbackThrows_ShouldReportErrorAndNotThrow()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ChannelReadBytesException = new InvalidOperationException("read failed"),
            };

            handler.TriggerSafeChannelReadForTest(Unpooled.WrappedBuffer(new byte[] { 0x01 }));

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler read callback failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_ChannelActive_WhenActiveCallbackThrows_ShouldResetStateAndReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ChannelActiveException = new InvalidOperationException("active failed"),
            };
            handler.PrepareActiveStateForTest(new IPEndPoint(IPAddress.Loopback, 9527), true);

            handler.TriggerSafeChannelActiveForTest();

            Assert.IsFalse(handler.IsCurrentContextAssignedForTest);
            Assert.IsFalse(handler.IsCurrentSessionLoginForTest);
            Assert.IsNull(handler.CurrentRemoteIpEndPointForTest);
            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler channel active callback failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseClientChannelHandler_ChannelInactive_WhenReconnectCallbackThrows_ShouldReportErrorAndNotThrow()
        {
            var context = CreateClientContext();
            context.CurrentReconnectGeneration = 1;
            context.CurrentConnectToServerAction = _ => throw new InvalidOperationException("reconnect callback failed");
            var handler = new TestThrowingReconnectClientChannelHandler(context);

            handler.TriggerSafeChannelInactiveForTest();

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler channel inactive callback failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_ExceptionCaught_WhenExceptionCallbackThrowsObjectDisposed_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ExceptionCallbackException = new ObjectDisposedException("context disposed"),
            };

            handler.TriggerSafeExceptionForTest(new InvalidOperationException("transport failed"));

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_ExceptionCaught_WhenExceptionCallbackThrowsUnexpectedException_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ExceptionCallbackException = new InvalidOperationException("exception callback failed"),
            };

            handler.TriggerSafeExceptionForTest(new InvalidOperationException("transport failed"));

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler exception callback failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_UserEventTriggered_WhenCallbackThrowsUnexpectedException_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                UserEventTriggeredException = new InvalidOperationException("user event failed"),
            };

            handler.TriggerSafeUserEventForTest(new object());

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler user event callback failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_UserEventTriggered_WhenWriterIdleEventOccurs_ShouldDispatchToWriterTimeout()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext());

            handler.TriggerSafeUserEventForTest(IdleStateEvent.FirstWriterIdleStateEvent);

            Assert.AreEqual(1, handler.WriterIdleTriggeredCount);
            Assert.AreEqual(0, handler.ReaderIdleTriggeredCount);
            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_UserEventTriggered_WhenReaderIdleEventOccurs_ShouldDispatchToReaderTimeout()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext());

            handler.TriggerSafeUserEventForTest(IdleStateEvent.FirstReaderIdleStateEvent);

            Assert.AreEqual(0, handler.WriterIdleTriggeredCount);
            Assert.AreEqual(1, handler.ReaderIdleTriggeredCount);
            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_ChannelReadComplete_WhenFlushThrowsUnexpectedException_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ChannelReadCompleteException = new InvalidOperationException("flush failed"),
            };

            handler.TriggerChannelReadCompleteForTest();

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler flush read complete failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_ChannelReadComplete_WhenFlushThrowsObjectDisposed_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ChannelReadCompleteException = new ObjectDisposedException("channel disposed"),
            };

            handler.TriggerChannelReadCompleteForTest();

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_SendBytes_WhenScheduleThrows_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ScheduleSendBytesException = new InvalidOperationException("schedule send failed"),
            };

            handler.TriggerSendBytesForTest(new byte[] { 0x01 });

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler schedule send failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_SendBytes_WhenWriteThrows_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                WriteBytesException = new InvalidOperationException("write send failed"),
            };

            handler.TriggerSendBytesForTest(new byte[] { 0x01 });

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler write send failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_SendBytes_WhenWriteThrowsObjectDisposed_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                WriteBytesException = new ObjectDisposedException("context disposed"),
            };

            handler.TriggerSendBytesForTest(new byte[] { 0x01 });

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_SendBytes_WhenScheduleReturnsCanceledTask_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ScheduleSendBytesException = new OperationCanceledException("schedule send canceled"),
                ReturnFaultedScheduleSendTaskForTest = true,
            };

            handler.TriggerSendBytesForTest(new byte[] { 0x01 });

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_SendBytes_WhenScheduleReturnsFaultedTask_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                ScheduleSendBytesException = new InvalidOperationException("schedule send failed asynchronously"),
                ReturnFaultedScheduleSendTaskForTest = true,
            };

            handler.TriggerSendBytesForTest(new byte[] { 0x01 });

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler schedule send failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenScheduleReturnsFaultedTask_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseSchedulingForTest = true,
                ScheduleCloseContextException = new InvalidOperationException("schedule close failed asynchronously"),
                ReturnFaultedScheduleCloseTaskForTest = true,
            };

            handler.TriggerDelayedContextCloseForTest();

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler schedule close context failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenDelayedCloseRequestedRepeatedly_ShouldScheduleOnlyOnce()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseSchedulingForTest = true,
            };

            handler.TriggerDelayedContextCloseForTest();
            handler.TriggerDelayedContextCloseForTest();

            Assert.AreEqual(1, handler.ScheduleCloseContextCallCount);
            Assert.AreEqual(1, handler.CurrentCloseRequestStateForTest);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenScheduleReturnsFaultedObjectDisposedTask_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseSchedulingForTest = true,
                ScheduleCloseContextException = new ObjectDisposedException("context disposed"),
                ReturnFaultedScheduleCloseTaskForTest = true,
            };

            handler.TriggerDelayedContextCloseForTest();

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenScheduleReturnsCanceledTask_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseSchedulingForTest = true,
                ScheduleCloseContextException = new OperationCanceledException("schedule close canceled"),
                ReturnFaultedScheduleCloseTaskForTest = true,
            };

            handler.TriggerDelayedContextCloseForTest();

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenCloseReturnsFaultedTask_ShouldReportError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
                CloseContextException = new InvalidOperationException("close failed asynchronously"),
                ReturnFaultedCloseContextTaskForTest = true,
            };

            handler.TriggerContextCloseForTest();

            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler close context failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenRequestedRepeatedly_ShouldExecuteCloseOnlyOnce()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
            };

            handler.TriggerContextCloseForTest();
            handler.TriggerContextCloseForTest();

            Assert.AreEqual(1, handler.ExecuteCloseContextCallCount);
            Assert.AreEqual(2, handler.CurrentCloseRequestStateForTest);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenFirstCloseFails_ShouldAllowRetry()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
                CloseContextException = new InvalidOperationException("close failed asynchronously"),
                ReturnFaultedCloseContextTaskForTest = true,
            };

            handler.TriggerContextCloseForTest();
            handler.TriggerContextCloseForTest();

            Assert.AreEqual(2, handler.ExecuteCloseContextCallCount);
            Assert.AreEqual(2, handler.HandlerErrors.Count);
            Assert.AreEqual(0, handler.CurrentCloseRequestStateForTest);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenCloseReturnsFaultedObjectDisposedTask_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
                CloseContextException = new ObjectDisposedException("context disposed"),
                ReturnFaultedCloseContextTaskForTest = true,
            };

            handler.TriggerContextCloseForTest();

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_OnContextClose_WhenCloseReturnsCanceledTask_ShouldIgnore()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
                CloseContextException = new OperationCanceledException("close canceled"),
                ReturnFaultedCloseContextTaskForTest = true,
            };

            handler.TriggerContextCloseForTest();

            Assert.AreEqual(0, handler.HandlerErrors.Count);
        }

        [TestMethod]
        public void BaseChannelHandler_UserEventTriggered_WhenReaderIdleCloseReturnsFaultedTask_ShouldReportCloseError()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
                CloseContextException = new InvalidOperationException("reader idle close failed asynchronously"),
                ReturnFaultedCloseContextTaskForTest = true,
            };

            handler.TriggerSafeUserEventForTest(IdleStateEvent.FirstReaderIdleStateEvent);

            Assert.AreEqual(1, handler.ReaderIdleTriggeredCount);
            Assert.AreEqual(1, handler.HandlerErrors.Count);
            Assert.AreEqual("NettyChannelHandler close context failed.", handler.HandlerErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelHandler_ChannelInactive_ShouldResetCloseRequestState()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext())
            {
                AllowNullContextCloseForTest = true,
            };

            handler.TriggerContextCloseForTest();

            Assert.AreEqual(2, handler.CurrentCloseRequestStateForTest);

            handler.TriggerChannelInactiveForTest();

            Assert.AreEqual(0, handler.CurrentCloseRequestStateForTest);
        }

        [TestMethod]
        public void BaseChannelHandler_ResetCurrentChannelState_ShouldResetSessionState()
        {
            var handler = new TestBaseChannelHandler(CreateClientContext());
            handler.PrepareActiveStateForTest(new IPEndPoint(IPAddress.Loopback, 9527), true);

            handler.TriggerResetCurrentChannelStateForTest();

            Assert.IsFalse(handler.IsCurrentContextAssignedForTest);
            Assert.IsFalse(handler.IsCurrentSessionLoginForTest);
            Assert.IsNull(handler.CurrentRemoteIpEndPointForTest);
        }

        [TestMethod]
        public async Task BaseSocketHost_StartAsync_WhenConcurrentCallsOverlap_ShouldInvokeOnStartOnlyOnce()
        {
            var host = CreateSocketHost();
            host.BlockStartForTest = true;

            var firstStartTask = host.StartAsync();
            await host.StartEnteredTask;

            var secondStartTask = host.StartAsync();

            Assert.AreEqual(1, host.StartCallCount);

            host.ReleaseStartForTest();

            await Task.WhenAll(firstStartTask, secondStartTask);

            Assert.AreEqual(1, host.StartCallCount);
            Assert.IsTrue(host.IsRunning);
        }

        [TestMethod]
        public async Task BaseSocketHost_StopAsync_WhenConcurrentCallsOverlap_ShouldInvokeOnStopOnlyOnce()
        {
            var host = CreateSocketHost();
            await host.StartAsync();
            host.BlockStopForTest = true;

            var firstStopTask = host.StopAsync();
            await host.StopEnteredTask;

            var secondStopTask = host.StopAsync();

            Assert.AreEqual(1, host.StopCallCount);

            host.ReleaseStopForTest();

            await Task.WhenAll(firstStopTask, secondStopTask);

            Assert.AreEqual(1, host.StopCallCount);
            Assert.IsFalse(host.IsRunning);
        }

        [TestMethod]
        public async Task BaseSocketHost_StopAsync_WhenCalledDuringStartAsync_ShouldWaitForStartThenStop()
        {
            var host = CreateSocketHost();
            host.BlockStartForTest = true;

            var startTask = host.StartAsync();
            await host.StartEnteredTask;

            var stopTask = host.StopAsync();

            Assert.AreEqual(0, host.StopCallCount);

            host.ReleaseStartForTest();

            await Task.WhenAll(startTask, stopTask);

            Assert.AreEqual(1, host.StartCallCount);
            Assert.AreEqual(1, host.StopCallCount);
            Assert.IsFalse(host.IsRunning);
        }

        [TestMethod]
        public async Task BaseSocketHost_StartAsync_WhenOnStartFails_ShouldRollbackIsRunning()
        {
            var host = CreateSocketHost
            (
            );
            host.StartException = new InvalidOperationException("start failed");

            var startException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => host.StartAsync());

            Assert.AreEqual("start failed", startException.Message);
            Assert.AreEqual(1, host.StartCallCount);
            Assert.IsFalse(host.IsRunning);
        }

        [TestMethod]
        public async Task BaseSocketHost_DisposeAsync_WhenRunning_ShouldStopAndDisposeAndRejectRestart()
        {
            var host = CreateSocketHost();
            await host.StartAsync();

            await host.DisposeAsync();

            Assert.AreEqual(1, host.StopCallCount);
            Assert.AreEqual(1, host.DisposeCallCount);
            Assert.IsFalse(host.IsRunning);

            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => host.StartAsync());
        }

        [TestMethod]
        public async Task BaseSocketHost_DisposeAsync_WhenStopAndDisposeFail_ShouldAggregateExceptions()
        {
            var host = CreateSocketHost();
            host.SetIsRunningForTest(true);
            host.StopException = new InvalidOperationException("stop failed");
            host.DisposeException = new InvalidOperationException("dispose failed");

            var disposeException = await Assert.ThrowsExactlyAsync<AggregateException>(async () => await host.DisposeAsync());

            Assert.AreEqual(2, disposeException.InnerExceptions.Count);
            Assert.AreEqual("stop failed", disposeException.InnerExceptions[0].Message);
            Assert.AreEqual("dispose failed", disposeException.InnerExceptions[1].Message);
            Assert.AreEqual(1, host.StopCallCount);
            Assert.AreEqual(1, host.DisposeCallCount);
            Assert.IsFalse(host.IsRunning);

            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => host.StartAsync());
        }

        [TestMethod]
        public async Task BaseSocketHost_DisposeAsync_WhenConcurrentWithStopAsync_ShouldWaitForStopThenDisposeOnce()
        {
            var host = CreateSocketHost();
            await host.StartAsync();
            host.BlockStopForTest = true;

            var stopTask = host.StopAsync();
            await host.StopEnteredTask;

            var disposeTask = host.DisposeAsync().AsTask();

            Assert.AreEqual(1, host.StopCallCount);
            Assert.AreEqual(0, host.DisposeCallCount);

            host.ReleaseStopForTest();

            await Task.WhenAll(stopTask, disposeTask);

            Assert.AreEqual(1, host.StopCallCount);
            Assert.AreEqual(1, host.DisposeCallCount);
            Assert.IsFalse(host.IsRunning);

            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => host.StartAsync());
        }

        [TestMethod]
        public void BaseChannelInitializer_InitChannel_WhenGetChannelHandlerThrows_ShouldCloseChannelAndThrowWrappedException()
        {
            var initializer = new TestFaultInjectingChannelInitializer(CreateClientContext())
            {
                GetChannelHandlerException = new InvalidOperationException("get handler failed"),
            };

            var initException = Assert.ThrowsExactly<InvalidOperationException>(() => initializer.TriggerInitChannelForTest(new TcpSocketChannel()));

            Assert.AreEqual("NettyChannelInitializer init channel failed.", initException.Message);
            Assert.AreEqual("get handler failed", initException.InnerException?.Message);
            Assert.AreEqual(1, initializer.CloseChannelAfterInitFailureCallCount);
            Assert.AreEqual(0, initializer.InitChannelErrors.Count);
        }

        [TestMethod]
        public void BaseChannelInitializer_InitChannel_WhenGetChannelHandlerReturnsNull_ShouldCloseChannelAndThrowWrappedException()
        {
            var initializer = new TestFaultInjectingChannelInitializer(CreateClientContext())
            {
                ReturnNullChannelHandler = true,
            };

            var initException = Assert.ThrowsExactly<InvalidOperationException>(() => initializer.TriggerInitChannelForTest(new TcpSocketChannel()));

            Assert.AreEqual("NettyChannelInitializer init channel failed.", initException.Message);
            Assert.AreEqual("NettyChannelInitializer create channel handler returned null.", initException.InnerException?.Message);
            Assert.AreEqual(1, initializer.CloseChannelAfterInitFailureCallCount);
            Assert.AreEqual(0, initializer.InitChannelErrors.Count);
        }

        [TestMethod]
        public void BaseChannelInitializer_InitChannel_WhenAddTerminalChannelHandlersThrowsAndCloseFails_ShouldReportCloseErrorAndThrowWrappedException()
        {
            var initializer = new TestFaultInjectingChannelInitializer(CreateClientContext())
            {
                AddTerminalChannelHandlersException = new InvalidOperationException("add handler failed"),
                CloseChannelAfterInitFailureException = new InvalidOperationException("close channel failed"),
                ReturnFaultedCloseChannelAfterInitFailureTask = true,
            };

            var initException = Assert.ThrowsExactly<InvalidOperationException>(() => initializer.TriggerInitChannelForTest(new TcpSocketChannel()));

            Assert.AreEqual("NettyChannelInitializer init channel failed.", initException.Message);
            Assert.AreEqual("add handler failed", initException.InnerException?.Message);
            Assert.AreEqual(1, initializer.CloseChannelAfterInitFailureCallCount);
            Assert.AreEqual(1, initializer.InitChannelErrors.Count);
            Assert.AreEqual("NettyChannelInitializer close channel after init failed.", initializer.InitChannelErrors[0].Message);
        }

        [TestMethod]
        public void BaseChannelInitializer_InitChannel_WhenCloseAfterInitReturnsCanceledTask_ShouldIgnoreCloseNoiseAndThrowWrappedException()
        {
            var initializer = new TestFaultInjectingChannelInitializer(CreateClientContext())
            {
                AddTerminalChannelHandlersException = new InvalidOperationException("add handler failed"),
                CloseChannelAfterInitFailureException = new OperationCanceledException("close channel canceled"),
                ReturnFaultedCloseChannelAfterInitFailureTask = true,
            };

            var initException = Assert.ThrowsExactly<InvalidOperationException>(() => initializer.TriggerInitChannelForTest(new TcpSocketChannel()));

            Assert.AreEqual("NettyChannelInitializer init channel failed.", initException.Message);
            Assert.AreEqual("add handler failed", initException.InnerException?.Message);
            Assert.AreEqual(1, initializer.CloseChannelAfterInitFailureCallCount);
            Assert.AreEqual(0, initializer.InitChannelErrors.Count);
        }

        [TestMethod]
        public void BaseNettySocketClient_WhenReconnectRequestGenerationIsStale_ShouldIgnoreRequest()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(1);

            var staleHandler = new TestNettyClientHandler(client.CurrentContext);
            client.SetReconnectGenerationForTest(2);

            staleHandler.TriggerChannelInactiveForTest();

            Assert.AreEqual(0, client.ConnectAttemptCount);
            Assert.IsNull(client.CurrentReconnectTaskForTest);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_WhenReconnectRequestRepeatsInSameGeneration_ShouldStartOnlyOneLoop()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(3);

            client.TriggerReconnectForTest(3);
            client.TriggerReconnectForTest(3);

            Assert.AreEqual(1, client.ConnectAttemptCount);
            CollectionAssert.AreEqual(new List<long> { 3L }, client.ConnectAttemptGenerations);

            client.ReleaseReconnectLoopForTest();
            await client.CurrentReconnectTaskForTest;
        }

        [TestMethod]
        public async Task BaseNettySocketClient_WhenPreviousGenerationLoopCompleted_ShouldAllowNextGenerationReconnect()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(5);

            client.TriggerReconnectForTest(5);
            client.ReleaseReconnectLoopForTest();
            await client.CurrentReconnectTaskForTest;

            client.ResetConnectGateForTest();
            client.SetReconnectGenerationForTest(6);
            client.TriggerReconnectForTest(6);

            Assert.AreEqual(2, client.ConnectAttemptCount);
            CollectionAssert.AreEqual(new List<long> { 5L, 6L }, client.ConnectAttemptGenerations);

            client.ReleaseReconnectLoopForTest();
            await client.CurrentReconnectTaskForTest;
        }

        [TestMethod]
        public void BaseNettySocketClient_WhenStoppedBeforeBindingConnectedChannel_ShouldIgnoreNewChannel()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(7);
            client.SetIsRunningForTest(false);

            var result = client.TryBindConnectedChannelForTest(7, CancellationToken.None, null);

            Assert.IsFalse(result);
            Assert.IsNull(client.CurrentChannelHostForTest);
        }

        [TestMethod]
        public void BaseNettySocketClient_WhenActiveChannelAlreadyBound_ShouldIgnoreSecondConnectedChannel()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(8);
            client.ForceHasActiveBoundChannel = true;

            var secondResult = client.TryBindConnectedChannelForTest(8, CancellationToken.None, null);

            Assert.IsFalse(secondResult);
            Assert.IsNull(client.CurrentChannelHostForTest);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_OnStopAsync_WhenCloseShutdownAndDisposeFail_ShouldReportErrorsAndClearState()
        {
            var client = CreateClient();
            var reconnectCancellationTokenSource = new CancellationTokenSource();
            var channel = new TcpSocketChannel();
            var bossGroup = new MultithreadEventLoopGroup(1);

            try
            {
                client.PrepareStopStateForTest(channel, bossGroup, Task.CompletedTask, reconnectCancellationTokenSource, new Bootstrap());
                client.CloseChannelException = new InvalidOperationException("close failed");
                client.ShutdownBossGroupException = new InvalidOperationException("shutdown failed");
                client.DisposeReconnectCancellationTokenSourceException = new InvalidOperationException("dispose failed");

                await client.StopCoreAsyncForTest();

                Assert.AreEqual(3, client.StopErrors.Count);
                Assert.AreEqual("NettySocketClient stop close channel failed.", client.StopErrors[0].Message);
                Assert.AreEqual("NettySocketClient stop shutdown boss group failed.", client.StopErrors[1].Message);
                Assert.AreEqual("NettySocketClient stop dispose reconnect token source failed.", client.StopErrors[2].Message);
                Assert.IsNull(client.CurrentChannelHostForTest);
                Assert.IsNull(client.CurrentBossGroupForTest);
                Assert.IsNull(client.CurrentReconnectTaskForTest);
                Assert.IsNull(client.CurrentReconnectCancellationTokenSourceForTest);
                Assert.IsNull(client.CurrentBootstrapForTest);
            }
            finally
            {
                reconnectCancellationTokenSource.Dispose();
                await bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        [TestMethod]
        public async Task BaseNettySocketClient_OnStopAsync_WhenReconnectTaskThrows_ShouldReportErrorAndClearState()
        {
            var client = CreateClient();
            var reconnectCancellationTokenSource = new CancellationTokenSource();

            client.PrepareStopStateForTest(null, null, Task.CompletedTask, reconnectCancellationTokenSource, new Bootstrap());
            client.AwaitReconnectTaskException = new InvalidOperationException("reconnect failed");

            try
            {
                await client.StopCoreAsyncForTest();

                Assert.AreEqual(1, client.StopErrors.Count);
                Assert.AreEqual("NettySocketClient stop await reconnect task failed.", client.StopErrors[0].Message);
                Assert.IsNull(client.CurrentReconnectTaskForTest);
                Assert.IsNull(client.CurrentReconnectCancellationTokenSourceForTest);
                Assert.IsNull(client.CurrentBootstrapForTest);
            }
            finally
            {
                reconnectCancellationTokenSource.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseNettySocketClient_OnStopAsync_WhenReconnectTaskIsCanceled_ShouldIgnoreCancellationAndClearState()
        {
            var client = CreateClient();
            var reconnectCancellationTokenSource = new CancellationTokenSource();

            client.PrepareStopStateForTest(null, null, Task.CompletedTask, reconnectCancellationTokenSource, new Bootstrap());
            client.AwaitReconnectTaskException = new OperationCanceledException("reconnect canceled");

            try
            {
                await client.StopCoreAsyncForTest();

                Assert.AreEqual(0, client.StopErrors.Count);
                Assert.IsNull(client.CurrentReconnectTaskForTest);
                Assert.IsNull(client.CurrentReconnectCancellationTokenSourceForTest);
                Assert.IsNull(client.CurrentBootstrapForTest);
            }
            finally
            {
                reconnectCancellationTokenSource.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseNettySocketClient_StartAsync_WhenEnsureReconnectLoopStartedThrows_ShouldRollbackStateAndAllowRetry()
        {
            var client = CreateClient();

            try
            {
                client.EnsureReconnectLoopStartedException = new InvalidOperationException("start reconnect failed");

                var startException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.StartAsync());

                Assert.AreEqual("NettySocketClient start failed.", startException.Message);
                Assert.IsFalse(client.IsRunning);
                Assert.IsNull(client.CurrentBossGroupForTest);
                Assert.IsNull(client.CurrentBootstrapForTest);
                Assert.IsNull(client.CurrentReconnectCancellationTokenSourceForTest);
                Assert.IsNull(client.CurrentReconnectTaskForTest);
                Assert.IsNull(client.CurrentChannelHostForTest);
                Assert.AreEqual(0, client.StartErrors.Count);

                client.EnsureReconnectLoopStartedException = null;

                await client.StartAsync();

                Assert.IsTrue(client.IsRunning);
                Assert.IsNotNull(client.CurrentBossGroupForTest);
                Assert.IsNotNull(client.CurrentBootstrapForTest);
                Assert.IsNotNull(client.CurrentReconnectCancellationTokenSourceForTest);
                Assert.AreEqual(2, client.EnsureReconnectLoopStartedCallCount);
            }
            finally
            {
                if (client.IsRunning)
                {
                    await client.StopAsync();
                }
            }
        }

        [TestMethod]
        public async Task BaseNettySocketClient_StartAsync_WhenRollbackStepsFail_ShouldReportStartErrorsAndClearState()
        {
            var client = CreateClient();

            client.EnsureReconnectLoopStartedException = new InvalidOperationException("start reconnect failed");
            client.ShutdownBossGroupException = new InvalidOperationException("shutdown failed");
            client.DisposeReconnectCancellationTokenSourceException = new InvalidOperationException("dispose failed");

            var startException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => client.StartAsync());

            Assert.AreEqual("NettySocketClient start failed.", startException.Message);
            Assert.AreEqual(2, client.StartErrors.Count);
            Assert.AreEqual("NettySocketClient reset start shutdown boss group failed.", client.StartErrors[0].Message);
            Assert.AreEqual("NettySocketClient reset start dispose reconnect token source failed.", client.StartErrors[1].Message);
            Assert.IsFalse(client.IsRunning);
            Assert.IsNull(client.CurrentBossGroupForTest);
            Assert.IsNull(client.CurrentBootstrapForTest);
            Assert.IsNull(client.CurrentReconnectCancellationTokenSourceForTest);
            Assert.IsNull(client.CurrentReconnectTaskForTest);
            Assert.IsNull(client.CurrentChannelHostForTest);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_ConnectToServerAsync_WhenUnexpectedExceptionOccurs_ShouldReportErrorAndRetryDelay()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(9);
            client.SetCurrentBootstrapForTest(new Bootstrap());
            client.ConnectChannelException = new InvalidOperationException("connect crashed");
            client.StopAfterDelayForTest = true;

            await client.RunBaseConnectToServerAsyncForTest(9);

            Assert.AreEqual(1, client.ConnectChannelCallCount);
            Assert.AreEqual(1, client.DelayBeforeReconnectCallCount);
            Assert.AreEqual(1, client.ConnectErrors.Count);
            Assert.AreEqual("NettySocketClient connect attempt failed.", client.ConnectErrors[0].Message);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_ConnectToServerAsync_WhenSocketExceptionOccurs_ShouldRetryWithoutReportingError()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(10);
            client.SetCurrentBootstrapForTest(new Bootstrap());
            client.ConnectChannelException = new SocketException((int)SocketError.ConnectionRefused);
            client.StopAfterDelayForTest = true;

            await client.RunBaseConnectToServerAsyncForTest(10);

            Assert.AreEqual(1, client.ConnectChannelCallCount);
            Assert.AreEqual(1, client.DelayBeforeReconnectCallCount);
            Assert.AreEqual(0, client.ConnectErrors.Count);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_ConnectToServerAsync_WhenConnectReturnsNullChannel_ShouldReportErrorAndRetryDelay()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(10);
            client.SetCurrentBootstrapForTest(new Bootstrap());
            client.StopAfterDelayForTest = true;

            await client.RunBaseConnectToServerAsyncForTest(10);

            Assert.AreEqual(1, client.ConnectChannelCallCount);
            Assert.AreEqual(1, client.DelayBeforeReconnectCallCount);
            Assert.AreEqual(1, client.ConnectErrors.Count);
            Assert.AreEqual("NettySocketClient connect attempt failed.", client.ConnectErrors[0].Message);
            Assert.AreEqual("NettySocketClient connect returned null channel.", client.ConnectErrors[0].InnerException?.Message);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_ConnectToServerAsync_WhenCanceledDuringConnect_ShouldIgnoreAndNotDelay()
        {
            var client = CreateClient();
            client.PrepareReconnectStateForTest(11);
            client.SetCurrentBootstrapForTest(new Bootstrap());
            client.ConnectChannelException = new OperationCanceledException("connect canceled");
            client.CancelReconnectTokenBeforeThrowingConnectChannelException = true;

            await client.RunBaseConnectToServerAsyncForTest(11);

            Assert.AreEqual(1, client.ConnectChannelCallCount);
            Assert.AreEqual(0, client.DelayBeforeReconnectCallCount);
            Assert.AreEqual(0, client.ConnectErrors.Count);
        }

        [TestMethod]
        public async Task BaseNettySocketClient_StartAsync_WhenStopIsAwaitingReconnectTask_ShouldWaitForStopThenStartFresh()
        {
            var client = CreateClient();
            var reconnectCancellationTokenSource = new CancellationTokenSource();

            try
            {
                client.BlockAwaitReconnectTaskForTest = true;
                client.ResetAwaitReconnectTaskForTest();
                client.PrepareStopStateForTest(null, null, Task.CompletedTask, reconnectCancellationTokenSource, new Bootstrap());
                client.SetIsRunningForTest(true);

                var stopTask = client.StopAsync();
                await client.AwaitReconnectTaskEnteredTask;

                var startTask = client.StartAsync();

                Assert.AreEqual(0, client.CreateBossGroupCallCount);
                Assert.IsFalse(startTask.IsCompleted);

                client.BlockAwaitReconnectTaskForTest = false;
                client.ReleaseAwaitReconnectTaskForTest();

                await Task.WhenAll(stopTask, startTask);

                Assert.IsTrue(client.IsRunning);
                Assert.AreEqual(1, client.CreateBossGroupCallCount);
                Assert.IsNotNull(client.CurrentBossGroupForTest);
                Assert.IsNotNull(client.CurrentBootstrapForTest);
                Assert.IsNotNull(client.CurrentReconnectCancellationTokenSourceForTest);
            }
            finally
            {
                client.BlockAwaitReconnectTaskForTest = false;
                client.ReleaseAwaitReconnectTaskForTest();
                client.ReleaseReconnectLoopForTest();

                if (client.CurrentReconnectTaskForTest != null)
                {
                    await client.CurrentReconnectTaskForTest;
                }

                if (client.IsRunning)
                {
                    await client.StopAsync();
                }

                reconnectCancellationTokenSource.Dispose();
            }
        }

        [TestMethod]
        public async Task BaseNettySocketServer_StartAsync_WhenBindFails_ShouldRollbackStateAndAllowRetry()
        {
            var server = CreateServer();

            try
            {
                server.BindServerException = new InvalidOperationException("bind failed");

                var startException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => server.StartAsync());

                Assert.AreEqual("NettySocketServer start failed.", startException.Message);
                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentChannelHostForTest);
                Assert.IsNull(server.CurrentBossGroupForTest);
                Assert.IsNull(server.CurrentWorkerGroupForTest);
                Assert.IsNull(server.CurrentBootstrapForTest);
                Assert.AreEqual(0, server.StartErrors.Count);

                server.BindServerException = null;

                await server.StartAsync();

                Assert.IsTrue(server.IsRunning);
                Assert.IsNotNull(server.CurrentBossGroupForTest);
                Assert.IsNotNull(server.CurrentWorkerGroupForTest);
                Assert.IsNotNull(server.CurrentBootstrapForTest);
                Assert.AreEqual(2, server.BindServerCallCount);
            }
            finally
            {
                if (server.IsRunning)
                {
                    await server.StopAsync();
                }

                foreach (var currentBossGroup in server.CreatedBossGroups)
                {
                    await currentBossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                }

                foreach (var currentWorkerGroup in server.CreatedWorkerGroups)
                {
                    await currentWorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                }
            }
        }

        [TestMethod]
        public async Task BaseNettySocketServer_StartAsync_WhenRollbackStepsFail_ShouldReportStartErrorsAndClearState()
        {
            var server = CreateServer();

            try
            {
                server.BindServerException = new InvalidOperationException("bind failed");
                server.ShutdownBossGroupException = new InvalidOperationException("shutdown boss failed");
                server.ShutdownWorkerGroupException = new InvalidOperationException("shutdown worker failed");

                var startException = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => server.StartAsync());

                Assert.AreEqual("NettySocketServer start failed.", startException.Message);
                Assert.AreEqual(2, server.StartErrors.Count);
                Assert.AreEqual("NettySocketServer reset start shutdown boss group failed.", server.StartErrors[0].Message);
                Assert.AreEqual("NettySocketServer reset start shutdown worker group failed.", server.StartErrors[1].Message);
                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentChannelHostForTest);
                Assert.IsNull(server.CurrentBossGroupForTest);
                Assert.IsNull(server.CurrentWorkerGroupForTest);
                Assert.IsNull(server.CurrentBootstrapForTest);
            }
            finally
            {
                foreach (var currentBossGroup in server.CreatedBossGroups)
                {
                    await currentBossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                }

                foreach (var currentWorkerGroup in server.CreatedWorkerGroups)
                {
                    await currentWorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                }
            }
        }

        [TestMethod]
        public async Task BaseNettySocketServer_StopAsync_WhenCloseAndShutdownFail_ShouldReportErrorsAndClearState()
        {
            var server = CreateServer();
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup(1);

            try
            {
                server.PrepareStopStateForTest(null, bossGroup, workerGroup, new ServerBootstrap());
                server.CloseChannelException = new InvalidOperationException("close failed");
                server.ShutdownBossGroupException = new InvalidOperationException("shutdown boss failed");
                server.ShutdownWorkerGroupException = new InvalidOperationException("shutdown worker failed");

                await server.StopAsync();

                Assert.AreEqual(3, server.StopErrors.Count);
                Assert.AreEqual("NettySocketServer stop close channel failed.", server.StopErrors[0].Message);
                Assert.AreEqual("NettySocketServer stop shutdown boss group failed.", server.StopErrors[1].Message);
                Assert.AreEqual("NettySocketServer stop shutdown worker group failed.", server.StopErrors[2].Message);
                Assert.IsFalse(server.IsRunning);
                Assert.IsNull(server.CurrentChannelHostForTest);
                Assert.IsNull(server.CurrentBossGroupForTest);
                Assert.IsNull(server.CurrentWorkerGroupForTest);
                Assert.IsNull(server.CurrentBootstrapForTest);
                Assert.AreEqual(0, server.CurrentContext.CurrentChannelDictionary.Count);
            }
            finally
            {
                await bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
                await workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        [TestMethod]
        public async Task BaseNettySocketServer_StopAsync_WhenTrackedHandlersRemain_ShouldClearDictionary()
        {
            var context = CreateServerContext();
            var server = new TestNettyServer(context);
            var handler = new TestNettyServerHandler(context);

            handler.TriggerServerChannelActiveForTest();
            server.PrepareStopStateForTest(null, null, null, new ServerBootstrap());

            await server.StopAsync();

            Assert.AreEqual(0, context.CurrentChannelDictionary.Count);
        }

        [TestMethod]
        public async Task BaseNettySocketServer_StopAsync_WhenClearTrackedHandlersThrows_ShouldReportError()
        {
            var context = CreateServerContext();
            var server = new TestNettyServer(context);
            var handler = new TestNettyServerHandler(context);

            handler.TriggerServerChannelActiveForTest();
            server.PrepareStopStateForTest(null, null, null, new ServerBootstrap());
            server.CleanupTrackedChannelHandlersException = new InvalidOperationException("clear tracked handlers failed");

            await server.StopAsync();

            Assert.AreEqual(1, server.StopErrors.Count);
            Assert.AreEqual("NettySocketServer stop clear tracked handlers failed.", server.StopErrors[0].Message);
        }

        [TestMethod]
        public void BaseServerChannelHandler_ChannelInactive_WhenSessionOwnershipTransferred_ShouldKeepNewHandlerRegistered()
        {
            var context = CreateServerContext();
            var sharedSession = new TestNettySession();
            var staleHandler = new TestNettyServerHandler(context);
            var currentHandler = new TestNettyServerHandler(context);

            staleHandler.SetCurrentChannelSessionForTest(sharedSession);
            currentHandler.SetCurrentChannelSessionForTest(sharedSession);

            staleHandler.TriggerServerChannelActiveForTest();
            currentHandler.TriggerServerChannelActiveForTest();
            staleHandler.TriggerServerChannelInactiveForTest();

            Assert.IsTrue(context.CurrentChannelDictionary.TryGetValue(sharedSession.SessionID, out var registeredHandler));
            Assert.AreSame(currentHandler, registeredHandler);
        }

        [TestMethod]
        public void BaseServerChannelHandler_ChannelInactive_WhenHandlerStillOwnsSession_ShouldRemoveRegistration()
        {
            var context = CreateServerContext();
            var handler = new TestNettyServerHandler(context);

            handler.TriggerServerChannelActiveForTest();
            handler.TriggerServerChannelInactiveForTest();

            Assert.IsFalse(context.CurrentChannelDictionary.ContainsKey(handler.CurrentChannelSession.SessionID));
        }

        private static TestNettyClient CreateClient()
        {
            return new TestNettyClient(CreateClientContext());
        }

        private static void AssignReconnectActionForGcTest(TestNettyClientContext context, Action<long> reconnectAction)
        {
            context.CurrentConnectToServerAction = reconnectAction;
        }

        private static void ForceFullGcForTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static TestSocketHost CreateSocketHost()
        {
            return new TestSocketHost(CreateClientContext());
        }

        private static TestNettyClientContext CreateClientContext()
        {
            return new TestNettyClientContext(new ClientChannelOptions("127.0.0.1", 9527, 0, 4, 0, 0, true, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1));
        }

        private static TestNettyServer CreateServer()
        {
            return new TestNettyServer(CreateServerContext());
        }

        private static TestNettyServerContext CreateServerContext()
        {
            return new TestNettyServerContext(new ServerChannelOptions(9527, 0, 4, 0, 0, true, sendDataIntervalMilliseconds: 1, intervalHeartTotalMilliseconds: 1));
        }
    }
}
