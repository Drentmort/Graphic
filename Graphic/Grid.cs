using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Graphic
{

    class GridLineParam
    {
        protected Rectangle field;
        protected Point start;
        protected Point end;
        protected Pen pen;

        public GridLineParam(Rectangle rectangle, int coordinate, bool isYDir)
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

    class Axis : GridLineParam
    {
        public Axis(Rectangle rectangle, int coordinate, bool isYDir):
            base(rectangle, coordinate, isYDir)
        {        
            pen = new Pen(Color.Black, 2f);
        } 

    }

    class BigStepLine : GridLineParam
    {
        public BigStepLine(Rectangle rectangle, int coordinate, bool isYDir) :
            base(rectangle, coordinate, isYDir)
        {
            pen = new Pen(Color.Gray, 1.5f);
        }
    }

    class SmallStepLine : GridLineParam
    {
        public SmallStepLine(Rectangle rectangle, int coordinate, bool isYDir) :
            base(rectangle, coordinate, isYDir)
        {
            pen = new Pen(Color.LightGray, 1.0f);
            float[] dashValues = { 8, 4, 8 };
            pen.DashPattern = dashValues;
        }
    }

    class Grid
    {
        private MyList<GridLineParam> lines;
        private GraphicsPath assign;
        private double scale = 1.0;
        private Point zero;
        private Point graphCenter;
        private Rectangle bounds;
        private int horRes;
        private int verRes;
        private int linesCount;

        public Grid()
        {
            lines = new MyList<GridLineParam>();
            zero = new Point();
        }

        public Grid(Rectangle rectangle, int linesCount)
        {
            graphCenter = new Point(rectangle.Width / 2, rectangle.Height / 2);
            zero = new Point(graphCenter.X, graphCenter.Y);
            lines = new MyList<GridLineParam>();
            bounds = rectangle;
            this.linesCount = linesCount;
            assign = new GraphicsPath();
            AddGridLines();
            AddAxises();
            AddAssign();
        }

        public void Shift(Point offset)
        {
            lines.Clear();
            AddGridLines();
            zero.X += offset.X;
            zero.Y += offset.Y;
            AddAxises();
            AddAssign();
        }

        public void Resize(Rectangle rectangle) 
        {
            graphCenter = new Point(rectangle.Width / 2, rectangle.Height / 2);
            zero.X += (rectangle.Width - bounds.Width) / 2;
            zero.Y += (rectangle.Height - bounds.Height) / 2;
            bounds = rectangle;
            lines.Clear();
            AddGridLines();
            AddAxises();
            AddAssign();
        }

        private void AddAxises()
        {
            if(zero.Y > 0 && zero.Y <= bounds.Height)
            {
                lines.Add(new Axis(bounds, zero.Y, true));
            }

            if(zero.X > 0 && zero.X <= bounds.Width)
            {
                lines.Add(new Axis(bounds, zero.X, false));
            }             
        }

        private void AddGridLines()
        {
            horRes = (bounds.Width / 2) / (linesCount + 1);
            verRes = (bounds.Height / 2) / (linesCount + 1);

            int xOffset = (zero.X - graphCenter.X) % horRes;
            if (xOffset < 0)
                xOffset += horRes;

            int yOffset = (zero.Y - graphCenter.Y) % verRes;
            if (yOffset < 0)
                yOffset += verRes;
            
            for (int i = 0; i <= 2 * linesCount + 1; i++)
            {
                if(xOffset + horRes * i!=zero.X)
                    lines.Add(new BigStepLine(bounds, xOffset + horRes * i, false));
                if (yOffset + verRes * i != zero.Y)
                    lines.Add(new BigStepLine(bounds, yOffset + verRes * i, true));
            }
        }

        private void AddAssign()
        {
            horRes = (bounds.Width / 2) / (linesCount + 1);
            verRes = (bounds.Height / 2) / (linesCount + 1);

            int xOffset = (zero.X - graphCenter.X) % horRes;
            if (xOffset < 0)
                xOffset += horRes;

            int yOffset = (zero.Y - graphCenter.Y) % verRes;
            if (yOffset < 0)
                yOffset += verRes;

            FontFamily family = new FontFamily("Arial");
            int fontStyle = (int)FontStyle.Italic;

            for (int i = 0; i <= 2 * linesCount + 1; i++)
            {
                if (xOffset + horRes * i != zero.X)
                    assign.AddString(i.ToString(), family, fontStyle, 12, new Point(xOffset + horRes * i, 0), StringFormat.GenericDefault);
                if (yOffset + verRes * i != zero.Y)
                    lines.Add(new BigStepLine(bounds, yOffset + verRes * i, true));
            }
        }

        public void Draw(Graphics e)
        {
            foreach(var line in lines)
            {
                line.Draw(e);
            }
            //e.DrawLine(Pens.Black, graphCenter, new Point(graphCenter.X + 1, graphCenter.Y + 1));
        }
    }
}
