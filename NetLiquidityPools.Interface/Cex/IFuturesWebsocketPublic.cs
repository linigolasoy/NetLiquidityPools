using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    /// <summary>
    /// Public websocket interface
    /// </summary>
    public interface IFuturesWebsocketPublic
    {

        public IFuturesMarket Market { get; }   

        public Task<bool> Subscribe(IFuturesSymbol oSymbol);

        public Task<bool> UnSubscribe(IFuturesSymbol oSymbol);


        public ISocketManager<IFundingRate> FundingRateManager { get; }
        public ISocketManager<IFuturesTicker> TickerManager { get; }

    }
}
