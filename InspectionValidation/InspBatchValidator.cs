using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using WidmShared;

namespace InspectionValidation
{
    public class InspBatchValidator
    {
        private InspBaseValidationManager _validationManager;

        public InspBatchValidator(InspBaseValidationManager validationManager)
        {
            _validationManager = validationManager;
        }

        IList<SuspInspInfo> Validate(IList<Insp> inspections)
        {
            List<SuspInspInfo> allInfos = new List<SuspInspInfo>();
            foreach (Insp insp in inspections)
            {
                allInfos.Concat(_validationManager.Validate(insp));
            }

            return allInfos;
        }
    }
}
