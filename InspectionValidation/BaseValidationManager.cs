using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using InspectionValidation;
using InspPoolCommunication;


namespace InspectionValidation
{
    abstract public class BaseValidationManager
    {
        protected IInspPoolCommunicator _inspPoolCommunicator;

        public BaseValidationManager(IInspPoolCommunicator communicator)
        {
            _inspPoolCommunicator = communicator;
        }

        public abstract IList<SuspInspInfo> Validate(Insp insp);
    }
}
