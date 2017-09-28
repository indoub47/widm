using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public class NepirmRecordValidator : BaseRecordValidator
    {
        public NepirmRecordValidator(string[] mapping, IRecordTagMaker tagMaker) : base(mapping, tagMaker)
        {
            //id, linija, kelias, km, pk, m, siule, salyginis_kodas, tikrinimo_data, kelintas_tikrinimas, aparatas
            validationMethods = new ValidationMethod[] 
            {
                RecordValidationMethods.ValidateId,
                RecordValidationMethods.ValidateLinija,
                RecordValidationMethods.ValidateKelias,
                RecordValidationMethods.ValidateKm,
                RecordValidationMethods.ValidatePk,
                RecordValidationMethods.ValidateM,
                RecordValidationMethods.ValidateSiule,
                RecordValidationMethods.ValidateSalKodas,
                RecordValidationMethods.ValidateTikrinimoData,
                RecordValidationMethods.ValidateTikrinimoDataIsReal,
                RecordValidationMethods.ValidateKelintas,
                RecordValidationMethods.ValidateNegaliButiPirmas,
                RecordValidationMethods.ValidateAparatas
            };
        }

    }
}
