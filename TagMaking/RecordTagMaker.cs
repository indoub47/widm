using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace TagMaking
{
    public abstract class RecordTagMaker : IRecordTagMaker
    {
        protected string[] mapping;

        public abstract string Make(IList<object> record);

        // pagamina vietos kodo surogatą iš nepatikrintų duomenų - reikalingas kažkaip 
        // pažymėti, kurioje Google Sheet eilutėje aptiktos problemos
        protected string dataRowToVietosKodas(IList<object> record)
        {
            return string.Format("{0:00}.{1:0}{2:000}.{3:#00}.{4:#00}.{5:##0}",
                getRowCellObject("linija", record),
                getRowCellObject("kelias", record),
                getRowCellObject("km", record),
                getRowCellObject("pk", record),
                getRowCellObject("m", record),
                getRowCellObject("siule", record));
        }

        // jeigu tuščias row cell, grąžina brūkšnelį. 
        protected object getRowCellObject(string columnName, IList<object> record)
        {
            object obj = getRowItem(columnName, record);
            if (isEmpty(obj))
            {
                return "_";
            }

            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return obj;
            }

        }

        private object getRowItem(string colName, IList<object> record)
        {
            // iš tikrųjų čia apsisaugoti nuo to dalyko, kad row gali būti trumpesnis
            // negu mapping
            int ind = Array.IndexOf(mapping, colName);
            if (record.Count - 1 < ind) return null;
            return record[ind];
        }

        private bool isEmpty(object value)
        {
            return value == null || value.ToString().Trim() == string.Empty;
        }
    }
}
