using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Dmp.Examples.GrundriskIntegration
{
    public static class Preliminaryscreening
    {
        public static async Task StartPreliminarySceening(GrundriskClient grundriskClient)
        {
            var json = await System.IO.File.ReadAllTextAsync("request.json");
            var preliminaryScreeningCommand = JsonConvert.DeserializeObject<StartPreliminaryScreeningCommand>(json);

            Console.WriteLine("Retrieving preliminaryscreening details");
            var preliminaryScreeningResult = await grundriskClient.StartPreliminaryScreeningAsync(preliminaryScreeningCommand);

            
            foreach (var screening in preliminaryScreeningResult)
            {  

                Console.WriteLine($"Retrieving prelimaryscreenings from {screening.CompoundName} with a concentration {screening.Conc100mGrundRisk} and a factor of {screening.Factor} and with flag value {screening.Flag}");
               
            }
        }

    }
}
