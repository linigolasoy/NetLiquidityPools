using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Bot
{
    public enum BotStatus
    {
        Start,
        CheckPool,
        CheckPosition,  
        Log,
        Fees,
        Rebalance,
        Position,
        Stop
    }

    public interface ILiquidityBot
    {

        public ICommonLogger Logger { get; }
        public ICryptoSetup Setup { get; }

        public Task<bool> Start();

        public BotStatus Status { get; }

        public Task Stop();
    }
}
