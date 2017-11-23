using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public class PirmRecValidator : BaseRecValidator
    {
        public PirmRecValidator(string[] mapping, IRecTagMaker tagMaker) : base(mapping, tagMaker)
        {
            //linija, kelias, km, pk, m, siule, salyginis_kodas, suvirino, tikrinimo_data, aparatas
            validationMethods = new ValidationMethod[] 
            {
                RecValidationMethods.ValidateLinija,
                RecValidationMethods.ValidateKelias,
                RecValidationMethods.ValidateKm,
                RecValidationMethods.ValidatePk,
                RecValidationMethods.ValidateM,
                RecValidationMethods.ValidateSiule,
                RecValidationMethods.ValidatePrivalomasPk,
                RecValidationMethods.ValidateSiuleIesmeEmpty,
                RecValidationMethods.ValidateSalKodas,
                RecValidationMethods.ValidateSuvirino,
                RecValidationMethods.ValidateTikrinimoData,
                RecValidationMethods.ValidateTikrinimoDataIsReal,
                RecValidationMethods.ValidateAparatas,
                RecValidationMethods.ValidatePavojingumas
            };
            //mapping = Properties.Settings.Default.PirmiejiMapping.Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
