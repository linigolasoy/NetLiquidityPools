using BingX.Net.Clients;
using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using CryptoClients.Net;
using CryptoClients.Net.Interfaces;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using Nethereum.Model;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxExchange : ICexExchange
    {

        private IExchangeRestClient m_oGlobalClient;
        public const int TASK_COUNT = 20;

        public BingxExchange(ICryptoSetup oSetup, ICommonLogger? oLogger)
        {
            Setup = oSetup;
            Logger = oLogger;
            /*
            IApiKey? oKeyFound = oSetup.ApiKeys.FirstOrDefault(p => p.ExchangeType == this.ExchangeType);
            if (oKeyFound == null) throw new Exception("No api key found");
            m_oApiKey = oKeyFound;
            */

            BingXRestClient.SetDefaultOptions(options =>
            {
                options.ApiCredentials = new ApiCredentials(Setup.LeverageSetup.HedgeApiKey, Setup.LeverageSetup.HedgeApiSecret);
            });
            m_oGlobalClient = new ExchangeRestClient();
            IFuturesSymbol[]? aSymbols = GetSymbols();
            if (aSymbols == null) throw new Exception("No symbols");
            SymbolManager = new FuturesSymbolManager(aSymbols);
            Trading = new BingxTrading(this);
            Account = new BingxAccount(this);
            Market = new BingxMarket(this);
        }
        public ICryptoSetup Setup { get; }

        internal IExchangeRestClient GlobalClient { get => m_oGlobalClient; }
        public ICommonLogger? Logger { get; }

        public ExchangeType ExchangeType { get=> ExchangeType.Bingx; }

        public IFuturesMarket Market { get; }

        public IFuturesTrading Trading { get; }

        public IFuturesAccount Account { get; }

        public IFuturesSymbolManager SymbolManager { get; }


        private IFuturesSymbol[]? GetSymbols()
        {
            Task<CryptoExchange.Net.Objects.WebCallResult<IEnumerable<BingXContract>>> oTask = m_oGlobalClient.BingX.PerpetualFuturesApi.ExchangeData.GetContractsAsync();
            oTask.Wait();

            var oResult = oTask.Result;
            if (oResult == null || !oResult.Success) return null;
            if (oResult.Data == null) return null;
            if (oResult.Data.Count() <= 0) return null;

            List<IFuturesSymbol> aResult = new List<IFuturesSymbol>();
            foreach (BingXContract oData in oResult.Data)
            {
                aResult.Add(new BingxSymbol(this, oData));
            }

            return aResult.ToArray();

        }

    }
}
