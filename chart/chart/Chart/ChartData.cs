using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathChart.Chart
{
    public class ChartData
    {
        private Matrix resolution;
        protected List<PointF> rawData;

        public Matrix Resolution
        {
            get { return resolution; }
            set
            {
                resolution = value;
            }
        }

        public ChartData()
        {
            rawData = new List<PointF>();
        }

        public virtual void AddPoint(double abscissa, double ordinate)
        {
            float x = (float)abscissa;
            float y = (float)ordinate;
            rawData.Add(new PointF(x, y));
        }

        public void Draw(Graphics e)
        {
            PointF[] temp = rawData.ToArray();
            resolution.TransformPoints(temp);           
            using (GraphicsPath gr = new GraphicsPath())
            {
                gr.AddCurve(temp);
                e.DrawPath(Pens.Red, gr);
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
