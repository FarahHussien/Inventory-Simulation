using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }

        public int ending_Sum = 0,
                   shortage_Sum = 0;

        public void RunSimulation()
        {
            SimulationCase simulationCase;
            int cycle = 1,
                orderQuantity = StartOrderQuantity;

            for (int i = 0; i < NumberOfDays; i++)
            {
                simulationCase = new SimulationCase();
                simulationCase.Day = i + 1;
                simulationCase.DayWithinCycle = (i) % (ReviewPeriod) + 1;
                simulationCase.Cycle = cycle;
                //day1
                if (i == 0)
                {
                    simulationCase.BeginningInventory = StartInventoryQuantity;
                    simulationCase.DaysTillArrival = StartLeadDays - 1;
                }
                //day 2
                else if (i == 1)
                {
                    simulationCase.BeginningInventory = SimulationTable[i - 1].EndingInventory;
                }
                else
                {
                    if (SimulationTable[i - 2].DaysTillArrival == 1)
                        simulationCase.BeginningInventory = orderQuantity + SimulationTable[i - 1].EndingInventory;
                    else
                        simulationCase.BeginningInventory = SimulationTable[i - 1].EndingInventory;

                }

                simulationCase.RandomDemand = GenerateRandomNumber(1, 101);
                simulationCase.Demand = GetRange(simulationCase.RandomDemand, DemandDistribution);

                if (simulationCase.Demand >= simulationCase.BeginningInventory)
                {
                    simulationCase.EndingInventory = 0;
                    //day 1
                    if (i == 0)
                    {
                        simulationCase.ShortageQuantity = simulationCase.Demand - simulationCase.BeginningInventory;
                    }
                    else
                    {
                        if (SimulationTable[i - 1].ShortageQuantity != 0)
                            simulationCase.ShortageQuantity = SimulationTable[i - 1].ShortageQuantity + simulationCase.Demand - simulationCase.BeginningInventory;

                        else
                            simulationCase.ShortageQuantity = simulationCase.Demand - simulationCase.BeginningInventory;

                    }
                }
                // demand < BeginningInventory
                else
                {
                    if (i == 0)
                    {
                        simulationCase.EndingInventory = simulationCase.BeginningInventory - simulationCase.Demand;

                    }
                    else
                    {
                        if (SimulationTable[i - 1].ShortageQuantity != 0)
                        {
                            if (simulationCase.BeginningInventory - simulationCase.Demand - SimulationTable[i - 1].ShortageQuantity >= 0)
                                simulationCase.EndingInventory = simulationCase.BeginningInventory - simulationCase.Demand - SimulationTable[i - 1].ShortageQuantity;
                            else
                            {
                                simulationCase.EndingInventory = 0;
                                simulationCase.ShortageQuantity = -(simulationCase.BeginningInventory - simulationCase.Demand - SimulationTable[i - 1].ShortageQuantity);

                            }
                        }
                        else
                            simulationCase.EndingInventory = simulationCase.BeginningInventory - simulationCase.Demand;

                    }
                }

                if (i != 0)
                {
                    if (SimulationTable[i - 1].DaysTillArrival != 0)
                        simulationCase.DaysTillArrival = SimulationTable[i - 1].DaysTillArrival - 1;
                }

                if (simulationCase.DayWithinCycle == ReviewPeriod)
                {
                    cycle++;

                    simulationCase.OrderQuantity = OrderUpTo - simulationCase.EndingInventory + simulationCase.ShortageQuantity;
                    orderQuantity = simulationCase.OrderQuantity;
                    simulationCase.RandomLeadDays = GenerateRandomNumber(1, 101);
                    simulationCase.LeadDays = GetRange(simulationCase.RandomLeadDays, LeadDaysDistribution);
                    simulationCase.DaysTillArrival = simulationCase.LeadDays;

                }

                ending_Sum += simulationCase.EndingInventory;
                shortage_Sum += simulationCase.ShortageQuantity;
                SimulationTable.Add(simulationCase);

            }

            PerformanceMeasures.EndingInventoryAverage = (decimal)((decimal)ending_Sum / (decimal)NumberOfDays);
            PerformanceMeasures.ShortageQuantityAverage = (decimal)((decimal)shortage_Sum / (decimal)NumberOfDays);
        }

        private int GenerateRandomNumber(int minNum, int maxNum)
        {
            Random random = new Random();
            int randomNum = random.Next(minNum, maxNum);
            Thread.Sleep(10);
            return randomNum;
        }

        private int GetRange(int randomNum, List<Distribution> DistributionList)
        {
            for (int i = 0; i < DistributionList.Count(); i++)
                if (randomNum <= DistributionList[i].MaxRange && randomNum >= DistributionList[i].MinRange)
                    return DistributionList[i].Value;

            return 0;
        }

    }
}