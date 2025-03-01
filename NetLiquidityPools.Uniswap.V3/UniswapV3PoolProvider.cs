using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.PositionManager;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3
{
    public class UniswapV3PoolProvider : ILiguidityPoolProvider
    {

        public UniswapV3PoolProvider(ICryptoSetup oSetup)
        {
            Setup = oSetup;
        }
        public ICryptoSetup Setup { get; }

        public LiquidityPoolType Type {get=> LiquidityPoolType.UniswapV3;}

        private UniswapNonFungibleManager? m_oPositionManager = null;

        public async Task<ILiquidityPool[]?> GetPoolsOfAddress(string strAddress)
        {
            if (m_oPositionManager == null) m_oPositionManager = new UniswapNonFungibleManager(Setup);

            BigInteger? oBalance = await m_oPositionManager.BalanceOf(strAddress);
            if (oBalance == null) return null;
            BigInteger oIndex = oBalance.Value;
            List<ILiquidityPool> aFound = new List<ILiquidityPool>();   
            while( aFound.Count < 10 && oIndex > 0 )
            {
                oIndex--;
                BigInteger? oId = await m_oPositionManager.TokenOfOwner(strAddress, oIndex);
                if (oId == null) break;
                aFound.Add(new UniswapLiquidityPool(this, oId.Value));
            }

            return aFound.ToArray();
        }
    }
}
