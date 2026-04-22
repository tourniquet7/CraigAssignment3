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
    

    public void PopulateRawMaterials(JsonNode json)
    {

        // define the SQL query to be used to get records
        string query = "INSERT INTO raw_material (RawMaterialID, PreferredVendorID, Name, UnitOfMeasurement, CurrentInventory, LowInventoryLevel, InventoryReplenishLevel)" +
            "VALUES (@RAWMATERIALID, @PREFERREDVENDORID, @NAME, @UNITOFMEASUREMENT, @CURRENTINVENTORY, @LOWINVENTORYLEVEL, @INVENTORYREPLENISHLEVEL);";




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

            
        }
    }
}