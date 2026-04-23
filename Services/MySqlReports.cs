using MySql.Data.MySqlClient;
using System.Text.Json.Nodes;

namespace FISSystem.Services;

public class FisMySqlHelperReports
{
    string connStr = "server=misdb.wright.edu;" +
        "user=w109cdn;" +
        "database=w109cdn_Assignment3;" +
        "password=WRU0ZgM78H4;";

    bool setPastDueNextTime = true;

    FisMySqlHelperAccountsGeneral mysqlHelperGeneral = new FisMySqlHelperAccountsGeneral();

    public JsonArray GetAccountsPayable()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM accounts_payable;";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                conn.Open();

                // Use ExecuteReader to fetch rows (ExecuteNonQuery returns affected rows count)
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = mysqlHelperGeneral.GetObjectFromReader(reader);
                        results.Add(obj);
                    }
                }

                conn.Close();
            }
        }

        return results;
    }

    public JsonArray GetAccountsReceivable()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM accounts_receivable;";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                conn.Open();

                // Use ExecuteReader to fetch rows (ExecuteNonQuery returns affected rows count)
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = mysqlHelperGeneral.GetObjectFromReader(reader);
                        results.Add(obj);
                    }
                }

                conn.Close();
            }
        }

        return results;
    }

    public JsonArray GetTransactions()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM transactions " +
                "ORDER BY Date DESC;";
            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                conn.Open();

               
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = mysqlHelperGeneral.GetObjectFromReader(reader);
                        results.Add(obj);
                    }
                }

                conn.Close();
            }
        }

        return results;
    }
}