using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public class PirmRecordValidator : SingleRecordValidator
    {
        public PirmRecordValidator()
        {
            //linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
            validationMethods = new ValidationMethod[] 
            {
                ValidationMethods.ValidateLinija,
                ValidationMethods.ValidateKelias,
                ValidationMethods.ValidateKm,
                ValidationMethods.ValidatePk,
                ValidationMethods.ValidateM,
                ValidationMethods.ValidateSiule,
                ValidationMethods.ValidatePrivalomasPk,
                ValidationMethods.ValidateSiuleIesmeEmpty,
                ValidationMethods.ValidateSalKodas,
                ValidationMethods.ValidateSuvirino,
                ValidationMethods.ValidateTikrinimoData,
                ValidationMethods.ValidateTikrinimoDataIsReal,
                ValidationMethods.ValidateAparatas
            };
            mapping = Properties.Settings.Default.PirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
