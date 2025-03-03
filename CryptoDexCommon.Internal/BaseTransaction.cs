using Nethereum.RPC.Eth.DTOs;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal
{
    internal class BaseTransaction : ICryptoTransaction
    {

        public BaseTransaction( TransactionReceipt oReceipt ) 
        {
            TransactionId = oReceipt.TransactionHash;
            Success = oReceipt.Succeeded();
            if( !Success )
            {
                Message = oReceipt.ToString();  
            }
        }
        public string TransactionId { get; }

        public bool Success { get; }

        public string? Message { get; private set; } = null;
    }
}
