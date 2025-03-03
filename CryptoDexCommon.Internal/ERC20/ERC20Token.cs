using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NetLiquidityPools.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal.ERC20
{
    internal class ERC20Token
    {
        public ERC20Token(IToken oToken, ICryptoClient oClient)
        {
            Token = oToken;
            Client = (IWeb3Client)oClient;
            Wallet = oClient.Wallet;
        }
        internal IToken Token { get; }
        internal IWeb3Client Client { get; }
        internal ICryptoWallet Wallet { get; }

        internal async Task<decimal?> GetBalance()
        {
            Web3 oWeb3 = Client.Web3Client;

            var oMessage = new BalanceOfFunction()
            {
                Account = Wallet.PublicKey
            };
            var oHandler = oWeb3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var oResult = await oHandler.QueryAsync<BigInteger>(Token.Address, oMessage);

            double nPow = Math.Pow(10, Token.Decimals);
            double nResult = (double)oResult / nPow;
            return (decimal)nResult;
        }

        internal async Task <ICryptoTransaction?> Send( decimal nAmount, string strTo )
        {
            Web3 oWeb3 = Client.AccountClient;
            double nPow = Math.Pow(10, Token.Decimals);
            double nToSend = (double)nAmount * nPow;
            BigInteger oBig = new BigInteger(nToSend);

            var oMessage = new TransferFunction()
            {
                To = strTo,
                Value = oBig    
            };


            var oHandler = oWeb3.Eth.GetContractTransactionHandler<TransferFunction>();
            var oInput = await oHandler.CreateTransactionInputEstimatingGasAsync(Token.Address, oMessage);


            var oResult = await oWeb3.TransactionManager.SendTransactionAndWaitForReceiptAsync(oInput);
            return new BaseTransaction(oResult);

            /*
            HexBigInteger oValue = new HexBigInteger(oBig);

            TransactionInput oInput = new TransactionInput()
            {
                ChainId = new HexBigInteger((int)Account.ChainId!),
                From = this.PublicKey,
                To = strTo,
                Value = oValue
            };

            var oEstimate = await Account.TransactionManager.EstimateGasAsync(oInput);
            oInput.Gas = new HexBigInteger(oEstimate.Value);


            var oResult = await Account.TransactionManager.SendTransactionAndWaitForReceiptAsync(oInput);
            return new BaseTransaction(oResult);
            */
        }
    }
}
