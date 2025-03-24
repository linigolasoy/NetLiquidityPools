using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public interface ILeverageSetup
    {
        public string MintContract { get; } 
        public string ContractAddress { get; set; }  

        public decimal PercentDown { get; }
        public decimal PercentUp { get; }

        public string HedgeApiKey { get; }
        public string HedgeApiSecret { get; }
    }
}
