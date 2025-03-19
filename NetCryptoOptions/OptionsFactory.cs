using NetCryptoOptions.Deribit;
using NetCryptoOptions.Interface;

namespace NetCryptoOptions
{
    internal class DummySetup : IOptionSetup
    {

    }
    public class OptionsFactory
    {

        public static IOptionSetup CreateSetup(string strSetupFile)
        {
            return new DummySetup();
        }

        public static IOptionExchange CreateExchange( IOptionSetup oSetup, bool bTest = true )
        {
            return new DeribitExchange(oSetup, bTest);
        }
    }
}
