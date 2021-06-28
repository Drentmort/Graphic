using Graphic.MyList;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Graphic.ChartBox
{
    class Grid : MyList<GridLine>
    {
        #region Common params
        private readonly int initSignStep = 2;

        private Rectangle bounds;

        private PointF scaleFactor;
        private PointF scaleSteps;

        private PointF zero;

        private PointF period;
        private Point ceils;
        private Point visibleCount;

        private Matrix transLines;
        private Matrix transSigns;

        #endregion

        #region Events
        public event Action<PointF, Point> ScaleEvent;
        public event Action<float, float> ShiftEvent;
        event Action<float, float> XSignChanged;
        event Action<float, float> YSignChanged;
        #endregion  

        public Grid():base()
        {
            transLines = new Matrix();
            transSigns = new Matrix();
            scaleFactor = new PointF(1, 1); 
            scaleSteps = new PointF(1,1);
            period = new PointF();
            visibleCount = new Point();
            ScaleEvent += LinesScale;
            ShiftEvent += LinesShift;
            XSignChanged += SetXIndexes;
            YSignChanged += SetYIndexes;
        }
        
        public Grid(Rectangle Bounds, int Ceils):this()
        {
            bounds = Bounds;
            ceils.X = Ceils;
            ceils.Y = Ceils;
            period.X = bounds.Width / (ceils.X);
            period.Y = bounds.Height / (ceils.Y);
            zero = new PointF(bounds.Width / 2, bounds.Height / 2);
            InitLines();
            GetVisibleParams();       
        }

        public void Scale(PointF ratio, Point mouseLocation)
        {
            ScaleEvent.Invoke(ratio, mouseLocation);
        }

        public void Shift(float xShift, float yShift)
        {
            ShiftEvent.Invoke(xShift, yShift);        
        }

        private void InitLines()
        {
            for(int i = -ceils.X; i <= 2*ceils.X; i++)
            {
                Add(new VerGridLine(bounds, i * (int)period.X));
                this.Last().Index = (i - ceils.X / 2) * initSignStep;
                this.Last().Font = new Font(FontFamily.GenericSansSerif, 14);
                this.Last().Brush = Brushes.Black;
            }
                

            for (int i = -ceils.Y; i <= 2*ceils.Y; i++)
            {
                Add(new HorGridLine(bounds, i * (int)period.Y));
                this.Last().Index = -(i - ceils.Y / 2) * initSignStep;
                this.Last().Font = new Font(FontFamily.GenericSansSerif, 14);
                this.Last().Brush = Brushes.Black;
            }
               
        }

        private void LinesScale(PointF ratio, Point mouseLocation)
        {         
            scaleFactor.X *= ratio.X;
            scaleFactor.Y *= ratio.Y;                  

            transLines.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, -mouseLocation.Y), MatrixOrder.Append); 
            transSigns.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, -mouseLocation.Y), MatrixOrder.Append);

            if (scaleFactor.X < 0.5 || scaleFactor.X > 1.6)
            {
                scaleSteps.X /= scaleFactor.X > 1 ? 2f : 0.5f;

                if (scaleFactor.X > 1) 
                    XSignChanged.Invoke(0.5f, 0); 
                else 
                    XSignChanged.Invoke(2, 0);

                scaleFactor.X = 1;

                transLines.Multiply(new Matrix(1/transLines.Elements[0], 0, 0, 1, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1 / transLines.Elements[0], 0, 0, 1, 0, 0), MatrixOrder.Append);
            }
            else
            {
                transLines.Multiply(new Matrix(ratio.X, 0, 0, 1, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(ratio.X, 0, 0, 1, 0, 0), MatrixOrder.Append);
            }

            if (scaleFactor.Y < 0.5 || scaleFactor.Y > 1.6)
            {
                scaleSteps.Y /= scaleFactor.Y > 1 ? 2f : 0.5f;


                if (scaleFactor.Y > 1)
                    YSignChanged.Invoke(0.5f, 0);
                else
                    YSignChanged.Invoke(2, 0);

                scaleFactor.Y = 1;

                transLines.Multiply(new Matrix(1, 0, 0, 1 / transLines.Elements[3], 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1, 0, 0, 1 / transLines.Elements[3], 0, 0), MatrixOrder.Append);
            }
            else
            {
                transLines.Multiply(new Matrix(1, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
            }


            transLines.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, mouseLocation.Y), MatrixOrder.Append);
            transSigns.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, mouseLocation.Y), MatrixOrder.Append);

            foreach (var line in this)
            {
                line.ScaleLineLength(ratio);
            }
        }

        private void LinesShift(float xShift, float yShift)
        {
            transSigns.Translate(xShift, yShift);

            double stepX = Math.IEEERemainder(xShift, period.X * scaleFactor.X);
            double stepY = Math.IEEERemainder(yShift, period.Y * scaleFactor.X);

            double x = Math.IEEERemainder(transLines.OffsetX + stepX, period.X * scaleFactor.X);
            double y = Math.IEEERemainder(transLines.OffsetY + stepY, period.Y * scaleFactor.Y);

            float shift;
            if (transLines.OffsetX + stepX != x)
            {
                if (x >= 0)
                    shift = initSignStep * scaleSteps.X * 
                        (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.X)));
                    
                else
                    shift = -initSignStep * scaleSteps.X *
                        (float)Math.Ceiling(Math.Abs(xShift / (period.X * scaleFactor.X)));
                XSignChanged.Invoke(1, shift);
            }

            if (transLines.OffsetY + stepY != y)
            {
                if (y >= 0)
                    shift = -initSignStep * scaleSteps.Y * 
                        (float)Math.Ceiling(Math.Abs(yShift / (period.Y * scaleFactor.Y)));
                else
                    shift = initSignStep * scaleSteps.Y * 
                        (float)Math.Ceiling(Math.Abs(yShift / (period.Y * scaleFactor.Y)));
                YSignChanged.Invoke(1, shift);
            }

            transLines.Translate(-transLines.OffsetX, -transLines.OffsetY, MatrixOrder.Append);
            transLines.Translate((float)x, (float)y, MatrixOrder.Append);

            foreach (var line in this)
            {
                line.ScaleLineLength(new PointF((float)x, (float)y));
            }
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

        private void SetXIndexes(float ratio, float shift)
        {
            foreach(var line in this)
            {
                if (!(line is VerGridLine))
                    continue;
                line.Index *= ratio;
                line.Index += shift;
                if (line.Index == 0)
                    zero.X = line.Coordinate;
            }
        }

        private void SetYIndexes(float ratio, float shift)
        {
            foreach (var line in this)
            {
                if (!(line is HorGridLine))
                    continue;
                line.Index *= ratio;
                line.Index += shift;
                if (line.Index == 0)
                    zero.Y = line.Coordinate;
            }
        }

        private void DrawSigns(Graphics e)
        {      
            foreach (GridLine line in this)
            {
                PointF temp = zero;
                if (temp.X < 0)
                    temp.X = -transLines.OffsetX;
                if (temp.Y < 0)
                    temp.Y = -transLines.OffsetY;

                line.DrawSign(e, temp);
            }
        }

        public void Draw(Graphics e)
        {
            e.Transform = transLines;
            foreach (GridLine line in this)
            {
                line.Draw(e);
            }
            DrawSigns(e);
        }  
        
    }
}
