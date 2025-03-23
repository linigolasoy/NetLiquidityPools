using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxFuturesPosition: BaseFuturesPosition, IFuturesPosition
    {
        public BingxFuturesPosition(IFuturesSymbol oSymbol, BingXPosition oPosition) : base(oSymbol)
        {
            this.Direction = PositionDirection.Long;
            if( oPosition.Side == BingX.Net.Enums.TradeSide.Short) this.Direction = PositionDirection.Short;
            this.DateUpdated = (  oPosition.UpdateTime == null ? this.DateOpen : oPosition.UpdateTime.Value.ToLocalTime());
            this.Quantity = oPosition.Size;
            this.PriceAverage = oPosition.AveragePrice;
        }
    }
}
