using InventoryModels;
using InventoryTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySimulation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string file = "D:\\Materials FCIS\\Year 4\\Semester 7\\Modeling\\Labs\\Lab 5_Task 3\\" +
                "InventorySimulation_withGUI\\InventorySimulation\\InventorySimulation\\TestCases\\TestCase4.txt";

            SimulationSystem system = new SimulationSystem();
            ReadFile readFile = new ReadFile();

            system = readFile.ReadSimulationFile(file);
            system.RunSimulation();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(system));

            string testingResult = TestingManager.Test(system, Constants.FileNames.TestCase4);
            MessageBox.Show(testingResult);
        }
    }
}
