using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Pool
{

    [FunctionOutput]
    internal class Slot0Output : IFunctionOutputDTO
    {
        [Parameter("uint160", "sqrtPriceX96", 1)]
        public BigInteger SqrtPriceX96 { get; set; }
        [Parameter("int24", "tick", 2)]
        public BigInteger Tick { get; set; }
        [Parameter("uint16", "observationIndex", 3)]
        public BigInteger ObservationIndex { get; set; }
        [Parameter("uint16", "observationCardinality", 4)]
        public BigInteger ObservationCardinality { get; set; }
        [Parameter("uint16", "observationCardinalityNext", 5)]
        public BigInteger observationCardinalityNext { get; set; }
        [Parameter("uint8", "feeProtocol", 6)]
        public BigInteger FeeProtocol { get; set; }
        [Parameter("bool", "unlocked", 7)]
        public bool Unlocked { get; set; }
    }

    [Function("slot0", typeof(Slot0Output))]
    internal class Slot0Function : FunctionMessage
    {
    }

}
