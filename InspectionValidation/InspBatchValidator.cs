using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using Interfaces;

namespace InspectionValidation
{
    public class InspBatchValidator
    {
        private InspBaseValidationManager _validationManager;

        public InspBatchValidator(InspBaseValidationManager validationManager)
        {
            _validationManager = validationManager;
        }

        IList<ISuspInspInfo> Validate(IList<Insp> inspections)
        {
            List<ISuspInspInfo> allInfos = new List<ISuspInspInfo>();
            foreach (Insp insp in inspections)
            {
                allInfos.Concat(_validationManager.Validate(insp));
            }

            return allInfos;
        }
    }
}
