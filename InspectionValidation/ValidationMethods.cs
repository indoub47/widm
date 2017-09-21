using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace InspectionValidation
{
    public delegate IList<SuspInspInfo> InspValidateMethod(Insp inspection, DataTable records);

    public static class ValidationMethods
    {
        /// <summary>
        /// Tikrina pirmąjį tikrinimą ar DB nėra su tokiu pat vietos kodu
        /// </summary>
        /// <param name="insp">Suvirinimo tikrinimo objektas, kurį reikia patikrinti</param>
        /// <param name="tblRecords">Įrašai DB, kurie turi tą patį vietos kodą kaip tikrinamas suvirinimas</param>
        /// <returns></returns>
        public static IList<SuspInspInfo> ValidatePirm(Insp insp, DataTable tblRecords)
        {
            var returnList = new List<SuspInspInfo>();
            if (insp.Kelintas != Kelintas.First)
            {
                throw new ArgumentException("Nepirmasis tikrinimas tikrinamas kaip pirmasis");
            }

            if (tblRecords.Rows.Count > 0)
            {
                string suspMessage = "ta pati vieta: " + string.Join(", ",
                    tblRecords.Rows.Cast<DataRow>().Select(r => r["id"].ToString()).ToArray());

                returnList.Add(new SuspInspInfo(suspMessage));
            }
            return returnList;
        }



        /// <summary>
        /// Tikrina nepirmąjį tikrinimą: ar atitinka vietos kodas, sąlyginis defekto kodas, ar tinkama tikrinimo data
        /// </summary>
        /// <param name="insp">Suvirinimo tikrinimo objektas, kurį reikia patikrinti</param>
        /// <param name="tblRecords">Įrašai DB, kurie turi tą patį ID kaip tikrinamas suvirinimas. 
        /// Turėtų būti tik vienas toks, nes keli vienodi ID neleidžiami DB</param>
        /// <returns></returns>
        public static IList<SuspInspInfo> ValidateNepirm(Insp insp, DataTable tblRecords)
        {
            if (insp.Kelintas == Kelintas.First)
            {
                throw new ArgumentException("Pirmasis tikrinimas tikrinamas kaip nepirmasis");
            }

            if (tblRecords.Rows.Count > 1)
            {
                throw new Exception("DB keli įrašai su tuo pačiu ID " + tblRecords.Rows[0]["id"]?.ToString());
            }

            var suspicions = new List<SuspInspInfo>();

            if (tblRecords.Rows.Count == 0)
            {
                suspicions.Add(new SuspInspInfo("toks įrašas nerastas ir jo duomenys negali būti pakeisti"));
                return suspicions;
            }

            DataRow record = tblRecords.Rows[0];

            checkIfVietosAgree(insp, suspicions, record);
            checkIfSkodaiAgree(insp, suspicions, record);

            if (insp.Kelintas != Kelintas.Extra)
            {
                string patDataField = "", formerPatDataField = "", nextPatDataField = "";
                switch (insp.Kelintas)
                {
                    case Kelintas.Second:
                        patDataField = "II_pat_data";
                        formerPatDataField = "I_pat_data";
                        nextPatDataField = "III_pat_data";
                        break;
                    case Kelintas.Third:
                        patDataField = "III_pat_data";
                        formerPatDataField = "II_pat_data";
                        nextPatDataField = "IV_pat_data";
                        break;
                    case Kelintas.Fourth:
                        patDataField = "IV_pat_data";
                        formerPatDataField = "III_pat_data";
                        break;
                }


                checkIfNotInspectedYet(insp, suspicions, record, patDataField);
                checkIfFormerInspIsDone(suspicions, record, formerPatDataField);
                checkIfLaterInspIsntDone(suspicions, record, nextPatDataField);
                checkIfFormerInspDateIsNotLater(insp, suspicions, record, patDataField, formerPatDataField);
            }

            return suspicions;
        }

        private static void checkIfFormerInspDateIsNotLater(
            Insp insp,
            List<SuspInspInfo> suspicions,
            DataRow record,
            string patDataField,
            string formerPatDataField)
        {
            // formerPatDataField turi būti ankstesnis už wi.Data
            if (Convert.ToDateTime(record[formerPatDataField]) > insp.TData)
            {
                suspicions.Add(new SuspInspInfo(string.Format(
                    "ankstesnis patikrinimas {0} atliktas vėliau ({1:d}) negu dabar siūlomas {2} ({3:d}).",
                    formerPatDataField,
                    record[formerPatDataField],
                    patDataField,
                    insp.TData)));
            }
        }

        private static void checkIfLaterInspIsntDone(
            List<SuspInspInfo> suspicions,
            DataRow record,
            string nextPatDataField)
        {
            // nextPatDataField turi būti tuščias.
            // Čia būtų arba db duomenų klaida - rodytų, kad šitas tikrinimas dar neatliktas, 
            // bet atliktas vėlesnis, arba įrašas būtų pažymėtas kaip panaikintas 
            // (tuomet surašomos visų tikrinimų fake datos), bet nepakeistas sąlyginis kodas į x.6?
            if (nextPatDataField != "" && record[nextPatDataField] != null &&
                record[nextPatDataField].ToString() != string.Empty)
            {
                suspicions.Add(new SuspInspInfo(string.Format(
                    "įrašyta, kad jau atliktas vėlesnis patikrinimas ({0} netuščias)",
                    nextPatDataField)));
            }
        }


        private static void checkIfFormerInspIsDone(
            List<SuspInspInfo> suspicions,
            DataRow record,
            string formerPatDataField)
        {
            // formerPatDataField turi būti netuščias
            if (record[formerPatDataField] == null ||
                record[formerPatDataField].ToString() == string.Empty)
            {
                suspicions.Add(new SuspInspInfo(string.Format(
                    "neatliktas ankstesnis patikrinimas ({0} tuščias)",
                    formerPatDataField)));
            }
        }


        private static void checkIfNotInspectedYet(
            Insp insp,
            List<SuspInspInfo> suspicions,
            DataRow record,
            string patDataField)
        {
            // patDataField turi būti tuščias
            if (record[patDataField] != null &&
                 record[patDataField].ToString() != string.Empty)
            {
                suspicions.Add(new SuspInspInfo(string.Format(
                    "{0} tikrinimas jau atliktas.",
                    (int)insp.Kelintas)));
            }
        }


        private static void checkIfSkodaiAgree(
            Insp insp,
            List<SuspInspInfo> suspicions,
            DataRow record)
        {
            if (record["salyginis_kodas"].ToString() != insp.SKodas) // nereikia tikrinti null,  nes yra privalomas
            {
                suspicions.Add(new SuspInspInfo(string.Format(
                    "esantis sąlyginis kodas {0} neatitinka siūlomo sąlyginio kodo {1}",
                    record["salyginis_kodas"],
                    insp.SKodas)));
            }
        }

        private static void checkIfVietosAgree(
            Insp insp,
            List<SuspInspInfo> suspicions,
            DataRow record)
        {
            string recordVietosKodas = string.Format("{0}.{1:0}{2:000}.{3:#00}.{4:#00}.{5:##0}",
                            record["linija"], record["kelias"], record["km"],
                            record["pk"], record["m"], record["siule"]);

            if (recordVietosKodas != insp.VietosKodas)
            {
                suspicions.Add(new SuspInspInfo("neatitinka vietos kodas"));
            }
        }
    }


}
