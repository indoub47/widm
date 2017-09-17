using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;

namespace InspectionValidation
{
    public struct InspectionContext
    {
        public List<object> Objects { get; set; }

        public static InspectionContext Create()
        {
            return new InspectionContext(new List<object>());
        }

        public static InspectionContext Create(params object[] context)
        {
            return new InspectionContext(context);
        }

        public static InspectionContext CreateFrom(InspectionContext from)
        {
            return new InspectionContext(from.Objects.ToArray<object>());
        }

        public InspectionContext(params object[] context)
        {
            Objects = new List<object>();
            AddContext(context);
        }

        public void AddContext(params object[] context)
        {
            foreach(var ob in context)
            {
                Objects.Add(ob);
            }
        }
    }
}
