using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspectionValidation
{
    interface IInspBatchValidator
    {
        IList<SuspInspInfo> Validate(IList<InspectionLib.Insp> inspections);
    }
}
