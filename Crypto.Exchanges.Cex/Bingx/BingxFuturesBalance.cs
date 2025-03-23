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
    internal class BingxFuturesBalance: BaseFuturesBalance, IFuturesBalance
    {
        public BingxFuturesBalance(ICexExchange oExchange, string sCurrency, BingXFuturesBalance oBalance) : 
            base(oExchange, sCurrency)
        {
            Balance = oBalance.Balance;
            UnrealizedProfit = oBalance.UnrealizedProfit;
            Equity = oBalance.Equity;
            MarginUsed = oBalance.UsedMargin;
            MarginFrozen = oBalance.FrozenMargin;   
        }   
    }

    internal class BingxSpotBalance : ICexBalance
    {
        public BingxSpotBalance(ICexExchange oExchange, BingXBalance oBalance) 
        {
            Exchange = oExchange;
            Currency = oBalance.Asset;
            Balance = oBalance.Total;
        }

        public ICexExchange Exchange { get; }

        public string Currency { get; }

        public decimal Balance { get; }
    }
}
