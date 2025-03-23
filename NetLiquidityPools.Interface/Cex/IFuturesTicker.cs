using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesTicker
    {
        public IFuturesSymbol Symbol { get; }
        public decimal LastPrice { get; }
        public decimal LastAsk { get; }
        public decimal LastBid { get; }
        public DateTime LastUpdate { get; }

        public void Update(IFuturesTicker oData);   
    }
}
