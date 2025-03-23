using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal class ManagerTicker : ISocketManager<IFuturesTicker>
    {

        private ConcurrentDictionary<string, IFuturesTicker> m_aItems = new ConcurrentDictionary<string, IFuturesTicker>();  
        public ManagerTicker(ICexExchange oExchange)
        {
            Exchange = oExchange;
        }
        public ICexExchange Exchange { get; }

        public DateTime LastUpdate { get; private set; } = DateTime.Now.AddHours(-1);

        public int UpdateCount { get; private set; } = 0;

        public IFuturesTicker? GetValue(string strKey)
        {
            IFuturesTicker? oItem = null;   
            if( m_aItems.TryGetValue(strKey, out oItem))
            {
                return oItem;
            }
            return null;
        }

        public IFuturesTicker[] GetValues()
        {
            return m_aItems.Values.ToArray();
        }

        public void Put(IFuturesTicker oItem)
        {
            m_aItems.AddOrUpdate(oItem.Symbol.Symbol, oItem, (k, v) => { v.Update(oItem); return v; });
            LastUpdate = DateTime.Now;
            UpdateCount++;
            return;
        }
    }
}
