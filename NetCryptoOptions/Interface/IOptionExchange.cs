using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCryptoOptions.Interface
{
    public interface IOptionExchange
    {
        public IOptionSetup Setup { get; }


        public Task<IOptionCurrency[]?> GetCurrencies();

        public Task<IOption[]?> GetOptions(IOptionCurrency? oCurrency = null);
    }
}
