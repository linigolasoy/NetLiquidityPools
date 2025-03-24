using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using NetLiquidityPools.Uniswap.V3.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{
    internal class LeveragePositionData : ILeveragedPositionData
    {
        public LeveragePositionData(IToken oToken0, IToken oToken1, GetPositionDataOutput oOutput, Slot0Output oSlot0, int nSpacing) 
        { 
            UniswapPoolMath oMath = new UniswapPoolMath(oToken0, oToken1);  
            Fee = oMath.BigToFee(oOutput.Fee);
            Token0 = oToken0;
            Token1 = oToken1;

            if ( oOutput.DebtAsset == oToken0.Address)
            {
                Debt0 = oMath.BigToAmount0(oOutput.Debt);
                Debt1 = 0;
            }
            else
            {
                Debt0 = 0;
                Debt1 = oMath.BigToAmount1(oOutput.Debt);
            }

            TickStep = nSpacing;

            PriceActual = oMath.TickToPrice(oSlot0.Tick);
            PriceLow = oMath.TickToPrice(oOutput.TickLower);
            PriceHigh = oMath.TickToPrice(oOutput.TickUpper);

            TickActual = oSlot0.Tick;   
            TickUpper = oOutput.TickUpper;
            TickLower = oOutput.TickLower;
            Liquidity = oOutput.Liquidity;

            Amount0 = oMath.BigToAmount0(oOutput.Amount0);
            Amount1 = oMath.BigToAmount1(oOutput.Amount1);

            Fees0 = oMath.BigToAmount0(oOutput.Fee0);
            Fees1 = oMath.BigToAmount1(oOutput.Fee1);
        }
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
        public BigInteger TickActual { get; }

        public BigInteger TickUpper { get; }
        public BigInteger TickLower { get; }
        public BigInteger Liquidity { get; }

        public int TickStep { get; }
    }
}
