using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;

namespace NetLiquidityPools.Bot
{
    public class BotFactory
    {

        public static ILiquidityBot CreateBot(ICryptoSetup oSetup)
        {
            return new LeverageLiquidityBot(oSetup);    
        }
    }
}
