using Nethereum.Hex.HexConvertors.Extensions;
using NetLiquidityPools.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Liquidity
{

    public class UniswapLiquidityData
    {
        public BigInteger Liquidity { get; set; }
        public BigInteger Amount0 { get; set; }
        public BigInteger Amount1 { get; set; }

        public BigInteger TickUpper { get; set; }
        public BigInteger TickLower { get; set; }

        public decimal MaxLoss { get; set; }
        public decimal MaxProfit { get; set; }

        public decimal LossProfitDifference { get; set; }
    }

    public class UniswapPoolMath
    {
        private double POW_CONSTANT = 1.0001;
        private double m_nPow0;
        private double m_nPow1;

        private static BigInteger Q96 = BigInteger.Pow(2, 96);
        public UniswapPoolMath( IToken oToken0, IToken oToken1)
        {
            Token0 = oToken0;
            Token1 = oToken1;
            m_nPow0 = Math.Pow(10, oToken0.Decimals);
            m_nPow1 = Math.Pow(10, oToken1.Decimals);
        }

        public IToken Token0 { get; }
        public IToken Token1 { get; }

        public BigInteger FeeToBig(decimal nFee)
        {
            int nResult = (int)(nFee * 10000.0M);
            return new BigInteger(nResult);
        }

        public decimal BigToFee(BigInteger oFee)
        {
            int nResult = (int)oFee;
            return (decimal)nResult / 10000.0M;
        }

        public int TickSpacing(decimal nFee)
        {
            BigInteger oFee = FeeToBig(nFee);
            return TickSpacing(oFee);
        }


        public int TickSpacing(BigInteger oFee)
        {
            if (oFee == 100) return 1;
            if (oFee == 500) return 10;
            if (oFee == 3000) return 30;
            return 100;
        }

        /*
        function getLiquidityForAmounts(
            uint160 sqrtRatioX96,
            uint160 sqrtRatioAX96,
            uint160 sqrtRatioBX96,
            uint256 amount0,
            uint256 amount1
        ) internal pure returns(uint128 liquidity)
        {
            if (sqrtRatioAX96 > sqrtRatioBX96) (sqrtRatioAX96, sqrtRatioBX96) = (sqrtRatioBX96, sqrtRatioAX96);

            if (sqrtRatioX96 <= sqrtRatioAX96)
            {
                liquidity = getLiquidityForAmount0(sqrtRatioAX96, sqrtRatioBX96, amount0);
            }
            else if (sqrtRatioX96 < sqrtRatioBX96)
            {
                uint128 liquidity0 = getLiquidityForAmount0(sqrtRatioX96, sqrtRatioBX96, amount0);
                uint128 liquidity1 = getLiquidityForAmount1(sqrtRatioAX96, sqrtRatioX96, amount1);

                liquidity = liquidity0 < liquidity1 ? liquidity0 : liquidity1;
            }
            else
            {
                liquidity = getLiquidityForAmount1(sqrtRatioAX96, sqrtRatioBX96, amount1);
            }
        }
        */

        public BigInteger LiquidityForAmount0( BigInteger oSqrtLow, BigInteger oSqrtHigh, BigInteger oAmount )
        {
            BigInteger oMin = BigInteger.Min(oSqrtLow, oSqrtHigh);
            BigInteger oMax = BigInteger.Max(oSqrtLow, oSqrtHigh);

            BigInteger oMult = oSqrtLow * oSqrtHigh;
            oMult /= Q96;

            BigInteger oMult2 = oAmount * oMult;
            BigInteger oResult =oMult2 / (oMax - oMin);

            return oResult;
        }

        public BigInteger LiquidityForAmount1(BigInteger oSqrtLow, BigInteger oSqrtHigh, BigInteger oAmount)
        {
            BigInteger oMin = BigInteger.Min(oSqrtLow, oSqrtHigh);
            BigInteger oMax = BigInteger.Max(oSqrtLow, oSqrtHigh);

            BigInteger oMult = oAmount * Q96;
            BigInteger oResult = oMult / (oMax - oMin);
            // return toUint128(FullMath.mulDiv(amount1, FixedPoint96.Q96, sqrtRatioBX96 - sqrtRatioAX96));

            return oResult;
        }

        public BigInteger LiquidityFromAmountsSqrt(BigInteger oSqrtActual, BigInteger oParSqrtLow, BigInteger oParSqrtHigh, BigInteger oAmount0, BigInteger oAmount1)
        {
            BigInteger oHigh = BigInteger.Max(oParSqrtLow, oParSqrtHigh);
            BigInteger oLow = BigInteger.Min(oParSqrtLow, oParSqrtHigh);

            if( oSqrtActual < oLow )
            {
                return LiquidityForAmount0(oLow, oHigh, oAmount0 );
            }
            else if( oSqrtActual < oHigh )
            {
                BigInteger oLiquidity0 = LiquidityForAmount0(oSqrtActual, oHigh, oAmount0);
                BigInteger oLiquidity1 = LiquidityForAmount1(oLow, oSqrtActual, oAmount1);

                if (oLiquidity0 < oLiquidity1) return oLiquidity0;
                return oLiquidity1;

            }
            else
            {
                return LiquidityForAmount1(oLow, oHigh, oAmount1);
            }
        }


        public BigInteger LiquidityFromAmountsTicks( BigInteger oTickActual, BigInteger oTickLow, BigInteger oTickHigh, BigInteger oAmount0, BigInteger oAmount1 )
        {
            BigInteger oSqrtActual  = PriceToSqrt( TickToPrice(oTickActual) );
            BigInteger oSqrtLow     = PriceToSqrt(TickToPrice(oTickLow));
            BigInteger oSqrtHigh    = PriceToSqrt(TickToPrice(oTickHigh));
            return LiquidityFromAmountsSqrt(oSqrtActual, oSqrtLow, oSqrtHigh, oAmount0, oAmount1);
        }


        public decimal BigToAmount0(BigInteger oAmount0)
        {
            double nResult = (double)oAmount0 / m_nPow0;;
            return (decimal)nResult;
        }

        public decimal BigToAmount1(BigInteger oAmount1)
        {
            double nResult = (double)oAmount1 / m_nPow1; ;
            return (decimal)nResult;
        }

        public BigInteger Amount0Big( decimal nAmount0 )
        {
            double nResult = Math.Floor((double)nAmount0 * m_nPow0);
            return new BigInteger(nResult);
        }

        public BigInteger Amount1Big(decimal nAmount1)
        {
            double nResult = Math.Floor((double)nAmount1 * m_nPow1);
            return new BigInteger(nResult);
        }


        /// <summary>
        /// Optimize pool range
        /// </summary>
        /// <param name="nAmount0"></param>
        /// <param name="nAmount1"></param>
        /// <param name="nPrice"></param>
        /// <param name="nMaxLossPercent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UniswapLiquidityData GetOptimalData( decimal nAmount0, decimal nAmount1, decimal nPrice, decimal nMinRange, decimal nMaxRange )
        {
            decimal nMultiplier = nMaxRange / 100.0M;
            decimal nPriceMin = nPrice * (1.0M - nMultiplier);
            decimal nPriceMax = nPrice * (1.0M + nMultiplier);

            BigInteger oTickActual = PriceToTick(nPrice);
            BigInteger oTickMax = PriceToTick(nPriceMax);
            BigInteger oTickMin = PriceToTick(nPriceMin);

            BigInteger oAmount0 = Amount0Big(nAmount0);
            BigInteger oAmount1 = Amount1Big(nAmount1);

            decimal nInitialAmount = (nAmount0 * nPrice) + nAmount1;


            UniswapLiquidityData? oBestData = null;

            for ( int nLow = (int)oTickMin; nLow < (int)oTickActual; nLow++ )
            {
                for( int nHigh = (int)oTickActual + 1; nHigh < (int)oTickMax; nHigh ++ )
                {
                    BigInteger oLow = new BigInteger(nLow);
                    BigInteger oHigh = new BigInteger(nHigh);   

                    BigInteger oLiquidity = LiquidityFromAmountsTicks(oTickActual, oLow, oHigh, oAmount0, oAmount1 );

                    // Profit and Los
                    BigInteger[] aAmountsInit = CalculateAmounts(oLiquidity, oTickActual, oLow, oHigh);
                    BigInteger[] aAmountsHigh = CalculateAmounts(oLiquidity, oHigh, oLow, oHigh);
                    BigInteger[] aAmountsLow  = CalculateAmounts(oLiquidity, oLow, oLow, oHigh);
                    decimal nPriceLow = TickToPrice(oLow);
                    decimal nPriceHigh = TickToPrice(oHigh);    
                    decimal nTotalHigh = BigToAmount0( aAmountsHigh[0] ) * nPriceHigh + BigToAmount1(aAmountsHigh[1] );
                    decimal nTotalLow  = BigToAmount0( aAmountsLow[0]  ) * nPriceLow  + BigToAmount1(aAmountsLow[1]);
                    decimal nTotalInit = BigToAmount0( aAmountsInit[0] ) * nPrice     + BigToAmount1(aAmountsInit[1]);



                    decimal nLoss   = nTotalInit - nTotalLow;
                    decimal nProfit = nTotalHigh - nTotalInit;

                    if (nLoss < 0 || nProfit < 0) continue;

                    decimal nMin = Math.Min(nLoss, nProfit);
                    decimal nMax = Math.Max(nLoss, nProfit);
                    decimal nPercent = (nMax - nMin) * 100.0M / nMin;

                    if(oBestData == null || nPercent < oBestData.LossProfitDifference )
                    {
                        oBestData = new UniswapLiquidityData()
                        {
                            Amount0 = oAmount0,
                            Amount1 = oAmount1,
                            Liquidity = oLiquidity,
                            TickUpper = oHigh,
                            TickLower = oLow,
                            MaxLoss = nLoss,
                            MaxProfit = nProfit,
                            LossProfitDifference = nPercent
                        };

                    }               

                }
            }

            return oBestData!;    
        }


        public decimal TickToPrice( BigInteger oTick )
        {
            return TickToPrice((int)oTick);
        }
        public decimal TickToPrice( int nTick )
        {
            double nPriceRaw = Math.Pow( POW_CONSTANT, nTick );
            nPriceRaw *= (m_nPow0 / m_nPow1);
            return (decimal)nPriceRaw;
        }

        public BigInteger PriceToTick( decimal nPrice, int nSpacing = 1 )
        {
            double nPriceRaw = (double)nPrice;
            nPriceRaw /= (m_nPow0 / m_nPow1);

            double nTick = Math.Log(nPriceRaw) / Math.Log(POW_CONSTANT);

            int nTickInt = (int)nTick;
            if( nSpacing > 1 )
            {
                int nMod = Math.Abs(nTickInt % nSpacing); 
                if( nMod != 0 )
                {
                    if( nMod < nSpacing / 2 )
                    {
                        nTickInt += nMod;    
                    }
                    else nTickInt -= (nSpacing - nMod);
                }
            }
            return new BigInteger(nTickInt);
            /*

            BigInteger oSqrt = PriceToSqrt(nPrice);

            // Math.floor(Math.log((sqrtPriceX96/Q96)**2)/Math.log(1.0001));
            double nQuotient = (double)oSqrt / (double)Q96;
            double nSquare = nQuotient * nQuotient;  
            double nLog1 = Math.Log(nQuotient); 
            double nLog2 = Math.Log(POW_CONSTANT);
            double nResult = Math.Floor(nLog1 / nLog2);
            return new BigInteger(nResult);
            */
        }

        public BigInteger PriceToSqrt(decimal nPrice)
        {
            double nPriceRaw = (double)nPrice * (m_nPow1 / m_nPow0);
            double nSqrt = Math.Sqrt((double)nPriceRaw);
            double nQ96 = (double)Q96;
            double nMult = nSqrt * nQ96;

            return new BigInteger(nMult);
        }


        public BigInteger[] CalculateAmounts(BigInteger oLiquidity, BigInteger nTickActual, BigInteger nTickLower, BigInteger nTickUpper)
        {

            double nRatioA = (double)Math.Sqrt(Math.Pow(POW_CONSTANT, (double)nTickLower));
            double nRatioB = (double)Math.Sqrt(Math.Pow(POW_CONSTANT, (double)nTickUpper));

            BigInteger nCurrentTick = nTickActual;

            double nNewSqrt = (double)Math.Sqrt(Math.Pow(POW_CONSTANT, (double)nTickActual));
            // double nNewSqrtQ96 = (double)oSlot0.SqrtPriceX96 / (double)Q96;

            double nAmount0 = 0;
            double nAmount1 = 0;

            double nLiquidity = (double)oLiquidity;
            if (nCurrentTick < nTickLower)
            {
                nAmount0 = Math.Floor(nLiquidity * ((nRatioB - nRatioA) / (nRatioA * nRatioB)));
            }
            else if (nCurrentTick >= nTickUpper)
            {
                nAmount1 = Math.Floor(nLiquidity * (nRatioB - nRatioA));
            }
            else // if (nCurrentTick >= tickLow && currentTick < tickHigh)
            {
                nAmount0 = Math.Floor(nLiquidity * ((nRatioB - nNewSqrt) / (nNewSqrt * nRatioB)));
                nAmount1 = Math.Floor(nLiquidity * (nNewSqrt - nRatioA));
            }
            return new BigInteger[] { new BigInteger(nAmount0), new BigInteger(nAmount1) };

        }
    }
}
