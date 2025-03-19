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

            var oTx = await oLeveraged.CollectFees("0x708a2c5dd9670197a0acb8a6cd7efa1f52a605b2");
            Assert.IsNotNull(oTx);


            var oPosData = await oLeveraged.GetPositionData("0x708a2c5dd9670197a0acb8a6cd7efa1f52a605b2");
            Assert.IsNotNull(oPosData);

            ILeveragedContract[]? aContracts = await oLeveraged.GetContracts();   
            Assert.IsNotNull(aContracts);
            Assert.IsTrue(aContracts.Any());    
            decimal nAmount0 = 5M / 2000M ;
            decimal nAmount1 = 5;
            var oResult = await oLeveraged.CreatePool(oToken0, oToken1, 0.05M, nAmount0, nAmount1, 5, 3);
            Assert.IsNotNull(oResult);
            Assert.IsTrue(oResult.Success); 
        }

    }
}