using CryptoDexCommon.Internal;
using Nethereum.ABI;
using Nethereum.Util;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Pool;
using NetLiquidityPools.Uniswap.V3.PositionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Liquidity
{
    internal class UniswapLiquidityPool : ILiquidityPool
    {
        private const double TICK_CONSTANT = 1.0001;
        private static BigInteger Q96 = BigInteger.Pow(2, 96);

        private UniswapNonFungibleManager m_oManager;
        // private PositionsOutput? m_oOutput;
        private UniswapV3PoolProvider m_oProvider;

        private double m_nPower0;
        private double m_nPower1;
        private UniswapLiquidityPool(UniswapV3PoolProvider oProvider, BigInteger nId, IToken oToken0, IToken oToken1, decimal nFee)
        {
            m_oProvider = oProvider;
            Id = nId;
            m_oManager = new UniswapNonFungibleManager((IWeb3Client)oProvider.Client);
            Token0 = oToken0;
            Token1 = oToken1;
            Fee = nFee;

            m_nPower0 = Math.Pow(10, Token0.Decimals);
            m_nPower1 = Math.Pow(10, Token1.Decimals);

        }
        public ILiguidityPoolProvider Provider { get => m_oProvider; }

        public BigInteger Id { get; }

        public IToken Token0 { get; }
        public IToken Token1 { get; }
        public decimal Fee { get; private set; } = 0;
        public decimal Fees0 { get; private set; } = 0;
        public decimal Fees1 { get; private set; } = 0;
        public decimal Amount0 { get; private set; } = 0;
        public decimal Amount1 { get; private set; } = 0;

        private BigInteger GetTickAtSqrtPrice(BigInteger oSqrtPriceX96)
        {
            double nSqrtPriceX96 = (double)oSqrtPriceX96;
            double nQ96 = (double)Q96;

            double nDiv1 = nSqrtPriceX96 / nQ96;
            double nPow1 = Math.Pow(nDiv1, 2);

            double nLog1 = Math.Log(nPow1);
            double nLog2 = Math.Log(TICK_CONSTANT);

            double nRes = nLog1 / nLog2;    
            // let tick = Math.floor(
            //     Math.log(
            //      (sqrtPriceX96/Q96)
            //      **2
            //     )
            //     /
            //     Math.log(1.0001));
            // return (BigInteger)Math.Floor(Math.Log(Math.Pow((double)oSqrtPriceX96 / Q96, 2) / Math.Log(TICK_CONSTANT)));
            BigInteger oRes = new BigInteger(Math.Floor(nRes));
            return oRes;

        }
        private void CalculateAmounts(PositionsOutput oPos, Slot0Output oSlot0)
        {
            double nRatioA = (double)Math.Sqrt(Math.Pow(TICK_CONSTANT, (double)oPos.TickLower));
            double nRatioB = (double)Math.Sqrt(Math.Pow(TICK_CONSTANT, (double)oPos.TickUpper));
            
            BigInteger nCurrentTick = GetTickAtSqrtPrice(oSlot0.SqrtPriceX96);

            double nNewSqrtQ96 = (double)oSlot0.SqrtPriceX96 / (double)Q96;

            double nAmount0 = 0;
            double nAmount1 = 0;

            double nLiquidity = (double)oPos.Liquidity;
            if (nCurrentTick < oPos.TickLower)
            {
                nAmount0 = Math.Floor(nLiquidity * ((nRatioB - nRatioA) / (nRatioA * nRatioB)));
            }
            else if (nCurrentTick >= oPos.TickUpper)
            {
                nAmount1 = Math.Floor(nLiquidity * (nRatioB - nRatioA));
            }
            else // if (nCurrentTick >= tickLow && currentTick < tickHigh)
            {
                nAmount0 = Math.Floor(nLiquidity * ((nRatioB - nNewSqrtQ96) / (nNewSqrtQ96 * nRatioB)));
                nAmount1 = Math.Floor(nLiquidity * (nNewSqrtQ96 - nRatioA));
            }


            Amount0 = (decimal)( nAmount0 / m_nPower0 );
            Amount1 = (decimal)(nAmount1 / m_nPower1);
        }



        internal static async Task<ILiquidityPool?> Create(UniswapV3PoolProvider oProvider, BigInteger nId)
        {

            var oManager = new UniswapNonFungibleManager((IWeb3Client)(oProvider.Client));

            var oPosOutput = await oManager.Positions(nId);

            if (oPosOutput == null) return null;
            decimal nFee = (decimal)oPosOutput.Fee / 10000;


            UniswapV3Factory oFactory = new UniswapV3Factory((IWeb3Client)(oProvider.Client), oProvider.FactoryAddress);

            UniswapPool oPool = await oFactory.GetPool(oPosOutput.Token0, oPosOutput.Token1, oPosOutput.Fee);

            IToken? oToken0 = CryptoDexFactory.GetToken(oProvider.Client.Setup, oProvider.Client.Setup.NetworkType, oPool.Token0);
            if (oToken0 == null) return null;
            IToken? oToken1 = CryptoDexFactory.GetToken(oProvider.Client.Setup, oProvider.Client.Setup.NetworkType, oPool.Token1);
            if( oToken1 == null) return null;

            return new UniswapLiquidityPool(oProvider, nId, oToken0, oToken1, nFee);

        }

        public async Task Refresh()
        {
            var oPosOutput = await m_oManager.Positions(Id);

            if (oPosOutput == null) return;
            Fee = (decimal)oPosOutput.Fee / 10000;

            Fees0 = UnitConversion.Convert.FromWei(oPosOutput.TokensOwed0, 18);
            Fees1 = UnitConversion.Convert.FromWei(oPosOutput.TokensOwed1, 6);


            UniswapV3Factory oFactory = new UniswapV3Factory((IWeb3Client)m_oProvider.Client, m_oProvider.FactoryAddress);
            UniswapPool oPool = await oFactory.GetPool(oPosOutput.Token0, oPosOutput.Token1, oPosOutput.Fee);


            var oSlot0 = await oPool.GetSlot0(m_oProvider.Client.Setup);


            CalculateAmounts(oPosOutput, oSlot0);   

        }


        public override string ToString()
        {
            return $"{Token0.Symbol}/{Token1.Symbol} ({Fee.ToString()} %)";
        }

    }
}
