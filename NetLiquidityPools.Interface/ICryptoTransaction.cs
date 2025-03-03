using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public interface ICryptoTransactionResult
    {
    }


    public interface ICryptoTransaction
    {
        public string TransactionId { get; }    
        public bool Success { get; }    
        public string? Message { get; } 
    }
}
