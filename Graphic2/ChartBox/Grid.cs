using Graphic.MyList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Mail;
using System.Xml;

namespace Graphic.ChartBox
{
    class Grid:MyList<GridLine>
    {
        #region Common params
        private readonly int initSignStep = 2;

        private Rectangle bounds;

        private PointF scaleFactor;
        private PointF scaleSteps;
        private PointF shiftSteps;
        private PointF zero;

        private PointF period;
        private Point ceils;
        private Point visibleCount;

        private Matrix transLines;
        private Matrix transValues;

        private Font font;
        private Brush brush;
        #endregion

        #region Events
        public event Action<PointF, Point> ScaleEvent;
        public event Action<float, float> ShiftEvent;
        #endregion

        public Grid():base()
        {
            scaleFactor = new PointF(1, 1);
            transLines = new Matrix();
            transValues = new Matrix();
            scaleSteps = new PointF(1,1);
            shiftSteps = new PointF();
            period = new PointF();
            visibleCount = new Point();
            ScaleEvent += LinesScale;
            ScaleEvent += ValueScale;
            ShiftEvent += ValueShift;
            ShiftEvent += LinesShift;
            
        }
        
        public Grid(Rectangle Bounds, int Ceils):this()
        {
            bounds = Bounds;
            ceils.X = Ceils;
            ceils.Y = Ceils;
            period.X = bounds.Width / (ceils.X);
            period.Y = bounds.Height / (ceils.Y);
            zero = new PointF(bounds.Width / 2, bounds.Height / 2);
            CreateLines();
            GetVisibleParams();
            SetIndexes();         
        }

        public void Scale(PointF ratio, Point mouseLocation)
        {
            ScaleEvent.Invoke(ratio, mouseLocation);
            SetIndexes();
        }

        public void Shift(float xShift, float yShift)
        {
            ShiftEvent.Invoke(xShift, yShift);
            SetIndexes();
        }

        private void CreateLines()
        {
            for(int i = -ceils.X; i <= 2*ceils.X; i++)
            {
                Add(new VerGridLine(bounds, i * (int)period.X));
                this.Last().Font = new Font(FontFamily.GenericSansSerif, 14);
                this.Last().Brush = Brushes.Black;
            }
                

            for (int i = -ceils.Y; i <= 2*ceils.Y; i++)
            {
                Add(new HorGridLine(bounds, i * (int)period.Y));
                this.Last().Font = new Font(FontFamily.GenericSansSerif, 14);
                this.Last().Brush = Brushes.Black;
            }
               
        }

        private void LinesScale(PointF ratio, Point mouseLocation)
        {         
            scaleFactor.X *= ratio.X;
            scaleFactor.Y *= ratio.Y;                  

            transLines.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, -mouseLocation.Y), MatrixOrder.Append); 
            transValues.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, -mouseLocation.Y), MatrixOrder.Append);

            if (scaleFactor.X < 0.5 || scaleFactor.X > 1.6)
            {
                scaleSteps.X /= scaleFactor.X > 1 ? 2f : 0.5f;
                scaleSteps.Y /= scaleFactor.Y > 1 ? 2f : 0.5f;
                scaleFactor.X = 1;
                scaleFactor.Y = 1;
                transLines.Multiply(new Matrix(1/transLines.Elements[0], 0, 0, 1/transLines.Elements[3], 0, 0), MatrixOrder.Append);
                transValues.Multiply(new Matrix(1 / transValues.Elements[0], 0, 0, 1 / transValues.Elements[0], 0, 0), MatrixOrder.Append);
            }
            else
            {
                transLines.Multiply(new Matrix(ratio.X, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
                transValues.Multiply(new Matrix(ratio.X, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
            }
                                       
            transLines.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, mouseLocation.Y), MatrixOrder.Append);
            transValues.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, mouseLocation.Y), MatrixOrder.Append);

            foreach (var line in this)
            {
                line.ScaleLineLength(ratio);
            }
        }

        private void ValueScale(PointF ratio, Point mouseLocation)
        {
            PointF[] temp = new PointF[1]  { zero };
            transLines.TransformPoints(temp);
            zero = temp[0];
            SetIndexes();
        }

        private void LinesShift(float xShift, float yShift)
        {

            double stepX = Math.IEEERemainder(xShift, period.X * scaleFactor.X);
            double stepY = Math.IEEERemainder(yShift, period.Y * scaleFactor.X);

            double x = Math.IEEERemainder(transLines.OffsetX + stepX, period.X * scaleFactor.X);
            double y = Math.IEEERemainder(transLines.OffsetY + stepY, period.Y * scaleFactor.X);

            if(transLines.OffsetX + stepX != x)
            {
                if (x >= 0)
                    shiftSteps.X += (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.X)));                     
                else
                    shiftSteps.X -= (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.X)));
            }

            if (transLines.OffsetY + stepY != y)
            {
                if (x >= 0)
                    shiftSteps.Y += (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.Y)));
                else
                    shiftSteps.Y -= (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.Y)));
            }

            transLines.Translate(-transLines.OffsetX, -transLines.OffsetY, MatrixOrder.Append);
            transLines.Translate((float)x, (float)y, MatrixOrder.Append);

            foreach (var line in this)
            {
                line.ScaleLineLength(new PointF((float)x, (float)y));
            }
        }

        private void ValueShift(float xShift, float yShift)
        {
            transValues.Translate(xShift / scaleFactor.X, yShift / scaleFactor.Y);
            SetIndexes();
        }

        private void GetVisibleParams()
        {
            foreach(var line in this)
            {
                line.SetVisible(transLines, period);
                if (line.Visible)
                {
                    if (line is HorGridLine)
                        visibleCount.X++;

                    if (line is VerGridLine)
                        visibleCount.Y++;
                }
            }
        }

        private void SetIndexes()
        {
            GetVisibleParams();

            int i = 0, j = 0;
            foreach(var line in this)
            {
                if (line.Visible)
                {
                    if (line is VerGridLine)
                    {
                        line.Index =  (i + shiftSteps.X) * scaleSteps.X;
                        i++;
                    }
                       
                    if (line is HorGridLine)
                    {
                        line.Index = (j + shiftSteps.Y) * scaleSteps.Y;
                        j++;
                    }                       
                }
            }
        }

        public void Draw(Graphics e)
        {
            e.Transform = transLines;
            foreach (GridLine line in this)
            {
                line.Draw(e);
            }

            e.Transform = transValues;
            PointF[] temp = new PointF[1] { zero };
            transValues.TransformPoints(temp);
            if (temp[0].X <= 0)
                temp[0].X = 0;
            if (temp[0].X >= bounds.Width - period.X)
                temp[0].X = bounds.Width - period.X;
            if (temp[0].Y <= 0)
                temp[0].Y = 0;
            if (temp[0].Y >= bounds.Height - period.Y)
                temp[0].Y = bounds.Height - period.Y;

            transValues.Invert();
            transValues.TransformPoints(temp);
            zero = temp[0];
            transValues.Invert();
            foreach (GridLine line in this)   
                line.DrawSign(e, zero, new PointF(period.X * scaleFactor.X, period.Y * scaleFactor.Y));
        }  
        
    }
}
