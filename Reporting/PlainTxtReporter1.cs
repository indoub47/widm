using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InspectionLib;
using WidmShared;

namespace Reporting
{
    public class PlainTxtReporter1 : IReporter
    {
        public virtual StringBuilder ReportDbUpdate(IList<Insp> inspList)
        {
            StringBuilder sb = new StringBuilder(DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString()).AppendLine();

            var groupped = inspList.OrderBy(x1 => x1.TData).GroupBy(xx1 => xx1.TData.Date, (key1, group1) => new
            {
                TData = key1.Date,
                GrByTData = group1.OrderBy(x2 => x2.Aparatas).GroupBy(xx2 => xx2.Aparatas, (key2, group2) => new
                {
                    Aparatas = key2,
                    GrByAparatas = group2.OrderBy(x3 => x3.Operatorius).GroupBy(xx3 => xx3.Operatorius, (key3, group3) => new
                    {
                        Operatorius = key3,
                        GrByOperatorius = group3.OrderBy(x4 => x4.SKodas).GroupBy(xx4 => xx4.SKodas, (key4, group4) => new
                        {
                            SKodas = key4,
                            GrBySKodas = group4.OrderBy(x5 => x5.Kelintas).GroupBy(xx5 => xx5.Kelintas, (key5, group5) => new
                            {
                                Kelintas = key5,
                                GrByKelintas = group5.OrderBy(x6 => x6.VietosKodas)
                            })
                        })
                    })
                })
            });

            foreach (var a1 in groupped)
            {
                sb.AppendLine(a1.TData.ToShortDateString());
                foreach (var a2 in a1.GrByTData)
                {
                    sb.AppendLine("\t" + a2.Aparatas);
                    foreach (var a3 in a2.GrByAparatas)
                    {
                        sb.AppendLine("\t\t" + a3.Operatorius);
                        foreach (var a4 in a3.GrByOperatorius)
                        {
                            foreach (var a5 in a4.GrBySKodas)
                            {
                                foreach (var a6 in a5.GrByKelintas)
                                {
                                    sb.AppendLine("\t\t\t" + a4.SKodas + " - " + Kl.Roman(a5.Kelintas) + " - " + a6.VietosKodas);
                                }
                            }
                        }
                    }
                }
                sb.AppendLine();
            }
            return sb;
        }

        public virtual StringBuilder ReportInspValidation(IList<InvalidInfo> invalidInfoList, bool addHeader = true)
        {
            IEnumerable<Dictionary<string, object>> iiList = invalidInfoList;
            StringBuilder sb = new StringBuilder();

            var groupped = iiList.OrderBy(x1 => x1["operatorId"]).GroupBy(xx1 => xx1["operatorId"], (key1, group1) => new
            {
                Operatorius = key1,
                GrByOperatorius = group1.OrderBy(x2 => x2["sheetName"]).GroupBy(xx2 => xx2["sheetName"], (key2, group2) => new
                {
                    Sheet = key2.ToString(),
                    GrBySheet = group2.OrderBy(x3 => x3["tag"]).GroupBy(xx3 => xx3["tag"], (key3, group3) => new
                    {
                        Tag = key3,
                        GrByTag = group3.OrderBy(x4 => x4["message"])
                    })
                })
            });

            foreach (var a1 in groupped)
            {
                foreach (var a2 in a1.GrByOperatorius)
                {
                    if (addHeader)
                        sb.AppendLine("Operatorius: " + a1.Operatorius + ", lentelė: " + a2.Sheet);
                    foreach (var a3 in a2.GrBySheet)
                    {
                        sb.AppendLine("\tįrašas: " + a3.Tag);
                        foreach (var a4 in a3.GrByTag)
                        {
                            sb.AppendLine("\t\t - " + a4["message"]);
                        }
                    }
                }
            }

            return sb;
        }

        public virtual StringBuilder ReportRepeatedInsps(Dictionary<string, List<Insp>> repeatedInsps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var rep in repeatedInsps)
            {
                sb.AppendLine($"{rep.Key}: " + String.Join(", ", rep.Value.Select(insp => $"{insp.Operatorius}-{Kl.Roman(insp.Kelintas)}")));
            }
            return sb;
        }

        public virtual StringBuilder ReportRecValidation(IList<InvalidInfo> invalidRecordInfoList, bool addHeader = true)
        {
            IEnumerable<Dictionary<string, object>> iriList = invalidRecordInfoList;
            StringBuilder sb = new StringBuilder();

            var groupped = iriList.OrderBy(x1 => x1["operatorId"]).GroupBy(xx1 => xx1["operatorId"], (key1, group1) => new
            {
                Operatorius = key1,
                GrByOperatorius = group1.OrderBy(x2 => x2["sheetName"]).GroupBy(xx2 => xx2["sheetName"], (key2, group2) => new
                {
                    Sheet = key2.ToString(),
                    GrBySheet = group2.OrderBy(x3 => x3["tag"]).GroupBy(xx3 => xx3["tag"], (key3, group3) => new
                    {
                        Tag = key3,
                        GrByTag = group3.OrderBy(x4 => x4["message"])
                    })
                })
            });

            foreach (var a1 in groupped)
            {
                foreach (var a2 in a1.GrByOperatorius)
                {
                    if (addHeader)
                        sb.AppendLine("Operatorius: " + a1.Operatorius + ", lentelė: " + a2.Sheet);
                    foreach (var a3 in a2.GrBySheet)
                    {
                        sb.AppendLine("\tįrašas: " + a3.Tag);
                        foreach (var a4 in a3.GrByTag)
                        {
                            sb.AppendLine("\t\t - " + a4["message"]);
                        }
                    }
                }
            }

            return sb;
        }
    }
}



