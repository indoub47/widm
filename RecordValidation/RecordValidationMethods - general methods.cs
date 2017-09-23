using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WidmShared;

namespace RecordValidation
{
    public static partial class RecordValidationMethods
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
        private static InvalidRecordInfo validateIntegralNumber(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage,
            object[] attributes)
        {

            object value = GetRowItem(propertyName, record, mapping);
            var idi = new InvalidRecordInfo(errorMessage);

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
        private static InvalidRecordInfo ValidatePositiveLong(
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
        private static InvalidRecordInfo ValidatePositiveIntOrNull(
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
        private static InvalidRecordInfo ValidatePositiveIntOrZero(
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
        private static InvalidRecordInfo ValidatePositiveIntOrZeroOrNull(
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

        private static InvalidRecordInfo ValidatePositiveInt(
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

        private static InvalidRecordInfo ValidateStringFromArray(
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
                return new InvalidRecordInfo(errorMessage);
            }
            else
            {
                return null;
            }
        }
    }
}
