using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public static partial class RecordValidationMethods
    {
        public static InvalidInfo ValidateId(IList<object> record, string[] mapping)
        {
            return ValidatePositiveLong(
                "id",
                record,
                mapping,
                "suvirinimo ID"
                );
        }

        public static InvalidInfo ValidateLinija(IList<object> record, string[] mapping)
        {
            return ValidateStringFromArray(
                "linija",
                Properties.Settings.Default.Linijos.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "linija");
        }

        public static InvalidInfo ValidateKelias(
            IList<object> record, string[] mapping)
        {
            return ValidatePositiveInt("kelias", record, mapping, "oo.Xooo.oo.oo.o");
        }

        public static InvalidInfo ValidateKm(IList<object> record, string[] mapping)
        {
            return ValidatePositiveInt("km", record, mapping, "oo.oXXX.oo.oo.o");
        }

        public static InvalidInfo ValidatePk(IList<object> record, string[] mapping)
        {
            return ValidatePositiveIntOrZeroOrNull("pk", record, mapping, "oo.oooo.XX.oo.o");
        }

        public static InvalidInfo ValidateM(IList<object> record, string[] mapping)
        {
            return ValidatePositiveIntOrZero("m", record, mapping, "oo.oooo.oo.XX.o");
        }

        public static InvalidInfo ValidateSiule(IList<object> record, string[] mapping)
        {
            return ValidatePositiveIntOrZeroOrNull("siule", record, mapping, "oo.oooo.oo.oo.X");
        }

        public static InvalidInfo ValidateSalKodas(IList<object> record, string[] mapping)
        {
            return ValidateStringFromArray(
                "skodas",
                Properties.Settings.Default.SalygKodai.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "salyginis kodas");
        }

        public static InvalidInfo ValidateAparatas(IList<object> record, string[] mapping)
        {
            return ValidateStringFromArray(
                "aparatas",
                Properties.Settings.Default.Aparatai.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "defektoskopo kodas");
        }

        public static InvalidInfo ValidateTikrinimoData(IList<object> record, string[] mapping)
        {
            object value = GetRowItem("tdata", record, mapping);

            if (IsEmpty(value))
            {
                return new InvalidInfo("tikrinimo data");
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
                return new InvalidInfo("blogas datos formatas");
            }

            try
            {
                DateTime data = Convert.ToDateTime(value);
                return null;
            }
            catch
            {
                return new InvalidInfo("blogas datos formatas");
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
        public static InvalidInfo ValidateTikrinimoDataIsReal(IList<object> record, string[] mapping)
        {
            if (
                !Properties.Settings.Default.CheckIfDateIfReal ||
                ValidateTikrinimoData(record, mapping) != null
               )
            {
                return null;
            }

            object value = GetRowItem("tdata", record, mapping);
            DateTime data = Convert.ToDateTime(value);
            if (IsNotReal(data))
            {
                return new InvalidInfo(string.Format("tikrinimo data {0:yyyy-MM-dd} neatrodo reali", data));
            }
            else
            {
                return null;
            }  
        }

        public static InvalidInfo ValidateSuvirino(IList<object> record, string[] mapping)
        {
            return ValidateStringFromArray(
                "suvirino",
                Properties.Settings.Default.Suvirino.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "kas suvirino");
        }

        public static InvalidInfo ValidateKelintas(IList<object> record, string[] mapping)
        {
            return ValidateStringFromArray(
                "kelintas",
                Properties.Settings.Default.Kelinti.Split(',').Select(x => x.Trim()).ToArray(),
                record,
                mapping,
                "kelintas tikrinimas");
        }

        public static InvalidInfo ValidateNegaliButiPirmas(IList<object> record, string[] mapping)
        {
            // jeigu "kelintas" yra išvis blogas - bus išgaudytas, ten, kur tikrinamas kelintas
            if (ValidateKelintas(record, mapping) != null)
            {
                return null;
            }

            object value = GetRowItem("kelintas", record, mapping);

            // tam atvejui, jeigu kelintas būtų nebūtinai privalomas
            if (IsEmpty(value))
            {
                return null;
            }

            string kelintas = value.ToString().Trim();

            // jeigu pirmas
            if (kelintas == "1")
            {
                return new InvalidInfo("šitas suvirinimas negali būti pirmasis");
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
        public static InvalidInfo ValidatePrivalomasPk(IList<object> record, string[] mapping)
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
            object obKelias = GetRowItem("kelias", record, mapping);
            int kelias = Convert.ToInt32(obKelias);
            object obPk = GetRowItem("pk", record, mapping);

            if (kelias != 8 && (IsEmpty(obPk) || Convert.ToInt32(obPk) == 0))
            {
                return new InvalidInfo("pk čia negali būti tuščias arba nulis");
            }
            else if (kelias == 8 && !IsEmpty(obPk) && Convert.ToInt32(obPk) != 0)
            {
                return new InvalidInfo("didelės stoties iešme pk turi būti tuščias arba nulis");
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
        public static InvalidInfo ValidateSiuleIesmeEmpty(IList<object> record, string[] mapping)
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
            object obKelias = GetRowItem("kelias", record, mapping);
            int kelias = Convert.ToInt32(obKelias);
            object obSiule = GetRowItem("siule", record, mapping);

            if ((kelias == 8 || kelias == 9) && !IsEmpty(obSiule))
            {
                return new InvalidInfo("suvirinimas iešme, o nurodyta siūlė");
            }
            else if (kelias != 8 && kelias != 9 && IsEmpty(obSiule))
            {
                return new InvalidInfo("suvirinimas ne iešme, o siūlė nenurodyta");
            }
            else
            {
                return null;
            }
        }


    }
}
