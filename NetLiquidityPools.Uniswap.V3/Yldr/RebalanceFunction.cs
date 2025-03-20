using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{
    internal class RebalanceTuple
    {
        [Parameter("address", "assetConverter", 1)]
        public string AssetConverter { get; set; } = string.Empty;
        [Parameter("uint256", "maxSwapSlippage", 2)]
        public BigInteger MaxSlippage { get; set; } = new BigInteger(50);
        [Parameter("int24", "newTickLower", 3)]
        public BigInteger NewTickLower { get; set; }
        [Parameter("int24", "newTickUpper", 3)]
        public BigInteger NewTickUpper { get; set; }
    }

    [Function("rebalance")]
    internal class RebalanceFunction : FunctionMessage
    {

        [Parameter("address", "flashloanProvider", 1)]
        public string FlashLoanProvider { get; set; } = string.Empty;

        [Parameter("tuple", "params", 2, "RebalanceTuple")]
        public RebalanceTuple Params { get; set; } = new RebalanceTuple();


    }
}
