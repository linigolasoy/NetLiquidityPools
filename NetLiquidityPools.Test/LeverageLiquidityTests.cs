using Nethereum.Contracts.Standards.ENS.ETHRegistrarController.ContractDefinition;
using Nethereum.Signer;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
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

            var oTxFees = await oLeveraged.CollectFees();
            Assert.IsNotNull(oTxFees);
            Assert.IsTrue(oTxFees.Success);


            var oPosData = await oLeveraged.GetPositionData();
            Assert.IsNotNull(oPosData);

            var oTxRebalance = await oLeveraged.Rebalance();
            Assert.IsNotNull(oTxRebalance);
            Assert.IsTrue(oTxRebalance.Success);




        }

    }
}