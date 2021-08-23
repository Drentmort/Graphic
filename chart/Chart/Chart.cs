using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using MathChart.ListG;

namespace MathChart.Chart
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
        private readonly float refreshStep = 2;
        private readonly float wheelStep = (float)Math.Pow(2, 0.1);
        private readonly float maxScaleLimit = 2f;
        private readonly float minScaleLimit = 0.5f;
        private readonly int signFieldSize = 5;

        private Font font;
        private ListG<Color> queryColors;

        private Side scaleType;
        private Point startLoc;
        private Point endLoc;

        private PointF globScale;
        private PointF scaleCount;
        private PointF center;
        private Matrix gridTransform;
        private Matrix dataTransform;
        private ListG<GridLine> gridLines;
        private Dictionary<int, ChartData> data;
        private Dictionary<int, Color> colors;

        public Matrix DataTransform
        {
            get 
            {
                BuildDataTransform();
                return dataTransform; 
            }
        }

        public Chart()
        {
            gridLines = new ListG<GridLine>();
            gridTransform = new Matrix();
            dataTransform = new Matrix();
            scaleType = Side.both;
            globScale = new PointF(1, 1);
            scaleCount = new PointF(1, 1);
            font = new Font(FontFamily.GenericSansSerif, 12);
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.BlueViolet, Color.OrangeRed, Color.GreenYellow};
            queryColors = new ListG<Color>(colors);
        }

        public void SetData(ChartData value)
        {
            if (data == null)
            {
                data = new Dictionary<int, ChartData>();
                colors = new Dictionary<int, Color>();
                data.Add(0, value);
                colors.Add(0, queryColors[0]);                
            }
            else
            {
                data.Add(data.Last().Key + 1, value);
                colors.Add(colors.Last().Key + 1, queryColors[(colors.Last().Key + 1) % queryColors.Count]);
            }
            data.Values.Last().Palette = this;
            PointF loc = new PointF(value.XMin, value.YMax);
            SizeF size = new SizeF(value.XMax - value.XMin, value.YMax - value.YMin);
            RescaleByRect(new RectangleF(loc, size));

            Invalidate();
        }

        public void RescaleByRect(RectangleF newField)
        {
            PointF point = GetPointByIndex(newField.Location);
            RectangleF rect = new RectangleF(point.X, point.Y,
                newField.Width * ceilSize,
                newField.Height * ceilSize);
            scaleCount = new PointF(1, 1);
            PointF[] points = new PointF[3]
            {
                new PointF(Bounds.Left,Bounds.Top),
                new PointF(Bounds.Right,Bounds.Top),
                new PointF(Bounds.Left,Bounds.Bottom)
            };
            gridTransform = new Matrix(rect, points);

            if (gridTransform.Elements[0] < minScaleLimit)
                scaleCount.X *= (float)Math.Pow(minScaleLimit, (int)Math.Log((1 / gridTransform.Elements[0]), 2));           

            if (gridTransform.Elements[3] < minScaleLimit)
                scaleCount.Y *= (float)Math.Pow(minScaleLimit, (int)Math.Log((1 / gridTransform.Elements[3]), 2));

            if (gridTransform.Elements[0] > maxScaleLimit)
                scaleCount.X *= (float)Math.Pow(maxScaleLimit, (int)Math.Log((gridTransform.Elements[0]), 2));

            if (gridTransform.Elements[3] > maxScaleLimit)
                scaleCount.Y *= (float)Math.Pow(maxScaleLimit, (int)Math.Log((gridTransform.Elements[3]), 2));

            float scX, scY;
            scX = 1 / scaleCount.X;
            scY = 1 / scaleCount.Y;
            gridTransform.Scale(scX, scY, MatrixOrder.Append);

            PointF zero = GetPointByIndex(new PointF(0, 0));
            gridTransform.Translate(-zero.X, -zero.Y, MatrixOrder.Append);
            point = GetPointByIndex(newField.Location);
            gridTransform.Translate(-point.X, -point.Y, MatrixOrder.Append);

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
            if (!new Rectangle(new Point(), Size).Contains(e.Location))
                return;
            float factor;
            if (e.Delta > 0)
            {
                factor = wheelStep;
            }
            else factor = 1 / wheelStep;

            gridTransform.Translate(-e.Location.X, -e.Location.Y, MatrixOrder.Append);

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
                line.BuildPen(scaleCount);
                line.Draw(e.Graphics);
                line.DrawSign(e.Graphics, centerSign);
            }
            base.OnPaint(e);

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Focus();
            base.OnMouseEnter(e);
        }

        private void RebuildTransfomations(PointF mouse)
        {
            PointF currentIndex = GetIndexByPoint(mouse);

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
            }

            PointF loc = GetPointByIndex(currentIndex);

            gridTransform.Translate(mouse.X - loc.X, mouse.Y - loc.Y);

        }

        private void BuildGrid(RectangleF bounds, PointF location)
        {

            int start = (int)(location.X - ((int)location.X % ceilSize));
            GridLine line = new VerGridLine(bounds, start - ceilSize);
            line.Index = (start - ceilSize - center.X ) / ceilSize / scaleCount.X;
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
            //foreach (var dataLines in data.Values)
            //    dataLines.Resolution = dataTransform;
        }

        private PointF GetIndexByPoint(PointF mouse)
        {
            Matrix matrix = gridTransform.Clone();
            matrix.Invert();
            PointF[] output = new PointF[1] { mouse };
            matrix.TransformPoints(output);

            output[0].X -= center.X;
            output[0].Y -= center.Y;

            output[0].X /= ceilSize * scaleCount.X;
            output[0].Y /= -ceilSize * scaleCount.Y;


            return output[0];
        }

        private PointF GetPointByIndex(PointF index)
        {
            PointF[] output = new PointF[1] { index };
            output[0].X *= ceilSize * scaleCount.X;
            output[0].Y *= -ceilSize * scaleCount.Y;

            output[0].X += center.X;
            output[0].Y += center.Y;

            gridTransform.TransformPoints(output);
            return output[0];
        }

        private PointF BuildPointForSigns(PaintEventArgs e)
        {
            PointF temp = new PointF(center.X, center.Y);
            if (center.X < e.Graphics.ClipBounds.Left)
                temp.X = e.Graphics.ClipBounds.Left;
            if (center.X > e.Graphics.ClipBounds.Right - signFieldSize * font.Size / e.Graphics.Transform.Elements[0])
                temp.X = e.Graphics.ClipBounds.Right - signFieldSize * font.Size / e.Graphics.Transform.Elements[0];

            if (center.Y < e.Graphics.ClipBounds.Top)
                temp.Y = e.Graphics.ClipBounds.Top;
            if (center.Y > e.Graphics.ClipBounds.Bottom - font.Height / e.Graphics.Transform.Elements[3])
                temp.Y = e.Graphics.ClipBounds.Bottom - font.Height / e.Graphics.Transform.Elements[3];
            return temp;
        }

        private void Chart_Paint(object sender, PaintEventArgs e)
        {
            if (data == null)
                return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            BuildDataTransform();
            foreach (var dataLine in data)
            {
                dataLine.Value.ChPen.Color = colors[dataLine.Key];
                dataLine.Value.Draw(e.Graphics);
            }

        }
    }
}
