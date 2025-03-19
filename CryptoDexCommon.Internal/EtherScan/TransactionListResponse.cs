using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.EtherScan
{
    internal class TransactionListResponse
    {
        [JsonProperty("blockNumber")]    
        public string? BlockNumber { get; set; }
        [JsonProperty("blockHash")]
        public string? BlockHash { get; set; }
        [JsonProperty("timeStamp")]
        public string? TimeStamp { get; set; }
        [JsonProperty("hash")]
        public string? Hash { get; set; }

        [JsonProperty("from")]
        public string From { get; set; } = string.Empty;    
        [JsonProperty("to")]
        public string? To { get; set; }

        [JsonProperty("methodId")]
        public string? MethodId { get; set; }
        [JsonProperty("functionName")]
        public string? FunctionName { get; set; }
        [JsonProperty("isError")]
        public string? IsError { get; set; }


        /*
    {
      "blockHash": "0x3e555e31a406e8c2d584716fe88765d175511d5d18ce039b697db0c3e4cffc87",
      "nonce": "5619199",
      "transactionIndex": "1",
      "value": "2048640000000000",
      "gas": "112686",
      "gasPrice": "10000000",
      "gasPriceBid": "1200000000",
      "input": "0x",
      "contractAddress": "",
      "cumulativeGasUsed": "47299",
      "txreceipt_status": "1",
      "gasUsed": "47299",
      "confirmations": "5575243",
    },         */
    }
}
