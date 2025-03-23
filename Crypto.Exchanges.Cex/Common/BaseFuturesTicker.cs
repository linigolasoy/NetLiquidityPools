using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common  
{
    internal class BaseFuturesTicker
    {

        public BaseFuturesTicker(IFuturesSymbol oSymbol)
        {
            Symbol = oSymbol;
        }
        public IFuturesSymbol Symbol { get; }
        public decimal LastPrice { get; internal set; }
        public decimal LastAsk { get; internal set; }
        public decimal LastBid { get; internal set; }
        public DateTime LastUpdate { get; internal set; }

        public void Update(IFuturesTicker oData)
        {
            LastUpdate = oData.LastUpdate;
            LastPrice = oData.LastPrice;
            LastAsk = oData.LastAsk;
            LastBid = oData.LastBid;
        }

    }
}
