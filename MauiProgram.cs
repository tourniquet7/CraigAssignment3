using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using FISSystem.Services;

namespace FISSystem
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            LoadConfig();

            return builder.Build();
        }

        private static void LoadConfig()
        {
            FisMySqlHelperAccountsReceivable mySqlHelperAccountsReceivable = new FisMySqlHelperAccountsReceivable();
            FisMySqlHelperAccountsGeneral mysqlHelperGeneral = new FisMySqlHelperAccountsGeneral();
            FisMySqlHelperAccountsPayable mysqlHelperAccountsPayable = new FisMySqlHelperAccountsPayable();
            mysqlHelperGeneral.DeleteTableData();

            using var streamRawMaterials = FileSystem.OpenAppPackageFileAsync("RawMaterials.json").Result;
            using var readerRawMaterials = new StreamReader(streamRawMaterials);
            var jsonRawMaterials = readerRawMaterials.ReadToEnd();

            var jsonParseRawMaterials = JsonNode.Parse(jsonRawMaterials);

            mysqlHelperAccountsPayable.PopulateRawMaterialsTable(jsonParseRawMaterials);

            using var streamEmployee = FileSystem.OpenAppPackageFileAsync("EmployeePayables.json").Result;
            using var readerEmployee = new StreamReader(streamEmployee);
            var jsonEmployee = readerEmployee.ReadToEnd();

            var jsonParseEmployee = JsonNode.Parse(jsonEmployee);

            mysqlHelperAccountsPayable.PopulateAccountsPayableWithEmployeeData(jsonParseEmployee);

            using var streamCustomer = FileSystem.OpenAppPackageFileAsync("CustomerData.json").Result;
            using var readerCustomer = new StreamReader(streamCustomer);
            var jsonCustomer = readerCustomer.ReadToEnd();

            var jsonParseCustomer = JsonNode.Parse(jsonCustomer);

            mySqlHelperAccountsReceivable.PopulateAccountsReceivable(jsonParseCustomer);
        }
    }
}