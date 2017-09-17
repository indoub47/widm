using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public abstract class SingleRecordValidator
    {
        protected string[] mapping;
        protected ValidationMethod[] validationMethods;

        /// <summary>
        /// Performs all validations on the single record.
        /// </summary>
        /// <param name="record">welding inspection record</param>
        /// <param name="context">additional data, which is being passed along with a record</param>
        /// <returns>the list of InvalidDataInfo objects. In case there is no InvalidDataInfo returns an empty list</returns>
        public List<InvalidDataInfo> ValidateRecord(IList<object> record, RecordContext context)
        {
            List<InvalidDataInfo> invalidList = new List<InvalidDataInfo>();

            foreach (var validationMethod in validationMethods)
            {
                InvalidDataInfo info = validationMethod(record, mapping);
                if (info != null)
                {
                    info.ValidationMethod = validationMethod.Method.Name; // mostly for testing
                    info.Record = record;
                    info.Context = RecordContext.CreateFrom(context);
                    // (RecordContext kuriama kopija - tam, kad atsiradus poreikiui, būtų galima pridėti individualios info)
                    invalidList.Add(info);
                }
            }

            // always returns List<InvalidDataInfo> - empty or not
            return invalidList;
        }
    }
}
