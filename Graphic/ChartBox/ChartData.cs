using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Graphic.ChartBox
{
    class ChartData
    {
        protected Pen pen;
        protected List<PointF> rawData;
  
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

        public PointF[] GetPointsInRange(RectangleF window)
        {
            List<PointF> temp = new List<PointF>();
            foreach(var point in rawData)
            {
                if (point.X >= window.Left &&
                    point.X <= window.Right)
                    temp.Add(point);
                    
            }
            return temp.ToArray();
        }
    }

    class TimeData:ChartData
    {
        private Dictionary<double, double> data;

        public TimeData():base()
        {
            data = new Dictionary<double, double>();
        }

        public TimeData(Pen pen) : base()
        {
            data = new Dictionary<double, double>();
            this.pen = pen;
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
