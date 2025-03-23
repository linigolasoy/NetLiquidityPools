using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesSymbolManager
    {
        public IFuturesSymbol? GetSymbol(string strSymbol);
        public string[] GetAllKeys();
        public IFuturesSymbol[] GetAllValues();
    }
}
