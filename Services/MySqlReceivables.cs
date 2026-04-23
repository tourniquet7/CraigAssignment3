
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

    FisMySqlHelperAccountsGeneral mysqlHelperGeneral = new FisMySqlHelperAccountsGeneral();


    public void PopulateAccountsReceivable(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            // define the SQL query to be used to get records
            string insertQuery = "INSERT INTO accounts_receivable (AccountsReceivableID, CustomerID, Amount, DueDate, PaymentStatus)" +
                "VALUES (@ACCOUNTSRECEIVABLEID, @CUSTOMERID, @AMOUNT, @DUEDATE, @PAYMENTSTATUS);";

            foreach (var item in items.AsArray())
            {
                var accountsReceivableId = item["AccountsReceivableID"]?.GetValue<int>() ?? 0;
                var customerId = item["CustomerID"]?.GetValue<int>() ?? 0;
                var amount = item["Amount"]?.GetValue<double>() ?? 0.0;
                DateTime dueDate = ((DateTime)item["DueDate"]);
                string paymentStatus = "Pending";


                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {

                    // setup parameters for the INSERT statement
                    cmd.Parameters.AddWithValue("@ACCOUNTSRECEIVABLEID", accountsReceivableId);
                    cmd.Parameters.AddWithValue("@CUSTOMERID", customerId);
                    cmd.Parameters.AddWithValue("@AMOUNT", amount);
                    cmd.Parameters.AddWithValue("@DUEDATE", dueDate);
                    cmd.Parameters.AddWithValue("@PAYMENTSTATUS", paymentStatus);

                    // open connection
                    conn.Open();

                    // run query
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }
    }

    public JsonArray GetAccountsReceivableTransactions()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM transactions " +
                "WHERE AccountsReceivableID IS NOT NULL;";
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
                        var obj = mysqlHelperGeneral.GetObjectFromReader(reader);
                        results.Add(obj);
                    }
                }

                conn.Close();
            }
        }

        return results;
    }

    public void CreateCustomerPayment(string accountsReceivableID)
    {
        const string selectIndividualQuery = "SELECT AMOUNT " +
                "FROM accounts_receivable " +
                "WHERE AccountsReceivableID = @ID";

        double amount = 0.0;

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            using (MySqlCommand cmd = new MySqlCommand(selectIndividualQuery, conn))
            {

                cmd.Parameters.AddWithValue("@ID", accountsReceivableID);

                // open connection
                conn.Open();

                // run query
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        amount = reader["Amount"] != DBNull.Value ? Convert.ToDouble(reader["Amount"]) : 0.0;

                    }
                }

                conn.Close();

            }


            const string insertTransactionQuery = "INSERT INTO transactions (" +
                    "AccountsReceivableID," +    
                    "Amount," +
                    "Date" +
                ")" +
                "VALUES(" +
                    "@ACCOUNTSRECEIVABLEID," +
                    "@AMOUNT," +
                    "@DATE" +
                ");";


            using (MySqlCommand cmd = new MySqlCommand(insertTransactionQuery, conn))
            {

                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@ACCOUNTSRECEIVABLEID", accountsReceivableID);
                cmd.Parameters.AddWithValue("@AMOUNT", amount);
                cmd.Parameters.AddWithValue("@DATE", DateTime.Now);

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }

           
        const string updateOrderedQuery = "UPDATE accounts_receivable " +
            "SET PaymentStatus = 'Paid' " +
            "WHERE AccountsReceivableID = @ID;";

        using (MySqlCommand cmd = new MySqlCommand(updateOrderedQuery, conn))
        {

            // setup parameters for the INSERT statement
            cmd.Parameters.AddWithValue("@ID", accountsReceivableID);

            // open connection
            conn.Open();

            // run query
            cmd.ExecuteNonQuery();

            conn.Close();
        }
            
        }
    }

}