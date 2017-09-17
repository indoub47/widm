using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecordValidation
{
    public delegate InvalidDataInfo ValidationMethod(IList<object> record, string[] mapping); 

    public static class ValidationMethods
    {
        public static InvalidDataInfo ValidateId(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveLong(
                "id",
                record,
                mapping,
                "suvirinimo ID"
                );
        }

        public static InvalidDataInfo ValidateLinija(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidateStringFromArray(
                "linija",
                Properties.Settings.Default.Linijos.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "linija");
        }

        public static InvalidDataInfo ValidateKelias(
            IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveInt("kelias", record, mapping, "oo.Xooo.oo.oo.o");
        }

        public static InvalidDataInfo ValidateKm(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveInt("km", record, mapping, "oo.oXXX.oo.oo.o");
        }

        public static InvalidDataInfo ValidatePk(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveIntOrZeroOrNull("pk", record, mapping, "oo.oooo.XX.oo.o");
        }

        public static InvalidDataInfo ValidateM(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveIntOrZero("m", record, mapping, "oo.oooo.oo.XX.o");
        }

        public static InvalidDataInfo ValidateSiule(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidatePositiveIntOrZeroOrNull("siule", record, mapping, "oo.oooo.oo.oo.X");
        }

        public static InvalidDataInfo ValidateSalKodas(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidateStringFromArray(
                "salyginis_kodas",
                Properties.Settings.Default.SalygKodai.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "salyginis kodas");
        }

        public static InvalidDataInfo ValidateAparatas(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidateStringFromArray(
                "aparatas",
                Properties.Settings.Default.Aparatai.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "defektoskopo kodas");
        }

        public static InvalidDataInfo ValidateTikrinimoData(IList<object> record, string[] mapping)
        {
            string errorMessage = "tikrinimo data";
            object value = GeneralValidationMethods.GetRowItem("tikrinimo_data", record, mapping);

            if (GeneralValidationMethods.IsEmpty(value))
            {
                return new InvalidDataInfo(errorMessage);
            }

            Regex regex = new Regex(@"\d{4}[/\.\- ]\d{1,2}[/\.\- ]\d{1,2}");
            /*
             * yyyy-mm-dd  yyyy-m-d
             * yyyy.mm.dd  yyyy.m.d
             * yyyy/mm/dd  yyyy/m/d
             * yyyy mm dd  yyyy m d
            */ 
            if (!regex.IsMatch(value.ToString().Trim()))
            {
                errorMessage = "blogas datos formatas";
                return new InvalidDataInfo(errorMessage);
            }

            try
            {
                DateTime data = Convert.ToDateTime(value);
                return null;
            }
            catch
            {
                return new InvalidDataInfo(errorMessage);
            }
        }

        /// <summary>
        /// Tikrina ar reali data. Visais atvejais grąžina null, išskyrus vienintelį atvejį, kai: 
        /// 1. tikrinti, ar data reali, reikia
        /// 2. data egzistuoja
        /// 3. data neatrodo reali
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mapping"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static InvalidDataInfo ValidateTikrinimoDataIsReal(IList<object> record, string[] mapping)
        {
            if (
                !Properties.Settings.Default.CheckIfDateIfReal ||
                ValidateTikrinimoData(record, mapping) != null
               )
            {
                return null;
            }

            object value = GeneralValidationMethods.GetRowItem("tikrinimo_data", record, mapping);
            DateTime data = Convert.ToDateTime(value);
            if (GeneralValidationMethods.IsNotReal(data))
            {
                string errorMessage = string.Format("tikrinimo data {0:yyyy-MM-dd} neatrodo reali", data);
                return new InvalidDataInfo(errorMessage);
            }
            else
            {
                return null;
            }  
        }

        public static InvalidDataInfo ValidateSuvirino(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidateStringFromArray(
                "suvirino",
                Properties.Settings.Default.Suvirino.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "kas suvirino");
        }

        public static InvalidDataInfo ValidateKelintas(IList<object> record, string[] mapping)
        {
            return GeneralValidationMethods.ValidateStringFromArray(
                "kelintas_tikrinimas",
                Properties.Settings.Default.Kelinti.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "kelintas tikrinimas");
        }

        public static InvalidDataInfo ValidateNegaliButiPirmas(IList<object> record, string[] mapping)
        {
            // jeigu "kelintas" yra išvis blogas - bus išgaudytas, ten, kur tikrinamas kelintas
            if (ValidateKelintas(record, mapping) != null)
            {
                return null;
            }

            object value = GeneralValidationMethods.GetRowItem("kelintas_tikrinimas", record, mapping);

            // tam atvejui, jeigu kelintas būtų nebūtinai privalomas
            if (GeneralValidationMethods.IsEmpty(value))
            {
                return null;
            }

            string kelintas = value.ToString().Trim();

            // jeigu pirmas
            if (kelintas == "1")
            {
                string errorMessage = "šitas suvirinimas negali būti pirmasis";
                return new InvalidDataInfo(errorMessage);
            }

            // jeigu ne pirmas
            return null;
        }

        /// <summary>
        /// Yra vienintelis atvejis, kai vietos kode Pk neturi būti - kai Kelias == 9
        /// Visais kitais atvejais PK privalomas. Jeigu Kelias netinkamas, tai grąžina null
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static InvalidDataInfo ValidatePrivalomasPk(IList<object> record, string[] mapping)
        {
            // specifinis pirmiesiems

            // jeigu kelias ir piketas blogi, tai pakibs, kai bus tikrinami ant gerumo
            if (
                ValidateKelias(record, mapping) != null ||
                ValidatePk(record, mapping) != null
               )
            {
                return null;
            }

            // tikrinama, kad būtų gera kelio ir piketo kombinacija:
            // 1. kai kelias yra 8, pk gali būti 0 arba empty
            // 2. kai kelias != 8, pk negali būti nulis arba empty
            object obKelias = GeneralValidationMethods.GetRowItem("kelias", record, mapping);
            int kelias = Convert.ToInt32(obKelias);
            object obPk = GeneralValidationMethods.GetRowItem("pk", record, mapping);

            if (kelias != 8 && (GeneralValidationMethods.IsEmpty(obPk) || Convert.ToInt32(obPk) == 0))
            {
                string errorMessage = "pk čia negali būti tuščias arba nulis";
                return new InvalidDataInfo(errorMessage);
            }
            else if (kelias == 8 && !GeneralValidationMethods.IsEmpty(obPk) && Convert.ToInt32(obPk) != 0)
            {
                string errorMessage = "didelės stoties iešme pk turi būti tuščias arba nulis";
                return new InvalidDataInfo(errorMessage);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Kai Kelias yra 8 arba 9, Siule turi būti null
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static InvalidDataInfo ValidateSiuleIesmeEmpty(IList<object> record, string[] mapping)
        {
            // specifinis pirmiesiems

            // jeigu kelias arba siūlė yra blogi, tai turėtų nepraeiti
            // patikrinimo, kai bus tikrinami ant gerumo
            if (
                ValidateKelias(record, mapping) != null ||
                ValidateSiule(record, mapping) != null
               )
            {
                return null;
            }

            // tikrinama, kad būtų tinkama kelio ir siūlės kombinacija:
            // 1. kai kelias yra 8 arba 9, siūlė turi būti empty
            // 2. kai kelias nėra 8 arba 9, siūlė negali būti empty
            object obKelias = GeneralValidationMethods.GetRowItem("kelias", record, mapping);
            int kelias = Convert.ToInt32(obKelias);
            object obSiule = GeneralValidationMethods.GetRowItem("siule", record, mapping);

            if ((kelias == 8 || kelias == 9) && !GeneralValidationMethods.IsEmpty(obSiule))
            {
                string errorMessage = "suvirinimas iešme, o nurodyta siūlė";
                return new InvalidDataInfo(errorMessage);
            }
            else if (kelias != 8 && kelias != 9 && GeneralValidationMethods.IsEmpty(obSiule))
            {
                string errorMessage = "suvirinimas ne iešme, o siūlė nenurodyta";
                return new InvalidDataInfo(errorMessage);
            }
            else
            {
                return null;
            }
        }


    }
}
