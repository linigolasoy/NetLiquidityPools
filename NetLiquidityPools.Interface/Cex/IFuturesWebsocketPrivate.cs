using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface IFuturesWebsocketPrivate
    {
        public IFuturesAccount Account { get; }

        public ISocketManager<IFuturesBalance> BalanceManager { get; }  
    }
}
