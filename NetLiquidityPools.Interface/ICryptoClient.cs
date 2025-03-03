using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{

    /// <summary>
    /// Crypto client
    /// </summary>
    public interface ICryptoClient
    {
        public ICryptoSetup Setup { get; }
        public ICryptoWallet Wallet { get; }

        public IToken[] Tokens { get; } 
    }
}
