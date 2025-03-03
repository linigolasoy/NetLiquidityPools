using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Factory
{
    internal class CryptoWalletSetup : ICryptoWalletSetup
    {

        public CryptoWalletSetup( ICryptoSetup oSetup, string strName, string strAddress, string? strPrivateKey ) 
        {
            Setup = oSetup;
            Name = strName;
            Address = strAddress.Trim().ToUpper();
            PrivateKey = strPrivateKey;
        }
        public ICryptoSetup Setup { get; }

        public string Name { get; }

        public string Address { get; }

        public string? PrivateKey { get; }

        public override string ToString()
        {
            return $"{Name} ({Address})";
        }
    }
}
