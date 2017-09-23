using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using Interfaces;

namespace InspectionValidation
{
    public struct InspContext : IInspContext
    {
        public List<object> Objects { get; set; }

        public static InspContext Create()
        {
            return new InspContext(new List<object>());
        }

        public static InspContext Create(params object[] context)
        {
            return new InspContext(context);
        }

        public static InspContext CreateFrom(InspContext from)
        {
            return new InspContext(from.Objects.ToArray<object>());
        }

        public InspContext(params object[] context)
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
