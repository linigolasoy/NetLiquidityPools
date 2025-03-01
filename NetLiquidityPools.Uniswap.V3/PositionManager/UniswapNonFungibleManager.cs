using Nethereum.Web3;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.PositionManager
{

    /// <summary>
    /// Uniswap V3 non fungible position manager
    /// </summary>
    internal class UniswapNonFungibleManager
    {


        private static Dictionary<NetworkType, string>? m_aAddresses = null;
        public UniswapNonFungibleManager(ICryptoSetup oSetup)
        {
            Setup = oSetup;
            CreateAddresses();
            if (m_aAddresses == null || !m_aAddresses.ContainsKey(oSetup.NetworkType)) throw new Exception("No adress found");
            Address = m_aAddresses[oSetup.NetworkType];
        }

        private static void CreateAddresses()
        {
            if (m_aAddresses != null) return;
            m_aAddresses = new Dictionary<NetworkType, string>();
            m_aAddresses.Add(NetworkType.Arbitrum, "0xC36442b4a4522E871399CD717aBDD847Ab11FE88");
            m_aAddresses.Add(NetworkType.Polygon, "0xC36442b4a4522E871399CD717aBDD847Ab11FE88");
        }


        internal ICryptoSetup Setup { get; }
        internal string Address { get; }


        private Web3 CreateClient()
        {
            return new Nethereum.Web3.Web3(Setup.Web3Url);
        }


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

            var oWeb3 = CreateClient(); 

            var balanceHandler = oWeb3.Eth.GetContractQueryHandler<BalanceOfFunction>();
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

            var oWeb3 = CreateClient();

            var oHandler = oWeb3.Eth.GetContractQueryHandler<TokenOfOwnerFunction>();
            var oResult = await oHandler.QueryAsync<BigInteger>(Address, oFunction);
            return oResult;

        }


        internal async Task<PositionsOutput?> Positions( BigInteger nPositionId )
        {
            var oFunction = new PositionsFunction()
            {
                TokenId = nPositionId
            };

            var oWeb3 = CreateClient();

            var oHandler = oWeb3.Eth.GetContractQueryHandler<PositionsFunction>();
            var oResult = await oHandler.QueryDeserializingToObjectAsync<PositionsOutput>(oFunction, Address);
            return oResult;

        }
    }
}
