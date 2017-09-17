using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace InspectionValidation
{
    public class SuspiciousInspectionInfo
    {
        public Inspection Inspection { get; set; }
        public string Message { get; set; }
        public string ValidationMethod { get; set; }
        public InspectionContext Context { get; set; }

        // must be overriden
        public string Tag
        {
            get { return ""; }
        }

        public SuspiciousInspectionInfo(string errorMessage)
        {
            Message = errorMessage;
        }
    }
}
