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
        private UniswapNonFungibleManager m_oManager;
        private PositionsOutput? m_oOutput;
        public UniswapLiquidityPool(ILiguidityPoolProvider oProvider, BigInteger nId)
        {
            Provider = oProvider;
            Id = nId;
            m_oManager = new UniswapNonFungibleManager(oProvider.Setup);
        }
        public ILiguidityPoolProvider Provider { get; }

        public BigInteger Id { get; }

        public async Task Refresh()
        {
            m_oOutput = await m_oManager.Positions(Id);
        }

    }
}
