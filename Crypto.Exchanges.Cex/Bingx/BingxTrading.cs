using BingX.Net.Enums;
using BingX.Net.Objects.Models;
using Crypto.Exchanges.Cex.Common;
using CryptoClients.Net.Interfaces;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XT.Net.Objects.Models;

namespace Crypto.Exchanges.Cex.Bingx
{
    internal class BingxTrading : IFuturesTrading
    {
        private IExchangeRestClient m_oGlobalClient;
        private bool m_bMultiAsset = false;
        public BingxTrading(ICexExchange oExchange)
        {
            Exchange = oExchange;
            m_oGlobalClient = ((BingxExchange)oExchange).GlobalClient;
        }
        public ICexExchange Exchange { get; }

        public async Task<ITradingResult<bool>> CancelOrder(IFuturesSymbol oSymbol, string strOrderId)
        {
            var oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Trading.CancelOrderAsync(oSymbol.Symbol, long.Parse(strOrderId));
            if (oResult == null) return new TradingResult<bool>("Result returned null");
            if (!oResult.Success) return new TradingResult<bool>(oResult.Error!.ToString());
            if (oResult.Data == null) return new TradingResult<bool>("Result returned data null");

            return new TradingResult<bool>(true);
        }

        /// <summary>
        /// Close position
        /// </summary>
        /// <param name="oPosition"></param>
        /// <param name="nPrice"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ITradingResult<bool>> ClosePosition(IFuturesPosition oPosition, decimal? nPrice = null)
        {
            try
            {
                OrderSide eOrderSide = (oPosition.Direction == PositionDirection.Long ? OrderSide.Sell : OrderSide.Buy);
                PositionSide ePositionSide = (oPosition.Direction == PositionDirection.Long ? PositionSide.Long : PositionSide.Short);
                FuturesOrderType eType = (nPrice == null ? FuturesOrderType.Market : FuturesOrderType.Limit);   
                WebCallResult<BingXFuturesOrder>? oResult = null;

                oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Trading.PlaceOrderAsync(
                        oPosition.Symbol.Symbol,
                        eOrderSide,
                        eType,
                        ePositionSide,
                        oPosition.Quantity, 
                        nPrice
                    );

                if (oResult == null) return new TradingResult<bool>("Result returned null");
                if (!oResult.Success) return new TradingResult<bool>(oResult.Error!.ToString());
                if (oResult.Data == null) return new TradingResult<bool>("Result returned data null");

                return new TradingResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new TradingResult<bool>(ex);
            }
        }

        public async Task<ITradingResult<string>> PlaceOrderLimit(IFuturesSymbol oSymbol, bool bBuy, decimal nAmount, decimal nPrice, decimal? nStopLoss = null, decimal? nTakeProfit = null)
        {

            try
            {
                TakeProfitStopLossMode? eModeSl = (nStopLoss == null ? null : TakeProfitStopLossMode.StopMarket);
                TakeProfitStopLossMode? eModeTp = (nTakeProfit == null ? null : TakeProfitStopLossMode.TakeProfitMarket);
                TriggerType? eTriggerTypeSl = (nStopLoss == null ? null : TriggerType.LastPrice);
                TriggerType? eTriggerTypeTp = (nTakeProfit == null ? null : TriggerType.LastPrice);
                //DUDU
                var oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Trading.PlaceOrderAsync(
                    oSymbol.Symbol,
                    (bBuy ? BingX.Net.Enums.OrderSide.Buy : BingX.Net.Enums.OrderSide.Sell),
                    BingX.Net.Enums.FuturesOrderType.Limit,
                    (bBuy ? BingX.Net.Enums.PositionSide.Long : BingX.Net.Enums.PositionSide.Short),
                    nAmount,
                    nPrice,
                    null, // reduce only
                    null, // decimal ? stopPrice = null
                    null, // decimal ? priceRate = null,
                    eModeSl,
                    nStopLoss,
                    nStopLoss,
                    eTriggerTypeSl,
                    null,
                    eModeTp,
                    nTakeProfit,
                    nTakeProfit,
                    eTriggerTypeTp
                );


                if (oResult == null) return new TradingResult<string>("Result returned null");
                if (!oResult.Success) return new TradingResult<string>(oResult.Error!.ToString());
                if (oResult.Data == null) return new TradingResult<string>("Result returned data null");

                return new TradingResult<string>(oResult.Data.OrderId.ToString(), true);
            }
            catch (Exception ex)
            {
                return new TradingResult<string>(ex);
            }

        }

