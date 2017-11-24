using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public class BatchRecValidator
    {
        public BaseRecValidator RecordValidator { get; }
        public Dictionary<string, object> Context;

        public BatchRecValidator(BaseRecValidator recordValidator)
        {
            RecordValidator = recordValidator;
            Context = new Dictionary<string, object>();
        }
        
        public List<InvalidInfo> ValidateBatch(List<IList<object>> records)
        {
            List<InvalidInfo> allInvalids = new List<InvalidInfo>();

            foreach(var record in records)
            {
                List<InvalidInfo> singleRecordInvalids = RecordValidator.ValidateRecord(record, Context);
                allInvalids.AddRange(singleRecordInvalids);
            }

            return allInvalids;
        }

    }
}
