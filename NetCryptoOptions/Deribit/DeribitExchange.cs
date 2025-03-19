using NetCryptoOptions.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCryptoOptions.Deribit
{
    internal class DeribitExchange : IOptionExchange
    {

        private bool m_bTest;

        private const string BASE_TEST_URL = "https://test.deribit.com/api/v2";
        private const string BASE_PROD_URL = "https://www.deribit.com/api/v2";


        private const string ENDP_CURRENCIES = "/public/get_currencies";

        private string m_strUrl = BASE_PROD_URL;
        public DeribitExchange( IOptionSetup oSetup, bool bTest ) 
        { 
            Setup = oSetup;
            m_bTest = bTest;    
            if( bTest ) m_strUrl = BASE_TEST_URL;  
        }
        public IOptionSetup Setup { get; }

        public async Task<IOptionCurrency[]?> GetCurrencies()
        {
            HttpClient oClient = new HttpClient();

            string strUrl = $"{m_strUrl}{ENDP_CURRENCIES}";
            var oResult = await oClient.GetAsync(strUrl);
            if (oResult == null || !oResult.IsSuccessStatusCode) return null;

            string? strResponse = await oResult.Content.ReadAsStringAsync();
            if( strResponse == null ) return null;  

            throw new NotImplementedException();
        }

        public async Task<IOption[]?> GetOptions(IOptionCurrency? oCurrency = null)
        {
            throw new NotImplementedException();
        }
    }
}
