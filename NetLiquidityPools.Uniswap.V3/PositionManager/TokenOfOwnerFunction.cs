using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.PositionManager
{
    [Function("tokenOfOwnerByIndex", "uint256")]
    internal class TokenOfOwnerFunction: FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; } = string.Empty;

        [Parameter("uint256", "_index", 2)]
        public BigInteger Index { get; set; }

    }
}
