using System;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Lanymy.Common.ExtensionFunctions;
using Lanymy.Common.Instruments.Server;

namespace Lanymy.Common.Instruments.Common
{



    public abstract class BaseSocketHost<TChannelInitializer, TChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelHandler> : IAsyncDisposable
        where TChannelInitializer : BaseChannelInitializer<TChannelContext, TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelHandler>
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TChannelHandler : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelContext>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelSession : BaseChannelSession, new()
        where TChannelFixedHeaderPackageFilter : BaseChannelFixedHeaderPackageFilter<TReceivePackage, TSendPackage, TChannelSession>, new()
    {


        protected readonly object _Locker = new object();

        private bool _IsRunning = false;

        public bool IsRunning
        {
            get { return _IsRunning; }
            //private set
            protected set
            {

                if (_IsRunning == value)
                    return;

                lock (_Locker)
                {
                    if (!_IsRunning.Equals(value))
                    {
                        _IsRunning = value;
                    }
                }

            }
        }

        protected IChannel _CurrentChannelHost;
        protected IEventLoopGroup _CurrentBossGroup;

        protected readonly TChannelContext _CurrentChannelContext;
        protected readonly ChannelOptions _CurrentChannelOptions;


        protected BaseSocketHost(TChannelContext serverChannelContext)
        {

            _CurrentChannelContext = serverChannelContext;

            _CurrentChannelOptions = _CurrentChannelContext.CurrentChannelOptions;

        }



        public async Task StartAsync()
        {

            if (IsRunning)
            {
                return;
            }

            IsRunning = true;


            await OnStartAsync();


        }


        public async Task StopAsync()
        {


            if (!IsRunning)
            {
                return;
            }

            IsRunning = false;

            await OnStopAsync();

        }


        protected abstract Task OnStartAsync();


        protected abstract Task OnStopAsync();


        protected abstract Task OnDisposeAsync();


        public async ValueTask DisposeAsync()
        {

            await OnDisposeAsync();

        }



    }

}
