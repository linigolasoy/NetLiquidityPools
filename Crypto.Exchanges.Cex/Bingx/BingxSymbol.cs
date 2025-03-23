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
    internal class BingxSymbol : BaseFuturesSymbol, IFuturesSymbol
    {

        public BingxSymbol(ICexExchange oExchange, BingXContract oContract) :
            base(oExchange)
        {
            Symbol = oContract.Symbol;
            Base = oContract.Asset;
            Quote = oContract.Currency;

            LeverageMax = (int)(oContract.MaxLongLeverage < oContract.MaxShortLeverage ? oContract.MaxLongLeverage : oContract.MaxShortLeverage);
            FeeMaker = oContract.MakerFeeRate;
            FeeTaker = oContract.TakerFeeRate;
            Decimals = oContract.QuantityPrecision;
            Minimum = oContract.MinOrderQuantity;
            ContractSize = oContract.ContractSize;
            QuantityDecimals = oContract.QuantityPrecision;
        }

    }
}
