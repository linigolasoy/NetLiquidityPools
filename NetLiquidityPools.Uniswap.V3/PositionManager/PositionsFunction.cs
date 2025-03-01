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

    [FunctionOutput]
    internal class PositionsOutput : IFunctionOutputDTO
    {
        [Parameter("uint96", "nonce", 1)]
        public BigInteger Nonce { get; set; }
        [Parameter("address", "address", 2)]
        public string Address { get; set; } = string.Empty;
        [Parameter("address", "token0", 3)]
        public string Token0 { get; set; } = string.Empty;
        [Parameter("address", "token1", 4)]
        public string Token1 { get; set; } = string.Empty;
        [Parameter("uint24", "fee", 5)]
        public BigInteger Fee { get; set; }
        [Parameter("int24", "tickLower", 6)]
        public BigInteger TickLower { get; set; }
        [Parameter("int24", "tickUpper", 7)]
        public BigInteger TickUpper { get; set; }
        [Parameter("uint128", "liquidity", 8)]
        public BigInteger Liquidity { get; set; }
        [Parameter("uint256", "feeGrowthInside0LastX128", 9)]
        public BigInteger FeeGrowthInside0LastX128 { get; set; }
        [Parameter("uint256", "feeGrowthInside1LastX128", 10)]
        public BigInteger FeeGrowthInside1LastX128 { get; set; }
        [Parameter("uint128", "tokensOwed0", 11)]
        public BigInteger TokensOwed0 { get; set; }
        [Parameter("uint128", "tokensOwed1", 12)]
        public BigInteger TokensOwed1 { get; set; }
    }

    [Function("positions", typeof(PositionsOutput))]
    internal class PositionsFunction: FunctionMessage
    {
        [Parameter("uint256", "tokenId", 1)]
        public BigInteger TokenId { get; set; }
    }
}
