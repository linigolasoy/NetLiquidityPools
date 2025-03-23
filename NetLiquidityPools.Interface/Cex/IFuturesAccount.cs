using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesAccount
    {
        public ICexExchange Exchange { get; }

        public Task<IFuturesBalance[]?> GetBalances();

        public Task<ICexBalance[]?> GetBalancesSpot();

        // Transfering money
        public Task<bool> TransferToSpot( string strCurrency, decimal nAmount);
        public Task<bool> TransferFromSpot(string strCurrency, decimal nAmount);

        public Task<string?> GetDepositAddres(string strCurrency, NetworkType eType);

        public Task<bool> WithDraw(string strCurrency, NetworkType eType, decimal nAmount, string strAddress);

        // Positions and orders
        public Task<IFuturesPosition[]?> GetPositions();    


        // Sockets
        public IFuturesWebsocketPrivate? Websocket { get; }

        public Task<bool> StartSockets();
        public Task<bool> StopSockets();
    }
}
