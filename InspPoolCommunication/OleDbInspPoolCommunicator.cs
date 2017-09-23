﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WidmShared;
using DbMappings;
using InspectionLib;

namespace InspPoolCommunication
{
    public class OleDbInspPoolCommunicator : IInspPoolCommunicator
    {
        private string connectionString;
        private DataTable dTable = new DataTable();

        public OleDbInspPoolCommunicator(string dbPath = null)
        {
            if (dbPath == null)
                dbPath = Properties.Settings.Default.DbPath;
            connectionString = string.Format(Properties.Settings.Default.ConnectionString, dbPath);
        }

        public DataTable FetchById(long id)
        {
            OleDbCommand cmd = new OleDbCommand(Properties.Settings.Default.FetchByIdQuery);
            cmd.Parameters.AddWithValue("@id", id);

            DataTable dataTable;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    dataTable = fillDataTable(reader);
                }
            }
            return dataTable;
        }

        public DataTable FetchByVieta(Insp insp)
        {
            OleDbCommand cmd = new OleDbCommand(Properties.Settings.Default.FetchByVietaQuery);
            cmd.Parameters.AddWithValue("@linija", insp.Linija);
            cmd.Parameters.AddWithValue("@kelias", insp.Kelias);
            cmd.Parameters.AddWithValue("@km", insp.Km);
            addNullableParam(cmd.Parameters, insp.Pk, "@pk");
            addNullableParam(cmd.Parameters, insp.M, "@m");
            addNullableParam(cmd.Parameters, insp.Siule, "@siule");

            DataTable dataTable;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    dataTable = fillDataTable(reader);
                }
            }
            return dataTable;
        }

        public int InsertInsp(Insp insp)
        {
            OleDbCommand cmd = new OleDbCommand(Properties.Settings.Default.InsertQuery);

            setupInsertCommand(insp, cmd);

            int result;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        public int BatchInsertInsp(IList<Insp> insps)
        {
            OleDbCommand cmd = new OleDbCommand(Properties.Settings.Default.InsertQuery);
            int result = 0;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                foreach (Insp insp in insps)
                {
                    setupInsertCommand(insp, cmd);
                    result += cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        public int UpdateInspInfo(Insp insp)
        {
            OleDbCommand cmd = new OleDbCommand();
            setupUpdateCommand(insp, cmd);
            int result;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        public int BatchUpdateInsp(IList<Insp> insps)
        {
            OleDbCommand cmd = new OleDbCommand();
            int result = 0;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                foreach (Insp insp in insps)
                {
                    setupUpdateCommand(insp, cmd);
                    result += cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        private void setupInsertCommand(Insp insp, OleDbCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@linija", insp.Linija);
            cmd.Parameters.AddWithValue("@kelias", insp.Kelias);
            cmd.Parameters.AddWithValue("@km", insp.Km);
            addNullableParam(cmd.Parameters, insp.Pk, "@pk");
            cmd.Parameters.AddWithValue("@m", insp.M);
            addNullableParam(cmd.Parameters, insp.Siule, "@siule");
            cmd.Parameters.AddWithValue("@skodas", insp.SKodas);
            addNullableParam(cmd.Parameters, insp.Nr, "@suv_numer");
            cmd.Parameters.AddWithValue("@suvirino", insp.Suvirino);
            cmd.Parameters.AddWithValue("@ifas", Properties.Settings.Default.Ifas);
            cmd.Parameters.AddWithValue("@pak_suv_data", insp.TData);
            cmd.Parameters.AddWithValue("@data", insp.TData);
            cmd.Parameters.AddWithValue("@aparatas", insp.Aparatas);
            cmd.Parameters.AddWithValue("@operatorius", insp.Operatorius);
            cmd.Parameters.AddWithValue("@pastaba", insp.Pastaba);
        }

        private void setupUpdateCommand(Insp insp, OleDbCommand cmd)
        {
            string patData = "", aparatas = "", operatorius = ""; // įrašomi tinkamų stulpelių pavadinimai
            cmd.Parameters.Clear();
            if (insp.Kelintas == Kelintas.Extra)
            {
                cmd.CommandText = Properties.Settings.Default.UpdateExtraQuery;
                cmd.Parameters.AddWithValue("@pastaba", insp.Pastaba);
                cmd.Parameters.AddWithValue("@id", insp.Id);
            }
            else
            {
                switch (insp.Kelintas)
                {
                    case Kelintas.Second:
                        patData = "II_pat_data";
                        aparatas = "II_pat_aparat";
                        operatorius = "II_pat_operator";
                        break;
                    case Kelintas.Third:
                        patData = "III_pat_data";
                        aparatas = "III_pat_aparat";
                        operatorius = "III_pat_operaqtor";
                        break;
                    case Kelintas.Fourth:
                        patData = "IV_pat_data";
                        aparatas = "IV_pat_aparat";
                        operatorius = "IV_pat_operator";
                        break;
                }

                cmd.CommandText = string.Format(Properties.Settings.Default.UpdateQueryFormat, patData, aparatas, operatorius);
                cmd.Parameters.AddWithValue("@data", insp.TData);
                cmd.Parameters.AddWithValue("@aparatas", insp.Aparatas);
                cmd.Parameters.AddWithValue("@operatorius", insp.Operatorius);
                cmd.Parameters.AddWithValue("@pastaba", insp.Pastaba);
                cmd.Parameters.AddWithValue("@id", insp.Id);
            }
        }

        private void populateColumns()
        {
            foreach (MappingField fld in Mappings.AllFields)
            {
                dTable.Columns.Add(fld.Name, fld.Tipas);
            }
        }

        private DataTable fillDataTable(OleDbDataReader reader)
        {
            if (dTable.Columns.Count == 0)
            {
                populateColumns();
            }

            DataTable table = dTable.Clone();
            while (reader.Read())
            {
                DataRow row = table.NewRow();
                foreach (DataColumn col in table.Columns)
                {
                    row[col.ColumnName] = reader[Mappings.Get(col.ColumnName).DbName];
                }
                table.Rows.Add(row);
            }
            return table;
        }

        private void addNullableParam(OleDbParameterCollection parameters,
            object value, string paramName)
        {
            if (value == null)
                parameters.AddWithValue(paramName, DBNull.Value);
            else
                parameters.AddWithValue(paramName, value);
        }
    }    
}
