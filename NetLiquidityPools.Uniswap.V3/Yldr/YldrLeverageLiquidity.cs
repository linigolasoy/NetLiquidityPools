using CryptoDexCommon.Internal;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using NetLiquidityPools.Uniswap.V3.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{
    public class YldrLeverageLiquidity : ILeverageLiquidity
    {
        public YldrLeverageLiquidity( ICryptoWallet oWallet ) 
        {
            Wallet = oWallet;
            Setup = oWallet.Client.Setup;   
        }
        public ICryptoSetup Setup { get;}

        public ICryptoWallet Wallet { get; }



        private async Task<MintFunction?> PrepareMintInput(IToken oToken0, IToken oToken1, decimal nFee, decimal nAmount0, decimal nAmount1, decimal nRangeMax, decimal nLeverage)
        {
            UniswapPoolMath oMath = new UniswapPoolMath(oToken0, oToken1);

            BigInteger nBigFee = new BigInteger((int)(nFee * 10000));
            IWeb3Client oClient = (IWeb3Client)Wallet.Client;

            UniswapV3Factory oFactory = new UniswapV3Factory(oClient, UniswapV3PoolProvider.FactoryAddress);


            UniswapPool oPool = await oFactory.GetPool(oToken0.Address, oToken1.Address, nBigFee);

            var oSlot = await oPool.GetSlot0(Setup);


            decimal nPrice = oMath.TickToPrice(oSlot.Tick);

            decimal nPriceMax = nPrice * (100M + nRangeMax) / 100M;
            decimal nPriceMin = nPrice * (100M - nRangeMax) / 100M;

            BigInteger oTickMax = oMath.PriceToTick(nPriceMax);
            BigInteger oTickMin = oMath.PriceToTick(nPriceMin);

            BigInteger oAmount0 = oMath.Amount0Big(nAmount0);
            BigInteger oAmount1 = oMath.Amount0Big(nAmount1);

            DateTimeOffset oOffset = DateTimeOffset.UtcNow.AddMinutes(3);
            BigInteger oDeadLine = new BigInteger(oOffset.ToUnixTimeSeconds());

            decimal nTotalAmount = nAmount0 * nPrice + nAmount1;
            decimal nToBorrow = nTotalAmount * (nLeverage - 1M);

            BigInteger oToBorrow = oMath.Amount1Big(nToBorrow); 
            MintFunction oFunction = new MintFunction()
            {
                LiquidityParams = new MintLiquidityParams()
                {
                    Token0 = oToken0.Address,
                    Token1 = oToken1.Address,
                    Fee = nBigFee,
                    TickLower = oTickMin,
                    TickUpper = oTickMax,
                    Amount0Desired = oAmount0,
                    Amount1Desired = oAmount1,
                    Recipient = Wallet.PublicKey,
                    DeadLine = oDeadLine
                },
                InitParams = new MintInitParams()
                {
                    TokenToBorrow = oToken1.Address,
                    AmountToBorrow = oToBorrow,
                    FlashLoanProvider = "0x58011b287553E8053baA91587F6A516fa6614883",
                    AssetConverter = "0xAA81Ea3AbB14fD0c9b3bE83e7a2fC2f8c7D87707",
                    Owner = Wallet.PublicKey
                }

            };
            return oFunction;

        }
        /// <summary>
        /// Create pool
        /// </summary>
        /// <param name="oToken0"></param>
        /// <param name="oToken1"></param>
        /// <param name="nFee"></param>
        /// <param name="nAmount0"></param>
        /// <param name="nAmount1"></param>
        /// <param name="nRangeMax"></param>
        /// <param name="nLeverage"></param>
        /// <returns></returns>
        public async Task<ICryptoTransaction?> CreatePool(IToken oToken0, IToken oToken1, decimal nFee, decimal nAmount0, decimal nAmount1, decimal nRangeMax, decimal nLeverage)
        {

            MintFunction? oFuction = await PrepareMintInput(oToken0, oToken1, nFee, nAmount0, nAmount1, nRangeMax, nLeverage);
            if (oFuction == null) return null;

            IWeb3Client oClient = (IWeb3Client)(Wallet.Client);

            var oContract = oClient.Web3Client.Eth.GetContract<MintFunction>(Setup.LeverageSetupData.MintContract);

            var oContractFunction = oContract.GetFunction<MintFunction>();
            var oInput = oContractFunction.CreateTransactionInput(oFuction, Wallet.PublicKey);

            oInput.Gas = new Nethereum.Hex.HexTypes.HexBigInteger(3000000);

            var oPrevious = await oClient.Web3Client.TransactionManager.SendTransactionAsync(oInput);

            var oResult = await oClient.Web3Client.TransactionManager.SendTransactionAndWaitForReceiptAsync(oInput);
            


            throw new NotImplementedException();
        }
    }
}
