using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using InspectionLib;


namespace WidmShared
{
    public delegate IList<InvalidInfo> InspValidateMethod(Insp inspection, DataTable records);

    public class InspValidator
    {
        protected InspValidateMethod _validationMethod;
        protected DataFetchMethod _dataFetchMethod;
        protected IInspTagMaker _tagMaker;

        public InspValidator(InspValidateMethod validateMethod, DataFetchMethod fetchMethod, IInspTagMaker tagMaker)
        {
            _validationMethod = validateMethod;
            _dataFetchMethod = fetchMethod;
            _tagMaker = tagMaker;
        }

        public IList<InvalidInfo> Validate(Insp insp, Dictionary<string, object> context)
        {
            DataTable dTable = _dataFetchMethod(insp);
            IList<InvalidInfo> invalidInfoList = _validationMethod(insp, dTable);
            foreach (InvalidInfo ii in invalidInfoList)
            {
                ii["insp"] = insp;
                ii["tag"] = _tagMaker.Make(insp);
                ii.Incorporate(context);
            }
            return invalidInfoList;
        }
    }
}
