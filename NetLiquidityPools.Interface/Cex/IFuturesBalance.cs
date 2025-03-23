using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesBalance: ICexBalance
    {
        public decimal UnrealizedProfit { get; }
        public decimal Equity { get; }
        public decimal MarginUsed { get; }
        public decimal MarginFrozen { get; }
    }
}
