using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;
using NetLiquidityPools.Interface.Cex;

namespace NetLiquidityPools.Bot
{
    public class BotFactory
    {

        public static ILiquidityBot CreateBot(ICryptoSetup oSetup)
        {
            return new LeverageLiquidityBot(oSetup);    
        }


        public static IHedgedLiquidity CreateHedgedLiquidity(ICexExchange oExchange, ICryptoSetup oSetup, ICommonLogger oLogger)
        {
            return new HedgedLiquidity(oExchange, oSetup, oLogger);
        }
    }
}
