using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    /// <summary>
    /// Market interface
    /// </summary>
    public interface IFuturesMarket
    {

        public ICexExchange Exchange { get; }


        public Task<IFundingRate[]?> GetFundingRates();

        public Task<IFuturesTicker[]?> GetTickers();

        public IFuturesWebsocketPublic? Websocket { get; }
        public Task<bool> StartSockets();
        public Task<bool> EndSockets();

    }
}
