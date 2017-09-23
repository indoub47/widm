using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IInvalidDataInfo
    {
        IList<object> Record { get; set; }
        string Message { get; set; }
        string ValidationMethod { get; set; }
        IRecordContext Context { get; set; }
    }
}
