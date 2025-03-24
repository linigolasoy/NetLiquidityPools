using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;
using NetLiquidityPools.Interface.Cex;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Bot
{
    internal class HedgedLiquidity : IHedgedLiquidity
    {

        private ILeverageLiquidity m_oLiquidity;
        private ICryptoClient m_oClient;
        private bool m_bSocketStarted = false;  

        private int m_nLEVERAGE = 40;    

        private enum WrappedSymbols
        {
            WETH,
            WBTC,
            WPOL
        }

        public HedgedLiquidity(ICexExchange oExchange, ICryptoSetup oSetup, ICommonLogger oLogger)
        {
            Exchange = oExchange;
            Setup = oSetup;
            Logger = oLogger;
            m_oClient = CommonDexFactory.CreateClient(oSetup);
            m_oLiquidity = CommonDexFactory.CreateLeveragedLiquidity(m_oClient.Wallet, oSetup);

            int nLeverage = 100 / (int)oSetup.LeverageSetup.PercentDown;
            if (nLeverage > 50) nLeverage = 50;
            m_nLEVERAGE = nLeverage;   
        }
        public ICexExchange Exchange { get; }
        public ICryptoSetup Setup { get; }
        public ICommonLogger Logger { get; }

        public ILeveragedPositionData? LastData { get; private set; } = null;

        public IFuturesPosition? Position { get; private set; } = null;

        public IFuturesSymbol? Symbol { get; private set; } = null;

        public HedgeStatus Status { get; private set; } = HedgeStatus.None;

        public decimal ProfitPool { get; private set; } = 0;

        public decimal ProfitHedge { get; private set; } = 0;


        /// <summary>
        /// Refresh
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            try
            {
                HedgeStatus eStatusPrev = Status;
                switch (Status)
                {
                    case HedgeStatus.None:
                        await DoInitialGather();
                        break;
                    case HedgeStatus.Hedge:
                        await DoHedge();
                        break;
                    case HedgeStatus.Refresh:
                        await RefreshStatus();  
                        break;
                    case HedgeStatus.Liquidate:
                        await DoLiquidate();
                        break;
                    case HedgeStatus.Error:
                        break;

                }
                if( Status != eStatusPrev)
                {
                    Logger.Info($"   Hedge Status [{eStatusPrev}] -> [{Status}]");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception in refresh", ex);
            }


        }

        /// <summary>
        /// Get pool data status
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetPoolStatus()
        {
            var oData = await m_oLiquidity.GetPositionData();
            if (oData == null)
            {
                Logger.Warning("Could not get position data");
                return false;
            }

            LastData = oData;
            return true;
        }

        private string TokenToCurrency(IToken oToken)
        {
            if (oToken.Symbol == WrappedSymbols.WETH.ToString()) return "ETH";
            if (oToken.Symbol == WrappedSymbols.WBTC.ToString()) return "BTC";
            if (oToken.Symbol == WrappedSymbols.WPOL.ToString()) return "POL";
            return oToken.Symbol;
        }

        /// <summary>
        /// Get actual position 
        /// </summary>
        /// <returns></returns>
        private async Task<IFuturesPosition?> GetActualPosition()
        {
            if(Symbol == null) return null; 
            IFuturesPosition[]? aPositions = await Exchange.Account.GetPositions();
            if (aPositions == null)
            {
                Logger.Warning($"{Exchange.ExchangeType.ToString()}Positions returned null");
                return null;
            }
            IFuturesPosition? oPosition = aPositions.FirstOrDefault(p => p.Symbol.Symbol == Symbol.Symbol);
            return oPosition;
        }

        /// <summary>
        /// Get position status 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetPositionStatus()
        {
            if(LastData == null) return false;  
            if(Symbol == null) 
            {
                string strCurrency = TokenToCurrency( LastData.Token0 );
                string strQuote = "USDT";
                IFuturesSymbol? oSymbol = Exchange.SymbolManager.GetAllValues().FirstOrDefault(s => s.Base == strCurrency && s.Quote == strQuote);  
                if(oSymbol == null) return false;
                Symbol = oSymbol;
                Logger.Info($"   Hedge Symbol is [{Symbol.Symbol}]");
            }
            if( !m_bSocketStarted)
            {
                m_bSocketStarted = true;
                await Exchange.Market.StartSockets();
                await Task.Delay(1000);
                await Exchange.Market.Websocket!.Subscribe(Symbol);
            }
            IFuturesPosition? oPosition = await GetActualPosition();

            if (oPosition != null) Position = oPosition;
            Logger.Info($"   {(oPosition == null ? "No": "Found")} Position on [{Symbol.Symbol}]");

            // Set leverage
            if (Position == null)
            {
                var oResult = await Exchange.Trading.SetLeverage(Symbol, m_nLEVERAGE);
                if (!oResult.Success) return false;
            }

            return true;
        }


        /// <summary>
        /// Gather initial data 
        /// </summary>
        /// <returns></returns>
        private async Task DoInitialGather()
        {
            // Get contract
            string? strContract = await m_oLiquidity.GetLastContractAddress();
            if (strContract == null)
            {
                Logger.Warning("Contract address null");
                return;
            }
            Setup.LeverageSetup.ContractAddress = strContract;
            Logger.Info($"  Leverage contract address is [{Setup.LeverageSetup.ContractAddress}]");
            // Get pool data
            bool bStatus = await GetPoolStatus();
            if (!bStatus) return;
            bStatus = await GetPositionStatus();
            if (!bStatus) return;
            if (Position == null)
            {
                Status = HedgeStatus.Hedge;
            }
            else 
            {
                Status = HedgeStatus.Refresh;
            }
        }

        /// <summary>
        /// Hedge 
        /// </summary>
        /// <returns></returns>
        private async Task DoHedge()
        {
            if(Position != null || Symbol == null ) return;
            if( LastData == null) return;
            bool bStatus = await GetPoolStatus();   
            if( !bStatus ) return;

            // Prices 
            BigInteger oTickDiff = LastData.TickUpper - LastData.TickLower;
            BigInteger oTickStep = oTickDiff / 4;
            BigInteger oTickInitial = LastData.TickLower + oTickStep;

            BigInteger oNewLower = oTickInitial - oTickStep;
            BigInteger oNewUpper = oTickInitial + oTickStep;

            UniswapPoolMath oMath = new UniswapPoolMath(LastData.Token0, LastData.Token1);


            BigInteger[] aAmountsInit = oMath.CalculateAmounts(LastData.Liquidity, oTickInitial, LastData.TickLower, LastData.TickUpper);
            BigInteger[] aAmountsLow  = oMath.CalculateAmounts(LastData.Liquidity, oNewLower,    LastData.TickLower, LastData.TickUpper);
            BigInteger[] aAmountsHigh = oMath.CalculateAmounts(LastData.Liquidity, oNewUpper,    LastData.TickLower, LastData.TickUpper);

            decimal nPriceLow  = oMath.TickToPrice(oNewLower);
            decimal nPriceInit = oMath.TickToPrice(oTickInitial);
            decimal nPriceHigh = oMath.TickToPrice(oNewUpper);

            decimal nAmountLow  = oMath.BigToAmount0(aAmountsLow[0]) * nPriceLow + oMath.BigToAmount1(aAmountsLow[1]);
            decimal nAmountInit = oMath.BigToAmount0(aAmountsInit[0]) * nPriceInit + oMath.BigToAmount1(aAmountsInit[1]);
            decimal nAmountHigh = oMath.BigToAmount0(aAmountsHigh[0]) * nPriceHigh + oMath.BigToAmount1(aAmountsHigh[1]);

            decimal nDiffHigh = nAmountHigh - nAmountInit;
            decimal nDiffLow = nAmountInit - nAmountLow;

            decimal nDiff = (nDiffHigh + nDiffLow) / 2M;
            decimal nSl = Math.Round( nPriceHigh, Symbol.Decimals);
            decimal nTp = Math.Round(nPriceLow, Symbol.Decimals);

            decimal nDiffSlTp = nSl - nPriceInit;
            decimal nQuantity = Math.Round( nDiff / nDiffSlTp, Symbol.QuantityDecimals);

            Logger.Info($"   Hedge [{Symbol.Symbol}] Qty [{nQuantity}] Sl [{nSl}] Tp [{nTp}]"); 
            var oResult = await Exchange.Trading.PlaceOrderMarket(Symbol, false, nQuantity, nSl, nTp );
            if(oResult == null || !oResult.Success )
            {
                string strMessage = ((oResult == null || oResult.Message == null) ? "Undefined" : oResult.Message);

                Logger.Error($"   Hedge [{Symbol.Symbol}] Qty [{nQuantity}] Sl [{nSl}] Tp [{nTp}] Failed!!! : {strMessage}");
            }
            else
            {
                await Task.Delay(1000);
                IFuturesPosition? oPosition = await GetActualPosition();
                if (oPosition != null) Position = oPosition;
                Status = HedgeStatus.Refresh;
            }
        }

        /// <summary>
        /// Refresh 
        /// </summary>
        /// <returns></returns>
        private async Task RefreshStatus()
        {
            if( Position == null || Symbol == null) return;   
            bool bStatus = await GetPoolStatus();
            if (!bStatus) return;
            IFuturesPosition? oPosition = await GetActualPosition();    
            // No position, close all and rebalance
            if( oPosition == null)
            {
                Logger.Info($"   Hedge Position [{Symbol.Symbol}] is closed. Rebalancing...");
                Position = null;
                Status = HedgeStatus.Liquidate;
                return;
            }

            // Check if we need to close pool
            if(LastData == null) return;
            decimal nPriceDiff = LastData.PriceLow / ((100M - Setup.LeverageSetup.PercentDown) / 100) - LastData.PriceLow;
            decimal nPriceHigh = LastData.PriceLow + 2M * nPriceDiff;

            if (LastData.PriceActual <= LastData.PriceLow || LastData.PriceActual >= nPriceHigh)
            {
                Logger.Info($"   PositionData [{Symbol.Symbol}] range done. Rebalancing...");
                Status = HedgeStatus.Liquidate;
                return;
            }
            CalculateProfits();

        }
        private void CalculateProfits()
        {
            if( Exchange.Market.Websocket == null || LastData == null) return;  
            if( Symbol == null) return;
            if(Position == null) return;

            UniswapPoolMath oMath = new UniswapPoolMath(LastData.Token0, LastData.Token1);

            // Prices 
            BigInteger oTickDiff = LastData.TickUpper - LastData.TickLower;
            BigInteger oTickStep = oTickDiff / 4;
            BigInteger oTickInitial = LastData.TickLower + oTickStep;

            BigInteger[] aAmountsInit   = oMath.CalculateAmounts(LastData.Liquidity, oTickInitial, LastData.TickLower, LastData.TickUpper);
            BigInteger[] aAmountsActual = oMath.CalculateAmounts(LastData.Liquidity, LastData.TickActual, LastData.TickLower, LastData.TickUpper);

            decimal nPriceInit = oMath.TickToPrice(oTickInitial);
            decimal nPriceActual = oMath.TickToPrice(LastData.TickActual);

            decimal nAmountActual = oMath.BigToAmount0(aAmountsActual[0]) * nPriceActual + oMath.BigToAmount1(aAmountsActual[1]);
            decimal nAmountInit   = oMath.BigToAmount0(aAmountsInit[0]) * nPriceInit + oMath.BigToAmount1(aAmountsInit[1]);

            decimal nProfitPool = nAmountActual - nAmountInit;
            ProfitPool = nProfitPool;

            IFuturesTicker? oTicker = Exchange.Market.Websocket.TickerManager.GetValue(Symbol.Symbol);
            ProfitHedge = 0;
            if (oTicker == null) return;
            decimal nPrice = oTicker.LastPrice; 
            decimal nPriceDiff = Position.PriceAverage - nPrice;

            ProfitHedge = nPriceDiff * Position.Quantity;
        }
        private async Task<bool> DoRebalancePool()
        {

            Logger.Info($"   Collecting fees");
            var oResultFees = await m_oLiquidity.CollectFees();
            if (oResultFees == null || !oResultFees.Success) 
            {
                Logger.Error($"   Collect fees failed!!");
                return false;
            }
            await Task.Delay(1000);

            Logger.Info($"   Rebalancing pool");
            var oResultRebalance = await m_oLiquidity.Rebalance();
            if (oResultRebalance == null || !oResultRebalance.Success)
            {
                Logger.Error($"   Rebalance failed!!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Close position
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DoClosePosition()
        {
            if(Position == null || Symbol == null) return true;
            var oResult = await Exchange.Trading.ClosePosition(Position);
            if (oResult == null || !oResult.Success)
            {
                Logger.Error($"   Close Position [{Symbol.Symbol}] failed");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Liquidate position
        /// </summary>
        /// <returns></returns>
        private async Task DoLiquidate()
        {
            List<Task<bool>> aTasks = new List<Task<bool>>();   
            if( Position != null)
            {
                aTasks.Add(DoClosePosition());
            }
            aTasks.Add(DoRebalancePool());

            await Task.WhenAll(aTasks);
            if( aTasks.Any(t => !t.Result))
            {
                Logger.Error("   Liquidation failed");
                Status = HedgeStatus.Error;
                return;
            }
            Status = HedgeStatus.Hedge;
            await Task.Delay(2000);


        }
    }
}
