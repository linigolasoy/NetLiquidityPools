using BingX.Net.Objects.Models;
using CryptoClients.Net;
using CryptoClients.Net.Interfaces;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxAccount : IFuturesAccount
    {
        private IExchangeRestClient m_oGlobalClient;
        public BingxAccount(ICexExchange oExchange)
        {
            Exchange = oExchange;
            m_oGlobalClient = new ExchangeRestClient();
        }
        public ICexExchange Exchange {get; }

        public IFuturesWebsocketPrivate? Websocket { get; }


        /// <summary>
        /// Balances
        /// </summary>
        public async Task<IFuturesBalance[]?> GetBalances()
        {

            var oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Account.GetBalancesAsync();
            if (oResult == null || !oResult.Success ) return null;
            if (oResult.Data == null) return null;
            List<IFuturesBalance> aResult = new  List<IFuturesBalance>();
            foreach (var oBalance in oResult.Data)
            {
                aResult.Add(new BingxFuturesBalance(this.Exchange, oBalance.Asset, oBalance));
            }
            return aResult.ToArray();   
        }

        public async Task<ICexBalance[]?> GetBalancesSpot()
        {

            var oResult = await m_oGlobalClient.BingX.SpotApi.Account.GetBalancesAsync();
            if (oResult == null || !oResult.Success) return null;
            if (oResult.Data == null) return null;
            List<ICexBalance> aResult = new List<ICexBalance>();
            foreach (var oBalance in oResult.Data)
            {
                aResult.Add(new BingxSpotBalance(this.Exchange, oBalance));
            }
            return aResult.ToArray();
        }

        /// <summary>
        /// Transfer to spot    
        /// </summary>
        /// <param name="strCurrency"></param>
        /// <param name="nAmount"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> TransferToSpot(string strCurrency, decimal nAmount)
        {
            var oResult = await m_oGlobalClient.BingX.SpotApi.Account.TransferAsync(BingX.Net.Enums.TransferType.PerpetualFuturesToFunding, strCurrency, nAmount);
            if( oResult == null || !oResult.Success) return false;  
            if( oResult.Data == null ) return false;
            return true;
        }

        /// <summary>
        /// Transfer from spot to futures
        /// </summary>
        /// <param name="strCurrency"></param>
        /// <param name="nAmount"></param>
        /// <returns></returns>
        public async Task<bool> TransferFromSpot(string strCurrency, decimal nAmount)
        {
            var oResult = await m_oGlobalClient.BingX.SpotApi.Account.TransferAsync(BingX.Net.Enums.TransferType.FundingToPerpetualFutures, strCurrency, nAmount);
            if (oResult == null || !oResult.Success) return false;
            if (oResult.Data == null) return false;
            return true;
        }

        public async Task<string?> GetDepositAddres(string strCurrency, NetworkType eType)
        {
            var oResult = await m_oGlobalClient.BingX.SpotApi.Account.GetDepositAddressAsync(strCurrency);
            if (oResult == null || !oResult.Success || oResult.Data == null) return null;

            string strTypeUpper = eType.ToString().ToUpper();   
            var oFound = oResult.Data.Data.FirstOrDefault(p => p.Network.ToUpper() == strTypeUpper); 
            if( oFound == null) return null;
            return oFound.Address;
        }

        public async Task<bool> WithDraw(string strCurrency, NetworkType eType, decimal nAmount, string strAddress)
        {
            if( eType != NetworkType.Arbitrum) return false;
            var oResult = await m_oGlobalClient.BingX.SpotApi.Account.WithdrawAsync(strCurrency, strAddress, nAmount, BingX.Net.Enums.AccountType.Perpetual, eType.ToString());
            if (oResult == null || !oResult.Success || oResult.Data == null) return false;
            return true;
        }
        public async Task<bool> StartSockets()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StopSockets()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get positions   
        /// </summary>
        /// <returns></returns>
        public async Task<IFuturesPosition[]?> GetPositions()
        {
            var oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Trading.GetPositionsAsync();
            if (oResult == null || !oResult.Success) return null;
            if (oResult.Data == null) return null;
            List<IFuturesPosition> aResult = new List<IFuturesPosition>();
            foreach (var oData in oResult.Data)
            {
                IFuturesSymbol? oSymbol = this.Exchange.SymbolManager.GetSymbol(oData.Symbol);
                if (oSymbol == null) continue;
                aResult.Add(new BingxFuturesPosition(oSymbol, oData));
            }
            return aResult.ToArray();
        }
    }
}
