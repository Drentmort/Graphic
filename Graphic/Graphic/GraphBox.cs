using Graphic.ChartBox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Graphic
{
    public class GraphBox : Control
    {

        private Grid grid = null;
        private Point mouseStartLocation = new Point(-1,-1);
        private Point mouseEndLocation;


        public GraphBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
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
            grid.Shift(new Point(mouseEndLocation.X - mouseStartLocation.X,
                mouseEndLocation.Y - mouseStartLocation.Y));
            mouseStartLocation = mouseEndLocation;
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
            grid.Shift(new Point(mouseEndLocation.X - mouseStartLocation.X,
                mouseEndLocation.Y - mouseStartLocation.Y));
            mouseStartLocation = mouseEndLocation;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Point location = e.Location;
            if (!Bounds.Contains(e.Location))
                return;
            location.X -= Bounds.X;
            location.Y -= Bounds.Y;
            if(e.Delta>0)
                grid.Scale(0.99, 0.99, location);
            else 
                grid.Scale(0.99, 0.99, location);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //if(grid!=null)
            //    grid.Resize(Bounds);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            grid = (grid == null) ? new Grid(Bounds) : grid;
            grid.Draw(e.Graphics);
        }


    }
}
