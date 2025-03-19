using Nethereum.Contracts.Standards.ENS.ETHRegistrarController.ContractDefinition;
using Nethereum.Signer;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using System.Numerics;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class BaseNethereumTests
    {

        [TestMethod]
        public async Task UniswapPoolMathTest()
        {
            ICryptoClient oClient = TestCommon.CreateClient();


            IToken? oToken0 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "WETH");
            Assert.IsNotNull(oToken0);

            IToken? oToken1 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "USDC");
            Assert.IsNotNull(oToken1);

            ILiguidityPoolProvider oProvider = CommonDexFactory.CreatePoolProvider(oClient, LiquidityPoolType.UniswapV3);


            decimal nPrice = 2099;

            decimal nAmount0 = 3;
            decimal nAmount1 = 3000M;
            decimal nPercentMin = 5;
            decimal nPercentMax = 20;



            ILiquidityRangeCalculator oCalculator = oProvider.CreateRangeCalculator(oToken0, oToken1, nAmount0, nAmount1, nPrice, nPercentMin, nPercentMax);

            ILiquidityRangeResult? oResult = oCalculator.CalculateBest();

            UniswapPoolMath oMath = new UniswapPoolMath(oToken0, oToken1);
            BigInteger oTick = oMath.PriceToTick(nPrice);
            decimal nPriceTick = Math.Floor( oMath.TickToPrice(oTick) );
            Assert.AreEqual(nPrice, nPriceTick);


            var oOptimal = oMath.GetOptimalData(nAmount0, nAmount1, nPrice, nPercentMin, nPercentMax) ;
        }

        decimal[] CalculateProfitLoss(UniswapPoolMath oMath, decimal nAmountUsdc, decimal nPriceEth, decimal nPriceEthLow, decimal nPriceEthHigh )
        {
            decimal nAmount0 = nAmountUsdc / 2M / nPriceEth;
            decimal nAmount1 = nAmountUsdc / 2M;
            BigInteger oTickLow = oMath.PriceToTick(nPriceEthLow);
            BigInteger oTickHigh = oMath.PriceToTick(nPriceEthHigh);
            BigInteger oTickActual = oMath.PriceToTick(nPriceEth);

            BigInteger oAmount0 = oMath.Amount0Big(nAmount0);
            BigInteger oAmount1 = oMath.Amount1Big(nAmount1);
            BigInteger oLiquidity = oMath.LiquidityFromAmountsTicks(oTickActual, oTickLow, oTickHigh, oAmount0, oAmount1);

            BigInteger[] aAmountsInitial = oMath.CalculateAmounts(oLiquidity, oTickActual, oTickLow, oTickHigh);

            decimal nPriceHighLow = nPriceEth + (nPriceEth - nPriceEthLow);
            BigInteger oTickHighLow = oMath.PriceToTick(nPriceHighLow);
            BigInteger[] aAmountsHighLow = oMath.CalculateAmounts(oLiquidity, oTickHighLow, oTickLow, oTickHighLow);

            BigInteger[] aAmountsLow = oMath.CalculateAmounts(oLiquidity, oTickLow, oTickLow, oTickHigh);
            BigInteger[] aAmountsHigh = oMath.CalculateAmounts(oLiquidity, oTickHigh, oTickLow, oTickHigh);

            decimal nAmountInitial = oMath.BigToAmount0(aAmountsInitial[0]) * nPriceEth + oMath.BigToAmount1(aAmountsInitial[1]);
            decimal nAmountLow = oMath.BigToAmount0(aAmountsLow[0]) * nPriceEthLow + oMath.BigToAmount1(aAmountsLow[1]);
            decimal nAmountHigh = oMath.BigToAmount0(aAmountsHigh[0]) * nPriceEthHigh + oMath.BigToAmount1(aAmountsHigh[1]);

            // decimal nLoss = 
            return new decimal[] { nAmountInitial - nAmountLow, nAmountHigh - nAmountInitial };
        }

        decimal[] CalculateProfitLossExt(UniswapPoolMath oMath, decimal nAmountUsdc, 
            decimal nPriceEth, decimal nRangeDn, decimal nRangeUp, 
            decimal nRangeRebalance
            )
        {
            decimal nMultiplier = (nRangeUp + nRangeDn) / nRangeDn;
            decimal nMultiplierDn = nRangeDn / (nRangeUp + nRangeDn);
            decimal nMultiplierUp = nRangeUp / (nRangeUp + nRangeDn);

            decimal nAmountUsdcEth  = nAmountUsdc  * nMultiplierUp;
            decimal nAmountUsdcUsdc = nAmountUsdc * nMultiplierDn;
            decimal nAmount0 = nAmountUsdcEth / nPriceEth;
            decimal nAmount1 = nAmountUsdcUsdc;

            decimal nPriceEthLow = nPriceEth * (100M - nRangeDn) / 100M;
            decimal nPriceEthHigh = nPriceEth * (100M + nRangeUp) / 100M;
            BigInteger oTickLow = oMath.PriceToTick(nPriceEthLow);
            BigInteger oTickHigh = oMath.PriceToTick(nPriceEthHigh);
            BigInteger oTickActual = oMath.PriceToTick(nPriceEth);

            BigInteger oAmount0 = oMath.Amount0Big(nAmount0);
            BigInteger oAmount1 = oMath.Amount1Big(nAmount1);
            BigInteger oLiquidity = oMath.LiquidityFromAmountsTicks(oTickActual, oTickLow, oTickHigh, oAmount0, oAmount1);

             BigInteger[] aAmountsInitial = oMath.CalculateAmounts(oLiquidity, oTickActual, oTickLow, oTickHigh);

            decimal nPriceCloseDn = nPriceEth * (100M - nRangeRebalance) / 100M;
            decimal nPriceCloseUp = nPriceEth * (100M + nRangeRebalance) / 100M;
            BigInteger oTickCloseLow = oMath.PriceToTick(nPriceCloseDn);
            BigInteger oTickCloseHigh = oMath.PriceToTick(nPriceCloseUp);


            BigInteger[] aAmountsLow = oMath.CalculateAmounts(oLiquidity, oTickCloseLow, oTickLow, oTickHigh);
            BigInteger[] aAmountsHigh = oMath.CalculateAmounts(oLiquidity, oTickCloseHigh, oTickLow, oTickHigh);

            decimal nAmountInitial = oMath.BigToAmount0(aAmountsInitial[0]) * nPriceEth + oMath.BigToAmount1(aAmountsInitial[1]);
            decimal nAmountLow = oMath.BigToAmount0(aAmountsLow[0]) * nPriceCloseDn + oMath.BigToAmount1(aAmountsLow[1]);
            decimal nAmountHigh = oMath.BigToAmount0(aAmountsHigh[0]) * nPriceCloseUp + oMath.BigToAmount1(aAmountsHigh[1]);

            // decimal nLoss = 
            return new decimal[] { nAmountInitial - nAmountLow, nAmountHigh - nAmountInitial };
        }


        [TestMethod]
        public async Task UniswapPoolMathTest2()
        {
            ICryptoClient oClient = TestCommon.CreateClient();


            IToken? oToken0 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "WETH");
            Assert.IsNotNull(oToken0);

            IToken? oToken1 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "USDC");
            Assert.IsNotNull(oToken1);

            // ILiguidityPoolProvider oProvider = CommonDexFactory.CreatePoolProvider(oClient, LiquidityPoolType.UniswapV3);


            decimal nInitialAmount = 5000;
            decimal nLeverage = 4M;


            decimal nRangeUpBest = -1;
            decimal nRangeDnBest = -1;

            decimal nRangeTotal = 30;
            decimal nRangeMin = 4M;
            decimal nStep = 0.1M;

            decimal nPrice = 2000;
            UniswapPoolMath oMath = new UniswapPoolMath(oToken0, oToken1);
            decimal nPnlTotalAmount = nInitialAmount * nLeverage;

            decimal nBestRelation = 9E10M;

            for ( decimal nRangeDn = nRangeMin; nRangeDn < nRangeTotal; nRangeDn += nStep)
            {
                decimal nRangeUp = nRangeTotal - nRangeDn;
                if (nRangeUp <= 0) continue;
                decimal nPricelow = (100M - nRangeDn) * nPrice / 100M;
                decimal nPriceHigh = (100M + nRangeUp) * nPrice / 100M;

                decimal nRangeClose = Math.Min(nRangeDn, nRangeUp);

                decimal[] aPnl = CalculateProfitLossExt(oMath, nPnlTotalAmount, nPrice, nRangeDn, nRangeUp, nRangeClose);

                decimal nMinPnl = Math.Min(aPnl[0], aPnl[1]);
                decimal nMaxPnl = Math.Max(aPnl[0], aPnl[1]);

                decimal nRelation = nMinPnl / nMaxPnl;

                decimal nDiff = Math.Abs(1M - nRelation);
                if( nDiff < nBestRelation )
                {
                    nBestRelation = nDiff;  
                }


            }

            /// decimal nRangeUp = 13;
            // decimal nRangeDn = 6;





        }


        [TestMethod]
        public async Task PoolListTest()
        {
            ICryptoClient oClient = TestCommon.CreateClient();  
            ILiguidityPoolProvider oProvider = CommonDexFactory.CreatePoolProvider(oClient, LiquidityPoolType.UniswapV3);


            string strAddress = "0x0e1E7dc11aeF1e88d265c1145552936F46Ffa6ce";

            ILiquidityPool[]? aPools = await oProvider.GetPoolsOfAddress(strAddress);   
            Assert.IsNotNull(aPools);   
            Assert.IsTrue(aPools.Any());    

        }

        [TestMethod]
        public async Task BalancesTest()
        {
            ICryptoClient oClient = TestCommon.CreateClient();

            decimal? nBalance = await oClient.Wallet.GetBalance();
            Assert.IsNotNull(nBalance);
            Assert.IsTrue(nBalance.Value > 0 );


            IToken? oToken = oClient.Tokens.FirstOrDefault(p => p.Symbol == "USDC");
            Assert.IsNotNull(oToken);
            decimal? nBalanceUsdc = await oClient.Wallet.GetBalance(oToken);
            Assert.IsNotNull(nBalanceUsdc);
            Assert.IsTrue(nBalanceUsdc.Value >= 0);

            ICryptoTransaction? oSendUsdcResult = await oClient.Wallet.Send(oToken, 0.1M, "0x57F463e71730aad10c5F29Cbd4d55770EbA8e55c");
            Assert.IsNotNull(oSendUsdcResult);
            Assert.IsTrue(oSendUsdcResult.Success);

            ICryptoTransaction? oSendEthResult= await oClient.Wallet.Send(0.000001M, "0x57F463e71730aad10c5F29Cbd4d55770EbA8e55c");
            Assert.IsNotNull (oSendEthResult);  
            Assert.IsTrue(oSendEthResult.Success);


        }
    }
}