using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace RecordValidation
{
    public class RecordsBatchValidator
    {
        public BaseRecordValidator RecordValidator { get; }
        public RecordContext Context { get; }

        public RecordsBatchValidator(BaseRecordValidator recordValidator, RecordContext context)
        {
            RecordValidator = recordValidator;
            Context = context;
        }
        
        public List<IInvalidDataInfo> ValidateBatch(List<IList<object>> records)
        {
            List<IInvalidDataInfo> allInvalids = new List<IInvalidDataInfo>();

            foreach(var record in records)
            {
                List<IInvalidDataInfo> singleRecordInvalids = RecordValidator.ValidateRecord(record, Context);
                allInvalids.AddRange(singleRecordInvalids);
            }

            return allInvalids;
        }

    }
}
