using NetLiquidityPools.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Factory
{

    internal class SetupJson
    {
        [JsonProperty("Web3Url")]
        public string Url { get; set; } = string.Empty;
        [JsonProperty("NetworkType")]
        public string NetworkTypeString { get; set; } = string.Empty;
        [JsonProperty("TokenFile")]
        public string TokenFile { get; set; } = string.Empty;
        [JsonProperty("LogPath")]
        public string LogPath { get; set; } = string.Empty;
        [JsonProperty("Wallets")]
        public List<WalletJson>? Wallets { get; set; }
        [JsonProperty("LeverageData")]
        public LeverageJson? LeverageData { get; set; }
    }

    internal class LeverageJson
    {
        [JsonProperty("MintContract")]
        public string MintContract { get; set; } = string.Empty;

        [JsonProperty("LeverageContract")]
        public string LeverageContract { get; set; } = string.Empty;
        [JsonProperty("PercentDown")]
        public decimal PercentDown { get; set; } = 0;

        [JsonProperty("PercentUp")]
        public decimal PercentUp { get; set; } = 0;
        [JsonProperty("HedgeApiKey")]
        public string HedgeApiKey { get; set; } = string.Empty;
        [JsonProperty("HedgeApiSecret")]
        public string HedgeApiSecret { get; set; } = string.Empty;

    }
    internal class WalletJson
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("Address")]
        public string Address { get; set; } = string.Empty;
        [JsonProperty("PrivateData")]
        public string? PrivateData { get; set; } = null;
    }


    internal class CryptoSetup: ICryptoSetup
    {
        private CryptoSetup( NetworkType eType, SetupJson oJson) 
        { 
            Web3Url = oJson.Url;   
            NetworkType = eType;
            TokenFile = oJson.TokenFile;
            LogPath = oJson.LogPath;

            LeverageSetup = new LeverageSetup(oJson.LeverageData!);

            List<ICryptoWalletSetup> aWallets = new List<ICryptoWalletSetup>();
            if( oJson.Wallets != null && oJson.Wallets.Count > 0 )
            {
                foreach (var oJsonWallet in oJson.Wallets   )
                {
                    aWallets.Add(new CryptoWalletSetup(this, oJsonWallet.Name, oJsonWallet.Address, oJsonWallet.PrivateData));
                }
            }
            Wallets = aWallets.ToArray();   
        }

        public string Web3Url { get; }
        // public ILeverageSetupData LeverageSetupData { get; }
        public NetworkType NetworkType { get; }

        public ILeverageSetup LeverageSetup { get; }

        public string TokenFile { get; }
        public string LogPath { get; }

        public ICryptoWalletSetup[] Wallets {get; }

        public static ICryptoSetup? Load(string strFile)
        {
            string strContent = File.ReadAllText(strFile);  

            SetupJson? oJson = JsonConvert.DeserializeObject<SetupJson>(strContent);
            if (oJson == null) throw new Exception("Invalid Json setup");

            NetworkType? eType = Enum.GetValues<NetworkType>().FirstOrDefault(p=> p.ToString() == oJson.NetworkTypeString);
            if (eType == null) throw new Exception("Invalid network type on setup file");


            return new CryptoSetup(eType.Value, oJson);
        }

    }
}
