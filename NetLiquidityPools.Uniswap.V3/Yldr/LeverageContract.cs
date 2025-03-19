using CryptoDexCommon.Internal;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using Nethereum.JsonRpc.Client;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{
    internal class LeverageContract : ILeveragedContract
    {
        public LeverageContract(ILeverageLiquidity oLeverageManager, string strAddress)
        {
            LeverageManager = oLeverageManager;
            Address = strAddress;
        }   
        public ILeverageLiquidity LeverageManager { get; }

        public string Address { get; }


        /// <summary>
        /// Collect fees from the contract  
        /// </summary>
        /// <returns></returns>
        public async Task<ICryptoTransaction?> CollectFees()
        {
            var oMessage = new ClaimFeesFunction()
            {
                FlashLoanProvider = YldrLeverageLiquidity.FLASLOAN_PROVIDER,
                Params = new ClaimFeesTuple()
                {
                    AssetConverter = YldrLeverageLiquidity.ASSET_CONVERTER,
                    MaxSlippage = new System.Numerics.BigInteger(50),
                    WithdrawFees = true
                }
            };


            var oWeb3 = ((IWeb3Client)LeverageManager.Wallet.Client).AccountClient;
            var oContract = oWeb3.Eth.GetContract<ClaimFeesFunction>(Address);

            var oContractFunction = oContract.GetFunction<ClaimFeesFunction>();
            var oInput = oContractFunction.CreateTransactionInput(oMessage, LeverageManager.Wallet.PublicKey);

            oInput.Gas = new Nethereum.Hex.HexTypes.HexBigInteger(3000000);

            var oResult = await oWeb3.TransactionManager.SendTransactionAndWaitForReceiptAsync(oInput);
            if(oResult == null) return null;    
            return new BaseTransaction(oResult);

        }
    }
}
