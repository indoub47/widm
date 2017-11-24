using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public interface IReporter
    {
        StringBuilder ReportRecValidation(IList<InvalidInfo> invalidRecordInfoList, bool addHeader = true);
        StringBuilder ReportInspValidation(IList<InvalidInfo> suspInspInfoList, bool addHeader = true);
        StringBuilder ReportDbUpdate(IList<Insp> inspList);
    }
}
