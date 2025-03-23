using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFundingRate
    {
        public IFuturesSymbol Symbol { get; }

        public decimal Rate { get; }

        public DateTime SettleDate { get; }

        public void Update(IFundingRate obj);
    }
}
