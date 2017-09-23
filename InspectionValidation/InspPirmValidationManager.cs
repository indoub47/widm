﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using Interfaces;
using System.Data;

namespace InspectionValidation
{
    class InspPirmValidationManager : InspBaseValidationManager
    {
        public InspPirmValidationManager(IInspPoolCommunicator communicator)
            : base(communicator)
        {
            
        }

        public override IList<ISuspInspInfo> Validate(Insp insp)
        {
            DataTable dTable = _inspPoolCommunicator.FetchByVieta(insp);
                return InspValidationMethods.ValidatePirm(insp, dTable);
        }
    }
}
