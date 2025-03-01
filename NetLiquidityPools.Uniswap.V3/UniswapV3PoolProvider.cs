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


            List<Task<BigInteger>> aTasks = new List<Task<BigInteger>>();
            int nDone = 0;
            while( nDone < 5 && oIndex > 0 )
            {
                oIndex--;
                aTasks.Add(m_oPositionManager.TokenOfOwner(strAddress, oIndex));
                nDone++;    
            }

            await Task.WhenAll(aTasks); 
            foreach (var oTask in aTasks) 
            { 
                if( oTask.IsCompleted) aFound.Add( new UniswapLiquidityPool(this, oTask.Result));
            }

            List<Task> aTasksRefresh = new List<Task>();    

            foreach (var oPool in aFound  ) 
            {
                aTasksRefresh.Add(oPool.Refresh());
            }
            await Task.WhenAll(aTasksRefresh);

            return aFound.ToArray();
        }
    }
}
