using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public class InvalidDataInfo
    {
        public IList<object> Record { get; set; }
        public string Message { get; set; }
        public string ValidationMethod { get; set; }
        public RecordContext Context { get; set; }

        // must be overriden
        public string Tag
        {
            get { return ""; }
        }

        public InvalidDataInfo(string errorMessage)
        {
            Message = errorMessage;
        }
    }
}
