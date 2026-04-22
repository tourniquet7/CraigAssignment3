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