using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graphic.ChartBox
{
    interface IAbsLocation
    {
        PointF Location { get; }
    }

    interface IDrawer
    {
        void Draw(Graphics e);
    }

    class GridLine : IDrawer, IDisposable
    {
        protected Rectangle field;
        protected Rectangle scaledField;
        protected Point start;
        protected Point end;
        protected Pen pen;

        public GridLine(Rectangle rectangle, int coordinate, bool isYDir)
        {
            field = rectangle;
            scaledField = rectangle;
            if (isYDir)
            {
                start = new Point(0, coordinate);
                end = new Point(scaledField.Width, coordinate);
            }
            else
            {
                start = new Point(coordinate, 0);
                end = new Point(coordinate, scaledField.Height);
            }
        }

        public void Shift(double offset, int step)
        {
            if(start.Y == end.Y)
            {
                int y = start.Y + (int)offset;
                if (y >= scaledField.Height + step/2)
                    y -= scaledField.Height + step;
                if (y <= -step/2)
                    y += scaledField.Height + step;
                start.Y = y;
                end.Y = y;
            }

            if (start.X == end.X)
            {
                int x = start.X + (int)offset;
                if (x >= scaledField.Width + step / 2) 
                    x -= scaledField.Width + step;
                if (x <=  - step / 2) 
                    x += scaledField.Width + step;
                start.X = x;
                end.X = x;
            }
        } 

        public void Scale(double verScale, double horScale)
        {
            scaledField.X = (int)(field.X * verScale);
            scaledField.Y = (int)(field.Y * horScale);
        }

        public void Draw(Graphics e)
        {
            using (GraphicsPath line = new GraphicsPath())
            {
                line.AddLine(start, end);
                if (start.X >= 0 && end.X <= field.Width &&
                    start.Y >= 0 && end.Y <= field.Height) 
                e.DrawPath(pen, line);
            }          
        }

        public void Dispose()
        {
            if(pen!=null)
                pen.Dispose();
        }
    }

    class HorGridLine : GridLine
    {
        public HorGridLine(int location, Rectangle bounds):
            base(bounds,location,true)
        {
            pen = Pens.Gray;
        }

        public override string ToString()
        {
            return "Horzontal";
        }
    }

    class VerGridLine : GridLine
    {
        public VerGridLine(int location, Rectangle bounds) :
            base(bounds, location, false)
        {
            pen = Pens.Gray;
        }
        public override string ToString()
        {
            return "Vertical";
        }
    }
}
