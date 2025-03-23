using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxFuturesTicker: BaseFuturesTicker, IFuturesTicker
    {
        public BingxFuturesTicker(IFuturesSymbol oSymbol, BingXFuturesTicker oData):
            base(oSymbol)
        {
            LastPrice = oData.LastPrice;
            LastAsk = oData.BestAskPrice;
            LastBid = oData.BestBidPrice;
            LastUpdate = DateTime.Now;  
        }

        public BingxFuturesTicker(IFuturesSymbol oSymbol, BingXFuturesTickerUpdate oData, DateTime dTime) :
            base(oSymbol)
        {
            LastPrice = oData.LastPrice;
            LastAsk = oData.BestAskPrice;
            LastBid = oData.BestBidPrice;
            LastUpdate = dTime;
        }
    }
}
