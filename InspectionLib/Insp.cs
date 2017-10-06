using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspectionLib
{
    public class Insp
    {
        public long? Id { private set; get; }
        public string Linija { private set; get; }
        public int Kelias { private set; get; }
        public int Km { private set; get; }
        public int? Pk { private set; get; }
        public int M { private set; get; }
        public int? Siule { private set; get; }
        public int? Nr { private set; get; }
        public string SKodas { private set; get; }
        public string Operatorius { private set; get; }
        public string Aparatas { private set; get; }
        public DateTime TData { private set; get; }
        public string Suvirino { private set; get; }
        public Kelintas Kelintas { private set; get; }
        public string Pastaba { set; get; }

        public Insp(
            long? id,
            string linija, int kelias, int km, int? pk, int m, int? siule, string salyginisKodas,
            string operatorius, string aparatas, DateTime tikrinimoData, string suvirino,
            Kelintas kelintasTikrinimas, string pastaba = "")
        {
            this.Id = id;
            this.Linija = linija;
            this.Kelias = kelias;
            this.Km = km;
            this.Pk = pk;
            this.M = m;
            this.Siule = siule;
            if (this.Kelias == 8 || this.Kelias == 9)
            {
                this.Nr = this.M;
            }
            else
            {
                this.Nr = null;
            }
            this.SKodas = salyginisKodas;
            this.Operatorius = operatorius;
            this.Aparatas = aparatas;
            this.TData = tikrinimoData;
            this.Suvirino = suvirino;
            this.Kelintas = kelintasTikrinimas;
            this.Pastaba = pastaba;
        }

        public static Insp FromValidRecord(IList<object> record, string[] mapping, string operatorId)
        {
            // jeigu record neturi lauko "kelintas_suvirinimas", priskiriamas pirmasis suvirinimas

            Insp insp = FromValidRecord(record, mapping);
            insp.Operatorius = operatorId;
            return insp;
        }

        /// <summary>
        /// Record must be validated. If record doesn't have field "kelintas_tikrinimas",
        /// property Kelintas is being initialized to value Kelintas.First.
        /// If record doesn't have field "operatorius", property Operatorius will be
        /// initialized with a string.Empty value.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static Insp FromValidRecord(IList<object> record, string[] mapping)
        {
            // record turi būti validated
            // jeigu record neturi lauko "kelintas_suvirinimas", priskiriamas pirmasis suvirinimas

            // id, linija, kelias, km, pk, m, siule, salyginis_kodas, operatorius, aparatas, 
            // tikrinimo_data, suvirino, kelintas_tikrinimas, pastaba
            long? id = null;
            int ind = Array.IndexOf(mapping, "id");
            try
            {
                id = Convert.ToInt64(record[ind].ToString().Trim());
            }
            catch { }

            string linija = record[Array.IndexOf(mapping, "linija")].ToString().Trim();
            int kelias = Convert.ToInt32(record[Array.IndexOf(mapping, "kelias")]);
            int km = Convert.ToInt32(record[Array.IndexOf(mapping, "km")]);

            int? pk = null;
            ind = Array.IndexOf(mapping, "pk");
            try
            {
                pk = Convert.ToInt32(record[ind]);
            }
            catch { }

            int m = Convert.ToInt32(record[Array.IndexOf(mapping, "m")]);

            int? siule = null;
            ind = Array.IndexOf(mapping, "siule");
            try
            {
                siule = Convert.ToInt32(record[ind]);
            }
            catch { }

            string salyginisKodas = record[Array.IndexOf(mapping, "skodas")].ToString().Trim();

            string operatorius = string.Empty;
            ind = Array.IndexOf(mapping, "operatorius");
            if (ind != -1)
                operatorius = record[ind].ToString();

            string aparatas = record[Array.IndexOf(mapping, "aparatas")].ToString().Trim();

            DateTime tikrinimoData = Convert.ToDateTime(record[Array.IndexOf(mapping, "tdata")]);

            string suvirino = string.Empty;
            ind = Array.IndexOf(mapping, "suvirino");
            if (ind != -1)
                suvirino = record[ind].ToString().Trim();
            
            Kelintas kelintasTikrinimas = Kelintas.Extra;
            ind = Array.IndexOf(mapping, "kelintas");
            if (ind == -1)
                kelintasTikrinimas = Kelintas.First;
            else
                kelintasTikrinimas = ParseKelintas(record[ind].ToString().Trim());

            ind = Array.IndexOf(mapping, "pastaba");
            string pastaba = string.Empty;
            if (ind != -1)
                pastaba = record[ind].ToString().Trim();

            return new Insp(id, linija, kelias, km, pk, m, siule, salyginisKodas, 
                operatorius, aparatas, tikrinimoData, suvirino, kelintasTikrinimas, pastaba);
        }

        public string VietosKodas
        {
            get
            {
                return string.Format("{0}.{1:0}{2:000}.{3:#00}.{4:#00}.{5:##0}", Linija, Kelias, Km, Pk, M, Siule);
            }
        }

        public string Description
        {
            get
            {
                if (Id != null)
                    return string.Format("ID: {0}, {1}", Id, VietosKodas);
                else
                    return string.Format("ID: __, {0}", VietosKodas);
            }
        }

        public static Kelintas ParseKelintas(string kelintas)
        {
            switch (kelintas)
            {
                case "1":
                case "I":
                    return Kelintas.First;
                case "2":
                case "II":
                    return Kelintas.Second;
                case "3":
                case "III":
                    return Kelintas.Third;
                case "4":
                case "IV":
                    return Kelintas.Fourth;
                default:
                    return Kelintas.Extra;
            }
        }

    }
}
