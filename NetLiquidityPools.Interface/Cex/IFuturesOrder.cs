using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesOrder
    {
        public IFuturesSymbol Symbol { get; }
    }
}
