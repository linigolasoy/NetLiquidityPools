using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal interface IWebsocketInternal
    {
        public Task<bool> Start();
        public Task<bool> Stop();
    }


    internal interface IWebsocketInternalPublic : IWebsocketInternal, IFuturesWebsocketPublic
    {
        
    }

    /*
    internal interface IWebsocketInternalPrivate : IWebsocketInternal, IFuturesWebsocketPublic
    {

    }
    */

}
