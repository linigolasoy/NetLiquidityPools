using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Liquidity
{

    internal class UniswapRangeResult : ILiquidityRangeResult
    {
        public BigInteger Liquidity { get; internal set; }   

        public BigInteger Amount0 { get; internal set; }

        public BigInteger Amount1 { get; internal set; }

        public BigInteger[]? AmountsOnLower { get; internal set; }
        public BigInteger[]? AmountsOnUpper { get; internal set; }

        public BigInteger TickUpper { get; internal set; }

        public BigInteger TickLower { get; internal set; }

        public decimal PriceUpper { get; internal set; }
        public decimal PriceLower { get; internal set; }

        public decimal Quantity0 { get; internal set; }
        public decimal Quantity1 { get; internal set; }


        public decimal MaxLoss { get; internal set; }

        public decimal MaxProfit { get; internal set; }

        public decimal LossProfitDifference { get; internal set; }
    }


    internal class UniswapRangeCalculator : ILiquidityRangeCalculator
    {

        public UniswapRangeCalculator(
            IToken oToken0, IToken oToken1,
            decimal nAmount0, decimal nAmount1,
            decimal nPrice,
            decimal nPercentMin, decimal nPercentMax)

        {
            Token0 = oToken0;
            Token1 = oToken1;
            Amount0 = nAmount0;
            Amount1 = nAmount1;
            Price = nPrice;
            PercentRangeMin = nPercentMin;
            PercentRangeMax = nPercentMax;
            UniMath = new UniswapPoolMath(Token0, Token1);
            TickActual = UniMath.PriceToTick(Price);
        }

        public IToken Token0 { get; }

        public IToken Token1 { get; }

        public decimal Amount0 { get; }
        public decimal Amount1 { get; }

        public decimal Price { get; }

        public decimal PercentRangeMax { get; }

        public decimal PercentRangeMin { get; }

        private BigInteger TickActual {  get; }
        private UniswapPoolMath UniMath { get; }

        /// <summary>
        /// Calculates
        /// </summary>
        /// <param name="nTickLow"></param>
        /// <param name="nTickHigh"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private ILiquidityRangeResult? Calculate( BigInteger oTickLow, BigInteger oTickHigh )
        {

            BigInteger oTickActual = UniMath.PriceToTick(Price);
            BigInteger oAmount0 = UniMath.Amount0Big(Amount0);
            BigInteger oAmount1 = UniMath.Amount1Big(Amount1);
            BigInteger oLiquidity = UniMath.LiquidityFromAmountsTicks(oTickActual, oTickLow, oTickHigh, oAmount0, oAmount1);

            // Profit and Los
            BigInteger[] aAmountsInit   = UniMath.CalculateAmounts(oLiquidity, oTickActual, oTickLow, oTickHigh);
            BigInteger[] aAmountsHigh   = UniMath.CalculateAmounts(oLiquidity, oTickHigh, oTickLow, oTickHigh);
            BigInteger[] aAmountsLow    = UniMath.CalculateAmounts(oLiquidity, oTickLow, oTickLow, oTickHigh);

            decimal nPriceLow           = UniMath.TickToPrice(oTickLow);
            decimal nPriceHigh          = UniMath.TickToPrice(oTickHigh);
            decimal nTotalHigh          = UniMath.BigToAmount0(aAmountsHigh[0]) * nPriceHigh + UniMath.BigToAmount1(aAmountsHigh[1]);
            decimal nTotalLow           = UniMath.BigToAmount0(aAmountsLow[0]) * nPriceLow + UniMath.BigToAmount1(aAmountsLow[1]);
            decimal nTotalInit          = UniMath.BigToAmount0(aAmountsInit[0]) * Price + UniMath.BigToAmount1(aAmountsInit[1]);



            decimal nLoss = nTotalInit - nTotalLow;
            decimal nProfit = nTotalHigh - nTotalInit;

            if (nLoss < 0 || nProfit < 0) return null;

            decimal nMin = Math.Min(nLoss, nProfit);
            decimal nMax = Math.Max(nLoss, nProfit);
            decimal nPercent = (nMax - nMin) * 100.0M / nMin;

            ILiquidityRangeResult oResult = new UniswapRangeResult()
            {
                Liquidity = oLiquidity,
                Amount0 = aAmountsInit[0],
                Amount1 = aAmountsInit[1],
                TickUpper = oTickHigh,
                TickLower = oTickLow,
                MaxLoss = nLoss,
                MaxProfit = nProfit,
                LossProfitDifference = nPercent,
                AmountsOnLower = aAmountsLow,
                AmountsOnUpper = aAmountsHigh
            };

            return oResult;
        }
        /// <summary>
        /// Calculate best range
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ILiquidityRangeResult? CalculateBest()
        {

            decimal nMultiplierMin = PercentRangeMin / 100.0M;
            decimal nMultiplierMax = PercentRangeMax / 100.0M;

            decimal nPriceLowMin = (1.0M - nMultiplierMax) * Price;
            decimal nPriceLowMax = (1.0M - nMultiplierMin) * Price;

            int nTickLowMin = (int)UniMath.PriceToTick(nPriceLowMin);
            int nTickLowMax = (int)UniMath.PriceToTick(nPriceLowMax);

            ILiquidityRangeResult? oBest = null;

            for( int nLow = nTickLowMin; nLow <= nTickLowMax; nLow ++ )
            {
                decimal nPriceHighMin = (1.0M + nMultiplierMin) * Price;
                decimal nPriceHighMax = (1.0M + nMultiplierMax) * Price;

                BigInteger oTickLow = new BigInteger(nLow);
                int nTickHighMin = (int)UniMath.PriceToTick(nPriceHighMin);
                int nTickHighMax = (int)UniMath.PriceToTick(nPriceHighMax);
                for (int nHigh = nTickHighMin; nHigh <= nTickHighMax; nHigh++)
                {
                    BigInteger oTickHigh = new BigInteger(nHigh);
                    ILiquidityRangeResult? oNew = Calculate(oTickLow, oTickHigh);
                    if (oNew == null) continue;
                    if( oBest == null || oBest.LossProfitDifference > oNew.LossProfitDifference )
                    {
                        oBest = oNew;   
                    }
                }
            }

            UniswapRangeResult oBestResult = (UniswapRangeResult)oBest!;
            oBestResult.PriceLower = UniMath.TickToPrice(oBestResult.TickLower);
            oBestResult.PriceUpper = UniMath.TickToPrice(oBestResult.TickUpper);

            oBestResult.Quantity0 = UniMath.BigToAmount0(oBestResult.Amount0);
            oBestResult.Quantity1 = UniMath.BigToAmount1(oBestResult.Amount1);
            return oBest;
        }
    }
}
