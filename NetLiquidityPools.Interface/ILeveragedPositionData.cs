using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    /// <summary>
    /// Position data for leveraged positions
    /// </summary>
    public interface ILeveragedPositionData
    {
        public IToken Token0 { get; }
        public IToken Token1 { get; }
        public decimal Fee { get; }

        public decimal Debt0 { get; }
        public decimal Debt1 { get; }
        public decimal Amount0 { get; }
        public decimal Amount1 { get; }

        public decimal Fees0 { get; }
        public decimal Fees1 { get; }

        public decimal PriceLow { get; }
        public decimal PriceHigh { get; }

        public decimal PriceActual { get; }

        public int TickStep { get; }    
    }
}
