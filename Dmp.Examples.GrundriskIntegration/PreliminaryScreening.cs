using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Dmp.Examples.GrundriskIntegration
{
    public static class Preliminaryscreening
    {
        public static async Task StartPreliminarySceening(GrundriskClient grundriskClient)
        {

            var preliminaryScreeningCommand = new StartPreliminaryScreeningCommand();
            List<string> compounds = new List<string>();
            List<BranchActivity> branchactivities = new List<BranchActivity>();        
            List<string> v1Shapes = new List<string>();
            List<string> v2Shapes = new List<string>();
            compounds.Add("0703");
            compounds.Add("0490");
            preliminaryScreeningCommand.PollutantComponentCodes = compounds;
       

            var activity1 = new BranchActivity();
            activity1.ActivityCode = "999";
            activity1.PollutionCauseCode = "50.20.10";
            branchactivities.Add(activity1);

            var activity2 = new BranchActivity();
            activity2.ActivityCode = "006";
            activity2.PollutionCauseCode = "50.50.00";
            branchactivities.Add(activity2);

            var activity3 = new BranchActivity();
            activity3.ActivityCode = "999";
            activity3.PollutionCauseCode = "25.12.00";
            branchactivities.Add(activity3);
            
            preliminaryScreeningCommand.Activities =branchactivities;

            var v1Shape1 = "POLYGON ((554931.9389 6145817.3598, 554943.7005 6145814.4546, 554943.7377 6145814.4366, 554957.3742 6145803.8961, 554963.2915 6145805.0073, 554963.3599 6145804.9957, 554982.2349 6145794.1282, 554982.2398 6145794.1252, 554983.5714 6145793.2533, 554995.1822 6145842.5366, 554992.8408 6145844.0876, 554965.4258 6145862.191, 554960.6086 6145861.1682, 554948.7069 6145842.9933, 554957.3674 6145837.738, 554957.4005 6145837.5998, 554948.9235 6145823.9228, 554948.7887 6145823.8888, 554939.6435 6145829.1403, 554931.9389 6145817.3598))";
            var v1Shape2 = "POLYGON ((554944.9397 6145837.2277, 554939.7532 6145829.3079, 554948.8044 6145824.1104, 554957.1773 6145837.6194, 554948.5973 6145842.8258, 554944.9397 6145837.2278, 554944.9397 6145837.2277))";

            v1Shapes.Add(v1Shape1);
            v1Shapes.Add(v1Shape2);
            preliminaryScreeningCommand.V1ShapeWkts = v1Shapes;
            
            var v2Shape1="POLYGON ((554944.9397 6145837.2277, 554939.7532 6145829.3079, 554948.8044 6145824.1104, 554957.1773 6145837.6194, 554948.5973 6145842.8258, 554944.9397 6145837.2278, 554944.9397 6145837.2277))";
            v2Shapes.Add(v2Shape1);
            preliminaryScreeningCommand.V2ShapeWkts = v2Shapes;


            Console.WriteLine("Retrieving preliminaryscreening details");
            var preliminaryScreeningResult = await grundriskClient.StartPreliminaryScreeningAsync(preliminaryScreeningCommand);

            
            foreach (var screening in preliminaryScreeningResult)
            {  

                Console.WriteLine($"Retrieving prelimaryscreenings from {screening.CompoundName} with a concentration {screening.Conc100mGrundRisk} and a factor of {screening.Factor} and with flag value {screening.Flag}");
               
            }
        }

    }
}
