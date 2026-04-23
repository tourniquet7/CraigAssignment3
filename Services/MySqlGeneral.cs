
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

public class FisMySqlHelperAccountsGeneral
{
    string connStr = "server=misdb.wright.edu;" +
        "user=w109cdn;" +
        "database=w109cdn_Assignment3;" +
        "password=WRU0ZgM78H4;";



    public void DeleteTableData()
    {
        using (MySqlConnection conn = new MySqlConnection(connStr))
        {


            string deleteQuery = "SET FOREIGN_KEY_CHECKS = 0;" +
                "TRUNCATE TABLE accounts_receivable;" +
                "TRUNCATE TABLE accounts_payable;" +
                "TRUNCATE TABLE accounts_receivable;" +
                "TRUNCATE TABLE raw_material;" +
                "TRUNCATE TABLE transactions;" +
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
}