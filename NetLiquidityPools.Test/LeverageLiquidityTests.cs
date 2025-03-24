using Crypto.Exchanges.Cex;
using CryptoDexCommon.Internal;
using Nethereum.Contracts.Standards.ENS.ETHRegistrarController.ContractDefinition;
using Nethereum.Signer;
using NetLiquidityPools.Bot;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;
using NetLiquidityPools.Interface.Cex;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using System.Numerics;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class LeverageLiquidityTests
    {

        [TestMethod]
        public async Task MintTest()
        {
            ICryptoClient oClient = TestCommon.CreateClient();


            IToken? oToken0 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "WETH");
            Assert.IsNotNull(oToken0);

            IToken? oToken1 = oClient.Tokens.FirstOrDefault(p => p.Symbol == "USDC");
            Assert.IsNotNull(oToken1);


            ILeverageLiquidity oLeveraged = CommonDexFactory.CreateLeveragedLiquidity(oClient.Wallet, oClient.Setup);

            string? strContract = await oLeveraged.GetLastContractAddress();

            var oTxFees = await oLeveraged.CollectFees();
            Assert.IsNotNull(oTxFees);
            Assert.IsTrue(oTxFees.Success);


            var oPosData = await oLeveraged.GetPositionData();
            Assert.IsNotNull(oPosData);

            var oTxRebalance = await oLeveraged.Rebalance();
            Assert.IsNotNull(oTxRebalance);
            Assert.IsTrue(oTxRebalance.Success);




        }


        [TestMethod]
        public async Task HedgedLiquidityTest()
        {

            ICryptoSetup oSetup = TestCommon.CreateSetup();
            ICexExchange oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);
            CancellationTokenSource oSource = new CancellationTokenSource();    
            ICommonLogger oLogger = CryptoDexFactory.CreateLogger(oSetup, this.GetType().Name, oSource.Token);

            IHedgedLiquidity oHedged = BotFactory.CreateHedgedLiquidity(oExchange, oSetup, oLogger);

            for( int i = 0; i < 10; i++)
            {
                await oHedged.Refresh();

                await Task.Delay(2000);
            }

        }

    }
}