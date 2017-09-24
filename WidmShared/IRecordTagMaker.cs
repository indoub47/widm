using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidmShared
{
    public interface IRecordTagMaker
    {
        string Make(IList<object> record);
    }
}
