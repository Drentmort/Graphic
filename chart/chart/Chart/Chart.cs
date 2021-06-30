using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using MathChart.Chart;

namespace chart.Chart
{
    enum Side
    {
        vertical,
        horizontal,
        both
    }

    public partial class Chart : UserControl
    {
        private readonly int ceilSize = 50;
        private readonly float WheelStep = 1.1f;

        private Side scaleType;
        private Point startLoc;
        private Point endLoc;


        private PointF center;
        private Matrix gridTransform;
        private Matrix dataTransform;
        private List<GridLine> gridLines;
        private Dictionary<int, ChartData> data;

        public ChartData Data
        {
            set 
            {
                if (data == null)
                {
                    data = new Dictionary<int, ChartData>();
                    data.Add(1, value);
                }
                else data.Add(data.Last().Key + 1, value);
                Invalidate();
            }
        } 
        
        public Chart()
        {       
            gridLines = new List<GridLine>();
            gridTransform = new Matrix();
            dataTransform = new Matrix();
            scaleType = Side.both;
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            startLoc = e.Location;
            base.OnMouseDown(e);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {            
            if (e.Button != MouseButtons.Left)
                return;
            if (!new Rectangle(new Point(), Size).Contains(e.Location))
                return;
            endLoc = e.Location;
            int x = (int)((endLoc.X - startLoc.X) / gridTransform.Elements[0]);
            int y = (int)((endLoc.Y - startLoc.Y) / gridTransform.Elements[3]);
            gridTransform.Translate(x, y);
            startLoc = endLoc;
            base.OnMouseMove(e);
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Shift)
                scaleType = Side.horizontal;
            else if (e.Alt)
                scaleType = Side.vertical;
            else
                scaleType = Side.both;
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            scaleType = Side.both;
            base.OnKeyUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            float factor;
            if (e.Delta > 0)
            {
                factor = WheelStep;
            }
            else factor = 1 / WheelStep;

            gridTransform.Translate(-e.Location.X, -e.Location.Y,MatrixOrder.Append);

            switch (scaleType)
            {
                case Side.vertical:
                    gridTransform.Scale(1, factor, MatrixOrder.Append);
                    break;
                case Side.horizontal:
                    gridTransform.Scale(factor, 1, MatrixOrder.Append);
                    break;
                default:
                    gridTransform.Scale(factor, factor, MatrixOrder.Append);
                    break;
            }
            
            gridTransform.Translate(e.Location.X, e.Location.Y, MatrixOrder.Append);

            base.OnMouseWheel(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {           
            gridLines.Clear();

            int x = (int)Math.Round((float)Bounds.Width / (float)ceilSize / 2);
            int y = (int)Math.Round((float)Bounds.Height / (float)ceilSize / 2);
            center = new PointF(x * ceilSize, y * ceilSize);

            e.Graphics.Transform = gridTransform;
            BuildGrid(e.Graphics.ClipBounds, e.Graphics.ClipBounds.Location); 
            
            foreach (var line in gridLines)
            {
                line.Draw(e.Graphics);
                line.DrawSign(e.Graphics, center);
            }
            base.OnPaint(e);
            
        }

        private void BuildGrid(RectangleF bounds, PointF location)
        {

            int start = (int)(location.X - ((int)location.X % ceilSize)) ;

            GridLine line = new VerGridLine(bounds, start-ceilSize);
            line.Index = start / ceilSize - center.X / ceilSize;
            gridLines.Add(line);

            while (true)
            {
                line = new VerGridLine(bounds, start);
                line.Index = start / ceilSize - center.X/ceilSize;
                gridLines.Add(line);
                start += ceilSize;
                if (start > bounds.Right)
                    break;
            }

            start = (int)(location.Y - ((int)location.Y % ceilSize)) ;
            while (true)
            {
                line = new HorGridLine(bounds, start);
                line.Index = -start / ceilSize + center.Y / ceilSize; ;
                gridLines.Add(line);
                start += ceilSize;
                if (start > bounds.Bottom)
                    break;
            }

        }

        private void BuildDataTransform()
        {
            dataTransform.Reset();
            dataTransform.Translate(center.X, center.Y, MatrixOrder.Append);
            dataTransform.Scale(ceilSize, ceilSize, MatrixOrder.Prepend);
            foreach (var dataLines in data.Values)
                dataLines.Resolution = dataTransform;
        }

        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            if (data == null)
                return;

            BuildDataTransform();
            foreach (var dataLines in data.Values)
                dataLines.Draw(e.Graphics);
        }
    }
}
