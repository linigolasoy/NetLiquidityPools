using Crypto.Exchanges.Cex;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Test
{
    [TestClass]
    public class BingxTests
    {

        [TestMethod]
        public async Task BingxMarketDataTest()
        {
            ICryptoSetup oSetup = TestCommon.CreateSetup(); 

            ICexExchange oExchange = CexFactory.CreateExchange(oSetup,  ExchangeType.Bingx);

            IFuturesSymbol[] aSymbols = oExchange.SymbolManager.GetAllValues();
            Assert.IsTrue(aSymbols.Length > 100);

            IFundingRate[]? aRates = await oExchange.Market.GetFundingRates();
            Assert.IsNotNull(aRates);
            Assert.IsTrue(aRates.Length > 100);


            IFuturesTicker[]? aTickers = await oExchange.Market.GetTickers();
            Assert.IsNotNull(aTickers);
            Assert.IsTrue(aTickers.Length > 100);
        }


        [TestMethod]
        public async Task BingxMarketSocketTest()
        {
            ICryptoSetup oSetup = TestCommon.CreateSetup();

            ICexExchange oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);

            IFuturesSymbol[] aSymbols = oExchange.SymbolManager.GetAllValues();
            Assert.IsTrue(aSymbols.Length > 100);

            IFuturesSymbol? oSymbol = aSymbols.FirstOrDefault(p=> p.Base == "ETH" && p.Quote == "USDT");
            Assert.IsNotNull(oSymbol);

            bool bStarted = await oExchange.Market.StartSockets();
            Assert.IsTrue(bStarted);
            await Task.Delay(2000);
            Assert.IsTrue(oExchange.Market.Websocket != null);

            bool bSubscribed = await oExchange.Market.Websocket.Subscribe(oSymbol);
            Assert.IsTrue(bSubscribed);
            await Task.Delay(10000);

            IFundingRate[] aRates = oExchange.Market.Websocket.FundingRateManager.GetValues();
            Assert.IsTrue(aRates.Length > 0);

            IFuturesTicker[] aTickers = oExchange.Market.Websocket.TickerManager.GetValues();
            Assert.IsTrue(aTickers.Length > 0);

            Assert.IsTrue(oExchange.Market.Websocket.FundingRateManager.UpdateCount > 5);
            Assert.IsTrue(oExchange.Market.Websocket.TickerManager.UpdateCount > 5);

            await oExchange.Market.EndSockets();    

        }


        [TestMethod]
        public async Task TransferRaul()
        {
            ICryptoSetup oSetup = TestCommon.CreateSetup(1);

            ICexExchange oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);

            IFuturesSymbol[] aSymbols = oExchange.SymbolManager.GetAllValues();
            Assert.IsTrue(aSymbols.Length > 100);

            IFuturesSymbol? oSymbol = aSymbols.FirstOrDefault(p => p.Base == "ETH" && p.Quote == "USDT");
            Assert.IsNotNull(oSymbol);


            string? strAddress = await oExchange.Account.GetDepositAddres("USDT", NetworkType.Arbitrum);
            Assert.IsNotNull(strAddress);

            IFuturesBalance[]? aBalances = await oExchange.Account.GetBalances();    
            Assert.IsNotNull(aBalances);
            Assert.IsTrue(aBalances.Length > 0);
            IFuturesBalance? oUsdt = aBalances.FirstOrDefault(p => p.Currency == "USDT");
            Assert.IsNotNull(oUsdt);

        }

        [TestMethod]
        public async Task BingxTransferTest()
        {
            ICryptoSetup oSetup = TestCommon.CreateSetup();

            ICexExchange oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);

            IFuturesSymbol[] aSymbols = oExchange.SymbolManager.GetAllValues();
            Assert.IsTrue(aSymbols.Length > 100);

            IFuturesSymbol? oSymbol = aSymbols.FirstOrDefault(p => p.Base == "ETH" && p.Quote == "USDT");
            Assert.IsNotNull(oSymbol);

            // bool bWithdraw = await oExchange.Account.WithDraw("USDT", NetworkType.Arbitrum, 5, "0x9945624b53Ec9858b8b914bd9d094B72256e04AC");

            string? strAddress = await oExchange.Account.GetDepositAddres("USDT", NetworkType.Arbitrum);
            Assert.IsNotNull(strAddress);

            IFuturesBalance[]? aBalances = await oExchange.Account.GetBalances();
            Assert.IsNotNull(aBalances);
            Assert.IsTrue(aBalances.Length > 0);
            IFuturesBalance? oBalance = aBalances.FirstOrDefault(p => p.Currency == "USDT");

            decimal nTransfer = 2;
            Assert.IsNotNull(oBalance);
            Assert.IsTrue(oBalance.Balance > nTransfer);
            decimal nBalanceStart = oBalance.Balance;

            ICexBalance[]? aSpotBalances = await oExchange.Account.GetBalancesSpot();
            Assert.IsNotNull(aSpotBalances);
            ICexBalance? oBalanceSpot = aSpotBalances.FirstOrDefault(p => p.Currency == "USDT");
            decimal nSpotBalanceStart = 0;
            if(oBalanceSpot != null)
            {
                nSpotBalanceStart = oBalanceSpot.Balance;
            }


            bool bTransferred = await oExchange.Account.TransferToSpot("USDT", nTransfer);
            await Task.Delay(1000);



            aSpotBalances = await oExchange.Account.GetBalancesSpot();
            Assert.IsNotNull(aSpotBalances);
            oBalanceSpot = aSpotBalances.FirstOrDefault(p => p.Currency == "USDT");
            Assert.IsNotNull(oBalanceSpot);
            Assert.IsTrue( Math.Abs( oBalanceSpot.Balance - nSpotBalanceStart - nTransfer) < 0.1M);

            bTransferred = await oExchange.Account.TransferFromSpot("USDT", nTransfer);
            await Task.Delay(1000);


            aBalances = await oExchange.Account.GetBalances();
            Assert.IsNotNull(aBalances);
            oBalance = aBalances.FirstOrDefault(p => p.Currency == "USDT");
            Assert.IsNotNull(oBalance);
            Assert.IsTrue(oBalance.Balance == nBalanceStart);

        }

        [TestMethod]
        public async Task BingxTradingTests()
        {
            ICryptoSetup oSetup = TestCommon.CreateSetup();

            ICexExchange oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);

            IFuturesSymbol[] aSymbols = oExchange.SymbolManager.GetAllValues();
            Assert.IsTrue(aSymbols.Length > 100);

            IFuturesSymbol? oSymbol = aSymbols.FirstOrDefault(p => p.Base == "XRP" && p.Quote == "USDT");
            Assert.IsNotNull(oSymbol);

            bool bStarted = await oExchange.Market.StartSockets();
            Assert.IsTrue(bStarted);
            await Task.Delay(2000);
            Assert.IsTrue(oExchange.Market.Websocket != null);

            bool bSubscribed = await oExchange.Market.Websocket.Subscribe(oSymbol);
            Assert.IsTrue(bSubscribed);
            await Task.Delay(1000);
            IFuturesTicker? oTicker = oExchange.Market.Websocket.TickerManager.GetValue(oSymbol.Symbol);
            Assert.IsNotNull(oTicker);

            decimal nPriceDiff = 0.5M;

            var oResultOrder = await oExchange.Trading.PlaceOrderLimit(oSymbol, true, 5, oTicker.LastPrice - nPriceDiff, oTicker.LastPrice - nPriceDiff * 2M, oTicker.LastPrice);
            Assert.IsNotNull(oResultOrder);
            Assert.IsTrue(oResultOrder.Success);
            Assert.IsNotNull(oResultOrder.Result);
            string strOrder = oResultOrder.Result;
            await Task.Delay(1000); 

            var oResultCancel = await oExchange.Trading.CancelOrder(oSymbol, strOrder);
            Assert.IsNotNull(oResultCancel);
            Assert.IsTrue(oResultCancel.Success);
            Assert.IsTrue(oResultCancel.Result);

            await Task.Delay(1000);

            // Market and positions
            oResultOrder = await oExchange.Trading.PlaceOrderMarket(oSymbol, true, 5, oTicker.LastPrice - nPriceDiff, oTicker.LastPrice + nPriceDiff);
            await Task.Delay(1000);
            Assert.IsNotNull(oResultOrder);
            Assert.IsTrue(oResultOrder.Success);
            Assert.IsNotNull(oResultOrder.Result);

            IFuturesPosition[] ? aPositions = await oExchange.Account.GetPositions();
            Assert.IsNotNull(aPositions);
            Assert.IsTrue(aPositions.Length > 0);

            IFuturesPosition? oPosition = aPositions.FirstOrDefault(p => p.Symbol.Symbol == oSymbol.Symbol);    
            Assert.IsNotNull(oPosition);

            var oResultClose = await oExchange.Trading.ClosePosition(oPosition);


            await Task.Delay(1000);
            aPositions = await oExchange.Account.GetPositions();
            Assert.IsNotNull(aPositions);
            oPosition = aPositions.FirstOrDefault(p => p.Symbol.Symbol == oSymbol.Symbol);
            Assert.IsNull(oPosition);

            await oExchange.Market.EndSockets();    

        }
    }
}
