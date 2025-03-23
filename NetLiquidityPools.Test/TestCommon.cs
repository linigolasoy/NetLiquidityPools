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
        public const string SETUP_FILE      = "d:/Data/NetLiquidityPools/NetLiquidityPoolsSetup.json";
        public const string OPTIONS_FILE    = "d:/Data/NetLiquidityPools/OptionsSetup.json";


        public static ICryptoSetup CreateSetup()
        {
            ICryptoSetup oSetup = CommonDexFactory.CreateSetup(TestCommon.SETUP_FILE);
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
