using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WidmShared
{

    public delegate InvalidInfo ValidationMethod(IList<object> record, string[] mapping);

    public abstract class BaseRecordValidator
    {
        protected string[] mapping;
        protected ValidationMethod[] validationMethods;
        protected IRecordTagMaker tagMaker;

        public BaseRecordValidator(string[] mapping, IRecordTagMaker tagMaker)
        {
            this.mapping = mapping;
            this.tagMaker = tagMaker;
        }

        /// <summary>
        /// Performs all validations on the single record.
        /// </summary>
        /// <param name="record">welding inspection record</param>
        /// <param name="context">additional data, which is being passed along with a record</param>
        /// <returns>the list of InvalidDataInfo objects. In case there is no InvalidDataInfo returns an empty list</returns>
        public List<InvalidInfo> ValidateRecord(IList<object> record, Dictionary<string, object> context)
        {
            List<InvalidInfo> invalidList = new List<InvalidInfo>();

            foreach (var validationMethod in validationMethods)
            {
                InvalidInfo info = validationMethod(record, mapping);
                if (info != null)
                {
                    info["method"] = validationMethod.Method.Name; // mostly for testing
                    info["record"] = record;
                    info["tag"] = tagMaker.Make(record);
                    info.Incorporate(context);
                    invalidList.Add(info);
                }
            }

            // always returns List<InvalidInfo> - empty or not
            return invalidList;
        }
    }
}
