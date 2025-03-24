using CryptoDexCommon.Internal;
using CryptoDexCommon.Internal.EtherScan;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using NetLiquidityPools.Uniswap.V3.Pool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private const string MINT_CONTRACT = "0xD6Ec016A6dfB19c83fBab7f9c24c090035E7bb97";
        private const string LEVERAGE_DATA_CONTRACT = "0xDe3fe78D4C154773718d5aAF3697B1d823849996";
        internal const string FLASLOAN_PROVIDER  = "0x0DCcc7E957a255132F396b74DD1e2E453F8bBa0e";
        internal const string ASSET_CONVERTER    = "0xAA81Ea3AbB14fD0c9b3bE83e7a2fC2f8c7D87707";
        private const string TAG_DATA = "data";
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

            var oContract = oClient.Web3Client.Eth.GetContract<MintFunction>(Setup.LeverageSetup.MintContract);

            var oContractFunction = oContract.GetFunction<MintFunction>();
            var oInput = oContractFunction.CreateTransactionInput(oFuction, Wallet.PublicKey);

            oInput.Gas = new Nethereum.Hex.HexTypes.HexBigInteger(3000000);

            var oPrevious = await oClient.Web3Client.TransactionManager.SendTransactionAsync(oInput);

            var oResult = await oClient.Web3Client.TransactionManager.SendTransactionAndWaitForReceiptAsync(oInput);
            


            throw new NotImplementedException();
        }


        private class TransactionLog
        {
            [JsonProperty("address")]
            public string Address { get; set; } = string.Empty;

            [JsonProperty("topics")]
            public List<string> Topics { get; set; } = new List<string>();
            [JsonProperty("data")]
            public string Data { get; set; } = string.Empty;
        }
        /// <summary>
        /// Get contracts of leveraged liquidity
        /// </summary>
        /// <returns></returns>
        public async Task<string?> GetLastContractAddress()
        {




            IWeb3Client oClient = (IWeb3Client)(Wallet.Client);
            var aTransactions = await oClient.GetTransactions(Wallet.PublicKey);
            if( aTransactions == null ) return null;

            IScanTransaction[]? aFound = aTransactions.Where(p => p.Success &&    
                    p.ToAddress != null && p.ToAddress.ToUpper().Equals( Setup.LeverageSetup.MintContract.ToUpper() ) &&
                    p.Function != null && p.Function.Contains("mint"))
                .OrderByDescending(p=> p.DateTime)
                .ToArray(); 

            if(aFound == null || aFound.Length <= 0 ) return null;

            string? strFound = null;
            foreach( var oFound in aFound )
            {
                var oResult2 = await oClient.Web3Client.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(oFound.TxHash);
                if (oResult2 != null && oResult2.Logs != null)
                {
                    if (!(oResult2.Logs is JArray)) return null;

                    var oLogsJson = JsonConvert.DeserializeObject<List<TransactionLog>>(oResult2.Logs.ToString());
                    if (oLogsJson == null) continue;
                    foreach( var oLog in oLogsJson)
                    {
                        if( oLog.Topics != null && oLog.Topics.Count == 2 )
                        {
                            if(oLog.Topics[0] == "0x1cf3b03a6cf19fa2baba4df148e9dcabedea7f8a5c07840e207e5c089be95d3e")
                            {
                                
                                strFound = oLog.Address;
                                break;
                            }
                        }
                        if (oLog.Address == Setup.LeverageSetup.ContractAddress)
                        {
                            Console.WriteLine("Lo encontre");
                        }

                    }
                    if (strFound != null) break;
                }

            }

            return strFound;

        }

        public async Task<ILeveragedPositionData?> GetPositionData(string? strContractAddress = null)
        {
            string strContract = (strContractAddress == null ? Setup.LeverageSetup.ContractAddress : strContractAddress);

            LeverageDataProvider oProvider = new LeverageDataProvider(LEVERAGE_DATA_CONTRACT, (IWeb3Client)(Wallet.Client));  
            return await oProvider.GetPositionData(strContract);
        }


        public async Task<ICryptoTransaction?> CollectFees(string? strContractAddress = null)
        {
            string strContract = (strContractAddress == null ? Setup.LeverageSetup.ContractAddress : strContractAddress);

            ILeveragedContract oContract = new LeverageContract(this, strContract);  
            return await oContract.CollectFees();   
        }
        public async Task<ICryptoTransaction?> Rebalance(string? strContractAddress = null, decimal? nPercentLow = null, decimal? nPercentHigh = null)
        {
            string strContract = (strContractAddress == null ? Setup.LeverageSetup.ContractAddress : strContractAddress);
            ILeveragedContract oContract = new LeverageContract(this, strContract);
            decimal nLow  = (nPercentLow == null  ? Setup.LeverageSetup.PercentDown : nPercentLow.Value);
            decimal nHigh = (nPercentHigh == null ? Setup.LeverageSetup.PercentUp   : nPercentHigh.Value);

            return await oContract.Rebalance(nLow, nHigh);
        }
    }
}
