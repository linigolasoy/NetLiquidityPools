using CryptoDexCommon.Internal.ERC20;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal
{
    internal class BaseWallet : ICryptoWallet
    {

        public BaseWallet(ICryptoClient oClient, string strName, Account oAccount) 
        { 
            Client = oClient;
            Name = strName;
            Account = oAccount;
            PublicKey = oAccount.Address;
            Account.TransactionManager.Client = ((IWeb3Client)Client).Web3Client.Client;
        }
        public ICryptoClient Client { get; }

        public string Name { get; }

        public string PublicKey { get; }

        internal Account Account { get; }


        public async Task<decimal?> GetBalance()
        {
            Web3 oClient = ((IWeb3Client)Client).Web3Client;
            BigInteger oBalance = await oClient.Eth.GetBalance.SendRequestAsync(PublicKey);

            double nPow = Math.Pow(10, 18);
            double nResult = (double)oBalance / nPow;


            return (decimal)nResult;
        }
        public async Task<decimal?> GetBalance(IToken oToken)
        {
            ERC20Token oErcToken = new ERC20Token(oToken, Client);

            return await oErcToken.GetBalance();
        }

        public async Task<ICryptoTransaction?> Send(decimal nAmount, string strTo)
        {
            Web3 oWeb3 = ((IWeb3Client)Client).Web3Client;


            double nPow = Math.Pow(10, 18);
            double nToSend = (double)nAmount * nPow;
            BigInteger oBig = new BigInteger(nToSend);  

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
        }
        public async Task<ICryptoTransaction?> Send(IToken oToken, decimal nAmount, string strTo)
        {
            ERC20Token oErcToken = new ERC20Token(oToken, Client);
            return await oErcToken.Send(nAmount, strTo);    
        }

        public async Task<ICryptoTransactionResult> Commit(ICryptoTransaction oTransaction)
        {
            throw new NotImplementedException();
        }

    }
}
