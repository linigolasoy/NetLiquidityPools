using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesTrading
    {
        public ICexExchange Exchange { get; }

        public Task<ITradingResult<string>> PlaceOrderMarket(IFuturesSymbol oSymbol, bool bBuy, decimal nAmount, decimal? nStopLoss = null, decimal? nTakeProfit = null);
        public Task<ITradingResult<string>> PlaceOrderLimit(IFuturesSymbol oSymbol, bool bBuy, decimal nAmount, decimal nPrice, decimal? nStopLoss = null, decimal? nTakeProfit = null);

        public Task<ITradingResult<bool>> CancelOrder(IFuturesSymbol oSymbol, string strOrderId);   

        public Task<ITradingResult<bool>> ClosePosition(IFuturesPosition oPosition, decimal? nPrice = null);
    }
}
