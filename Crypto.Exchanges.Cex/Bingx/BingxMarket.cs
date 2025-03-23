using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxMarket : IFuturesMarket
    {
        private BingxExchange m_oExchange;
        private IWebsocketInternalPublic? m_oWebsocket = null;
        public BingxMarket(ICexExchange oExchange)
        {
            m_oExchange = (BingxExchange)oExchange;
        }
        public ICexExchange Exchange { get=> m_oExchange; }

        public IFuturesWebsocketPublic? Websocket { get => m_oWebsocket; }


        /// <summary>
        /// Get funding rates   
        /// </summary>
        /// <returns></returns>
        public async Task<IFundingRate[]?> GetFundingRates()
        {
            var oResult = await m_oExchange.GlobalClient.BingX.PerpetualFuturesApi.ExchangeData.GetFundingRatesAsync();
            if (oResult == null || !oResult.Success) return null;
            if (oResult.Data == null) return null;

            List<IFundingRate> aResult = new List<IFundingRate>();
            foreach (BingXFundingRate oData in oResult.Data)
            {
                if (oData == null) continue;
                IFuturesSymbol? oFound = Exchange.SymbolManager.GetSymbol(oData.Symbol);    
                if (oFound == null) continue;
                aResult.Add(new BaseFundingRate(oFound, oData.LastFundingRate, oData.NextFundingTime.ToLocalTime()));

            }
            return aResult.ToArray();
        }


        /// <summary>
        /// Get tickers 
        /// </summary>
        /// <returns></returns>
        public async Task<IFuturesTicker[]?> GetTickers()
        {
            var oResult = await m_oExchange.GlobalClient.BingX.PerpetualFuturesApi.ExchangeData.GetTickersAsync();
            if (oResult == null || !oResult.Success) return null;
            if (oResult.Data == null) return null;

            List<IFuturesTicker> aResult = new List<IFuturesTicker>();
            foreach (BingXFuturesTicker oData in oResult.Data)
            {
                if (oData == null) continue;
                IFuturesSymbol? oFound = Exchange.SymbolManager.GetSymbol(oData.Symbol);
                if (oFound == null) continue;
                aResult.Add(new BingxFuturesTicker(oFound, oData));

            }
            return aResult.ToArray();
        }

        /// <summary>
        /// Starts websockets
        /// </summary>
        /// <returns></returns>
        public async Task<bool> EndSockets()
        {
            if (m_oWebsocket == null) return true;
            await m_oWebsocket.Stop();
            m_oWebsocket = null;
            return true;
        }

        /// <summary>
        /// End websockets  
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> StartSockets()
        {
            if (m_oWebsocket != null) return true;
            m_oWebsocket = new BingxWebsocketPublic(this);
            bool bResult = await m_oWebsocket.Start();
            if (!bResult) return false;
            IFuturesSymbol oSymbol = Exchange.SymbolManager.GetAllValues().First(p=> p.Quote == "USDT" && p.Base == "BTC");
            bResult = await m_oWebsocket.Subscribe(oSymbol);  
            if( !bResult ) await this.EndSockets(); 
            return bResult;
        }
    }
}
