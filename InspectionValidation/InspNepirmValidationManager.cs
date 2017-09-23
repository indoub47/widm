using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using WidmShared;
using System.Data;

namespace InspectionValidation
{
    class InspNepirmValidationManager : InspBaseValidationManager
    {
        public InspNepirmValidationManager(IInspPoolCommunicator communicator)
            : base(communicator)
        {
            
        }

        public override IList<SuspInspInfo> Validate(Insp insp)
        {
            DataTable dTable = _inspPoolCommunicator.FetchById((long)insp.Id);
                return InspValidationMethods.ValidateNepirm(insp, dTable);
        }
    }
}
