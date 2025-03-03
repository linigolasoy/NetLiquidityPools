using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public interface ICryptoWallet
    {
        public ICryptoClient Client { get; }    
        public string Name { get; } 
        public string PublicKey { get; }

        public Task<decimal?> GetBalance();
        public Task<decimal?> GetBalance(IToken oToken);

        public Task<ICryptoTransaction?> Send(decimal nAmount, string strTo);
        public Task<ICryptoTransaction?> Send(IToken oToken, decimal nAmount, string strTo);

        public Task<ICryptoTransactionResult> Commit(ICryptoTransaction oTransaction);
    }
}
