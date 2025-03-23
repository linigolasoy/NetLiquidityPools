using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal class BaseFuturesPosition
    {
        public BaseFuturesPosition(IFuturesSymbol oSymbol )
        {
            Symbol = oSymbol;  
        }
        public IFuturesSymbol Symbol { get; }
        public PositionDirection Direction { get; internal set; }
        public DateTime DateOpen { get; internal set; } = DateTime.Now;
        public DateTime DateUpdated { get; internal set; }

        public decimal Quantity { get; internal set; }
        public decimal PriceAverage { get; internal set; }
        public bool Closed { get; internal set; }

    }
}
