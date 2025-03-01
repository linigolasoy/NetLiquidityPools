using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public interface ILiquidityPool
    {
        public ILiguidityPoolProvider Provider { get; }
        public BigInteger Id { get; }   
    }
}
