using NetLiquidityPools.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoDexCommon.Internal
{
    internal class BaseToken : IToken
    {
        internal BaseToken( NetworkType eNet, string strSymbol, string strName, string strAddress, int nDecimals )
        {
            NetworkType = eNet;
            Symbol = strSymbol;
            Name = strName;
            Address = strAddress;
            Decimals = nDecimals;
        }
        public NetworkType NetworkType { get; }

        public string Symbol { get; }

        public string Name { get; }

        public string Address { get; }

        public int Decimals { get; }

        public override string ToString()
        {
            return $"{Symbol} - {NetworkType.ToString()} ({Address})";
        }
    }
}
