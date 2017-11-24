using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public interface IRecTagMaker
    {
        string Make(IList<object> record);
    }
}
