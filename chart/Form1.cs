using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathChart.Chart;

namespace chart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AddData_click(object sender, EventArgs e)
        {
            TimeData timeData = new TimeData();
            for (int i = -100; i <= 100; i++)
                timeData.AddPoint(i, Math.Sin(i));

            chart1.Data = timeData;
        }
    }
}
