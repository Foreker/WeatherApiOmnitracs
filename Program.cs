using System;
using Topshelf;

namespace DallasWeatherService
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<WeatherProcess>(s =>
                {
                    s.ConstructUsing(weatherProcess => new WeatherProcess());
                    s.WhenStarted(weatherProcess => weatherProcess.Start());
                    s.WhenStopped(weatherProcess => weatherProcess.Stop());
                });

                x.RunAsLocalService();
                x.SetServiceName("WeatherService");
                x.SetDisplayName("WeatherService");
                x.SetDescription("This is the weather service for Onmitracs");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
