using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace MathChart.Chart
{
    interface IDrawer
    {
        void Draw(Graphics e);
    }

    class GridLine : IDrawer, IDisposable
    {
        protected RectangleF field;
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

        public double Index { get { return index; } set { index = value; } }
        public virtual int Coordinate { get; set; }

        public GridLine(RectangleF rectangle)
        {
            font = new Font(FontFamily.GenericSansSerif, 12);
            brush = Brushes.Black;
            field = rectangle;
        }

        public virtual void ShiftLine(int x, int y)
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

        public virtual void DrawSign(Graphics e, PointF location) { }

        public void Dispose()
        {
            if (pen != null)
                pen.Dispose();
        }
    }

    class HorGridLine : GridLine
    {
        public override int Coordinate
        {
            get { return start.Y; }
            set
            {
                start.Y = value;
                end.Y = value;
            }
        }
        public HorGridLine(RectangleF bounds, int y) :
            base(bounds)
        {
            start = new Point((int)field.Left, y);
            end = new Point((int)field.Right, y);
            pen = Pens.Gray;
        }

        public override string ToString()
        {
            return "Horzontal";
        }

        public override void DrawSign(Graphics e, PointF location)
        {
            String num = String.Format("{0:F2}", index);
            PointF temp = new PointF(location.X, Coordinate);
            e.DrawString(num, font, brush, temp);
        }
    }

    class VerGridLine : GridLine
    {
        public override int Coordinate
        {
            get { return start.X; }
            set
            {
                start.X = value;
                end.X = value;
            }
        }

        public VerGridLine(RectangleF bounds, int x) :
            base(bounds)
        {
            start = new Point(x, (int)(field.Top));
            end = new Point(x, (int)(field.Bottom));
            pen = Pens.Gray;
        }

        public override string ToString()
        {
            return "Vertical";
        }

        public override void DrawSign(Graphics e, PointF location)
        {
            String num = String.Format("{0:F2}", index);
            PointF temp = new PointF(Coordinate, location.Y);
            e.DrawString(num, font, brush, temp);
        }
    }
}
