using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using WidmShared;

namespace InspectionValidation
{ 
    public static class InspValidationMethods
    {
        /// <summary>
        /// Tikrina pirmąjį tikrinimą ar DB nėra su tokiu pat vietos kodu
        /// </summary>
        /// <param name="insp">Suvirinimo tikrinimo objektas, kurį reikia patikrinti</param>
        /// <param name="tblRecords">Įrašai DB, kurie turi tą patį vietos kodą kaip tikrinamas suvirinimas</param>
        /// <returns></returns>
        public static IList<InvalidInfo> ValidatePirm(Insp insp, DataTable tblRecords)
        {
            var returnList = new List<InvalidInfo>();
            if (insp.Kelintas != Kelintas.First)
            {
                throw new ArgumentException("Nepirmasis tikrinimas tikrinamas kaip pirmasis");
            }

            if (tblRecords.Rows.Count > 0)
            {
                string suspMessage = "ta pati vieta: " + string.Join(", ",
                    tblRecords.Rows.Cast<DataRow>().Select(r => r["id"].ToString()).ToArray());

                returnList.Add(new InvalidInfo(suspMessage));
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
        public static IList<InvalidInfo> ValidateNepirm(Insp insp, DataTable tblRecords)
        {
            if (insp.Kelintas == Kelintas.First)
            {
                throw new ArgumentException("Pirmasis tikrinimas tikrinamas kaip nepirmasis");
            }

            if (tblRecords.Rows.Count > 1)
            {
                throw new Exception("DB keli įrašai su tuo pačiu ID " + tblRecords.Rows[0]["id"]?.ToString());
            }

            var suspicions = new List<InvalidInfo>();

            if (tblRecords.Rows.Count == 0)
            {
                suspicions.Add(new InvalidInfo("toks įrašas nerastas ir jo duomenys negali būti pakeisti"));
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
                        patDataField = "data2";
                        formerPatDataField = "data1";
                        nextPatDataField = "data3";
                        break;
                    case Kelintas.Third:
                        patDataField = "data3";
                        formerPatDataField = "data2";
                        nextPatDataField = "data4";
                        break;
                    case Kelintas.Fourth:
                        patDataField = "data4";
                        formerPatDataField = "data3";
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
            List<InvalidInfo> suspicions,
            DataRow record,
            string patDataField,
            string formerPatDataField)
        {
            // formerPatDataField turi būti ankstesnis už wi.Data
            if (
                record[formerPatDataField] != null && record[formerPatDataField].ToString().Trim() != string.Empty &&
                Convert.ToDateTime(record[formerPatDataField]) > insp.TData )
            {
                suspicions.Add(new InvalidInfo(string.Format(
                    "ankstesnis patikrinimas {0} atliktas vėliau ({1:d}) negu dabar siūlomas {2} ({3:d}).",
                    formerPatDataField,
                    record[formerPatDataField],
                    patDataField,
                    insp.TData)));
            }
        }

        private static void checkIfLaterInspIsntDone(
            List<InvalidInfo> suspicions,
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
                suspicions.Add(new InvalidInfo(string.Format(
                    "įrašyta, kad jau atliktas vėlesnis patikrinimas ({0} netuščias)",
                    nextPatDataField)));
            }
        }


        private static void checkIfFormerInspIsDone(
            List<InvalidInfo> suspicions,
            DataRow record,
            string formerPatDataField)
        {
            // formerPatDataField turi būti netuščias
            if (record[formerPatDataField] == null ||
                record[formerPatDataField].ToString() == string.Empty)
            {
                suspicions.Add(new InvalidInfo(string.Format(
                    "neatliktas ankstesnis patikrinimas ({0} tuščias)",
                    formerPatDataField)));
            }
        }


        private static void checkIfNotInspectedYet(
            Insp insp,
            List<InvalidInfo> suspicions,
            DataRow record,
            string patDataField)
        {
            // patDataField turi būti tuščias
            if (record[patDataField] != null &&
                 record[patDataField].ToString() != string.Empty)
            {
                suspicions.Add(new InvalidInfo(string.Format(
                    "{0} tikrinimas jau atliktas",
                    (int)insp.Kelintas)));
            }
        }


        private static void checkIfSkodaiAgree(
            Insp insp,
            List<InvalidInfo> suspicions,
            DataRow record)
        {
            if (record["skodas"].ToString() != insp.SKodas) // nereikia tikrinti null,  nes yra privalomas
            {
                suspicions.Add(new InvalidInfo(string.Format(
                    "esantis sąlyginis kodas {0} neatitinka siūlomo sąlyginio kodo {1}",
                    record["skodas"],
                    insp.SKodas)));
            }
        }

        private static void checkIfVietosAgree(
            Insp insp,
            List<InvalidInfo> suspicions,
            DataRow record)
        {
            string recordVietosKodas = string.Format("{0}.{1:0}{2:000}.{3:#00}.{4:#00}.{5:##0}",
                            record["linija"], record["kelias"], record["km"],
                            record["pk"], record["m"], record["siule"]);

            if (recordVietosKodas != insp.VietosKodas)
            {
                suspicions.Add(new InvalidInfo("neatitinka vietos kodas"));
            }
        }
    }


}
