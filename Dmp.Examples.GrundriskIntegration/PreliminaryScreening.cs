using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Dmp.Examples.GrundriskIntegration
{
    public static class Preliminaryscreening
    {
        public static async Task StartPreliminarySceening(GrundriskClient grundriskClient, string webUrl)
        {
            var json = await System.IO.File.ReadAllTextAsync("request.json");
            var preliminaryScreeningCommand = JsonConvert.DeserializeObject<StartPreliminaryScreeningCommand>(json);

            Console.WriteLine("Retrieving preliminaryscreening details");
            var result = await grundriskClient.StartPreliminaryScreeningAsync(preliminaryScreeningCommand);
            
            foreach (var screeningResult in result.PreliminaryScreeningResults)
            {  
                Console.WriteLine($"Retrieving prelimaryscreenings from {screeningResult.CompoundName} " +
                    $"with a concentration {screeningResult.Conc100mGrundRisk} " +
                    $"and a factor of {screeningResult.Factor} " +
                    $"and with flag value {screeningResult.Flag}");
            }

            Console.WriteLine($"Preliminary screening link: {webUrl}/screening/preliminary/location-details/{result.LocationId}");
            Console.WriteLine($"Preliminary riskassessment link: {webUrl}/riskassessment/preliminary/location-details/{result.LocationId}");
        }
    }
}
