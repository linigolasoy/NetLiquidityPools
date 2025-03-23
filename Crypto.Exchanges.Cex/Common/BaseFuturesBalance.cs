using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal class BaseFuturesBalance
    {
        public BaseFuturesBalance(ICexExchange oExchange, string sCurrency)
        {
            Exchange = oExchange;
            Currency = sCurrency;
        }

        public ICexExchange Exchange { get; }
        public string Currency { get; }
        public decimal Balance { get; internal set; } = 0;
        public decimal UnrealizedProfit { get; internal set; } = 0;
        public decimal Equity { get; internal set; } = 0;
        public decimal MarginUsed { get; internal set; } = 0;
        public decimal MarginFrozen { get; internal set; } = 0;

    }
}
