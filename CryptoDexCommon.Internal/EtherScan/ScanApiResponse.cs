using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.EtherScan
{
    internal class ScanApiResponse<T>
    {

        public ScanApiResponse()
        {

        }

        [JsonProperty("status")]    
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public T? Result { get; set; }  


        public static ScanApiResponse<T>? Create( string strData )
        {
            return JsonConvert.DeserializeObject<ScanApiResponse<T>>(strData); 
        }
    }
}
