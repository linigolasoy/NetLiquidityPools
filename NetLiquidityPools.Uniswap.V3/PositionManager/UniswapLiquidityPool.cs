using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.PositionManager
{
    internal class UniswapLiquidityPool : ILiquidityPool
    {

        public UniswapLiquidityPool(ILiguidityPoolProvider oProvider, BigInteger nId)
        {
            Provider = oProvider;
            Id = nId;
        }
        public ILiguidityPoolProvider Provider { get; }

        public BigInteger Id { get; }
    }
}
