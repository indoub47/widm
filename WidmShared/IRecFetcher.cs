using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidmShared
{
    public interface IRecFetcher
    {
        List<IList<object>> Fetch();

        string[] GetMapping();
    }
}
