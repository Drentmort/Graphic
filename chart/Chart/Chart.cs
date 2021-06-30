using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly int ceilSize = 64;
        private readonly float wheelStep = 1.05f;
        private readonly float maxScaleLimit = 1.75f;
        private readonly float minScaleLimit = 0.5f;
        private readonly float refreshStep = 2;
        private readonly int signFieldSize = 5;

        private Font font;
        private List<Color> queryColors;

        private Side scaleType;
        private Point startLoc;
        private Point endLoc;

        private PointF globScale;
        private PointF scaleCount;
        private PointF center;
        private Matrix gridTransform;
        private Matrix dataTransform;
        private List<GridLine> gridLines;
        private Dictionary<int, ChartData> data;
        private Dictionary<int, Color> colors;

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
            globScale = new PointF(1, 1);
            scaleCount = new PointF(1, 1);
            font = new Font(FontFamily.GenericSansSerif, 12);
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void RescaleByRect(RectangleF newField, bool inPixels)
        {
            float mult = 1;
            if (!inPixels)
                mult = ceilSize;
            PointF[] plgpts = new PointF[3]
            {
                new PointF(newField.Left*mult, newField.Top*mult),
                new PointF(newField.Right*mult, newField.Top*mult),
                new PointF(newField.Left*mult, newField.Bottom*mult)
            };
            gridTransform = new Matrix(Bounds, plgpts);
            RebuildTransfomations(newField.Location);
            Invalidate();
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
                factor = wheelStep;
            }
            else factor = 1/wheelStep;

            gridTransform.Translate(-e.Location.X, -e.Location.Y,MatrixOrder.Append);

            switch (scaleType)
            {
                case Side.vertical:
                    gridTransform.Scale(1, factor, MatrixOrder.Append);
                    globScale.Y *= factor;
                    break;
                case Side.horizontal:
                    gridTransform.Scale(factor, 1, MatrixOrder.Append);
                    globScale.X *= factor;
                    break;
                default:
                    gridTransform.Scale(factor, factor, MatrixOrder.Append);
                    globScale.X *= factor;
                    globScale.Y *= factor;            
                    break;
            }
            
            gridTransform.Translate(e.Location.X, e.Location.Y, MatrixOrder.Append);
            RebuildTransfomations(e.Location);
            base.OnMouseWheel(e);
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {           
            gridLines.Clear();

            int x = (int)Math.Round((float)Bounds.Width / (float)ceilSize / 2);
            int y = (int)Math.Round((float)Bounds.Height / (float)ceilSize / 2);
            center = new PointF(x * ceilSize, y * ceilSize);
            e.Graphics.Transform = gridTransform;
            BuildGrid(e.Graphics.ClipBounds, e.Graphics.ClipBounds.Location);

            PointF centerSign = BuildPointForSigns(e);
            foreach (var line in gridLines)
            {
                line.Draw(e.Graphics);
                line.DrawSign(e.Graphics, centerSign);
            }
            base.OnPaint(e);
            
        }

        private void RebuildTransfomations(PointF mouse)
        {
            float x = mouse.X;
            float y = mouse.Y;
            if (gridTransform.Elements[0] > maxScaleLimit ||
                gridTransform.Elements[0] < minScaleLimit)
            {
                if (gridTransform.Elements[0] > maxScaleLimit)
                    scaleCount.X *= refreshStep;
                if (gridTransform.Elements[0] < minScaleLimit)
                    scaleCount.X /= refreshStep;
                gridTransform.Translate(-x, -y, MatrixOrder.Append);
                gridTransform.Scale(1 / gridTransform.Elements[0], 1, MatrixOrder.Append);
                gridTransform.Translate(x, y, MatrixOrder.Append);
                isXSwitch = true;
            }

            if (gridTransform.Elements[3] > maxScaleLimit ||
                gridTransform.Elements[3] < minScaleLimit)
            {
                if (gridTransform.Elements[3] > maxScaleLimit)
                    scaleCount.Y *= refreshStep;
                if (gridTransform.Elements[3] < minScaleLimit)
                    scaleCount.Y /= refreshStep;
                gridTransform.Translate(-x, -y, MatrixOrder.Append);
                gridTransform.Scale(1, 1 / gridTransform.Elements[3], MatrixOrder.Append);
                gridTransform.Translate(x, y, MatrixOrder.Append);
                isYSwitch = true;
            }
        }

        private void BuildGrid(RectangleF bounds, PointF location)
        {

            int start = (int)(location.X - ((int)location.X % ceilSize));
            GridLine line = new VerGridLine(bounds, start-ceilSize);
            line.Index = (start - ceilSize - center.X) / ceilSize / scaleCount.X;
            line.Font = font;
            gridLines.Add(line);

            while (true)
            {
                line = new VerGridLine(bounds, start);
                line.Index = (start - center.X) / ceilSize / scaleCount.X;
                line.Font = font;
                gridLines.Add(line);
                start += ceilSize;
                if (start > bounds.Right)
                    break;
            }

            start = (int)(location.Y - ((int)location.Y % ceilSize));
            line = new HorGridLine(bounds, start - ceilSize);
            line.Index = -(start - center.Y - ceilSize) / ceilSize / scaleCount.Y;
            line.Font = font;
            gridLines.Add(line);

            while (true)
            {
                line = new HorGridLine(bounds, start);
                line.Index = -(start - center.Y) / ceilSize / scaleCount.Y;
                line.Font = font;
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
            dataTransform.Scale(ceilSize * scaleCount.X, ceilSize * scaleCount.Y, MatrixOrder.Prepend);
            foreach (var dataLines in data.Values)
                dataLines.Resolution = dataTransform;
        }

        private PointF BuildPointForSigns(PaintEventArgs e)
        {
            PointF temp = new PointF(center.X,center.Y);
            if (center.X < e.Graphics.ClipBounds.Left)
                temp.X = e.Graphics.ClipBounds.Left;
            if (center.X > e.Graphics.ClipBounds.Right - signFieldSize*font.Size)
                temp.X = e.Graphics.ClipBounds.Right - signFieldSize * font.Size;

            if (center.Y < e.Graphics.ClipBounds.Top)
                temp.Y = e.Graphics.ClipBounds.Top;
            if (center.Y > e.Graphics.ClipBounds.Bottom - font.Height)
                temp.Y = e.Graphics.ClipBounds.Bottom - font.Height;
            return temp;
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
