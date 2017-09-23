using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace RecordValidation
{
    public class InvalidDataInfo : IInvalidDataInfo
    {
        public IList<object> Record { get; set; }
        public string Message { get; set; }
        public string ValidationMethod { get; set; }
        public IRecordContext Context { get; set; }

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
