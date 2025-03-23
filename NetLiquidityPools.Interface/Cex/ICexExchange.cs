using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{

    public enum ExchangeType
    {
        Bingx
    }

    public interface ICexExchange
    {
        public ICryptoSetup Setup { get; }  
        public ICommonLogger? Logger { get; }
        public ExchangeType ExchangeType { get; }

        public IFuturesMarket Market { get; }
        public IFuturesTrading Trading { get; }
        public IFuturesAccount Account { get; }
        public IFuturesSymbolManager SymbolManager { get; }

    }
}
