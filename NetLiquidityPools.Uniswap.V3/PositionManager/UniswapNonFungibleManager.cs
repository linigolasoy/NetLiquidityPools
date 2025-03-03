using CryptoDexCommon.Internal;
using NetLiquidityPools.Interface;
using System.Numerics;

namespace NetLiquidityPools.Uniswap.V3.PositionManager
{

    /// <summary>
    /// Uniswap V3 non fungible position manager
    /// </summary>
    internal class UniswapNonFungibleManager
    {


        private static Dictionary<NetworkType, string>? m_aAddresses = null;

        private IWeb3Client Client { get; }
        public UniswapNonFungibleManager(IWeb3Client oClient)
        {
            Client = oClient;
            CreateAddresses();
            if (m_aAddresses == null || !m_aAddresses.ContainsKey(Client.Setup.NetworkType)) throw new Exception("No adress found");
            Address = m_aAddresses[Client.Setup.NetworkType];
        }

        private static void CreateAddresses()
        {
            if (m_aAddresses != null) return;
            m_aAddresses = new Dictionary<NetworkType, string>();
            m_aAddresses.Add(NetworkType.Arbitrum, "0xC36442b4a4522E871399CD717aBDD847Ab11FE88");
            m_aAddresses.Add(NetworkType.Polygon, "0xC36442b4a4522E871399CD717aBDD847Ab11FE88");
        }


        internal string Address { get; }




        /// <summary>
        /// Balance of function
        /// </summary>
        /// <param name="strAddress"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal async Task<BigInteger?> BalanceOf( string strAddress )
        {
            var balanceOfFunctionMessage = new BalanceOfFunction()
            {
                Owner = strAddress,
            };


            var balanceHandler = Client.Web3Client.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var oBalance = await balanceHandler.QueryAsync<BigInteger>(Address, balanceOfFunctionMessage);
            return oBalance;
        }


        /// <summary>
        /// Token id's of owner
        /// </summary>
        /// <param name="strAddres"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        internal async Task<BigInteger> TokenOfOwner( string strAddress, BigInteger nIndex )
        {
            var oFunction = new TokenOfOwnerFunction()
            {
                Owner = strAddress,
                Index = nIndex  
            };


            var oHandler = Client.Web3Client.Eth.GetContractQueryHandler<TokenOfOwnerFunction>();
            var oResult = await oHandler.QueryAsync<BigInteger>(Address, oFunction);
            return oResult;

        }


        internal async Task<PositionsOutput?> Positions( BigInteger nPositionId )
        {
            var oFunction = new PositionsFunction()
            {
                TokenId = nPositionId
            };



            var oHandler = Client.Web3Client.Eth.GetContractQueryHandler<PositionsFunction>();
            var oResult = await oHandler.QueryDeserializingToObjectAsync<PositionsOutput>(oFunction, Address);
            return oResult;

        }
    }
}
