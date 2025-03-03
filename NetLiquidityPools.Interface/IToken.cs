using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    /// <summary>
    /// Token
    /// </summary>
    public interface IToken
    {
        public NetworkType NetworkType { get; } 
        public string Symbol { get; }
        public string Name { get; } 
        public string Address { get; }
        public int Decimals { get; }    
    }
}
