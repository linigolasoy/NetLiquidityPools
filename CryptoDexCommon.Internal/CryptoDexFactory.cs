using NetLiquidityPools.Interface;

namespace CryptoDexCommon.Internal
{
    public class CryptoDexFactory
    {
        public static IToken? GetToken(ICryptoSetup oSetup, NetworkType eType, string strAddres)
        {
            return TokenManager.GetToken(oSetup, eType, strAddres);
        }
        public static IToken? GetCoreToken(ICryptoSetup oSetup, NetworkType eType)
        {
            return TokenManager.GetCoreToken(oSetup, eType);
        }


        public static ICryptoClient CreateClient( ICryptoSetup oSetup, int nWallet )
        {
            return new BaseWeb3Client( oSetup, nWallet );    
        }

        public static ICommonLogger CreateLogger( ICryptoSetup oSetup, string strFile, CancellationToken oToken )
        {
            return new CommonLogger(oSetup, strFile, oToken);

        }

    }
}
