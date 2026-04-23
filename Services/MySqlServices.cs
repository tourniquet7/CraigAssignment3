
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

public class FisMySqlHelper
{
    string connStr = "server=misdb.wright.edu;" +
        "user=w109cdn;" +
        "database=w109cdn_Assignment3;" +
        "password=WRU0ZgM78H4;";

    bool setPastDueNextTime = true;


    public void DeleteTableData()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {


            string deleteQuery = "SET FOREIGN_KEY_CHECKS = 0;" +
                "TRUNCATE TABLE transactions;" +
                "TRUNCATE TABLE accounts_payable;" +
                "TRUNCATE TABLE accounts_receivable;" +
                "TRUNCATE TABLE raw_material;" +
                "SET FOREIGN_KEY_CHECKS = 1;";
            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
            {

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }

        }
    }


    public void PopulateRawMaterialsTable(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            

            // define the SQL query to be used to get records
            string insertQuery = "INSERT INTO raw_material (RawMaterialID, PreferredVendorID, Name, UnitOfMeasurement, CurrentInventory, LowInventoryLevel, InventoryReplenishLevel, CurrentInventoryPlusOrdered)" +
                "VALUES (@RAWMATERIALID, @PREFERREDVENDORID, @NAME, @UNITOFMEASUREMENT, @CURRENTINVENTORY, @LOWINVENTORYLEVEL, @INVENTORYREPLENISHLEVEL, @CURRENTINVENTORYPLUSORDERED);";

            foreach (var item in items.AsArray())
            {
                var rawId = item["RawMaterialID"]?.GetValue<int>() ?? 0;
                var prefVendor = item["PreferredVendorID"]?.GetValue<int>() ?? 0;
                var name = item["Name"]?.GetValue<string>() ?? string.Empty;
                var unit = item["UnitOfMeasurement"]?.GetValue<string>() ?? string.Empty;
                var currentInv = item["CurrentInventory"]?.GetValue<decimal>() ?? 0m;
                var lowInv = item["LowInventoryLevel"]?.GetValue<decimal>() ?? 0m;
                var replenish = item["InventoryReplenishLevel"]?.GetValue<decimal>() ?? 0m;
                var currentInventoryPlusOrdered = item["CurrentInventoryPlusOrdered"]?.GetValue<int>() ?? 0;


                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {

                    // setup parameters for the INSERT statement
                    cmd.Parameters.AddWithValue("@RAWMATERIALID", rawId);
                    cmd.Parameters.AddWithValue("@PREFERREDVENDORID", prefVendor);
                    cmd.Parameters.AddWithValue("@NAME", name);
                    cmd.Parameters.AddWithValue("@UNITOFMEASUREMENT", unit);
                    cmd.Parameters.AddWithValue("@CURRENTINVENTORY", currentInv);
                    cmd.Parameters.AddWithValue("@LOWINVENTORYLEVEL", lowInv);
                    cmd.Parameters.AddWithValue("@INVENTORYREPLENISHLEVEL", replenish);
                    cmd.Parameters.AddWithValue("@CURRENTINVENTORYPLUSORDERED", currentInventoryPlusOrdered);

                    // open connection
                    conn.Open();

                    // run query
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }


        }
    }

    public void PopulateAccountsPayableWithEmployeeData(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {



            // define the SQL query to be used to get records
            string insertQuery = "INSERT INTO accounts_payable (EmployeeId, Amount, DueDate, PaymentStatus, EmployeeDirectDeposit)" +
                "VALUES (@EMPLOYEEID, @AMOUNT, @DUEDATE, @PAYMENTSTATUS, @DIRECTDEPOSIT);";

            foreach (var item in items.AsArray())
            {
                int employeeId = item["EmployeeId"]?.GetValue<int>() ?? 0;
                double amount = double.Parse(item["Amount"]?.ToString() ?? "0");
                var dueDate = item["DueDate"]?.GetValue<DateTime>() ?? DateTime.MinValue;
                string paymentStatus = item["PaymentStatus"]?.GetValue<string>() ?? string.Empty;
                bool directDeposit = item["EmployeeDirectDeposit"]?.GetValue<bool>() ?? false;


                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {

                    // setup parameters for the INSERT statement
                    cmd.Parameters.AddWithValue("@EMPLOYEEID", employeeId);
                    cmd.Parameters.AddWithValue("@AMOUNT", amount);
                    cmd.Parameters.AddWithValue("@DUEDATE", dueDate);
                    cmd.Parameters.AddWithValue("@PAYMENTSTATUS", paymentStatus);
                    cmd.Parameters.AddWithValue("@DIRECTDEPOSIT", directDeposit);

                    // open connection
                    conn.Open();

                    // run query
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }


        }
    }

    public void PopulateTransactions(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            
        }
    }


    private double GenerateRandomMoneyAmount()
    {
        int min = 200;
        int max = 10000;
        double randomNumber = System.Random.Shared.Next(min, max);
        double finalNumber = Math.Round(randomNumber, 2);

        return finalNumber;
    }

    public void CreateRawMaterialTransaction(string id, int orderAmount)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            const string selectIndividualQuery = "SELECT PreferredVendorID " +
                "FROM raw_material " +
                "WHERE RawMaterialID = @ID";

            string vendorId = null;

            using (MySqlCommand cmd = new MySqlCommand(selectIndividualQuery, conn))
            {

                cmd.Parameters.AddWithValue("@ID", int.Parse(id));

                // open connection
                conn.Open();

                // run query
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vendorId = reader["PreferredVendorID"] != DBNull.Value ? reader["PreferredVendorID"].ToString() : null;
                        
                    }
                }

                conn.Close();

            }

            const string insertTransactionQuery = "INSERT INTO accounts_payable (" +
                    "RawMaterialID," +
                    "RawMaterialQty," +
                    "VendorID," +
                    "Amount," +
                    "DueDate," +
                    "PaymentStatus" +
                ")" +
                "VALUES(" +
                    "@RAWMATERIALID," +
                    "@QUANTITYORDER," +
                    "@VENDORID," +
                    "@AMOUNT," +
                    "@DUEDATE," +
                    "@PAYMENTSTATUS" +
                ");";    


            using (MySqlCommand cmd = new MySqlCommand(insertTransactionQuery, conn))
            {

                var amount = GenerateRandomMoneyAmount();

                DateTime today = DateTime.Now;

                // Add 2 days
                DateTime twoDaysFromNow;

                if (setPastDueNextTime)
                {
                    twoDaysFromNow = today.AddDays(2);
                } else
                {
                    twoDaysFromNow = today.AddDays(-2);
                }

                setPastDueNextTime = !setPastDueNextTime;



                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@RAWMATERIALID", id);
                cmd.Parameters.AddWithValue("@QUANTITYORDER", orderAmount);
                cmd.Parameters.AddWithValue("@VENDORID", vendorId);
                cmd.Parameters.AddWithValue("@AMOUNT", amount);
                cmd.Parameters.AddWithValue("@DUEDATE", twoDaysFromNow);
                cmd.Parameters.AddWithValue("@PAYMENTSTATUS", "Pending");

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }

    public void CreateAccountsPayableTransaction(string accountsID, string payeeID, double amount, string transactionType)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            string insertTransactionQuery;

            if (transactionType == "vendor")
            {
                insertTransactionQuery = "INSERT INTO transactions (" +
                    "AccountsPayableID," +
                    "Amount," +
                    "Date," +
                    "VendorID" +
                ") " +
                "VALUES (" +
                    "@ACCOUNTSID," +
                    "@AMOUNT," +
                    "@DATE," +
                    "@PAYEEID" +
                ");";
            } else
            {
                insertTransactionQuery = "INSERT INTO transactions (" +
                    "AccountsPayableID," +
                    "Amount," +
                    "Date," +
                    "EmployeeId" +
                ")" +
                " VALUES (" +
                    "@ACCOUNTSID," +
                    "@AMOUNT," +
                    "@DATE," +
                    "@PAYEEID" +
                ");";
            }

            


            using (MySqlCommand cmd = new MySqlCommand(insertTransactionQuery, conn))
            {

                DateTime today = DateTime.Now;

                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@ACCOUNTSID", accountsID);
                cmd.Parameters.AddWithValue("@AMOUNT", -amount);
                cmd.Parameters.AddWithValue("@DATE", today);
                cmd.Parameters.AddWithValue("@PAYEEID", payeeID);

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }

    public void UpdateRawMaterialAfterOrder(string id, int orderAmount)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string updateOrderedQuery = "UPDATE raw_material " +
                "SET CurrentInventoryPlusOrdered = CurrentInventory + @ORDERAMOUNT " +
                "WHERE RawMaterialID = @ID;";

            using (MySqlCommand cmd = new MySqlCommand(updateOrderedQuery, conn))
            {

                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@ORDERAMOUNT", orderAmount);

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }

    public void UpdateAccountsPayableAfterTransaction(string id)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string updateOrderedQuery = "UPDATE accounts_payable " +
                "SET PaymentStatus = 'Paid' " +
                "WHERE AccountsPayableID = @ID;";

            using (MySqlCommand cmd = new MySqlCommand(updateOrderedQuery, conn))
            {

                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@ID", id);

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
    }

    public JsonObject GetRawMaterial(string id)
    {
        var result = new JsonObject();
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectIndividualQuery = "SELECT CurrentInventory, LowInventoryLevel, InventoryReplenishLevel, CurrentInventoryPlusOrdered " +
                "FROM raw_material " +
                "WHERE RawMaterialID = @ID";
            using (MySqlCommand cmd = new MySqlCommand(selectIndividualQuery, conn))
            {

                cmd.Parameters.AddWithValue("@ID", int.Parse(id));

                // open connection
                conn.Open();

                // run query
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result["CurrentInventory"] = reader["CurrentInventory"] != DBNull.Value ? JsonValue.Create(reader["CurrentInventory"]) : null;
                        result["LowInventoryLevel"] = reader["LowInventoryLevel"] != DBNull.Value ? JsonValue.Create(reader["LowInventoryLevel"]) : null;
                        result["InventoryReplenishLevel"] = reader["InventoryReplenishLevel"] != DBNull.Value ? JsonValue.Create(reader["InventoryReplenishLevel"]) : null;
                        result["CurrentInventoryPlusOrdered"] = reader["CurrentInventoryPlusOrdered"] != DBNull.Value ? JsonValue.Create(reader["CurrentInventoryPlusOrdered"]) : null;
                    }
                }

                conn.Close();

            }
        }
        return result;
    }

    public JsonObject GetAccountPayable(string id)
    {
        var result = new JsonObject();
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectIndividualQuery = "SELECT * " +
                "FROM accounts_payable " +
                "WHERE AccountsPayableID = @ID";
            using (MySqlCommand cmd = new MySqlCommand(selectIndividualQuery, conn))
            {

                cmd.Parameters.AddWithValue("@ID", int.Parse(id));

                // open connection
                conn.Open();

                // run query
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result["AccountsPayableID"] = reader["AccountsPayableID"] != DBNull.Value ? JsonValue.Create(reader["AccountsPayableID"]) : null;
                        result["VendorID"] = reader["VendorID"] != DBNull.Value ? JsonValue.Create(reader["VendorID"]) : null;
                        result["EmployeeId"] = reader["EmployeeId"] != DBNull.Value ? JsonValue.Create(reader["EmployeeId"]) : null; ;
                        result["Amount"] = reader["Amount"] != DBNull.Value ? JsonValue.Create(reader["Amount"]) : null;
                    }
                }

                conn.Close();

            }
        }
        return result;
    }



    public JsonArray GetRawMaterials()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM raw_material";
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

    public JsonArray GetAccountsPayableVendor()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM accounts_payable " +
                "WHERE VendorID IS NOT NULL " +
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

    public JsonArray GetAccountsPayableEmployee()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM accounts_payable " +
                "WHERE EmployeeId IS NOT NULL " +
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

    public JsonArray GetAccountsPayableTransactions()
    {
        var results = new JsonArray();

        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectQuery = "SELECT * FROM transactions " +
                "WHERE AccountsPayableID IS NOT NULL;";
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