using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{


    internal class MintLiquidityParams
    {
        [Parameter("address", "token0", 1)]
        public string Token0 { get; set; } = string.Empty;
        [Parameter("address", "token1", 2)]
        public string Token1 { get; set; } = string.Empty;
        [Parameter("uint24", "fee", 3)]
        public BigInteger Fee { get; set; }

        [Parameter("int24", "tickLower", 4)]
        public BigInteger TickLower { get; set; }
        [Parameter("int24", "tickUpper", 5)]
        public BigInteger TickUpper { get; set; }

        [Parameter("uint256", "amount0Desired", 6)]
        public BigInteger Amount0Desired { get; set; }

        [Parameter("uint256", "amount1Desired", 7)]
        public BigInteger Amount1Desired { get; set; }
        [Parameter("uint256", "amount0Min", 8)]
        public BigInteger Amount0Min { get; set; } = BigInteger.Zero;

        [Parameter("uint256", "amount1Min", 9)]
        public BigInteger Amount1Min { get; set; } = BigInteger.Zero;
        [Parameter("address", "recipient", 10)]
        public string Recipient { get; set; } = string.Empty;
        [Parameter("uint256", "deadline", 11)]
        public BigInteger DeadLine { get; set; }

    }


    internal class MintInitParams
    {

        [Parameter("uint256", "tokenId", 1)]
        public BigInteger TokenId { get; set; } = BigInteger.Zero;

        [Parameter("address", "tokenToBorrow", 2)]
        public string TokenToBorrow { get; set; } = string.Empty;
        [Parameter("uint256", "amountToBorrow", 3)]
        public BigInteger AmountToBorrow { get; set; }
        [Parameter("address", "flashLoanProvider", 4)]
        public string FlashLoanProvider { get; set; } = string.Empty;
        [Parameter("address", "assetConverter", 5)]
        public string AssetConverter { get; set; } = string.Empty;
        [Parameter("address", "owner", 6)]
        public string Owner { get; set; } = string.Empty;
        [Parameter("uint256", "maxSwapSlippage", 7)]
        public BigInteger MaxSwapSlippage { get; set; } = new BigInteger(50);

    }

    [Function("mint", "bool")]
    internal class MintFunction : FunctionMessage
    {
        [Parameter("tuple", "mintParams", 1, "MintLiquidityParams")]
        public MintLiquidityParams LiquidityParams { get; set; } = new MintLiquidityParams();


        [Parameter("tuple", "initParams", 2, "MintInitParams")]
        public MintInitParams InitParams { get; set; } = new MintInitParams();  
        

    }
}
