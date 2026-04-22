using FISSystem.Models;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
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


    const string insertTransactionQuery = "INSERT INTO accounts_payable (" +
            "RawMaterialID," +
            "RawMaterialQty," +
            "VendorID," +
            "EmployeeId," +
            "TransactionID," +
            "Amount," +
            "DueDate," +
            "PaymentStatus" +
        ")" +
"       VALUES(" +
            "@RAWMATERIALID," +
            "@QUANTITYORDER," +
            "@VENDORID," +
            "@EMPLOYEEID," +
            "@TRANSACTIONID," +

            "@AMOUNT," +
            "@DUEDATE," +
            "@PAYMENTSTATUS" +
        ");";



    public void PopulateRawMaterialsTable(JsonNode items)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            string deleteQuery = "DELETE FROM raw_material";
            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, conn))
            {

                // open connection
                conn.Open();

                // run query
                cmd.ExecuteNonQuery();

                conn.Close();
            }

            // define the SQL query to be used to get records
            string insertQuery = "INSERT INTO raw_material (RawMaterialID, PreferredVendorID, Name, UnitOfMeasurement, CurrentInventory, LowInventoryLevel, InventoryReplenishLevel)" +
                "VALUES (@RAWMATERIALID, @PREFERREDVENDORID, @NAME, @UNITOFMEASUREMENT, @CURRENTINVENTORY, @LOWINVENTORYLEVEL, @INVENTORYREPLENISHLEVEL);";

            foreach (var item in items.AsArray())
            {
                var rawId = item["RawMaterialID"]?.GetValue<int>() ?? 0;
                var prefVendor = item["PreferredVendorID"]?.GetValue<int>() ?? 0;
                var name = item["Name"]?.GetValue<string>() ?? string.Empty;
                var unit = item["UnitOfMeasurement"]?.GetValue<string>() ?? string.Empty;
                var currentInv = item["CurrentInventory"]?.GetValue<decimal>() ?? 0m;
                var lowInv = item["LowInventoryLevel"]?.GetValue<decimal>() ?? 0m;
                var replenish = item["InventoryReplenishLevel"]?.GetValue<decimal>() ?? 0m;


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

                    // open connection
                    conn.Open();

                    // run query
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }


        }
    }

    private double GenerateRandomMoneyAmount()
    {
        Random rnd = new Random();

        // Define range
        double min = 200.0;
        double max = 10000.0;

        // Generate random number
        double range = max - min;
        double sample = rnd.NextDouble();
        double scaled = (sample * range) + min;

        // Round to 2 decimal places
        double finalNumber = Math.Round(scaled, 2);

        return finalNumber;
    }

    public void CreateRawMaterialTransaction(string id, int orderAmount)
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {

            const string selectIndividualQuery = "SELECT VendorID " +
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
                        vendorId = reader["VendorID"] != DBNull.Value ? reader["VendorID"].ToString() : null;
                        
                    }
                }

                conn.Close();

            }

            using (MySqlCommand cmd = new MySqlCommand(insertTransactionQuery, conn))
            {

                var amount = GenerateRandomMoneyAmount();

                DateTime today = DateTime.Now;

                // Add 2 days
                DateTime twoDaysFromNow = today.AddDays(2);

                

                // setup parameters for the INSERT statement
                cmd.Parameters.AddWithValue("@RAWMATERIALID", id);
                cmd.Parameters.AddWithValue("@QUANTITYORDER", orderAmount);
                cmd.Parameters.AddWithValue("@VENDORID", vendorId);
                cmd.Parameters.AddWithValue("@EMPLOYEEID", "");
                cmd.Parameters.AddWithValue("@AMOUNT", amount);
                cmd.Parameters.AddWithValue("@DUEDATE", twoDaysFromNow);
                cmd.Parameters.AddWithValue("@PAYMENTSTATUS", "Incomplete");

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
    }

    public JsonObject GetRawMaterial(string id)
    {
        var result = new JsonObject();
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {
            const string selectIndividualQuery = "SELECT CurrentInventory, LowInventoryLevel, InventoryReplenishLevel " +
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
}