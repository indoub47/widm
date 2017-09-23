using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{

    public delegate IInvalidDataInfo ValidationMethod(IList<object> record, string[] mapping);

    public abstract class BaseRecordValidator
    {
        protected string[] mapping;
        protected ValidationMethod[] validationMethods;

        /// <summary>
        /// Performs all validations on the single record.
        /// </summary>
        /// <param name="record">welding inspection record</param>
        /// <param name="context">additional data, which is being passed along with a record</param>
        /// <returns>the list of InvalidDataInfo objects. In case there is no InvalidDataInfo returns an empty list</returns>
        public List<IInvalidDataInfo> ValidateRecord(IList<object> record, IRecordContext context)
        {
            List<IInvalidDataInfo> invalidList = new List<IInvalidDataInfo>();

            foreach (var validationMethod in validationMethods)
            {
                IInvalidDataInfo info = validationMethod(record, mapping);
                if (info != null)
                {
                    info.ValidationMethod = validationMethod.Method.Name; // mostly for testing
                    info.Record = record;
                    info.Context = (IRecordContext)context.Clone();
                    // (RecordContext kuriama kopija - tam, kad atsiradus poreikiui, būtų galima pridėti individualios info)
                    invalidList.Add(info);
                }
            }

            // always returns List<InvalidDataInfo> - empty or not
            return invalidList;
        }
    }
}
