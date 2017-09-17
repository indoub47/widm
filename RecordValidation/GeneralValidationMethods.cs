﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    internal static class GeneralValidationMethods
    {
        [AttributeUsage(AttributeTargets.Method)]
        private class AllowZeroAttribute : System.Attribute
        { }

        [AttributeUsage(AttributeTargets.Method)]
        private class AllowNullAttribute : System.Attribute
        { }

        [AttributeUsage(AttributeTargets.Method)]
        private class AllowNegativeAttribute : System.Attribute
        { }

        [AttributeUsage(AttributeTargets.Method)]
        private class Int64Attribute : System.Attribute
        { }

        /// <summary>
        /// Generic method which validates integral number, depending on the
        /// passed attribute collection
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="record"></param>
        /// <param name="mapping"></param>
        /// <param name="errorMessage"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private static InvalidDataInfo validateIntegralNumber(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage,
            object[] attributes)
        {

            object value = GetRowItem(propertyName, record, mapping);
            var idi = new InvalidDataInfo(errorMessage);

            if (IsEmpty(value))
            {
                if (attributes.FirstOrDefault(x => x is AllowNullAttribute) == null)
                {
                    return idi;
                }
                else
                {
                    return null;
                }
            }

            try
            {
                long converted;
                if (attributes.FirstOrDefault(x => x is Int64Attribute) != null)
                {
                    converted = Convert.ToInt64(value);
                }
                else
                {
                    converted = Convert.ToInt32(value);
                }

                if (
                    converted > 0 ||
                    (converted < 0 && attributes.FirstOrDefault(x => x is AllowNegativeAttribute) != null) ||
                    (converted == 0 && attributes.FirstOrDefault(x => x is AllowZeroAttribute) != null)
                   )
                {
                    return null;
                }
                else
                {
                    return idi;
                }
            }
            catch
            {
                return idi;
            }
        }

        [Int64]
        internal static InvalidDataInfo ValidatePositiveLong(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage)
        {
            object[] attributes = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttributes(false);

            return validateIntegralNumber(
                propertyName,
                record,
                mapping,
                errorMessage, 
                attributes);
        }

        [AllowNull]
        internal static InvalidDataInfo ValidatePositiveIntOrNull(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage)
        {
            object[] attributes = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttributes(false);

            return validateIntegralNumber(
                propertyName,
                record,
                mapping,
                errorMessage,
                attributes);
        }

        [AllowZero]
        internal static InvalidDataInfo ValidatePositiveIntOrZero(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage)
        {
            object[] attributes = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttributes(false);

            return validateIntegralNumber(
                propertyName,
                record,
                mapping,
                errorMessage,
                attributes);
        }

        [AllowZero]
        [AllowNull]
        internal static InvalidDataInfo ValidatePositiveIntOrZeroOrNull(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage)
        {
            object[] attributes = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttributes(false);

            return validateIntegralNumber(
                propertyName,
                record,
                mapping,
                errorMessage,
                attributes);
        }

        internal static InvalidDataInfo ValidatePositiveInt(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage)
        {
            object[] attributes = System.Reflection.MethodBase.GetCurrentMethod().GetCustomAttributes(false);

            return validateIntegralNumber(
                propertyName,
                record,
                mapping,
                errorMessage,
                attributes);
        }

        internal static InvalidDataInfo ValidateStringFromArray(
            string propertyName,
            string[] validValues,
            IList<object> record,
            string[] mapping,
            string errorMessage, bool nullIsAllowed = false)
        {
            object value = GetRowItem(propertyName, record, mapping);
            if (
                (IsEmpty(value) && !nullIsAllowed) || 
                Array.IndexOf(validValues, value.ToString().Trim()) == -1
               )
            {
                return new InvalidDataInfo(errorMessage);
            }
            else
            {
                return null;
            }
        }

        // Tikrina, ar patikrinimo data nėra ateityje arba per giliai praeityje
        internal static bool IsNotReal(DateTime tikrinimoData)
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
        internal static object GetRowItem(string colName, IList<object> record, string[] mapping)
        {
            int ind = Array.IndexOf(mapping, colName);
            if (ind == - 1 || record.Count - 1 < ind) return null;
            return record[ind];
        }

        /// <summary>
        /// Returns true if object is null or represents an empty string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsEmpty(object value)
        {
            return value == null || value.ToString().Trim() == string.Empty;
        }

        internal static RecordContext PushMethodName(RecordContext ctx)
        {
            string mname = System.Reflection.MethodBase.GetCurrentMethod().Name;
            ctx.Objects.Add(mname);
            return ctx;
        }
    }
}
