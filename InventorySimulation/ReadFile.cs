using InventoryModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace InventorySimulation
{
    internal class ReadFile
    {
        public SimulationSystem ReadSimulationFile(string filePath)
        {
            SimulationSystem system = new SimulationSystem
            {
                DemandDistribution = new List<Distribution>(),
                LeadDaysDistribution = new List<Distribution>()
            };

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int count = 6;

                while ((line = reader.ReadLine()) != null && count > 0)
                {
                    line = line.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string key = line;

                    line = reader.ReadLine();
                    if (line == null || string.IsNullOrWhiteSpace(line) || !IsNumeric(line))
                        continue;

                    string value = line.Trim();

                    switch (key)
                    {
                        case "OrderUpTo":
                            system.OrderUpTo = int.Parse(value);
                            count--;
                            break;
                        case "ReviewPeriod":
                            system.ReviewPeriod = int.Parse(value);
                            count--;
                            break;
                        case "NumberOfDays":
                            system.NumberOfDays = int.Parse(value);
                            count--;
                            break;
                        case "StartInventoryQuantity":
                            system.StartInventoryQuantity = int.Parse(value);
                            count--;
                            break;
                        case "StartLeadDays":
                            system.StartLeadDays = int.Parse(value);
                            count--;
                            break;
                        case "StartOrderQuantity":
                            system.StartOrderQuantity = int.Parse(value);
                            count--;
                            break;
                    }
                }

                // Read DemandDistribution
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == "DemandDistribution")
                        break;
                }

                while ((line = reader.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
                {
                    line = line.Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] values = line.Split(',');

                    int demand = int.Parse(values[0]);
                    decimal probability = decimal.Parse(values[1]);

                    system.DemandDistribution.Add(new Distribution
                    {
                        Value = demand,
                        Probability = probability
                    });
                }

                CalculateCumulativeProbabilities(system.DemandDistribution);

                // Read LeadDaysDistribution
                while ((line = reader.ReadLine()) != null && !string.IsNullOrWhiteSpace(line))
                {
                    line = line.Trim();
                    if (line == "LeadDaysDistribution")
                        break;
                }

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] values = line.Split(',');

                    int leadDays = int.Parse(values[0]);
                    decimal probability = decimal.Parse(values[1]);

                    system.LeadDaysDistribution.Add(new Distribution
                    {
                        Value = leadDays,
                        Probability = probability
                    });
                }

                CalculateCumulativeProbabilities(system.LeadDaysDistribution);
            }
            return system;
        }

        private void CalculateCumulativeProbabilities(List<Distribution> distributionList)
        {
            decimal cumulativeProbability = 0;
            int minRange = 1;

            foreach (var distribution in distributionList)
            {
                cumulativeProbability += distribution.Probability;
                distribution.CummProbability = cumulativeProbability;
                distribution.MinRange = minRange;
                distribution.MaxRange = (int)(cumulativeProbability * 100);
                minRange = distribution.MaxRange + 1;
            }
        }

        private bool IsNumeric(string str)
        {
            return decimal.TryParse(str, out _);
        }

    }
}
