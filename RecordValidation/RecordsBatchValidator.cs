using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public class RecordsBatchValidator
    {
        public SingleRecordValidator SingleRecordValidator { get; }
        public RecordContext Context { get; }

        public RecordsBatchValidator(SingleRecordValidator singleRecordValidator, RecordContext context)
        {
            SingleRecordValidator = singleRecordValidator;
            Context = context;
        }
        
        public List<InvalidDataInfo> ValidateBatch(List<IList<object>> records)
        {
            List<InvalidDataInfo> allInvalids = new List<InvalidDataInfo>();

            foreach(var record in records)
            {
                List<InvalidDataInfo> singleRecordInvalids = SingleRecordValidator.ValidateRecord(record, Context);
                allInvalids.AddRange(singleRecordInvalids);
            }

            return allInvalids;
        }

    }
}
