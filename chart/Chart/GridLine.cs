using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathChart.Chart
{
    abstract class GridLine
    {
        protected RectangleF field;
        protected Point start;
        protected Point end;
        protected Pen pen;
        protected Font font;
        protected Brush brush;
        protected double index;
        protected bool isAdditional;

        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public double Index { get { return index; } set { index = value; } }
        public abstract int Coordinate { get; set; }

        public GridLine(RectangleF rectangle)
        {
            font = new Font(FontFamily.GenericSansSerif, 12);
            brush = Brushes.Black;
            field = rectangle;
        }

        public virtual void Draw(Graphics e)
        {
            if (index == 0)
            {
                pen.Width *= 2;
                pen.DashStyle = DashStyle.Solid;
                pen.Color = Color.Black;
            }
            
            using (GraphicsPath line = new GraphicsPath())
            {
                line.AddLine(start, end);
                e.DrawPath(pen, line);
            }
        }    

        public abstract void DrawSign(Graphics e, PointF location);

        public abstract void BuildPen(PointF scale);
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
            if (isAdditional)
                return;
            PointF scale = new PointF(e.Transform.Elements[0], e.Transform.Elements[3]);
            String num = String.Format("{0:F2}", index);
            PointF temp = new PointF(location.X * scale.X, Coordinate * scale.Y);

            e.Transform = new Matrix(1, 0, 0, 1, e.Transform.OffsetX, e.Transform.OffsetY);
            e.DrawString(num, font, brush, temp);
            e.Transform = new Matrix(scale.X, 0, 0, scale.Y, e.Transform.OffsetX, e.Transform.OffsetY);
        }

        public override void BuildPen(PointF scale)
        {
            pen = new Pen(Color.Gray, 1);
            if (Math.IEEERemainder(index * scale.Y, 2) != 0)
            {
                pen.DashStyle = DashStyle.Dash;
                pen.Color = Color.LightGray;
                isAdditional = true;
            }
        }

        public override void Draw(Graphics e)
        {
            pen.Width /= e.Transform.Elements[3];
            base.Draw(e);
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
            if (isAdditional)
                return;
            PointF scale = new PointF(e.Transform.Elements[0], e.Transform.Elements[3]);
            String num = String.Format("{0:F2}", index);
            PointF temp = new PointF(Coordinate * scale.X, location.Y * scale.Y);

            e.Transform = new Matrix(1, 0, 0, 1, e.Transform.OffsetX, e.Transform.OffsetY);
            e.DrawString(num, font, brush, temp);
            e.Transform = new Matrix(scale.X, 0, 0, scale.Y, e.Transform.OffsetX, e.Transform.OffsetY);
        }

        public override void BuildPen(PointF scale)
        {
            pen = new Pen(Color.Gray, 1);
            if (Math.IEEERemainder(index * scale.X, 2) != 0)
            {
                pen.DashStyle = DashStyle.Dash;
                pen.Color = Color.LightGray;
                isAdditional = true;
            }
        }

        public override void Draw(Graphics e)
        {
            pen.Width /= e.Transform.Elements[0];
            base.Draw(e);
        }
    }
}
