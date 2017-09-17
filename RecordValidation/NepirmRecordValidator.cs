using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public class NepirmRecordValidator : SingleRecordValidator
    {
        public NepirmRecordValidator()
        {
            //id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas
            validationMethods = new ValidationMethod[] 
            {
                ValidationMethods.ValidateId,
                ValidationMethods.ValidateLinija,
                ValidationMethods.ValidateKelias,
                ValidationMethods.ValidateKm,
                ValidationMethods.ValidatePk,
                ValidationMethods.ValidateM,
                ValidationMethods.ValidateSiule,
                ValidationMethods.ValidateSalKodas,
                ValidationMethods.ValidateTikrinimoData,
                ValidationMethods.ValidateTikrinimoDataIsReal,
                ValidationMethods.ValidateKelintas,
                ValidationMethods.ValidateNegaliButiPirmas,
                ValidationMethods.ValidateAparatas
            };
            mapping = Properties.Settings.Default.NepirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray(); 
        }

    }
}
