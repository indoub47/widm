using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidmShared
{
    public class InvalidRecordInfo
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

        public InvalidRecordInfo(string errorMessage)
        {
            Message = errorMessage;
        }
    }
}
