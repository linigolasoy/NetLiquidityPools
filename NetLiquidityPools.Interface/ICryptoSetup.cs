using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public enum NetworkType
    {
        Arbitrum,
        Polygon
    }

    public interface ICryptoSetup
    {

        public string Web3Url { get; }  

        public NetworkType NetworkType { get; } 
    }
}
