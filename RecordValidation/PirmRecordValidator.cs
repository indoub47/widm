using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public class PirmRecordValidator : BaseRecordValidator
    {
        public PirmRecordValidator(IRecordTagMaker tagMaker) : base(tagMaker)
        {
            //linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
            validationMethods = new ValidationMethod[] 
            {
                RecordValidationMethods.ValidateLinija,
                RecordValidationMethods.ValidateKelias,
                RecordValidationMethods.ValidateKm,
                RecordValidationMethods.ValidatePk,
                RecordValidationMethods.ValidateM,
                RecordValidationMethods.ValidateSiule,
                RecordValidationMethods.ValidatePrivalomasPk,
                RecordValidationMethods.ValidateSiuleIesmeEmpty,
                RecordValidationMethods.ValidateSalKodas,
                RecordValidationMethods.ValidateSuvirino,
                RecordValidationMethods.ValidateTikrinimoData,
                RecordValidationMethods.ValidateTikrinimoDataIsReal,
                RecordValidationMethods.ValidateAparatas
            };
            mapping = Properties.Settings.Default.PirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
