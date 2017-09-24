using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using WidmShared;

namespace InspectionValidation
{
    public class InspBatchValidator : IBatchInspValidator
    {
        private BaseInspValidator _validator;
        public Dictionary<string, object> Context;

        public InspBatchValidator(BaseInspValidator validator)
        {
            _validator = validator;
        }

        public IList<InvalidInfo> Validate(IList<Insp> inspections)
        {
            List<InvalidInfo> allInfos = new List<InvalidInfo>();
            foreach (Insp insp in inspections)
            {
                allInfos.Concat(_validator.Validate(insp, Context));
            }

            return allInfos;
        }
    }
}
