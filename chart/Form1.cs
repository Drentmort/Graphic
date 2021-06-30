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
            TimeData timeData1 = new TimeData();
            for (float i = -100; i <= 100; i+=1)
                timeData1.AddPoint(i, Math.Sin(i));

            TimeData timeData2 = new TimeData();
            for (float i = -100; i <= 100; i += 1)
                timeData2.AddPoint(i, Math.Cos(i));

            chart1.Data = timeData1;
            chart1.Data = timeData2;
        }

        private void RectangleTransform_Click(object sender, EventArgs e)
        {
            chart1.RescaleByRect(new RectangleF(1, 1, 10f, 10f), false);
        }
    }
}
