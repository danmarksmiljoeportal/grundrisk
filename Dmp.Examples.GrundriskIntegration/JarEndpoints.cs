using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dmp.Examples.GrundriskIntegration
{
    public static class JarEndpoints
    {
        public static async Task GetJarScreeningDetailsAsync(GrundriskClient grundriskClient, string locationNumber)
        {
            Console.WriteLine($"Calling Jar endpoint /jar/locations/{locationNumber}/screenings");
            var result = await grundriskClient.GetJarScreeningDetailsAsync(locationNumber);
            var jsonResult = JsonConvert.SerializeObject(result);
            Console.WriteLine($"Receiving data:");
            Console.WriteLine($"{jsonResult}");
        }

        public static async Task GetJarRiskCalculationDetailsAsync(GrundriskClient grundriskClient, string locationNumber)
        {
            Console.WriteLine($"Calling Jar endpoint /jar/locations/{locationNumber}/riskCalculations");
            var result = await grundriskClient.GetJarRiskCalculationDetailsAsync(locationNumber);
            var jsonResult = JsonConvert.SerializeObject(result);
            Console.WriteLine($"Receiving data:");
            Console.WriteLine($"{jsonResult}");
        }
    }
}
