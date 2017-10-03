using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public static partial class RecValidationMethods
    {
        // Tikrina, ar patikrinimo data nėra ateityje arba per giliai praeityje
        private static bool IsNotReal(DateTime tikrinimoData)
        {
            return (tikrinimoData > DateTime.Now) ||
                (tikrinimoData < DateTime.Now.AddDays(-Properties.Settings.Default.AllowedDaysInPast));
        }

        /// <summary>
        /// Jeigu dataRow turi mažiau Items negu mapping, 
        /// vietoje nesamo dataRow item grąžina null. 
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="mapping"></param>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        private static object GetRowItem(string colName, IList<object> record, string[] mapping)
        {
            int ind = Array.IndexOf(mapping, colName);
            if (ind == -1 || record.Count - 1 < ind) return null;
            return record[ind];
        }

        /// <summary>
        /// Returns true if object is null or represents an empty string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsEmpty(object value)
        {
            return value == null || value.ToString().Trim() == string.Empty;
        }

    }
}