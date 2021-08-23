using System;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MathChart.ListG;
using MathChart.Chart;

namespace MathChart
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainForm());

            //List<GridLine> lines = new List<GridLine>();
            //lines.Add(new HorGridLine(new RectangleF(), 1));
            //lines.Add(new HorGridLine(new RectangleF(), 2));
            //lines.Add(new HorGridLine(new RectangleF(), 3));
            //lines.Add(new HorGridLine(new RectangleF(), 4));

            //lines.Add(new VerGridLine(new RectangleF(), 1));
            //lines.Add(new VerGridLine(new RectangleF(), 2));
            //lines.Add(new VerGridLine(new RectangleF(), 3));
            //lines.Add(new VerGridLine(new RectangleF(), 4));

            //int a = 0;

            //foreach (var line in lines)
            //{
            //    a += line.Coordinate;
            //}

            //GridLine[] grids = new GridLine[9];

            //lines.CopyTo(grids, 1);
            //int r = lines.IndexOf(new VerGridLine(new RectangleF(), 11));
            //grids[4].Coordinate = 90;

        }
    }
}
