using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryModels;
using InventoryTesting;

namespace InventorySimulation
{
    public partial class Form1 : Form
    {
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        public Form1(SimulationSystem system)
        {
            InitializeComponent();
            this.SimulationTable = system.SimulationTable;
            this.PerformanceMeasures = system.PerformanceMeasures;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Table_Form is loading...");
            DataTable table = new DataTable();

            table.Columns.Add("Day", typeof(int));
            table.Columns.Add("Cycle", typeof(int));
            table.Columns.Add("DayWithinCycle", typeof(string));
            table.Columns.Add("BeginningInventory", typeof(int));
            table.Columns.Add("RandomDemand", typeof(int));
            table.Columns.Add("Demand", typeof(int));
            table.Columns.Add("EndingInventory", typeof(int));
            table.Columns.Add("ShortageQuantity", typeof(int));
            table.Columns.Add("OrderQuantity", typeof(int));
            table.Columns.Add("RandomLeadDays", typeof(int));
            table.Columns.Add("LeadDays", typeof(int));
            table.Columns.Add("DaysTillArrival", typeof(int));

            foreach (var simCase in SimulationTable)
            {
                table.Rows.Add(
                    simCase.Day,
                    simCase.Cycle,
                    simCase.DayWithinCycle,
                    simCase.BeginningInventory,
                    simCase.RandomDemand,
                    simCase.Demand,
                    simCase.EndingInventory,
                    simCase.ShortageQuantity,
                    simCase.OrderQuantity,
                    simCase.RandomLeadDays,
                    simCase.LeadDays,
                    simCase.DaysTillArrival
                );
            }
            dataGridView1.DefaultCellStyle.NullValue = "0";
            dataGridView1.DataSource = table;
            label1.Text = PerformanceMeasures.EndingInventoryAverage.ToString();
            label2.Text = PerformanceMeasures.ShortageQuantityAverage.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
