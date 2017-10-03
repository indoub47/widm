using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public class NepirmRecValidator : BaseRecValidator
    {
        public NepirmRecValidator(string[] mapping, IRecTagMaker tagMaker) : base(mapping, tagMaker)
        {
            //id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas
            validationMethods = new ValidationMethod[] 
            {
                RecValidationMethods.ValidateId,
                RecValidationMethods.ValidateLinija,
                RecValidationMethods.ValidateKelias,
                RecValidationMethods.ValidateKm,
                RecValidationMethods.ValidatePk,
                RecValidationMethods.ValidateM,
                RecValidationMethods.ValidateSiule,
                RecValidationMethods.ValidateSalKodas,
                RecValidationMethods.ValidateTikrinimoData,
                RecValidationMethods.ValidateTikrinimoDataIsReal,
                RecValidationMethods.ValidateKelintas,
                RecValidationMethods.ValidateNegaliButiPirmas,
                RecValidationMethods.ValidateAparatas
            };
        }

    }
}
