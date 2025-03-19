using CryptoDexCommon.Internal;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Uniswap.V3.Yldr
{
    internal class LeverageDataProvider
    {

        public LeverageDataProvider( string address, IWeb3Client oClient )
        {
            Address = address;
            Client = oClient;
        }

        public string Address { get; }
        private IWeb3Client Client { get; } 

        internal async Task<ILeveragedPositionData?> GetPositionData( string strAddress)
        {

            var oHandler = Client.Web3Client.Eth.GetContractQueryHandler<GetPositionDataFunction>();

            var oMessage = new GetPositionDataFunction()
            {
                Address = strAddress
            };
            var oResult = await oHandler.QueryDeserializingToObjectAsync<GetPositionDataOutput>(oMessage, Address);


            throw new NotImplementedException();
        }   
    }
}
