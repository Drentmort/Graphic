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
        MyList<ChartData> data;
        
        private readonly int initSignStep = 2;

        private Rectangle bounds;

        private PointF scaleFactor;
        private PointF scaleSteps;
        private PointF shiftSteps;

        private PointF zero;

        private PointF period;
        private Point ceils;
        private readonly float emSize = 12;
        private readonly float signCountChar = 4;

        private Matrix transLines;
        private Matrix transSigns;

        #endregion

        #region Events
        public event Action<PointF, Point> ScaleEvent;
        public event Action<float, float> ShiftEvent;
        #endregion  

        public RectangleF CurBounds
        {
            get
            {
                float
                    left = float.MaxValue,
                    top = float.MaxValue;

                foreach(var line in this)
                {
                    line.SetVisible(transLines);

                    if (!line.Visible)
                        continue;

                    if (line is VerGridLine)
                    {
                        left = (float)line.Index < left ? (float)line.Index : left;    
                        left -= transLines.OffsetX / (period.X * scaleFactor.X);                      
                    }

                    if (line is HorGridLine)
                    {
                        top = (float)line.Index < top ? (float)line.Index : top;                      
                        top += transLines.OffsetY / (period.Y * scaleFactor.Y);                      
                    }    
                }
                return new RectangleF(left, top,
                    ceils.X * initSignStep * scaleSteps.X,
                    ceils.Y * initSignStep * scaleSteps.Y);
            }
        }

        public PointF Factor
        {
            get
            {
                return new PointF(period.X * scaleSteps.X / initSignStep,
                     period.Y * scaleSteps.Y / initSignStep);
            }
        }

        public PointF Zero
        {
            get { return zero; }
            
        }
   
        public Grid():base()
        {
            data = new MyList<ChartData>();
            transLines = new Matrix();
            transSigns = new Matrix();
            scaleFactor = new PointF(1, 1); 
            scaleSteps = new PointF(1,1);
            shiftSteps = new PointF();
            period = new PointF();
            ScaleEvent += XLinesScale;
            ScaleEvent += YLinesScale;
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
            InitLines();     
        }

        public void AddDataLine(ChartData dataLine)
        {
            data.Add(dataLine);
        }

        public void Scale(PointF ratio, Point mouseLocation)
        {           
            ScaleEvent.Invoke(ratio, mouseLocation);
        }

        public void Shift(float xShift, float yShift)
        {
            ShiftEvent.Invoke(xShift, yShift);        
        }

        public void Resize(Rectangle newBounds)
        {
            float xResizeRatio = (float)newBounds.Width / (float)bounds.Width;
            float yResizeRatio = (float)newBounds.Height / (float)bounds.Height;

            period.X *= xResizeRatio;
            period.Y *= yResizeRatio;
            zero.X *= xResizeRatio;
            zero.Y *= yResizeRatio;
            foreach (var line in this)
            {
                if (line is VerGridLine)
                    line.Coordinate = (int)(xResizeRatio * line.Coordinate);
                if (line is HorGridLine)
                    line.Coordinate = (int)(yResizeRatio * line.Coordinate);

                line.ScaleLineLength(new PointF(newBounds.Width, bounds.Height));
            }

            bounds = newBounds;
        }

        public void Draw(Graphics e)
        {
            e.Transform = transLines;
            foreach (GridLine line in this)
            {
                line.Draw(e);
            }
            DrawSigns(e);
            DrawData(e);
        }

        private void InitLines()
        {
            for(int i = -ceils.X; i <= 2*ceils.X; i++)
            {
                Add(new VerGridLine(bounds, i * (int)period.X));
                this.Last().Index = (i - ceils.X / 2) * initSignStep;
                this.Last().Font = new Font(FontFamily.GenericSansSerif, emSize);
                this.Last().Brush = Brushes.Black;
            }
                

            for (int i = -ceils.Y; i <= 2*ceils.Y; i++)
            {
                Add(new HorGridLine(bounds, i * (int)period.Y));
                this.Last().Index = -(i - ceils.Y / 2) * initSignStep;
                this.Last().Font = new Font(FontFamily.GenericSansSerif, emSize);
                this.Last().Brush = Brushes.Black;
            }
               
        }

        private void XLinesScale(PointF ratio, Point mouseLocation)
        {
            scaleFactor.X *= ratio.X;

            transLines.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, 0), MatrixOrder.Append);
            transSigns.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, 0), MatrixOrder.Append);

            if (scaleFactor.X < 0.5 || scaleFactor.X > 1.6)
            {
                scaleSteps.X /= scaleFactor.X > 1 ? 2f : 0.5f;

                if (scaleFactor.X > 1)
                    SetXIndexes(0.5f, 0);
                else
                    SetXIndexes(2, 0);


                scaleFactor.X = 1;

                transLines.Multiply(new Matrix(1 / transLines.Elements[0], 0, 0, 1, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1 / transLines.Elements[0], 0, 0, 1, 0, 0), MatrixOrder.Append);
                
            }
            else
            {
                transLines.Multiply(new Matrix(ratio.X, 0, 0, 1, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(ratio.X, 0, 0, 1, 0, 0), MatrixOrder.Append);
            }

            transLines.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, 0), MatrixOrder.Append);
            transSigns.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, 0), MatrixOrder.Append);

            foreach (var line in this)
            {
                if (line is VerGridLine)
                    line.ScaleLineLength(ratio);
            }
        }

        private void YLinesScale(PointF ratio, Point mouseLocation)
        {
            scaleFactor.Y *= ratio.Y;

            transLines.Multiply(new Matrix(1, 0, 0, 1, 0, -mouseLocation.Y), MatrixOrder.Append);
            transSigns.Multiply(new Matrix(1, 0, 0, 1, 0, -mouseLocation.Y), MatrixOrder.Append);

            if (scaleFactor.Y < 0.5 || scaleFactor.Y > 1.6)
            {
                scaleSteps.Y /= scaleFactor.Y > 1 ? 2f : 0.5f;


                if (scaleFactor.Y > 1)
                    SetYIndexes(0.5f, 0);
                else
                    SetYIndexes(2, 0);

                scaleFactor.Y = 1;

                transLines.Multiply(new Matrix(1, 0, 0, 1 / transLines.Elements[3], 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1, 0, 0, 1 / transLines.Elements[3], 0, 0), MatrixOrder.Append);
            }
            else
            {
                transLines.Multiply(new Matrix(1, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
                transSigns.Multiply(new Matrix(1, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);
            }

            transLines.Multiply(new Matrix(1, 0, 0, 1, 0, mouseLocation.Y), MatrixOrder.Append);
            transSigns.Multiply(new Matrix(1, 0, 0, 1, 0, mouseLocation.Y), MatrixOrder.Append);

            foreach (var line in this)
            {
                if(line is HorGridLine)
                    line.ScaleLineLength(ratio);
            }
        }

        private void LinesShift(float xShift, float yShift)
        {
            transSigns.Translate(xShift/scaleFactor.X, yShift / scaleFactor.Y);

            double stepX = Math.IEEERemainder(xShift * scaleFactor.X, period.X * scaleFactor.X);
            double stepY = Math.IEEERemainder(yShift * scaleFactor.Y, period.Y * scaleFactor.X);

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
                SetXIndexes(1, shift);
            }

            

            if (transLines.OffsetY + stepY != y)
            {
                if (y >= 0)
                    shift = -initSignStep * scaleSteps.Y * 
                        (float)Math.Ceiling(Math.Abs(yShift / (period.Y * scaleFactor.Y)));
                else
                    shift = initSignStep * scaleSteps.Y * 
                        (float)Math.Ceiling(Math.Abs(yShift / (period.Y * scaleFactor.Y)));
                SetYIndexes(1, shift);
 
            }

            transLines.Translate(-transLines.OffsetX, -transLines.OffsetY, MatrixOrder.Append);
            transLines.Translate((float)x, (float)y, MatrixOrder.Append);

            foreach (var line in this)
            {
                line.ScaleLineLength(new PointF((float)x, (float)y));
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
            MyList<RectangleF> signLocs = new MyList<RectangleF>();

            foreach (GridLine line in this)
            {
                float tempEmSize = emSize / scaleFactor.X;
                line.Font = new Font(FontFamily.GenericSansSerif, tempEmSize);

                PointF temp = zero;
                if (temp.X < -transLines.OffsetX / scaleFactor.X)
                    temp.X = -transLines.OffsetX / scaleFactor.X;
                if (temp.Y < -transLines.OffsetY / scaleFactor.Y)
                    temp.Y = -transLines.OffsetY / scaleFactor.Y;
                if (temp.X > bounds.Width / scaleFactor.X - signCountChar * tempEmSize - transLines.OffsetX / scaleFactor.X)
                    temp.X = bounds.Width / scaleFactor.X - signCountChar * tempEmSize - transLines.OffsetX / scaleFactor.X;
                if (temp.Y > bounds.Height / scaleFactor.Y - line.Font.Height/ scaleFactor.Y - transLines.OffsetY / scaleFactor.Y)
                    temp.Y = bounds.Height / scaleFactor.Y - line.Font.Height / scaleFactor.Y - transLines.OffsetY / scaleFactor.Y;

                RectangleF signBox;

                if (line is VerGridLine)
                {
                    signBox = new RectangleF(new PointF(line.Coordinate, temp.Y),
                        new SizeF(line.Font.Size, line.Font.Height));
                }
                else if (line is HorGridLine)
                {
                    signBox = new RectangleF(new PointF(temp.X, line.Coordinate),
                        new SizeF(line.Font.Size, line.Font.Height));
                }
                else signBox = new RectangleF();

                line.DrawSign(e, signBox.Location);

                signLocs.Add(signBox);
            }
        }
           
        private void DrawData(Graphics e)
        {
            foreach(var dataLine in data)
            {
                PointF[] points = dataLine.GetPointsInRange(CurBounds);



                using(GraphicsPath path = new GraphicsPath())
                {
                    if (points.Length != 0)
                    {
                        path.AddLines(points);
                        e.DrawPath(Pens.Red, path);
                    }       
                }
            }
        }
    }
}
