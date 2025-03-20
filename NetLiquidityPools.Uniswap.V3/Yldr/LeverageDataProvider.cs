using CryptoDexCommon.Internal;
using Nethereum.Contracts.Standards.ERC20.TokenList;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Uniswap.V3.Liquidity;
using NetLiquidityPools.Uniswap.V3.Pool;
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

            if(oResult == null) return null;
            IToken? oToken0 = CryptoDexFactory.GetToken(Client.Setup, Client.Setup.NetworkType, oResult.Token0);
            IToken? oToken1 = CryptoDexFactory.GetToken(Client.Setup, Client.Setup.NetworkType, oResult.Token1);

            if (oToken0 == null || oToken1 == null) return null;
            UniswapPoolMath oMath = new UniswapPoolMath(oToken0, oToken1); 

            UniswapV3Factory oFactory = new UniswapV3Factory(Client, UniswapV3PoolProvider.FactoryAddress);
            UniswapPool oPool = await oFactory.GetPool(oToken0.Address, oToken1.Address, oResult.Fee);

            var oSlot0 = await oPool.GetSlot0(Client.Setup);
            int nTickSpacing = oMath.TickSpacing(oResult.Fee);

            return new LeveragePositionData(oToken0, oToken1, oResult, oSlot0, nTickSpacing);
        }   
    }
}
