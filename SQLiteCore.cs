using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace sqlite_core
{
    public class SQLiteCore
    {
        #region Init

        #region AllowableConnectionParams
        public static readonly List<string> available_con_params = new List<string>()
         {
            "Data Souce=",
            "Version=",
            "New=",
            "UseUTF16Encoding=",
            "Password=",
            "Legacy Format=",
            "Pooling=",
            "MAX Pool Size=",
            "Read Only=",
            "DateTimeFormat=",
            "BinaryGUID=",
            "Cache Size=",
            "Page Size=",
            "Enlist=",
            "FailIfMissing=",
            "Max Page Count=",
            "Synchronous=",
            "Journal Mode=",
         };
        #endregion
        public List<string> tables_in_db
        {
            get
            {
                try
                {
                    DataTable query_result;
                    List<string> table_found = new List<string>();
                    query_result = this.run_query("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                    foreach (DataRow table_name in query_result.Rows)
                        table_found.Add(table_name["NAME"].ToString());
                    return table_found;
                }
                catch (Exception e)
                {
                    throw new Exception (e.Message + "\nObtain table names failed.");
                }
            }
        }

        public string database_connection_string { get; private set; }
        private SQLiteConnection connection = null;
        private SQLiteCommand command = null;
        private SQLiteDataReader data_reader = null;

        #endregion

        public SQLiteCore(string input_database_directory)
        {
            try
            {
                if (File.Exists(input_database_directory))
                    this.database_connection_string = String.Format("Data Source={0}", input_database_directory);
                else
                    throw new Exception("Database file not found.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SQLiteCore(Dictionary<string, string> input_connection_params)
        {
            foreach (KeyValuePair<string, string> param in input_connection_params)
            {
                if (available_con_params.Contains(param.Key))
                    this.database_connection_string += String.Format("{0}={1}", param.Key, param.Value);
            }
            if (String.Equals(this.database_connection_string, ""))
                throw new Exception("No valid connection params are identified.");
        }

        public DataTable run_query(string sql_query)
        {
            DataTable sql_result = new DataTable();
            try
            {
                using (connection = new SQLiteConnection(database_connection_string))
                {
                    connection.Open();
                    command = new SQLiteCommand(connection);
                    command.CommandText = sql_query;
                    data_reader = command.ExecuteReader();
                    sql_result.Load(data_reader);
                    data_reader.Close();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                close_connections();
                throw e;
            }
            return sql_result;
        }

        public int run_non_query(string non_sql_query)
        {
            int row_updated = 0;
            try
            {
                using (connection = new SQLiteConnection(database_connection_string))
                {
                    connection.Open();
                    command = new SQLiteCommand(connection);
                    command.CommandText = non_sql_query;
                    row_updated = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                close_connections();
                throw e;
            }
            return row_updated;
        }

        public string run_scalar_query(string sql_query)
        {
            string sql_result = "";
            using (connection = new SQLiteConnection(this.database_connection_string))
            {
                connection.Open();
                command = new SQLiteCommand(connection);
                command.CommandText = sql_query;
                var ret = command.ExecuteScalar();
                if (ret != null)
                    sql_query = ret.ToString();
            }
            return sql_query;
        }

        public void insert_row()
        {
        }

        public void update_db_element()
        {
        }

        public void delete_row()
        {
        }

        public void create_table()
        {
        }

        public void delete_table()
        {
        }

        private void close_connections()
        {
            if (this.data_reader != null && !this.data_reader.IsClosed)
                data_reader.Close();
            if (this.connection != null)
                connection.Close();
        }
    }
}
