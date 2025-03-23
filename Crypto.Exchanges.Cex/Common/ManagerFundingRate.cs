using NetLiquidityPools.Interface.Cex;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Exchanges.Cex.Common
{
    internal class ManagerFundingRate : ISocketManager<IFundingRate>
    {

        private ConcurrentDictionary<string, IFundingRate> m_aItems = new ConcurrentDictionary<string, IFundingRate>(); 

        public ManagerFundingRate(ICexExchange oExchange)
        {
            Exchange = oExchange;
        }
        public ICexExchange Exchange { get; }

        public DateTime LastUpdate { get; private set; } = DateTime.Now.AddHours(-1);

        public int UpdateCount { get; private set; } = 0;

        public IFundingRate? GetValue(string strKey)
        {
            IFundingRate? oResult = null;   
            if( m_aItems.TryGetValue(strKey, out oResult))
            {
                return oResult;
            }
            return null;
        }

        public IFundingRate[] GetValues()
        {
            return m_aItems.Values.ToArray();   
        }

        public void Put(IFundingRate oItem)
        {
            m_aItems.AddOrUpdate(oItem.Symbol.Symbol, oItem, (key, oldValue) => { oldValue.Update(oItem); return oldValue; });
            LastUpdate = DateTime.Now;
            UpdateCount++;
        }
    }
}
