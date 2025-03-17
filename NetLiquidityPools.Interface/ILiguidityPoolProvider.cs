namespace NetLiquidityPools.Interface
{

    public enum LiquidityPoolType
    {
        UniswapV3
    }

    /// <summary>
    /// Liquidity pool provider
    /// </summary>
    public interface ILiguidityPoolProvider
    {
        public ICryptoClient Client { get; }

        public LiquidityPoolType Type { get; }

        public Task<ILiquidityPool[]?> GetPoolsOfAddress(string strAddress);

        public Task<ILiquidityPool[]?> GetPools();

        public Task<ICryptoTransaction?> CreatePool(IToken oToken0, IToken oToken1, decimal nFee, decimal nAmount0, decimal nAmount1, decimal nRangeMax);

        public ILiquidityRangeCalculator CreateRangeCalculator(
            IToken oToken0, IToken oToken1, 
            decimal nAmount0, decimal nAmount1, 
            decimal nPrice,
            decimal nPercentMin, decimal nPercentMax);
    }
}