        public async Task<ITradingResult<string>> PlaceOrderMarket(IFuturesSymbol oSymbol, bool bBuy, decimal nAmount, decimal? nStopLoss = null, decimal? nTakeProfit = null)
        {
            try
            {
                TakeProfitStopLossMode? eModeSl = (nStopLoss == null ? null : TakeProfitStopLossMode.StopMarket);
                TakeProfitStopLossMode? eModeTp = (nTakeProfit == null ? null : TakeProfitStopLossMode.TakeProfitMarket);
                TriggerType? eTriggerTypeSl = (nStopLoss == null ? null : TriggerType.LastPrice);
                TriggerType? eTriggerTypeTp = (nTakeProfit == null ? null : TriggerType.LastPrice);
                //DUDU
                var oResult = await m_oGlobalClient.BingX.PerpetualFuturesApi.Trading.PlaceOrderAsync(
                    oSymbol.Symbol,
                    (bBuy ? BingX.Net.Enums.OrderSide.Buy : BingX.Net.Enums.OrderSide.Sell),
                    BingX.Net.Enums.FuturesOrderType.Market,
                    (bBuy ? BingX.Net.Enums.PositionSide.Long : BingX.Net.Enums.PositionSide.Short),
                    nAmount,
                    null,
                    null, // reduce only
                    null, // decimal ? stopPrice = null
                    null, // decimal ? priceRate = null,
                    eModeSl,
                    nStopLoss,
                    nStopLoss,
                    eTriggerTypeSl,
                    null,
                    eModeTp,
                    nTakeProfit,
                    nTakeProfit,
                    eTriggerTypeTp
                );


                if (oResult == null) return new TradingResult<string>("Result returned null");
                if (!oResult.Success) return new TradingResult<string>(oResult.Error!.ToString());
                if (oResult.Data == null) return new TradingResult<string>("Result returned data null");

                return new TradingResult<string>(oResult.Data.OrderId.ToString(), true);
            }
            catch (Exception ex)
            {
                return new TradingResult<string>(ex);
            }
        }

        /// <summary>
        /// Set leverage    
        /// </summary>
        /// <param name="oSymbol"></param>
        /// <param name="nLeverage"></param>
        /// <returns></returns>
        public async Task<ITradingResult<bool>> SetLeverage( IFuturesSymbol oSymbol, int nLeverage )
        {
            try
            {
                if (!m_bMultiAsset)
                {
                    var oResultMulti = await m_oGlobalClient.BingX.PerpetualFuturesApi.Account.SetMultiAssetModeAsync(MultiAssetMode.SingleAssetMode);
                    if (oResultMulti == null || !oResultMulti.Success) throw new Exception("Invalid multiasset");
                    m_bMultiAsset = true;
                }
                var oResultLong = await m_oGlobalClient.BingX.PerpetualFuturesApi.Account.SetLeverageAsync(oSymbol.Symbol, BingX.Net.Enums.PositionSide.Long, nLeverage);
                if (oResultLong == null) return new TradingResult<bool>("Result returned null");
                if (!oResultLong.Success) return new TradingResult<bool>(oResultLong.Error!.ToString());
                if (oResultLong.Data == null) return new TradingResult<bool>("Result returned data null");

                var oResultShort = await m_oGlobalClient.BingX.PerpetualFuturesApi.Account.SetLeverageAsync(oSymbol.Symbol, BingX.Net.Enums.PositionSide.Short, nLeverage);

                if (oResultShort == null) return new TradingResult<bool>("Result returned null");
                if (!oResultShort.Success) return new TradingResult<bool>(oResultShort.Error!.ToString());
                if (oResultShort.Data == null) return new TradingResult<bool>("Result returned data null");
                return new TradingResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new TradingResult<bool>(ex);
            }
        }
    }
}
