using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Bot
{

    public enum HedgeStatus
    {
        None,
        Hedge,
        Refresh,
        Liquidate,
        Error   

    }

    public interface IHedgedLiquidity
    {
        public ICexExchange Exchange { get; }
        public ICryptoSetup Setup { get; }
        public ICommonLogger Logger { get; }

        public ILeveragedPositionData? LastData { get; }

        public IFuturesPosition? Position { get; }
        public IFuturesSymbol? Symbol { get; }  
        public HedgeStatus Status { get; }  

        public decimal ProfitPool { get; }
        public decimal ProfitHedge { get; }
        public Task Refresh();
    }
}
