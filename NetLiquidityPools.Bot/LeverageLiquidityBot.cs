using CryptoDexCommon.Internal;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Bot
{
    internal class LeverageLiquidityBot : ILiquidityBot
    {
        private CancellationTokenSource m_oCancelSource = new CancellationTokenSource();    
        private Task? m_oMainTask = null;
        public LeverageLiquidityBot(ICryptoSetup oSetup)
        {
            Setup = oSetup;
            Logger = CryptoDexFactory.CreateLogger(oSetup, this.GetType().Name, m_oCancelSource.Token);
            m_oClient = CommonDexFactory.CreateClient(oSetup);
            m_oLiquidity = CommonDexFactory.CreateLeveragedLiquidity(m_oClient.Wallet, oSetup); 
        }
        public ICommonLogger Logger { get; }

        public ICryptoSetup Setup { get; }

        public BotStatus Status { get; private set; } = BotStatus.Start;

        private ILeverageLiquidity m_oLiquidity;
        private ICryptoClient m_oClient;

        private DateTime m_dLastLog = DateTime.Now;

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
            Status = BotStatus.CheckPosition;   
        }


        private async Task LogPositionData( string strMessage )
        {
            var oData = await m_oLiquidity.GetPositionData();
            if (oData != null)
            {
                decimal nBalance = Math.Round(oData.Amount0 * oData.PriceActual + oData.Amount1, 2);
                decimal nDebt = Math.Round(oData.Debt0 * oData.PriceActual + oData.Debt1, 2);

                decimal nFees = Math.Round(oData.Fees0 * oData.PriceActual + oData.Fees1, 2);

                decimal nPriceHigh = Math.Round(oData.PriceHigh, 2);
                decimal nPriceActual = Math.Round(oData.PriceActual, 2);
                decimal nPriceLow = Math.Round(oData.PriceLow, 2);

                Logger.Info($"{strMessage} Prices ({nPriceLow} / {nPriceActual} / {nPriceHigh} ) Total {nBalance}$ Fees {nFees}$ Debt {nDebt}$ Avaliable {nBalance - nDebt}$");

            }

        }

        /// <summary>
        /// Log status
        /// </summary>
        /// <returns></returns>
        private async Task DoLog()
        {
            DateTime dNow = DateTime.Now;   
            if ( (dNow - m_dLastLog).TotalMinutes >= 2 )
            {
                await LogPositionData("Position.");
                m_dLastLog = dNow;
            }
            Status = BotStatus.CheckPool;
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

        /// <summary>
        /// Main loop   
        /// </summary>
        /// <returns></returns>
        private async Task MainLoop()
        {
            while( !m_oCancelSource.IsCancellationRequested)
            {
                BotStatus ePrevStatus = Status;
                try
                {
                    switch (Status)
                    {
                        case BotStatus.Start:
                            await DoStart();
                            break;
                        case BotStatus.CheckPool:
                            await DoCheckPool();
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
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception in main loop", ex);
                }
                await Task.Delay(5000);
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
