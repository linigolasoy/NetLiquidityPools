using CryptoDexCommon.Internal;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3;

namespace NetLiquidityPools.Factory
{


    public class CommonDexFactory
    {
        public static ICryptoSetup CreateSetup( string strFile )
        {
            ICryptoSetup? oSetup = CryptoSetup.Load(strFile);
            if (oSetup == null) throw new Exception("Could not find setup file");
            return oSetup;
        }


        public static ICryptoClient CreateClient( ICryptoSetup oSetup, int nAccount = 0 )
        {
            return CryptoDexFactory.CreateClient(oSetup, nAccount); 
        }


        public static ILiguidityPoolProvider CreatePoolProvider(ICryptoClient oClient, LiquidityPoolType eType ) 
        { 
            switch( eType)
            {
                case LiquidityPoolType.UniswapV3:
                    return new UniswapV3PoolProvider(oClient);
                default:
                    throw new NotImplementedException();    
            }
        }



    }
}
