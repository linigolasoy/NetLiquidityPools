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
        public const string SETUP_FILE = "d:/Data/NetLiquidityPools/NetLiquidityPoolsSetup.json";


        public static ICryptoClient CreateClient()
        {
            ICryptoSetup oSetup = CommonDexFactory.CreateSetup(TestCommon.SETUP_FILE);
            ICryptoClient oClient = CommonDexFactory.CreateClient(oSetup);
            return oClient;
        }
    }
}
