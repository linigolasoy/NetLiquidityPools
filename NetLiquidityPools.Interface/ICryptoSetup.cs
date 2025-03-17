using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLiquidityPools.Interface
{
    public enum NetworkType
    {
        Arbitrum,
        Polygon,
        Ethereum
    }


    public interface ICryptoWalletSetup
    {
        public ICryptoSetup Setup { get; }
        public string Name { get; }
        public string Address { get; }
        public string? PrivateKey { get; }
    }


    public interface ILeverageSetupData
    {
        public string MintContract { get; }
    }

    public interface ICryptoSetup
    {

        public string Web3Url { get; }  

        public NetworkType NetworkType { get; } 

        public string TokenFile { get; }    

        public ILeverageSetupData LeverageSetupData { get; }

        public ICryptoWalletSetup[] Wallets { get; }    

    }
}
