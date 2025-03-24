using NetCryptoOptions;
using NetCryptoOptions.Interface;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Test
{
    internal class TestCommon
    {
        public static string[] SETUP_FILES      = new string[] { "d:/Data/NetLiquidityPools/NetLiquidityPoolsSetup.json", "d:/Data/NetLiquidityPools/NetLiquidityPoolsSetup_Raul.json" };
        public const string OPTIONS_FILE    = "d:/Data/NetLiquidityPools/OptionsSetup.json";


        public static ICryptoSetup CreateSetup( int nSetup = 0)
        {
            ICryptoSetup oSetup = CommonDexFactory.CreateSetup(TestCommon.SETUP_FILES[nSetup]);
            return oSetup;
        }
        public static ICryptoClient CreateClient()
        {
            ICryptoSetup oSetup = CreateSetup();    
            ICryptoClient oClient = CommonDexFactory.CreateClient(oSetup);
            return oClient;
        }
        public static IOptionExchange CreateOptionExchange()
        {
            IOptionSetup oSetup = OptionsFactory.CreateSetup(OPTIONS_FILE);
            IOptionExchange oExchange = OptionsFactory.CreateExchange(oSetup);  
            return oExchange;
        }
    }
}
