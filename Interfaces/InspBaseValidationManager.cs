﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;


namespace Interfaces
{
    abstract public class InspBaseValidationManager
    {
        protected IInspPoolCommunicator _inspPoolCommunicator;

        public InspBaseValidationManager(IInspPoolCommunicator communicator) => _inspPoolCommunicator = communicator;

        public abstract IList<ISuspInspInfo> Validate(Insp insp);
    }
}