using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public class InspBatchValidator : IBatchInspValidator
    {
        private InspValidator _validator;
        public Dictionary<string, object> Context;

        public InspBatchValidator(InspValidator validator)
        {
            _validator = validator;
            Context = new Dictionary<string, object>();
        }

        public IList<InvalidInfo> Validate(IList<Insp> inspections)
        {
            List<InvalidInfo> allInfos = new List<InvalidInfo>();
            foreach (Insp insp in inspections)
            {
                allInfos.AddRange(_validator.Validate(insp, Context));
            }

            return allInfos;
        }
    }
}
