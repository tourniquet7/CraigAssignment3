
using FISSystem.Models;
using FISSystem.Pages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace FISSystem.Services;

public class FisMySqlHelperAccountsReceivable
{
    string connStr = "server=misdb.wright.edu;" +
        "user=w109cdn;" +
        "database=w109cdn_Assignment3;" +
        "password=WRU0ZgM78H4;";


    public void PopulateAccountsReceivable(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            // define the SQL query to be used to get records
            string insertQuery = "INSERT INTO accounts_receivable (AccountsReceivableID, CustomerID, Amount, DueDate)" +
                "VALUES (@ACCOUNTSRECEIVABLEID, @CUSTOMERID, @AMOUNT, @DUEDATE);";

            foreach (var item in items.AsArray())
            {
                var accountsReceivableId = item["AccountsReceivableID"]?.GetValue<int>() ?? 0;
                var customerId = item["CustomerID"]?.GetValue<int>() ?? 0;
                var amount = item["Amount"]?.GetValue<double>() ?? 0.0;
                DateTime dueDate = ((DateTime)item["DueDate"]);


                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {

                    // setup parameters for the INSERT statement
                    cmd.Parameters.AddWithValue("@ACCOUNTSRECEIVABLEID", accountsReceivableId);
                    cmd.Parameters.AddWithValue("@CUSTOMERID", customerId);
                    cmd.Parameters.AddWithValue("@AMOUNT", amount);
                    cmd.Parameters.AddWithValue("@DUEDATE", dueDate);

                    // open connection
                    conn.Open();

                    // run query
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }


    public JsonArray GetAccountsReceivable()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM accounts_receivable " +
                "ORDER BY DueDate ASC;";
            using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
            {
                conn.Open();

                // Use ExecuteReader to fetch rows (ExecuteNonQuery returns affected rows count)
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

                        results.Add(obj);
                    }
                }

                conn.Close();
            }
        }

        return results;
    }
}