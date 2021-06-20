using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graphic.ChartBox
{
    interface IDrawer
    {
        void Draw(Graphics e);
    }

    class GridLine : IDrawer, IDisposable
    {
        protected Rectangle field;
        protected Point start;
        protected Point end;
        protected Pen pen;

        public Pen Pen
        {
            get { return pen; }
            set { pen = value; }
        }

        public GridLine(Rectangle rectangle)
        {
            field = rectangle;
        }

        public virtual void ScaleLineLength(PointF scale) { }

        public virtual void ShiftLine(int x , int y) 
        {
            start.X += x;
            start.Y += y;
            end.X += x;
            end.Y += y;
        }

        public void Draw(Graphics e)
        {
            using (GraphicsPath line = new GraphicsPath())
            {
                line.AddLine(start, end);
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
        public HorGridLine(Rectangle bounds, int y):
            base(bounds)
        {
            start = new Point(0, y);
            end = new Point(field.Width, y);
            pen = Pens.Gray;
        }

        public override void ScaleLineLength(PointF scale)
        {
            float ratio = (field.Width / 2) * scale.X;
            start.X -= (int)ratio;
            end.X += (int)ratio;
        }

        public override void ShiftLine(int x, int y)
        {
            base.ShiftLine(x, y);
        }

        public override string ToString()
        {
            return "Horzontal";
        }
    }

    class VerGridLine : GridLine
    {
        public VerGridLine(Rectangle bounds, int x) :
            base(bounds)
        {
            start = new Point(x, 0);
            end = new Point(x, field.Height);           
            pen = Pens.Gray;
        }

        public override void ScaleLineLength(PointF scale)
        {
            float ratio = (field.Height / 2) * scale.Y;
            start.Y -= (int)ratio;
            end.Y += (int)ratio;
        }

        public override void ShiftLine(int x, int y)
        {
            base.ShiftLine(x, y);
        }

        public override string ToString()
        {
            return "Vertical";
        }
    }
}
