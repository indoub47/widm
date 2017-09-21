using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using System.Data;

namespace InspectionValidation
{
    class PirmNepirmValidationManager : BaseValidationManager
    {
        public PirmNepirmValidationManager(InspPoolCommunication.IInspPoolCommunicator communicator)
            : base(communicator)
        {
            
        }

        public override IList<SuspInspInfo> Validate(Insp insp)
        {
            // Man čia nepatinka. Atrodo, geriau būtų, jeigu dataTable pasirinktų metodas
            // Galbūt reikėtų naudoti ne delegate, bet inherited class, kuris gautų communicatorių 
            // ir pasirinktų sau reikalingą datatable. Bet tokiu atveju inherited class turėtų būti injektuojamas
            if (insp.Kelintas == Kelintas.First)
            {
                DataTable dTable = _inspPoolCommunicator.FetchByVieta(insp);
                return ValidationMethods.ValidatePirm(insp, dTable);
            }
            else
            {
                DataTable dTable = _inspPoolCommunicator.FetchById((long)insp.Id);
                return ValidationMethods.ValidateNepirm(insp, dTable);
            }
        }
    }
}
