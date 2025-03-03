using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.ERC20
{
    [Function("transfer", "bool")]
    internal class TransferFunction : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public string To { get; set; } = string.Empty;
        [Parameter("uint256", "value", 2)]
        public BigInteger Value { get; set; }
    }
}
