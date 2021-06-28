using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Graphic.MyList;

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
        protected Font font;
        protected Brush brush;
        protected bool visible;
        protected double index;

        public Pen Pen
        {
            get { return pen; }
            set { pen = value; }
        }

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public Brush Brush
        {
            get { return brush; }
            set { brush = value; }
        }

        public bool Visible { get { return visible; } }
        public double Index{ get { return index; } set { index = value; } }
        public virtual int Coordinate { get; }

        public GridLine(Rectangle rectangle)
        {
            field = rectangle;
        }

        public virtual void SetVisible(Matrix transform, PointF linePeriod) { }

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
            Pen temp = pen;
            if (index == 0)
                pen = new Pen(Color.Black, 3);
            using (GraphicsPath line = new GraphicsPath())
            {
                line.AddLine(start, end);

                e.DrawPath(pen, line);
            }
            pen = temp;
        }

        public virtual void DrawSign(Graphics e, PointF zero)
        {}

        public void Dispose()
        {
            if(pen!=null)
                pen.Dispose();
        }
    }

    class HorGridLine : GridLine
    {
        public override int Coordinate => start.Y;
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

        public override void SetVisible(Matrix transform, PointF linePeriod)
        {
            Point[] points = { start, end };
            transform.TransformPoints(points);
            if (points[0].Y >= 0 &&
                points[0].Y <= field.Height)
                visible = true;
        }

        public override string ToString()
        {
            return "Horzontal";
        }

        public override void DrawSign(Graphics e, PointF zero)
        {
            String num = String.Format("{0:F2}", index);

            float x = zero.X;

            e.DrawString(num, font, brush, new PointF(x, start.Y));
        }
    }

    class VerGridLine : GridLine
    {
        public override int Coordinate => start.X;

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

        public override void SetVisible(Matrix transform, PointF linePeriod)
        {
            Point[] points = { start, end };
            transform.TransformPoints(points);
            if (points[0].X >= 0 && 
                points[0].X <= field.Width)
                visible = true;
        }

        public override string ToString()
        {
            return "Vertical";
        }

        public override void DrawSign(Graphics e, PointF zero)
        {
            String num = String.Format("{0:F2}", index);
            float y = zero.Y;       
            e.DrawString(num, font, brush, new PointF(start.X, y));
        }
    }
}
