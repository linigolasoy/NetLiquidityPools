using BingX.Net.Clients;
using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using CryptoExchange.Net.Objects.Sockets;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxWebsocketPublic : IWebsocketInternalPublic
    {
        private class BingxSubscription
        {
            public BingxSubscription(IFuturesSymbol oSymbol, int nId)
            {
                Symbol = oSymbol;
                Id = nId;
            }
            public IFuturesSymbol Symbol { get; }
            public int Id { get; }
        }

        private ConcurrentDictionary<string, BingxSubscription> m_aSubscribed = new ConcurrentDictionary<string, BingxSubscription>();    
        private BingXSocketClient? m_oSocketClient = null;

        private Task? m_oFundingTask = null;
        private CancellationTokenSource m_oTokenSource = new CancellationTokenSource(); 

        public BingxWebsocketPublic(IFuturesMarket oMarket)
        {
            Market = oMarket;
            FundingRateManager = new ManagerFundingRate(oMarket.Exchange);
            TickerManager = new ManagerTicker(oMarket.Exchange);    
        }   

        public IFuturesMarket Market { get; }

        public ISocketManager<IFundingRate> FundingRateManager { get;  }

        public ISocketManager<IFuturesTicker> TickerManager { get; }


        private async Task FundingRateLoop()
        {
            while (!m_oTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    IFundingRate[]? aRates = await Market.GetFundingRates();
                    if ( aRates != null && aRates.Length > 0 )
                    {
                        foreach( var oRate in aRates)
                        {
                            FundingRateManager.Put(oRate);
                        }
                    }
                }
                catch (Exception ex)
                {}
                await Task.Delay(2000);
            }
        }

        /// <summary>
        /// Start the socket    
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            if (m_oSocketClient != null) await Stop();
            m_oSocketClient = new BingXSocketClient();
            m_oTokenSource = new CancellationTokenSource();
            m_oFundingTask = FundingRateLoop(); 
            return true;
        }

        public async Task<bool> Stop()
        {
            if (m_oSocketClient == null) return true;

            m_oTokenSource.Cancel();
            await Task.Delay(1000);
            if (m_oFundingTask != null)
            {
                await m_oFundingTask;
                m_oFundingTask = null;
            }
            await m_oSocketClient.UnsubscribeAllAsync();
            await Task.Delay(1000);
            m_oSocketClient.Dispose();
            m_oSocketClient = null;
            return true;
        }

        public async Task<bool> Subscribe(IFuturesSymbol oSymbol)
        {
            if (m_aSubscribed.ContainsKey(oSymbol.Symbol)) return true;
            if (m_oSocketClient == null) return false;
            var oResultTicker = await m_oSocketClient.PerpetualFuturesApi.SubscribeToTickerUpdatesAsync(oSymbol.Symbol, OnTickerUpdate);
            if( oResultTicker == null || !oResultTicker.Success) return false;

            m_aSubscribed.TryAdd(oSymbol.Symbol, new BingxSubscription(oSymbol, oResultTicker.Data.Id));  
            return true;
        }

        private void OnTickerUpdate(DataEvent<BingXFuturesTickerUpdate> oEvent)
        {
            if( oEvent.Data == null) return;
            string? strSymbol = ( string.IsNullOrEmpty(oEvent.Data.Symbol) ? oEvent.Symbol : oEvent.Data.Symbol);
            if (strSymbol == null) return;
            IFuturesSymbol? oFound = Market.Exchange.SymbolManager.GetSymbol(strSymbol);
            if (oFound == null) return;

            IFuturesTicker oTicker = new BingxFuturesTicker(oFound, oEvent.Data, oEvent.ReceiveTime.ToLocalTime());
            TickerManager.Put(oTicker);
            return;
        }

        public async Task<bool> UnSubscribe(IFuturesSymbol oSymbol)
        {
            if (!m_aSubscribed.ContainsKey(oSymbol.Symbol)) return true;
            if (m_oSocketClient == null) return false;
            var oItem = m_aSubscribed[oSymbol.Symbol];  

            bool bUnsubscribed = await m_oSocketClient.PerpetualFuturesApi.UnsubscribeAsync(oItem.Id);
            if( !bUnsubscribed) return false;   

            m_aSubscribed.TryRemove(oSymbol.Symbol, out _); 

            return true;
        }
    }
}
