using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{


    public interface ILiquidityRangeResult
    {
        public BigInteger Liquidity { get; }
        public BigInteger Amount0 { get; }
        public BigInteger Amount1 { get; }

        public BigInteger TickUpper { get; }
        public BigInteger TickLower { get;  }

        public decimal MaxLoss { get; }
        public decimal MaxProfit { get; }

        public decimal LossProfitDifference { get; }

    }


    public interface ILiquidityRangeCalculator
    {
        public IToken Token0 { get; }   
        public IToken Token1 { get; }
        public decimal Amount0 { get; }
        public decimal Amount1 { get; }

        public decimal Price { get; }   
        public decimal PercentRangeMax { get; }
        public decimal PercentRangeMin { get; }


        public ILiquidityRangeResult? CalculateBest();
    }
}
