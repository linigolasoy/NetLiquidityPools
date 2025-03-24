using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public interface ILeverageLiquidity
    {

        public ICryptoSetup Setup { get; }

        public ICryptoWallet Wallet { get; }

        public Task<ICryptoTransaction?> CreatePool(IToken oToken0, IToken oToken1, decimal nFee, decimal nAmount0, decimal nAmount1, decimal nRangeMax, decimal nLeverage);

        // public Task<ICryptoTransaction?> CollectFees(string strContractAddress);


        public Task<string?> GetLastContractAddress();

        public Task<ILeveragedPositionData?> GetPositionData(string? strContractAddress = null);

        public Task<ICryptoTransaction?> CollectFees(string? strContractAddress = null);


        public Task<ICryptoTransaction?> Rebalance(string? strContractAddress = null, decimal? nPercentDown = null, decimal? nPercentUp = null);
    }
}
