using NetLiquidityPools.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal
{

    internal class TokenJson
    {
        [JsonProperty("Symbol")] 
        public string Symbol { get; set; } = string.Empty;
        [JsonProperty("Name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("Decimals")]
        public int Decimals { get; set; } = 18;
        [JsonProperty("Addresses")]
        public Dictionary<string, string> Addresses { get; set; } = new Dictionary<string, string>();

    }
    internal class TokenManager
    {

        private static Dictionary<NetworkType, IToken[]>? m_aTokens = null;


        private static void CreateTokens(ICryptoSetup oSetup)
        {
            if (m_aTokens != null) return;
            string strContent = File.ReadAllText(oSetup.TokenFile);
            if (strContent == null) throw new Exception("Could not find token file");
            List<TokenJson>? aFound = null;

            aFound = JsonConvert.DeserializeObject<List<TokenJson>>(strContent);
            if( aFound == null ) throw new Exception("Could not deserialize token file");

            Dictionary<NetworkType, List<IToken>> aCreate = new Dictionary<NetworkType, List<IToken>>(); 
            foreach( var oJson in aFound )
            {
                foreach( var oValue in oJson.Addresses )
                {
                    NetworkType? eType = Enum.GetValues<NetworkType>().FirstOrDefault(p=> p.ToString() == oValue.Key);
                    if( eType == null ) continue;
                    if (!aCreate.ContainsKey(eType.Value)) aCreate.Add(eType.Value, new List<IToken>());
                    aCreate[eType.Value].Add(new BaseToken(eType.Value, oJson.Symbol, oJson.Name, oValue.Value, oJson.Decimals));
                }
            }

            m_aTokens = new Dictionary<NetworkType, IToken[]>();
            foreach( var oCreate in aCreate )
            {
                m_aTokens.Add(oCreate.Key, oCreate.Value.ToArray());
            }
        }
        internal static IToken? GetToken(ICryptoSetup oSetup, NetworkType eType, string strAddres)
        {
            CreateTokens(oSetup);
            if (m_aTokens == null) return null;
            if (!m_aTokens.ContainsKey(eType)) return null;
            string strAddressUpper = strAddres.ToUpper();   
            IToken? oResult = m_aTokens[eType].FirstOrDefault(p => p.Address.ToUpper() == strAddressUpper);
            return oResult;
        }


        internal static IToken? GetCoreToken(ICryptoSetup oSetup, NetworkType eType)
        {
            CreateTokens(oSetup);
            if (m_aTokens == null) return null;
            if (!m_aTokens.ContainsKey(eType)) return null;
            IToken? oResult = m_aTokens[eType].FirstOrDefault(p => string.IsNullOrEmpty(p.Address.Trim()));
            return oResult;

        }


        internal static IToken[]? GetAllTokens(ICryptoSetup oSetup) 
        {
            CreateTokens(oSetup);
            if (m_aTokens == null) return null;
            if (!m_aTokens.ContainsKey(oSetup.NetworkType)) return null;

            IToken[] aResult = m_aTokens[oSetup.NetworkType].ToArray(); 
            return aResult;
        }
    }
}
