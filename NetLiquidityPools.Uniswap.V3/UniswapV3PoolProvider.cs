using CryptoDexCommon.Internal;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using NetLiquidityPools.Uniswap.V3.PositionManager;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3
{
    public class UniswapV3PoolProvider : ILiguidityPoolProvider
    {

        public UniswapV3PoolProvider(ICryptoClient oClient)
        {
            Client = oClient;
        }
        public ICryptoClient Client { get; }

        public LiquidityPoolType Type {get=> LiquidityPoolType.UniswapV3;}

        private UniswapNonFungibleManager? m_oPositionManager = null;

        public static string FactoryAddress { get => "0x1F98431c8aD98523631AE4a59f267346ea31F984"; }


        /// <summary>
        /// Get pools of address
        /// </summary>
        /// <param name="strAddress"></param>
        /// <returns></returns>
        public async Task<ILiquidityPool[]?> GetPoolsOfAddress(string strAddress)
        {
            if (m_oPositionManager == null) m_oPositionManager = new UniswapNonFungibleManager((IWeb3Client)Client);

            BigInteger? oBalance = await m_oPositionManager.BalanceOf(strAddress);
            if (oBalance == null) return null;
            BigInteger oIndex = oBalance.Value;
            List<ILiquidityPool> aFound = new List<ILiquidityPool>();   


            List<Task<BigInteger>> aTasks = new List<Task<BigInteger>>();
            int nDone = 0;
            while( nDone < 1 && oIndex > 0 )
            {
                oIndex--;
                aTasks.Add(m_oPositionManager.TokenOfOwner(strAddress, oIndex));
                nDone++;    
            }

            await Task.WhenAll(aTasks); 

            List<Task<ILiquidityPool?>> aTaskCreate = new List<Task<ILiquidityPool?>>();    
            foreach (var oTask in aTasks) 
            {

                if (oTask.IsCompleted) 
                {
                    aTaskCreate.Add(UniswapLiquidityPool.Create(this, oTask.Result));
                }
            }

            // Get pool data
            await Task.WhenAll(aTaskCreate);
            foreach (var oTask in aTaskCreate)
            {
                if( oTask.IsCompleted && oTask.Result != null ) { aFound.Add(oTask.Result); }
            }

            // Refresh pools
            List<Task> aTasksRefresh = new List<Task>();    

            foreach (var oPool in aFound  ) 
            {
                aTasksRefresh.Add(oPool.Refresh());
            }
            await Task.WhenAll(aTasksRefresh);

            return aFound.ToArray();
        }


        public async Task<ILiquidityPool[]?> GetPools()
        {
            return await GetPoolsOfAddress(Client.Wallet.PublicKey);
        }


        /// <summary>
        /// Create a pool
        /// </summary>
        /// <param name="oToken0"></param>
        /// <param name="oToken1"></param>
        /// <param name="nFee"></param>
        /// <param name="nAmount0"></param>
        /// <param name="nAmount1"></param>
        /// <param name="nRangeMax"></param>
        /// <returns></returns>
        public async Task<ICryptoTransaction?> CreatePool(IToken oToken0, IToken oToken1, decimal nFee, decimal nAmount0, decimal nAmount1, decimal nRangeMax)
        {
            throw new NotImplementedException();    
        }

        public ILiquidityRangeCalculator CreateRangeCalculator(
            IToken oToken0, IToken oToken1, 
            decimal nAmount0, decimal nAmount1, 
            decimal nPrice,
            decimal nPercentMin, decimal nPercentMax)
        {
            return new UniswapRangeCalculator(
                    oToken0, oToken1,
                    nAmount0, nAmount1,
                    nPrice,
                    nPercentMin, nPercentMax
                );
        }
    }
}
