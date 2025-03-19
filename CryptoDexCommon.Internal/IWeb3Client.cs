using CryptoDexCommon.Internal.EtherScan;
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



    public interface IWeb3Client: ICryptoClient
    {
        public string Url { get; }

        public Web3 Web3Client { get; }

        public Web3 AccountClient { get; }  
        public Account[] Accounts { get; }

        public Task<IScanTransaction[]?> GetTransactions(string strAddress);
    }
}
