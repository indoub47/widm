using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace WidmShared
{
    public class SuspInspInfo
    {
        public Insp Insp { get; set; }
        public string Message { get; set; }
        public string ValidationMethod { get; set; }
        public InspContext Context { get; set; }

        // must be overriden
        public string Tag
        {
            get { return ""; }
        }

        public SuspInspInfo(string errorMessage)
        {
            Message = errorMessage;
        }
    }
}
