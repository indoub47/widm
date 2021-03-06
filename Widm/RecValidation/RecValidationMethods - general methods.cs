﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Widm
{
    public static partial class RecValidationMethods
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

        private static string[] galimosLinijos = Properties.RecValidation.Default.Linijos.Split(',').Select(x => x.Trim()).ToArray();
        private static string[] galimiSkodai = Properties.RecValidation.Default.SalygKodai.Split(',').Select(x => x.Trim()).ToArray();
        private static string[] galimiAparatai = Properties.RecValidation.Default.Aparatai.Split(',').Select(x => x.Trim()).ToArray();
        private static string[] galimosSuvirinimoImones = Properties.RecValidation.Default.Suvirino.Split(',').Select(x => x.Trim()).ToArray();
        private static string[] galimiKelinti = Properties.RecValidation.Default.Kelinti.Split(',').Select(x => x.Trim()).ToArray();
        private static string[] galimiPavojingumai = Properties.RecValidation.Default.Pavojingumai.Split(',').Select(x => x.Trim()).ToArray();


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
        private static InvalidInfo validateIntegralNumber(
            string propertyName,
            IList<object> record,
            string[] mapping,
            string errorMessage,
            object[] attributes)
        {

            object value = GetRowItem(propertyName, record, mapping);
            InvalidInfo ii = new InvalidInfo(errorMessage);

            if (IsEmpty(value))
            {
                if (attributes.FirstOrDefault(x => x is AllowNullAttribute) == null)
                {
                    return ii;
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
                    return ii;
                }
            }
            catch
            {
                return ii;
            }
        }

        [Int64]
        private static InvalidInfo validatePositiveLong(
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
        private static InvalidInfo validatePositiveIntOrNull(
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
        private static InvalidInfo validatePositiveIntOrZero(
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
        private static InvalidInfo validatePositiveIntOrZeroOrNull(
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

        private static InvalidInfo validatePositiveInt(
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

        private static InvalidInfo validateStringFromArray(
            string propertyName,
            string[] validValues,
            IList<object> record,
            string[] mapping,
            string errorMessage, bool nullIsAllowed = false)
        {
            object value = GetRowItem(propertyName, record, mapping);

            if (IsEmpty(value))
            {
                if (nullIsAllowed)
                    return null;
                else
                    return new InvalidInfo(errorMessage);
            }

            if (Array.IndexOf(validValues, value.ToString().Trim()) == -1)
                return new InvalidInfo(errorMessage);
            else
                return null;
        }
    }
}
