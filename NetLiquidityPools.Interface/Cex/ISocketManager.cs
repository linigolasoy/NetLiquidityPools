using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface.Cex
{
    public interface ISocketManager<T>
    {
        public ICexExchange Exchange { get; }


        public void Put(T oItem);

        public T[] GetValues();

        public T? GetValue(string strKey);

        public DateTime LastUpdate { get; } 

        public int UpdateCount { get; } 
    }
}
