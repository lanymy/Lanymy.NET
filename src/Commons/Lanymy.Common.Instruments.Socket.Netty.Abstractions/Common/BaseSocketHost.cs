﻿using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;


namespace Lanymy.Common.Instruments.Common
{



    public abstract class BaseSocketHost<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelContext, TChannelHandler, TChannelInitializer> : IAsyncDisposable
        where TChannelInitializer : BaseChannelInitializer<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelContext, TChannelHandler>
        where TChannelContext : BaseChannelContext<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter>
        where TChannelHandler : BaseChannelHandler<TReceivePackage, TSendPackage, TChannelOptions, TChannelSession, TChannelFixedHeaderPackageFilter, TChannelContext>
        where TReceivePackage : class
        where TSendPackage : class
        where TChannelOptions : BaseChannelOptions
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
        protected readonly TChannelOptions _CurrentChannelOptions;


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
