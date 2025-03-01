using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace NetLiquidityPools.Uniswap.V3.PositionManager
{
    [Function("balanceOf", "uint256")]
    internal class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; } = string.Empty;   
    }
}
