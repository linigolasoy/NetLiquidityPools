using Nethereum.Signer;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class BaseNethereumTests
    {
        [TestMethod]
        public async Task ConnectBase()
        {
            ICryptoSetup oSetup = CommonDexFactory.CreateSetup();

            ILiguidityPoolProvider oProvider = CommonDexFactory.CreatePoolProvider(oSetup, LiquidityPoolType.UniswapV3);


            string strAddress = "0x0e1E7dc11aeF1e88d265c1145552936F46Ffa6ce";

            ILiquidityPool[]? aPools = await oProvider.GetPoolsOfAddress(strAddress);   
            Assert.IsNotNull(aPools);   
            Assert.IsTrue(aPools.Any());    

        }
    }
}