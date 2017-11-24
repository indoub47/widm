using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public class GSRecBatchValidator : BatchRecValidator
    {
        public GSRecBatchValidator(BaseRecValidator recordValidator)
            : base(recordValidator)
        {
            Context["operatorId"] = "undefined";
            Context["sheetName"] = "undefined";
        }

        public GSRecBatchValidator(BaseRecValidator recordValidator, string sheetName)
            : base(recordValidator)
        {
            Context["operatorId"] = "undefined";
            Context["sheetName"] = sheetName;
        }
        public GSRecBatchValidator(BaseRecValidator recordValidator, string operatorId, string sheetName)
            : base(recordValidator)
        {
            Context["operatorId"] = operatorId;
            Context["sheetName"] = sheetName;
        }

        public string OperatorId
        {
            get { return Context["operatorId"].ToString(); }
            set { Context["operatorId"] = value; }
        }

        public string SheetName
        {
            get { return Context["sheetName"].ToString(); }
            set { Context["sheetName"] = value; }
        }
    }
}
