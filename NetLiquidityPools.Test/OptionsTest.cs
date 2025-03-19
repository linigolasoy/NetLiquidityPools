using NetCryptoOptions.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public async Task BasicMarketData()
        {
            IOptionExchange oExchange = TestCommon.CreateOptionExchange();

            IOptionCurrency[]? aCurrencies = await oExchange.GetCurrencies();
            Assert.IsNotNull(aCurrencies);
            Assert.IsTrue(aCurrencies.Length > 0);
        }

    }
}
