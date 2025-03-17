using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Options
{
    public interface IOption
    {
        public IOptionExchange Exchange { get; }    
        public IOptionCurrency Currency { get; }
    }
}
