using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface ICexBalance
    {
        public ICexExchange Exchange { get; }   
        public string Currency { get; }
        public decimal Balance { get; }
    }
}
