﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace WidmShared
{
    public interface IReporter
    {
        StringBuilder ReportRecValidation(IList<InvalidInfo> invalidRecordInfoList);
        StringBuilder ReportInspValidation(IList<InvalidInfo> suspInspInfoList);
        StringBuilder ReportDbUpdate(IList<Insp> inspList);
    }
}
