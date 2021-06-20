using Graphic.MyList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace Graphic.ChartBox
{
    class Grid:MyList<GridLine>
    {
        #region Common params
        private Rectangle bounds;
        private PointF[] window;
        private PointF axisResol;
        private PointF scaleFactor;
        private Matrix transform;

        #endregion

        #region Horizontal lines params
        private int horHalfNum;
        private int horPeriod;
        #endregion

        #region Vertical lines params
        private int verHalfNum;
        private int verPeriod;
        #endregion


        public Grid():base()
        {
            horHalfNum = 3;
            verHalfNum = 3;
            scaleFactor = new PointF(1, 1);
            transform = new Matrix();
        }
        
        public Grid(Rectangle Bounds, PointF AxisResol):this()
        {
            bounds = Bounds;
            axisResol = AxisResol;
            window = new PointF[4] 
            {
                new PointF(-axisResol.X, -axisResol.Y),
                new PointF(axisResol.X, -axisResol.Y),
                new PointF(axisResol.X, -axisResol.Y),
                new PointF(-axisResol.X, axisResol.Y)
            };
            verPeriod = bounds.Width / (2 * verHalfNum);
            horPeriod = bounds.Height / (2 * horHalfNum);

            for (int i = -10* verHalfNum; i <= 10*verHalfNum; i++)
                Add(new VerGridLine(bounds, i * verPeriod));

            for (int i = -10*horHalfNum; i <= 10*horHalfNum; i++)
                Add(new HorGridLine(bounds, i * horPeriod));

            foreach(var line in this)
                line.ScaleLineLength(new PointF(4, 4));        
        }

        public void Scale(PointF ratio, Point mouseLocation)
        {
            scaleFactor.X *= ratio.X;
            scaleFactor.Y *= ratio.Y;                  

            transform.Multiply(new Matrix(1, 0, 0, 1, -mouseLocation.X, -mouseLocation.Y), MatrixOrder.Append);

            if (scaleFactor.X < 0.5 || scaleFactor.X > 1.6)
            {
                scaleFactor.X = 1;
                scaleFactor.Y = 1;
                transform.Multiply(new Matrix(1/transform.Elements[0], 0, 0, 1/transform.Elements[3], 0, 0), MatrixOrder.Append);
            }
            else
                transform.Multiply(new Matrix(ratio.X, 0, 0, ratio.Y, 0, 0), MatrixOrder.Append);

            transform.Multiply(new Matrix(1, 0, 0, 1, mouseLocation.X, mouseLocation.Y), MatrixOrder.Append);

            //transform.TransformPoints(window);
        }

        public void Shift(float xShift, float yShift)
        {      
            var matrixOrder = MatrixOrder.Append;
            double x = Math.IEEERemainder(transform.OffsetX + xShift, verPeriod * scaleFactor.X);
            double y = Math.IEEERemainder(transform.OffsetY + yShift, horPeriod * scaleFactor.X);
            transform.Translate(-transform.OffsetX, -transform.OffsetY, matrixOrder);
            transform.Translate((float)x, (float)y, MatrixOrder.Append);
            transform.TransformPoints(window);
        }
        
        public void Draw(Graphics e)
        {
            e.Transform = transform;
            foreach (GridLine line in this)
            {
                line.Draw(e);
            }
        }  
        
    }
}
