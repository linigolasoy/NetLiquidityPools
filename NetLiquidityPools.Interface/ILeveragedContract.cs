using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{

    /// <summary>
    /// Leveraged contract operations
    /// </summary>
    public interface ILeveragedContract
    {
        public ILeverageLiquidity LeverageManager { get; }  
        public string Address { get; }

        public Task<ICryptoTransaction?> CollectFees();

        public Task<ICryptoTransaction?> Rebalance( decimal nPercentDown, decimal nPercentUp);
    }
}
