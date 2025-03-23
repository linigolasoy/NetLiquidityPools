using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesSymbol 
    {
        public string Symbol { get; }
        public string Base { get; }
        public string Quote { get; }

        public ICexExchange Exchange { get; }
        public int LeverageMax { get; }
        public int LeverageMin { get; }

        public decimal FeeMaker { get; }
        public decimal FeeTaker { get; }

        public int Decimals { get; }
        public decimal ContractSize { get; }
        public bool UseContractSize { get; }
        public int QuantityDecimals { get; }

        public decimal Minimum { get; }
    }
}
