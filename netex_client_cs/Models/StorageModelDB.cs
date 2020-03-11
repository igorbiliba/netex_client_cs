using netex_client_cs.Components;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netex_client_cs.Models
{
    public class StorageModelDB
    {
        public DB db;
        const string TABLE_NAME = "storage";

        public StorageModelDB MigrateUp()
        {
            try
            {
                db.Execute(@"CREATE TABLE @storage (
                                key char(63) PRIMARY KEY NOT NULL UNIQUE, 
                                value char(255) NOT NULL
                            );".Replace("@storage", TABLE_NAME));
            }
            catch (Exception) { }

            return this;
        }

        public void Set(string key, string value)
        {
            try
            {
                string SQL = @"INSERT OR REPLACE INTO @storage (key, value) VALUES (@key, @value);"
                .Replace("@storage", TABLE_NAME);

                SQLiteParameter[] parameters = {
                    new SQLiteParameter("@key",   key),
                    new SQLiteParameter("@value", value)
                };

                db.Execute(SQL, parameters);
            }
            catch (Exception) { }
        }

        public string Get(string key)
        {
            List<string> listUsed = new List<string>();

            try
            {
                SQLiteParameter[] parameters = { new SQLiteParameter("@key", key) };

                DbDataReader reader = db.All(
                    @"SELECT value
                    FROM @storage
                    WHERE key LIKE @key;".Replace("@storage", TABLE_NAME)
                    , parameters
                );

                reader.Read();
                return reader.GetValue(0).ToString();
            }
            catch (Exception) { }

            return null;
        }
    }
}
