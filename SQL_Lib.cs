using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SQL_lib
{
    public class SQL_Queries
    {
        public static string PublicDatabaseName { get; set; }
        public static string PublicTableName { get; set; }

        static SqlConnection can = new SqlConnection();
        static SqlCommand cmd = new SqlCommand();
        static void CheckFieldsAndValues(string DataBaseName, string TableName, List<string> Field, object[] Value)
        {
            if (Field.Count != Value.Length)
            {
                throw new Exception("The number of fields and values must be equal");
            }
            int Columns_Count = GetSQLInfo.GetColumnsCount(DataBaseName, TableName);

            if (Field.Count > Columns_Count || Value.Length > Columns_Count)
            {
                throw new Exception("number of fileds or values can not be more than sql columns count");
            }
        }

        public class CRUD
        {
            public static bool Insert(string DataBaseName, string TableName, List<string> field, object[] values)
            {
                CheckFieldsAndValues(DataBaseName, TableName, field, values);
                try
                {
                    //===== Make a string for fields
                    string Table_fields = "(";
                    foreach (string n in field)
                    {
                        Table_fields += n + ",";
                    }
                    Table_fields = Table_fields.TrimEnd(',');
                    Table_fields += ")";

                    //===== Make a string for Values
                    string Table_Values = "(";
                    foreach (var n in values)
                    {
                        Table_Values += "N'" + n + "',";
                    }
                    Table_Values = Table_Values.TrimEnd(',');
                    Table_Values += ")";

                    //===== Make Command text
                    string FullCommandText = "insert into " + TableName + Table_fields + "values" + Table_Values;

                    //===== Body of insert to DB
                    can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                    can.Open();
                    cmd.CommandText = FullCommandText;
                    cmd.Connection = can;
                    cmd.ExecuteNonQuery();
                    can.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    can.Close();
                }
            }
            public static void Insert(List<string> field, object[] values)
            {
                Insert(PublicDatabaseName, PublicTableName, field, values);
            }
            public static void Insert(object[] values)
            {
                List<string> field = GetSQLInfo.GetColumnsName();
                Insert(PublicDatabaseName, PublicTableName, field, values);
            }
            public static void Insert(string DataBaseName, string TableName, object[] values)
            {
                List<string> field = GetSQLInfo.GetColumnsName();
                Insert(DataBaseName, TableName, field, values);
            }
            public static bool Edit(string DataBaseName, string TableName, List<string> field, object[] values, string keyField, object keyValue)
            {
                CheckFieldsAndValues(DataBaseName, TableName, field, values);
                try
                {
                    //===== Make Query string
                    string query_text = "update " + TableName + " set ";
                    for (int n = 0; n < field.Count; n++)
                    {
                        query_text += field[n] + " = N'" + values[n] + "',";
                    }

                    query_text = query_text.TrimEnd(',');
                    query_text = query_text + " where " + keyField + " = '" + keyValue + "'";

                    //===== Body of insert to DB
                    can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                    can.Open();
                    cmd.CommandText = query_text;
                    cmd.Connection = can;
                    cmd.ExecuteNonQuery();
                    can.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    can.Close();
                }
            }
            public static void Edit(List<string> field, object[] values, string keyField, object keyValue)
            {
                Edit(PublicDatabaseName, PublicTableName, field, values, keyField, keyValue);
            }
            public static void Edit(object[] values, string keyField, object keyValue)
            {
                List<string> field = GetSQLInfo.GetColumnsName();
                Edit(PublicDatabaseName, PublicTableName, field, values, keyField, keyValue);
            }
            public static void Edit(string DataBaseName, string TableName, object[] values, string keyField, object keyValue)
            {
                List<string> field = GetSQLInfo.GetColumnsName();
                Edit(DataBaseName, TableName, field, values, keyField, keyValue);
            }
            public static bool Delete(string DataBaseName, string TableName, string field, object value)
            {
                try
                {
                    string DeleteQuery = "DELETE FROM " + TableName + " where " + field + " = '" + value + "' ";
                    can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                    can.Open();
                    cmd.CommandText = DeleteQuery;
                    cmd.Connection = can;
                    cmd.ExecuteNonQuery();
                    can.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    can.Close();
                }
            }
            public static void Delete(string field, object value)
            {
                Delete(PublicDatabaseName, PublicTableName, field, value);
            }
            public static DataTable SelectAll()
            {
                return SelectAll(PublicDatabaseName, PublicTableName);
            }
            public static DataTable Search(string DataBaseName, string TableName, string field, object value)
            {
                string SearchQuery = "select * from " + TableName + " where " + field + " like N'%" + value + "%' ";
                can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                can.Open();
                cmd.CommandText = SearchQuery;
                cmd.Connection = can;
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(SearchQuery, can);
                da.Fill(dt);
                can.Close();
                return dt;
            }
            public static DataTable Search(string field, object value)
            {
                return Search(PublicDatabaseName, PublicTableName, field, value);
            }
            public static DataTable Search(string DataBaseName, string TableName, List<string> field, object[] value)
            {
                CheckFieldsAndValues(DataBaseName, TableName, field, value);
                string SearchQuery = "select * from " + TableName + " where ";

                string fieldQuery = "";
                for (int i = 0; i < field.Count(); i++)
                {
                    fieldQuery += field[i] + " like N'%" + value[i] + "%' and ";
                }
                fieldQuery = fieldQuery.Remove(fieldQuery.Length - 4);
                SearchQuery += fieldQuery;
                can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                can.Open();
                cmd.CommandText = SearchQuery;
                cmd.Connection = can;
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(SearchQuery, can);
                da.Fill(dt);
                can.Close();
                return dt;
            }
            public static DataTable Search(List<string> field, object[] value)
            {
                return Search(PublicDatabaseName, PublicTableName, field, value);
            }
            public static DataTable SelectAll(string DataBaseName, string TableName)
            {
                string SearchQuery = "select * from " + TableName;
                can.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(SearchQuery, can);
                da.Fill(dt);
                can.Close();
                return dt;
            }
        }

        public class GetSQLInfo
        {
            static SqlConnection can_2 = new SqlConnection();
            public static List<string> GetColumnsName(string DataBaseName, string TableName)
            {
                string queryStr = "SELECT * FROM " + TableName;
                can_2.ConnectionString = "data source=(local);initial catalog=" + DataBaseName + ";integrated security=true";
                SqlDataAdapter adapter = new SqlDataAdapter(queryStr, can_2);
                DataSet ds = new DataSet();
                adapter.Fill(ds, TableName);

                int CountColumns = ds.Tables[0].Columns.Count;
                List<string> ColumnArray = new List<string>();
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    ColumnArray.Add(ds.Tables[0].Columns[i].ToString());
                }
                can_2.Close();
                return ColumnArray;
            }
            public static List<string> GetColumnsName()
            {
                return GetColumnsName(PublicDatabaseName, PublicTableName);
            }
            public static int GetColumnsCount(string DataBaseName, string TableName)
            {
                return GetColumnsName(DataBaseName, TableName).Count;
            }
            public static int GetColumnsCount()
            {
                return GetColumnsCount(PublicDatabaseName, PublicTableName);
            }
        }
    }
}
