using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Widm
{
    public class MappingField
    {
        public string Name;
        public string DbName;
        public string TypeName;

        public Type Tipas
        {
            get
            {
                return Type.GetType(TypeName);
            }
        }
    }

    public static class Mappings
    {
        private static MappingField[] instance { get; set; }
        static Mappings()
        {
            instance = JsonConvert.DeserializeObject<MappingField[]>(
                    Properties.InspPoolMapping.Default.JsonString);
        }

        public static MappingField Get(string name)
        {
            return instance.First(mf => mf.Name == name);
        }

        public static MappingField GetByDBName(string dbName)
        {
            return instance.First(mf => mf.DbName == dbName);
        }

        public static int Index(string name)
        {
            return Array.FindIndex(instance, (x) => x.Name == name);
        }

        public static string DBColName(string name)
        {
            return Get(name).DbName;
        }

        public static int IndexByDbName(string dbName)
        {
            return Array.FindIndex(instance, (x) => x.DbName == dbName);
        }

        public static MappingField Get(int index)
        {
            return instance[index];
        }

        public static int Length
        {
            get
            {
                return instance.Length;
            }
        }

        public static MappingField[] AllFields
        {
            get
            {
                MappingField[] copied = new MappingField[Length];
                instance.CopyTo(copied, 0);
                return copied;
            }
        }
    }
}
