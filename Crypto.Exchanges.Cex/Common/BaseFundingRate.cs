using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal class BaseFundingRate: IFundingRate
    {
        public BaseFundingRate(IFuturesSymbol oSymbol, decimal nRate, DateTime oSettleDate)
        {
            Symbol = oSymbol;
            Rate = nRate;
            SettleDate = oSettleDate;
        }

        public IFuturesSymbol Symbol { get; }

        public decimal Rate { get; private set; }

        public DateTime SettleDate { get; private set; }

        public void Update(IFundingRate obj)
        {
            Rate = obj.Rate;
            SettleDate = obj.SettleDate;
        }

    }
}
