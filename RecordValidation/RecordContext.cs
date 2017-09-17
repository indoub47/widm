using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordValidation
{
    public struct RecordContext
    {
        public List<object> Objects { get; set; }

        public static RecordContext Create()
        {
            return new RecordContext(new List<object>());
        }

        public static RecordContext Create(params object[] context)
        {
            return new RecordContext(context);
        }

        public static RecordContext CreateFrom(RecordContext from)
        {
            return new RecordContext(from.Objects.ToArray<object>());
        }

        public RecordContext(params object[] context)
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
