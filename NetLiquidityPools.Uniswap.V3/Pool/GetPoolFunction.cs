using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3.Pool
{
    [Function("getPool", "address")]
    internal class GetPoolFunction : FunctionMessage
    {
        [Parameter("address", "_token0", 1)]
        public string Token0 { get; set; } = string.Empty;
        [Parameter("address", "_token1", 2)]
        public string Token1 { get; set; } = string.Empty;
        [Parameter("uint24", "_fee", 3)]
        public BigInteger Fee { get; set; }
    }
}
