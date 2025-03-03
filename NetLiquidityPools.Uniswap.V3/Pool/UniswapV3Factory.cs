using CryptoDexCommon.Internal;
using NetLiquidityPools.Interface;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3.Pool
{
    internal class UniswapV3Factory
    {
        public UniswapV3Factory(IWeb3Client oClient, string strAddress)
        {
            Address = strAddress;
            Client = oClient;
        }
        private static List<UniswapPool> m_aPools = new List<UniswapPool>();

        internal string Address { get; }

        private IWeb3Client Client { get; }


        internal async Task<string> GetPoolAddres(string strToken0, string strToken1, BigInteger nFee)
        {
            var oMessage = new GetPoolFunction()
            {
                Token0 = strToken0,
                Token1 = strToken1,
                Fee = nFee
            };


            var oHandler = Client.Web3Client.Eth.GetContractQueryHandler<GetPoolFunction>();
            var oResult = await oHandler.QueryAsync<string>(Address, oMessage);
            return oResult;
        }


        internal async Task<UniswapPool> GetPool(string strToken0, string strToken1, BigInteger nFee)
        {
            UniswapPool? oFound = m_aPools.FirstOrDefault(p => p.Equals(Client.Setup.NetworkType, strToken0, strToken1, nFee));
            if (oFound != null) return oFound;

            string strPoolAddress = await GetPoolAddres(strToken0, strToken1, nFee);

            string strTokenMin = (strToken0.CompareTo(strToken1) >0 ? strToken1: strToken0);
            string strTokenMax = (strToken0.CompareTo(strToken1) > 0 ? strToken0 : strToken1);

            oFound = new UniswapPool(Client, Client.Setup.NetworkType, strPoolAddress, strTokenMin, strTokenMax, nFee);
            m_aPools.Add(oFound);
            return oFound;

        }

    }
}
