using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Factory
{
    internal class LeverageSetup : ILeverageSetup
    {
        public LeverageSetup(LeverageJson oJson)
        {
            ContractAddress = oJson.LeverageContract;
            PercentDown = oJson.PercentDown;
            PercentUp = oJson.PercentUp;
            HedgeApiKey = oJson.HedgeApiKey;
            HedgeApiSecret = oJson.HedgeApiSecret;
            MintContract = oJson.MintContract;
        }

        public string MintContract { get; } = string.Empty;

        public string ContractAddress { get; set; } = string.Empty;

        public decimal PercentDown { get; }

        public decimal PercentUp { get; }

        public string HedgeApiKey { get; }

        public string HedgeApiSecret { get; }
    }
}
