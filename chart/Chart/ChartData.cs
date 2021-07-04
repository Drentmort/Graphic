using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace MathChart.Chart
{  
    public class ChartData
    {
        private List<PointF> drawData;
        private float minX = float.MaxValue;
        private float minY = float.MaxValue;
        private float maxX = float.MinValue;
        private float maxY = float.MinValue;
        public Matrix Resolution { get; set; }

        public Pen ChPen { get; set; }

        public float ChLineWidth { get; set; } 
        
        public float XMin { get { return minX; } }
        public float XMax { get { return maxX; } }
        public float YMin { get { return minY; } }
        public float YMax { get { return maxY; } }

        public ChartData()
        {
            drawData = new List<PointF>();
            ChLineWidth = 3;
            ChPen = new Pen(Color.Black, ChLineWidth);          
        }

        public virtual void AddPoint(double abscissa, double ordinate)
        {
            float x = (float)abscissa;
            float y = (float)ordinate;
            drawData.Add(new PointF(x, y));
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        public void Draw(Graphics e)
        {
            PointF[] temp = drawData.ToArray();
            Resolution.TransformPoints(temp);

            ChPen.Width = ChLineWidth;
            ChPen.Width /= (float)Math.Sqrt(Math.Pow(e.Transform.Elements[0], 2) + Math.Pow(e.Transform.Elements[3], 2));
            
            using (GraphicsPath gr = new GraphicsPath())
            {
                gr.AddCurve(temp);
                e.DrawPath(ChPen, gr);
            }
        }
    }

    class TimeData : ChartData
    {
        private Dictionary<double, double> data;

        public TimeData() : base()
        {
            data = new Dictionary<double, double>();
        }

        public override void AddPoint(double time, double func)
        {
            try
            {
                data.Add(time, func);
                base.AddPoint(time, func);
            }
            catch
            {
                return;
            }

        }

    }

    class SpartialData : ChartData
    {

    }
}
