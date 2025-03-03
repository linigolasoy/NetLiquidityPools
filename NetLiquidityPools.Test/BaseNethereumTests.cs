using Nethereum.Contracts.Standards.ENS.ETHRegistrarController.ContractDefinition;
using Nethereum.Signer;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class BaseNethereumTests
    {
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