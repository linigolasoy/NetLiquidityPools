using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal
{
    internal class BaseWeb3Client : IWeb3Client
    {

        public BaseWeb3Client(ICryptoSetup oSetup, int nWallet ) 
        { 
            Setup = oSetup;
            Url = oSetup.Web3Url;
            Web3Client = new Web3(Url);

            Chain? oChain = null;
            switch( Setup.NetworkType )
            {
                case NetworkType.Arbitrum:
                    oChain = Chain.Arbitrum; break;
                case NetworkType.Ethereum:
                    oChain = Chain.MainNet; break;
                case NetworkType.Polygon:
                    oChain = Chain.Polygon; break;
                default:
                    break;
            }
            if (oChain == null) throw new Exception("No Chain");
            ICryptoWalletSetup[] aPrivates = Setup.Wallets.Where(p=> p.PrivateKey != null).ToArray();
            if( aPrivates.Length <= 0 ) throw new Exception("No private wallets");
            List<Account> aAccounts = new List<Account>();

            foreach( var oPrivate in aPrivates )
            {
                Account oAccountNew = new Account(oPrivate.PrivateKey!, (int)oChain);
                aAccounts.Add(oAccountNew);
            }
            if (aAccounts.Count <= 0) throw new Exception("No accounts");
            if( nWallet < 0 || nWallet >= aAccounts.Count ) throw new Exception("No wallet");
            Accounts = aAccounts.ToArray(); 
            Account oAccount = aAccounts[nWallet];
            string strName = aPrivates.First(p=> p.Address.ToUpper() == oAccount.Address.ToUpper()).Name;
            Wallet = new BaseWallet(this, strName, oAccount);

            AccountClient = new Web3(oAccount, Url);
            IToken[]? aTokens = TokenManager.GetAllTokens(Setup);
            if (aTokens == null) throw new Exception("No tokens");
            Tokens = aTokens;
        }
        public ICryptoSetup Setup { get; }

        public string Url { get; }

        public Web3 Web3Client { get; }

        public Web3 AccountClient { get; }

        public IToken[] Tokens { get; } 

        public Account[] Accounts { get; private set; } = Array.Empty<Account>();

        public ICryptoWallet Wallet { get; }
    }
}
