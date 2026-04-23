using MySql.Data.MySqlClient;
using System.Data;
using System.Text.Json.Nodes;

namespace FISSystem.Services;

public class FisMySqlHelperAccountsGeneral
{
    string connStr = "server=misdb.wright.edu;" +
        "user=w109cdn;" +
        "database=w109cdn_Assignment3;" +
        "password=WRU0ZgM78H4;";

    public JsonObject GetObjectFromReader(IDataReader reader)
    {
        var obj = new JsonObject();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i);
            if (reader.IsDBNull(i))
            {
                obj[name] = null;
            }
            else
            {
                var value = reader.GetValue(i);
                obj[name] = JsonValue.Create(value);
            }
        }
        return obj;
    }

    public void DeleteTableData()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            string deleteQuery = "SET FOREIGN_KEY_CHECKS = 0;" +
                "TRUNCATE TABLE accounts_receivable;" +
                "TRUNCATE TABLE accounts_payable;" +
                "TRUNCATE TABLE raw_material;" +
                "TRUNCATE TABLE transactions;" +
                "SET FOREIGN_KEY_CHECKS = 1;";
            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
}