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
        public ICryptoSetup Setup { get; }

        public LiquidityPoolType Type { get; }

        public Task<ILiquidityPool[]?> GetPoolsOfAddress(string strAddress);   
    }
}
