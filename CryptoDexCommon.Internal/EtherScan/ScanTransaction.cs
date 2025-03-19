using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.EtherScan
{
    internal class ScanTransaction: IScanTransaction
    {
        public ScanTransaction(TransactionListResponse oResponse) 
        { 
            OtherData = oResponse;
            BlockNumber = int.Parse(oResponse.BlockNumber!);

            long nTimestamp = long.Parse(oResponse.TimeStamp!); 

            DateTimeOffset oOffset = DateTimeOffset.FromUnixTimeSeconds(nTimestamp);
            DateTime = oOffset.DateTime.ToLocalTime();

            TxHash = oResponse.Hash!;
            FromAddress = oResponse.From;
            ToAddress = oResponse.To;

            Function = oResponse.FunctionName;
            FunctionId = oResponse.MethodId;
            if( oResponse.IsError == null )
            {
                Success = true;    
            }
            else
            {
                int nError = int.Parse(oResponse.IsError);
                Success = (nError == 0);
            }
        }

        public int BlockNumber { get; }

        public DateTime DateTime { get; }

        public string TxHash { get; }

        public string FromAddress { get; }

        public string? ToAddress { get; }
        public string? Function { get; }

        public string? FunctionId { get; }
        public bool Success { get; }

        public object? OtherData { get; }
    }
}
