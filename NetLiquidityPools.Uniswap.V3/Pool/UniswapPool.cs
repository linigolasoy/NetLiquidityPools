using CryptoDexCommon.Internal;
using NetLiquidityPools.Interface;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3.Pool
{
    internal class UniswapPool
    {
        public UniswapPool(IWeb3Client oClient, NetworkType eType, string strAddress, string strToken0, string strToken1, BigInteger nFee)
        {
            NetworkType = eType;
            Address = strAddress;
            Token0 = strToken0;
            Token1 = strToken1;
            Fee = nFee;
            Client = oClient;   
        }
        public NetworkType NetworkType { get; }
        public string Address { get; }
        public string Token0 { get; }
        public string Token1 { get; }
        public BigInteger Fee { get; }

        private IWeb3Client Client { get; } 

        public bool Equals(NetworkType eType, string strToken0, string strToken1, BigInteger nFee)
        {
            if (NetworkType != eType) return false;
            if (Fee != nFee) return false;
            if ((Token0 != strToken0 || Token1 != strToken1) &&
                (Token1 != strToken0 || Token0 != strToken1)) return false;
            return true;
        }

        internal async Task<Slot0Output> GetSlot0(ICryptoSetup oSetup)
        {

            var oMessage = new Slot0Function();


            var oHandler = Client.Web3Client.Eth.GetContractQueryHandler<Slot0Function>();
            var oResult = await oHandler.QueryDeserializingToObjectAsync<Slot0Output>(oMessage, Address);
            return oResult;


        }

    }
}
