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

        public IToken Token0 { get; }   
        public IToken Token1 { get; }

        public decimal Fee { get; }

        public decimal Amount0 { get; } 
        public decimal Amount1 { get; }

        public decimal Fees0 { get; }
        public decimal Fees1 { get; }
        public Task Refresh();
    }
}
