using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace Interfaces
{
    public interface IInspBatchValidator
    {
        IList<ISuspInspInfo> Validate(IList<Insp> inspections);
    }
}
