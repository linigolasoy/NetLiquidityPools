using Crypto.Exchanges.Cex.Bingx;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Cex;

namespace Crypto.Exchanges.Cex
{
    public class CexFactory
    {

        public static ICexExchange CreateExchange(ICryptoSetup oSetup, ExchangeType eType, ICommonLogger? oLogger = null)
        {
            switch( eType )
            {
                case ExchangeType.Bingx:
                    return new BingxExchange(oSetup, oLogger);
                default:
                    break;
            }
            throw new NotImplementedException();    
        }

    }
}
