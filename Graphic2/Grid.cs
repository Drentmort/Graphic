using Graphic.MyList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Graphic
{
    interface IAbsLocation
    {
        PointF Location { get; }
    }

    interface IDrawer
    {
        void Draw(Graphics e);
    }

    class GridLine:IDrawer
    {
        protected Rectangle field;
        protected Point start;
        protected Point end;
        protected Pen pen;

        public int X
        {
            get{ return start.X; }
        }

        public int Y
        {
            get { return start.Y; }
        }

        public GridLine(Rectangle rectangle, int coordinate, bool isYDir)
        {
            field = rectangle;
            if (isYDir)
            {
                start = new Point(0, coordinate);
                end = new Point(field.Width, coordinate);
            }
            else
            {
                start = new Point(coordinate, 0);
                end = new Point(coordinate, field.Height);
            }
        }

        public void Draw(Graphics e)
        {
            GraphicsPath line = new GraphicsPath();
            line.AddLine(start, end);
            e.DrawPath(pen, line);
        }

    }

    class Axis : GridLine, IAbsLocation
    {
        public Axis(Rectangle rectangle, int coordinate, bool isYDir):
            base(rectangle, coordinate, isYDir)
        {        
            pen = new Pen(Color.Black, 2f);
        }

        public PointF Location
        {
            get
            {
                if (start.X == end.X)
                    return new PointF(0, float.NaN);
                else
                    return new PointF(float.NaN, 0);
            }
        }

        public override string ToString()
        {
            if(start.X == end.X)
                return "Ось Y";
            else
                return "Ось X";
        }


    }

    class BigStepLine : GridLine, IAbsLocation
    {
        public int a = 2;
        private PointF location;
        public BigStepLine(Rectangle rectangle, int coordinate, bool isYDir, PointF location) :
            base(rectangle, coordinate, isYDir)
        {
            pen = new Pen(Color.Gray, 1.5f);
            this.location = location;
        }

        public PointF Location
        {
            get { return location; }
        }

        public override string ToString()
        {
            if (start.X == end.X)
                return "Вертикальная линия";
            else
                return "Горизонтальная линия";
        }
    }

    class SmallStepLine : GridLine
    {
        public SmallStepLine(Rectangle rectangle, int coordinate, bool isYDir) :
            base(rectangle, coordinate, isYDir)
        {
            pen = new Pen(Color.LightGray, 1.0f);
            float[] dashValues = { 8, 4, 8 };
            pen.DashPattern = dashValues;
        }
    }

    class NumAssign:IDrawer
    {
        private Font font;
        private int emSize;
        private string format;
        private string text;
        private PointF value;
        private Point location;
        private int textLength;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(location, new Size(text.Length * emSize, font.Height));
            }
        }
        public NumAssign()
        {
            emSize = 10;
            font = new Font(new FontFamily("Arial"), emSize);
            format = "F1";
            text = "";
            textLength = 0;
        }

        public NumAssign(GridLine line):
            this()
        {
            value = (line as IAbsLocation).Location;
            if (float.IsNaN(value.X))
            {
                text = value.Y.ToString(format);
                textLength = value.Y >= 0 ? text.Length * emSize : (text.Length - 1) * emSize;
            }

            if (float.IsNaN(value.Y))
            {
                text = value.X.ToString(format);
                textLength = value.X >= 0 ? text.Length * emSize : (text.Length - 1) * emSize;
            }

        }

        public NumAssign(GridLine line, Point zero, Rectangle bounds) :
            this(line)
        {
            if (float.IsNaN(value.Y))
            {
                int yCoord = zero.Y;
                if (zero.Y <= 0)
                    yCoord = 0;
                if (zero.Y > bounds.Height - font.Height)
                    yCoord = bounds.Height - font.Height;
                location = new Point(line.X, yCoord);
            }

            if (float.IsNaN(value.X))
            {
                int xCoord = zero.X;
                if (zero.X <= 0)
                    xCoord = 0;
                if (zero.X > bounds.Width - textLength)
                    xCoord = bounds.Width - textLength;
                location = new Point(xCoord, line.Y);
            }
        }

        public void Draw(Graphics e)
        {
            e.DrawString(text, font, Brushes.Black, location);
        }
    }

    class Grid
    {
        private MyList<GridLine> verticalLines;
        private MyList<GridLine> horizontalLines;
        private MyList<IDrawer> drawer;
        private MyList<NumAssign> assigns;
        private double xScale;
        private double xDownLimit;
        private double yScale;
        private double yUpLimit;
        private Point zero;
        private Point graphCenter;
        private Rectangle bounds;
        private int horizontalPixelResolution;
        private int verticalPixelResolution;
        private int xOffset;
        private int yOffset;
        private int linesCount;

        public Grid()
        {
            zero = new Point();
            assigns = new MyList<NumAssign>();
            verticalLines = new MyList<GridLine>();
            horizontalLines = new MyList<GridLine>();
            drawer = new MyList<IDrawer>();
        }

        public Grid(Rectangle rectangle, int count):
            this()
        {
            graphCenter = new Point(rectangle.Width / 2, rectangle.Height / 2);
            zero = new Point(graphCenter.X, graphCenter.Y);
            bounds = rectangle;
            linesCount = count;
            xScale = 1;
            yScale = 1;
            xDownLimit = -xScale;
            yUpLimit = yScale;
            horizontalPixelResolution = bounds.Width / 2 / linesCount;
            verticalPixelResolution = bounds.Height / 2 / linesCount;
            Shift(new Point(0,0));
            AddAxises();
            AddHorGridLines();
            AddVerGridLines();
            AddAssign();
            
        }

        public void Shift(Point offset)
        {
            zero.X += offset.X;          
            xOffset = (zero.X - graphCenter.X) % horizontalPixelResolution;
            if (xOffset < 0)
                xOffset += horizontalPixelResolution;

            zero.Y += offset.Y;
            yOffset = (zero.Y - graphCenter.Y) % verticalPixelResolution;
            if (yOffset < 0)
                yOffset += verticalPixelResolution;

            yUpLimit += yScale * (offset.Y / (double)verticalPixelResolution);
            xDownLimit -= xScale * (offset.X / (double)horizontalPixelResolution);

            Refresh();
        }

        public void Resize(Rectangle rectangle) 
        {    
            graphCenter = new Point(rectangle.Width / 2, rectangle.Height / 2);
            zero.X += (rectangle.Width - bounds.Width) / 2;
            zero.Y += (rectangle.Height - bounds.Height) / 2;
            bounds = rectangle;
            horizontalPixelResolution = bounds.Width / 2 / linesCount;
            verticalPixelResolution = bounds.Height / 2 / linesCount;
            Refresh();
        }

        public void Scale(double factor)
        {
            xDownLimit /= xScale;
            yUpLimit /= yScale;
            xScale += factor;
            yScale += factor;
            xDownLimit *= xScale;
            yUpLimit *= yScale;

            Refresh();
        }

        private void AddAxises()
        {
            if (zero.Y > 0 && zero.Y <= bounds.Height)
            {
                drawer.Add(new Axis(bounds, zero.Y, true));
            }

            if (zero.X > 0 && zero.X <= bounds.Width)
            {
                drawer.Add(new Axis(bounds, zero.X, false));
            }
        }

        private void AddHorGridLines()
        {
            for (int i = 0; i < 2 * linesCount + 1; i++)
            {
                double step = 1 / (double)linesCount;
                double count = -1 - xDownLimit/ xScale;
                count = (int)count - count > 0 ? (int)count - 1 : (int)count;
                double absXLoc = -1  + (i / (double)linesCount) - (int)count * step;
                absXLoc *= xScale; 
                PointF temp = new PointF((float)absXLoc, float.NaN);

                horizontalLines.Add(new BigStepLine(bounds, xOffset + horizontalPixelResolution * i, false, temp));
                drawer.Add(horizontalLines[horizontalLines.Count - 1]);
            }
        }

        private void AddVerGridLines()
        {
            for (int i = 0; i < 2 * linesCount + 1; i++)
            {
                double step = 1 / (double)linesCount;
                double count = yUpLimit / yScale - 1;
                count = count - (int)count < 0 ? (int)count - 1: (int)count;
                double absYLoc = 1 - (i / (double)linesCount) + step * (int)count;
                absYLoc *= yScale;
                PointF temp = new PointF(float.NaN, (float)absYLoc);

                verticalLines.Add(new BigStepLine(bounds, yOffset + verticalPixelResolution * i, true, temp));
                drawer.Add(verticalLines[verticalLines.Count - 1]);
            }
        }

        private void AddAssign()
        {
            foreach(var obj in drawer)
            {
                if (obj as GridLine == null)
                    continue;

                NumAssign temp = new NumAssign(obj as GridLine, zero, bounds);
                Rectangle box1 = temp.BoundingBox;

                bool flag = true;
                foreach (var rec in assigns)
                {
                    Rectangle box2 = rec.BoundingBox;
                    if (Math.Abs(box1.X - box2.X) < box1.Width &&
                        Math.Abs(box1.Y - box2.Y) < box1.Height) 
                            flag = false;
                }
                if (flag)
                    assigns.Add(temp);
            }

            foreach (var assign in assigns)
            {
                drawer.Add(assign);                
            }
        }

        private void Refresh()
        {
            verticalLines.Clear();
            horizontalLines.Clear();
            assigns.Clear();
            drawer.Clear();
            AddAxises();
            AddHorGridLines();
            AddVerGridLines();
            AddAssign();
        }

        public void Draw(Graphics e)
        {
            foreach(var obj in drawer)
            {
                obj.Draw(e);
            }
        }
    }
}
