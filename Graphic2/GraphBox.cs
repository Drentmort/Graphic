using Graphic.ChartBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace Graphic
{
    public class GraphBox : Control
    {

        private Matrix transformation = null;
        private Grid grid = null;
        private PointF mouseStartLocation = new PointF(-1,-1);
        private PointF mouseEndLocation;
        private PointF location = new PointF();
        private PointF shift;

        private readonly int speedLimit = 15;

        public GraphBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            transformation = new Matrix();
            shift = new Point();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseStartLocation = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseEndLocation = e.Location;
            float signX = (mouseEndLocation.X - mouseStartLocation.X) / 
                Math.Abs(mouseEndLocation.X - mouseStartLocation.X);

            float signY = (mouseEndLocation.Y - mouseStartLocation.Y) /
                Math.Abs(mouseEndLocation.Y - mouseStartLocation.Y);

            float xDiff = Math.Abs(mouseEndLocation.X - mouseStartLocation.X) > speedLimit ?
                speedLimit * signX : mouseEndLocation.X - mouseStartLocation.X;

            float yDiff = Math.Abs(mouseEndLocation.Y - mouseStartLocation.Y) > speedLimit ?
                speedLimit * signY : mouseEndLocation.Y - mouseStartLocation.Y;


            grid.Shift(xDiff, yDiff);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)   
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.Left)
                return;

            if (mouseStartLocation != e.Location)
            {
                mouseEndLocation = e.Location;
            }

            float signX = (mouseEndLocation.X - mouseStartLocation.X) /
                 Math.Abs(mouseEndLocation.X - mouseStartLocation.X);

            float signY = (mouseEndLocation.Y - mouseStartLocation.Y) /
                 Math.Abs(mouseEndLocation.Y - mouseStartLocation.Y);

            float xDiff = Math.Abs(mouseEndLocation.X - mouseStartLocation.X) > speedLimit ?
                speedLimit * signX : mouseEndLocation.X - mouseStartLocation.X;
            float yDiff = Math.Abs(mouseEndLocation.Y - mouseStartLocation.Y) > speedLimit ?
                speedLimit * signY : mouseEndLocation.Y - mouseStartLocation.Y;


            grid.Shift(xDiff, yDiff);
            mouseStartLocation = mouseEndLocation;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            location = e.Location;
            if (!Bounds.Contains(e.Location))
                return;
            location.X -= Bounds.X;
            location.Y -= Bounds.Y;
            float K = e.Delta > 0 ? 1.05f : 0.95f;
            grid.Scale(new PointF(K, K), e.Location);           

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Bounds.Width-1, Bounds.Height-1));
            grid = (grid == null) ? new Grid(Bounds, 10) : grid;
            grid.Draw(e.Graphics);
            
        }

    }
}
