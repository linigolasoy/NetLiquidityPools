using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Factory
{
    internal class LeverageSetupData : ILeverageSetupData
    {
        public LeverageSetupData( string strMintContract ) 
        { 
            MintContract = strMintContract; 
        }
        public string MintContract { get; }
    }
}
