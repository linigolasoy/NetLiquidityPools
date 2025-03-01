using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3;

namespace NetLiquidityPools.Factory
{

    internal class DummySetup : ICryptoSetup
    {
        public string Web3Url { get => "https://arb-mainnet.g.alchemy.com/v2/oZtC_Mn147DrNcRHHKGJd32IegkWB6Oo"; }

        public NetworkType NetworkType { get => NetworkType.Arbitrum; }
    }

    public class CommonDexFactory
    {
        public static ICryptoSetup CreateSetup()
        {
            return new DummySetup();
        }

        public static ILiguidityPoolProvider CreatePoolProvider(ICryptoSetup oSetup, LiquidityPoolType eType ) 
        { 
            switch( eType)
            {
                case LiquidityPoolType.UniswapV3:
                    return new UniswapV3PoolProvider(oSetup);
                default:
                    throw new NotImplementedException();    
            }
        }
    }
}
