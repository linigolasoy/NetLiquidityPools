using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.EtherScan
{
    public interface IScanTransaction
    {
        public int BlockNumber { get; } 
        public DateTime DateTime { get; }

        public string TxHash {  get; }    

        public string FromAddress { get; }  
        public string? ToAddress { get; }

        public string? Function { get; }    

        public string? FunctionId { get; }
        public bool Success { get; }    

        public object? OtherData { get; }    
    }
}
