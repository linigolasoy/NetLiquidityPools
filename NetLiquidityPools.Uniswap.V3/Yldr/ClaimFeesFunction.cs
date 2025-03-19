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

    internal class ClaimFeesTuple
    {
        [Parameter("address", "assetConverter", 1)]
        public string AssetConverter { get; set; } = string.Empty;
        [Parameter("uint256", "maxSwapSlippage", 2)]
        public BigInteger MaxSlippage { get; set; } = new BigInteger(50);
        [Parameter("bool", "withdrawFees", 3)]
        public bool WithdrawFees { get; set; } = true;
    }

    [Function("claimFees")]
    internal class ClaimFeesFunction : FunctionMessage
    {

        [Parameter("address", "flashloanProvider", 1)]
        public string FlashLoanProvider { get; set; } = string.Empty;

        [Parameter("tuple", "params", 2, "MintLiquidityParams")]
        public ClaimFeesTuple Params { get; set; } = new ClaimFeesTuple();


    }
}
