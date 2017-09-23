using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

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
        
        public List<InvalidRecordInfo> ValidateBatch(List<IList<object>> records)
        {
            List<InvalidRecordInfo> allInvalids = new List<InvalidRecordInfo>();

            foreach(var record in records)
            {
                List<InvalidRecordInfo> singleRecordInvalids = RecordValidator.ValidateRecord(record, Context);
                allInvalids.AddRange(singleRecordInvalids);
            }

            return allInvalids;
        }

    }
}
