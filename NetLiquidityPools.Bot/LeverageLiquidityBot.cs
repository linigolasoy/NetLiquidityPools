using Crypto.Exchanges.Cex;
using CryptoDexCommon.Internal;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;
using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Bot
{
    internal class LeverageLiquidityBot : ILiquidityBot
    {
        private CancellationTokenSource m_oCancelSource = new CancellationTokenSource();    
        private Task? m_oMainTask = null;

        private ICexExchange m_oExchange;
        public LeverageLiquidityBot(ICryptoSetup oSetup)
        {
            Setup = oSetup;
            Logger = CryptoDexFactory.CreateLogger(oSetup, this.GetType().Name, m_oCancelSource.Token);
            // m_oClient = CommonDexFactory.CreateClient(oSetup);
            // m_oLiquidity = CommonDexFactory.CreateLeveragedLiquidity(m_oClient.Wallet, oSetup);
            m_oExchange = CexFactory.CreateExchange(oSetup, ExchangeType.Bingx);
        }
        public ICommonLogger Logger { get; }

        public ICryptoSetup Setup { get; }

        public BotStatus Status { get; private set; } = BotStatus.Start;

        private DateTime m_dLastLog = DateTime.Now;
        /*
        private ILeverageLiquidity m_oLiquidity;
        private ICryptoClient m_oClient;

        private ILeveragedPositionData? m_oLastData = null;


        /// <summary>
        /// Start status
        /// </summary>
        /// <returns></returns>
        private async Task DoStart()
        {
            Status = BotStatus.CheckPool;
        }

        /// <summary>
        /// Check Position
        /// </summary>
        /// <returns></returns>
        private async Task DoCheckPosition()
        {
            Status = BotStatus.Log;
            if (m_oLastData == null) return;
        }

        /// <summary>
        /// Check Pool
        /// </summary>
        /// <returns></returns>
        private async Task DoCheckPool()
        {
            var oData = await m_oLiquidity.GetPositionData();
            if (oData == null) return;
            decimal nPriceDiff = oData.PriceLow / ((100M - Setup.LeverageSetup.PercentDown) / 100) - oData.PriceLow;
            decimal nPriceHigh = oData.PriceLow + 2M * nPriceDiff;

            if( oData.PriceActual <= oData.PriceLow || oData.PriceActual >= nPriceHigh )
            {
                Status = BotStatus.Fees;
                return;
            }
            m_oLastData = oData;
            Status = BotStatus.CheckPosition;   
        }




        /// <summary>
        /// Fees status
        /// </summary>
        /// <returns></returns>
        private async Task DoFees()
        {
            Logger.Info("Collecting fees...");
            var oTxFees = await m_oLiquidity.CollectFees();
            if ( oTxFees == null || !oTxFees.Success )
            {
                Logger.Error($"Transaction Failed!!!!!!!{(oTxFees == null ? string.Empty:oTxFees.Message)}");
                return;
            }
            Logger.Info("Fees collected");
            Status = BotStatus.Rebalance;
        }

        /// <summary>
        /// Rebalance status
        /// </summary>
        /// <returns></returns>
        private async Task DoRebalance()
        {
            await LogPositionData("Rebalance before...");
            await Task.Delay(2000);
            Logger.Info("Rebalancing...");
            var oTxFees = await m_oLiquidity.Rebalance();   
            if (oTxFees == null || !oTxFees.Success)
            {
                Logger.Error($"Transaction Failed!!!!!!!{(oTxFees == null ? string.Empty : oTxFees.Message)}");
                return;
            }
            Logger.Info("Rebalanced");

            await Task.Delay(2000);
            await LogPositionData("Rebalance after....");

            Status = BotStatus.Position;
        }

        /// <summary>
        /// Rebalance status
        /// </summary>
        /// <returns></returns>
        private async Task DoPosition()
        {
            Status = BotStatus.CheckPool;
        }
        */

        private void LogCsv(string strMessage, IHedgedLiquidity oHedged)
        {
            DateTime dNow = DateTime.Now;
            string strFile = string.Format("{0}/LiquidityPoolData_{1}{2}{3}.csv", Setup.LogPath, dNow.Year.ToString("D4"), dNow.Month.ToString("D2"), dNow.Day.ToString("D2"));
            if (oHedged.LastData == null) return;
            if ( !File.Exists(strFile))
            {
                string strHeader = "Hora\tAccion\tETH Bajo\tETH Actual\tETH Alto\tTotal Pool\tFees\tDeuda\tDisponible\tGanancia Pool\tGanancia Hedge\tSaldo";
                File.WriteAllText(strFile, strHeader);
            }

            StringBuilder oBuild = new StringBuilder();

            decimal nBalance = Math.Round(oHedged.LastData.Amount0 * oHedged.LastData.PriceActual + oHedged.LastData.Amount1, 2);
            decimal nDebt = Math.Round(oHedged.LastData.Debt0 * oHedged.LastData.PriceActual + oHedged.LastData.Debt1, 2);

            decimal nFees = Math.Round(oHedged.LastData.Fees0 * oHedged.LastData.PriceActual + oHedged.LastData.Fees1, 2);

            decimal nPriceHigh = Math.Round(oHedged.LastData.PriceHigh, 2);
            decimal nPriceActual = Math.Round(oHedged.LastData.PriceActual, 2);
            decimal nPriceLow = Math.Round(oHedged.LastData.PriceLow, 2);

            oBuild.AppendLine();
            oBuild.Append($"{dNow.ToShortTimeString()}\t{strMessage}\t{nPriceLow}\t{nPriceActual}\t{nPriceHigh}\t{nBalance}\t{nFees}\t{nDebt}\t{nBalance - nDebt}\t");
            decimal nProfitPool = Math.Round(oHedged.ProfitPool, 2);
            decimal nProfitHedge = Math.Round(oHedged.ProfitHedge, 2);
            oBuild.Append($"{nProfitPool}\t{nProfitHedge}\t{nProfitPool + nProfitHedge}");
            File.AppendAllText(strFile, oBuild.ToString());
        }


        private void LogPositionData(string strMessage, IHedgedLiquidity oHedged)
        {
            if (oHedged.LastData == null) return;
            decimal nBalance = Math.Round(oHedged.LastData.Amount0 * oHedged.LastData.PriceActual + oHedged.LastData.Amount1, 2);
            decimal nDebt = Math.Round(oHedged.LastData.Debt0 * oHedged.LastData.PriceActual + oHedged.LastData.Debt1, 2);

            decimal nFees = Math.Round(oHedged.LastData.Fees0 * oHedged.LastData.PriceActual + oHedged.LastData.Fees1, 2);

            decimal nPriceHigh = Math.Round(oHedged.LastData.PriceHigh, 2);
            decimal nPriceActual = Math.Round(oHedged.LastData.PriceActual, 2);
            decimal nPriceLow = Math.Round(oHedged.LastData.PriceLow, 2);

            Logger.Info($"{strMessage} Prices ({nPriceLow} / {nPriceActual} / {nPriceHigh} ) Total {nBalance}$ Fees {nFees}$ Debt {nDebt}$ Avaliable {nBalance - nDebt}$");
            decimal nProfitPool = Math.Round(oHedged.ProfitPool, 2);
            decimal nProfitHedge = Math.Round(oHedged.ProfitHedge, 2);
            Logger.Info($"{strMessage} Profit Pool {nProfitPool} Profit Hedge {nProfitHedge} Total : {nProfitPool + nProfitHedge}");

            LogCsv(strMessage, oHedged);
        }

        /// <summary>
        /// Log status
        /// </summary>
        /// <returns></returns>
        private void DoLog(IHedgedLiquidity oLiquidity)
        {
            DateTime dNow = DateTime.Now;
            if ((dNow - m_dLastLog).TotalMinutes >= 2)
            {
                LogPositionData("Position.", oLiquidity);
                m_dLastLog = dNow;
            }
            Status = BotStatus.CheckPool;
        }

        /// <summary>
        /// Main loop   
        /// </summary>
        /// <returns></returns>
        private async Task MainLoop()
        {

            IHedgedLiquidity oHedged = BotFactory.CreateHedgedLiquidity(m_oExchange, Setup, Logger);

            while ( !m_oCancelSource.IsCancellationRequested)
            {
                try
                {
                    await oHedged.Refresh();
                    DoLog(oHedged);  
                    await Task.Delay(3000);
                    /*
                        switch (Status)
                        {
                            case BotStatus.Start:
                                await DoStart();
                                break;
                            case BotStatus.CheckPool:
                                await DoCheckPool();
                                nDelay = 100;
                                break;
                            case BotStatus.CheckPosition:
                                await DoCheckPosition();
                                break;
                            case BotStatus.Log:
                                await DoLog();
                                break;
                            case BotStatus.Fees:
                                await DoFees();
                                break;
                            case BotStatus.Rebalance:
                                await DoRebalance();
                                nDelay = 100;
                                break;
                            case BotStatus.Position:
                                await DoPosition();
                                break;
                        }

                        if (Status != ePrevStatus)
                        {
                            if (Status == BotStatus.Fees || Status == BotStatus.Rebalance || Status == BotStatus.Position)
                            {
                                Logger.Info($"Status changed from {ePrevStatus} to {Status}");
                            }
                        }
                    */
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception in main loop", ex);
                }
            }
            await Task.Delay(1000);
        }

        /// <summary>
        /// Starts main task
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            if( m_oMainTask != null)
            {
                Logger.Error("Main task already running");
                return false;
            }
            m_oMainTask = MainLoop();
            return true;
        }

        public async Task Stop()
        {
            m_oCancelSource.Cancel();
            await Task.Delay(1000); 
            if(m_oMainTask != null)
            {
                await m_oMainTask;
                m_oMainTask = null;
            }   
        }
    }
}
