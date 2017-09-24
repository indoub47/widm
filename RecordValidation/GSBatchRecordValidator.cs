using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public class GSBatchRecordValidator : BatchRecordValidator
    {
        public GSBatchRecordValidator(BaseRecordValidator recordValidator, string operatorId, string sheetName)
            : base(recordValidator)
        {
            Context["operatorId"] = operatorId;
            Context["sheetName"] = sheetName;
        }
    }
}
