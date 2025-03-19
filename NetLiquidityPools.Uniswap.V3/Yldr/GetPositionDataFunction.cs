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


    [FunctionOutput]
    internal class GetPositionDataOutput: FunctionOutputDTO
    {
        [Parameter("uint256", "tokenId", 1)]
        public BigInteger TokenId { get; set; } = BigInteger.Zero;
        [Parameter("address", "token0", 2)]
        public string Token0 { get; set; } = string.Empty;
        [Parameter("address", "token1", 3)]
        public string Token1 { get; set; } = string.Empty;
        [Parameter("uint24", "fee", 4)]
        public BigInteger Fee { get; set; } = BigInteger.Zero;
        [Parameter("int24", "tickLower", 5)]
        public BigInteger TickLower { get; set; } = BigInteger.Zero;
        // [Parameter("int24", "tickCurrent", 6)]
        // public BigInteger TickCurrent { get; set; } = BigInteger.Zero;
        [Parameter("int24", "tickUpper", 6)]
        public BigInteger TickUpper { get; set; } = BigInteger.Zero;

        // [Parameter("uint160", "sqrtPriceX96", 7)]
        // public BigInteger SqrtPriceX96 { get; set; } = BigInteger.Zero;
        [Parameter("uint256", "amount0", 7)]
        public BigInteger Amount0 { get; set; } = BigInteger.Zero;
        [Parameter("uint256", "amount1", 8)]
        public BigInteger Amount1 { get; set; } = BigInteger.Zero;

        [Parameter("uint256", "fee0", 9)]
        public BigInteger Fee0 { get; set; } = BigInteger.Zero;
        [Parameter("uint256", "fee1", 10)]
        public BigInteger Fee1 { get; set; } = BigInteger.Zero;
        [Parameter("uint128", "liquidity", 11)]
        public BigInteger Liquidity { get; set; } = BigInteger.Zero;

        [Parameter("address", "debtAsset", 12)]
        public string DebtAsset { get; set; } = string.Empty;
        [Parameter("uint256", "debt", 13)]
        public BigInteger Debt { get; set; } = BigInteger.Zero; 
    }


    [Function("getPositionData", typeof(GetPositionDataOutput))]
    internal class GetPositionDataFunction : FunctionMessage
    {
        [Parameter("address", "position", 1)]
        public string Address { get; set; } = string.Empty;

    }
}
