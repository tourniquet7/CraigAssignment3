using Microsoft.Extensions.Logging;
using System.Text.Json;

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



            return builder.Build();
        }


        private static void LoadConfig()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("RawMaterials.json").Result;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var test = JsonSerializer.Deserialize(json);
        }
    }
}
