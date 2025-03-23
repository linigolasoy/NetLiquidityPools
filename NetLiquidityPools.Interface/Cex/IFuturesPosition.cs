using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public enum PositionDirection
    {
        Long,
        Short
    }

    public interface IFuturesPosition
    {
        public IFuturesSymbol Symbol { get; }
        public PositionDirection Direction { get; } 
        public DateTime DateOpen { get; }
        public DateTime DateUpdated { get; }

        public decimal Quantity { get; }
        public decimal PriceAverage { get; }
        public bool Closed { get; } 
    }
}
