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

            FisMySqlHelper mysqlHelper = new FisMySqlHelper();
            mysqlHelper.DeleteTableData();

            using var streamRawMaterials = FileSystem.OpenAppPackageFileAsync("RawMaterials.json").Result;
            using var readerRawMaterials = new StreamReader(streamRawMaterials);
            var jsonRawMaterials = readerRawMaterials.ReadToEnd();

            var jsonParseRawMaterials = JsonNode.Parse(jsonRawMaterials);

            
            mysqlHelper.PopulateRawMaterialsTable(jsonParseRawMaterials);

            using var streamEmployee = FileSystem.OpenAppPackageFileAsync("EmployeePayables.json").Result;
            using var readerEmployee = new StreamReader(streamEmployee);
            var jsonEmployee = readerEmployee.ReadToEnd();

            var jsonParseEmployee = JsonNode.Parse(jsonEmployee);

            mysqlHelper.PopulateAccountsPayableWithEmployeeData(jsonParseEmployee);


        }
    }
}
