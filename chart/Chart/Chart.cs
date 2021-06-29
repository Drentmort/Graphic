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
        left,right,up,down
    }

    public partial class Chart : UserControl
    {
        private readonly int ceilSize = 50;

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
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            startLoc = e.Location;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.Left)
                return;
            if (!new Rectangle(new Point(), Size).Contains(e.Location))
                return;
            endLoc = e.Location;
            gridTransform.Translate(endLoc.X - startLoc.X, endLoc.Y - startLoc.Y);
            startLoc = endLoc;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {           
            gridLines.Clear();
            center = new PointF(Size.Width / 2, Size.Height / 2);
            BuildGrid();
            e.Graphics.Transform = gridTransform;
            foreach (var line in gridLines)
            {
                line.Draw(e.Graphics);
                line.DrawSign(e.Graphics, new Point());
            }
            base.OnPaint(e); 
        }

        private void BuildGrid()
        {
            AddLine(Side.up);
            AddLine(Side.down);
            AddLine(Side.right);
            AddLine(Side.left);
            gridLines.Add(new VerGridLine(Bounds, (int)center.X));
            gridLines.Add(new HorGridLine(Bounds, (int)center.Y));
        }

        private void AddLine(Side side)
        {
            int buildSign, indexSign;
            switch (side)
            {
                case (Side.left):
                    buildSign = -1;
                    indexSign = -1;
                    break;
                case (Side.right):
                    buildSign = 1;
                    indexSign = 1;
                    break;
                case (Side.up):
                    buildSign = -1;
                    indexSign = 1;
                    break;
                case (Side.down):
                    buildSign = 1;
                    indexSign = -1;
                    break;
                default:
                    buildSign = 0;
                    indexSign = 0;
                    break;
            }

            int count = 1;
            while (true)
            {

                GridLine line;
                if (side == Side.left || side == Side.right)
                {
                    int coordinate = (int)center.X + buildSign * count * ceilSize;
                    if (coordinate > Size.Width || coordinate < 0)
                        break;
                    line = new VerGridLine(Bounds, coordinate);
                }
                else
                {
                    int coordinate = (int)center.Y + buildSign * count * ceilSize;
                    if (coordinate > Size.Height || coordinate < 0)
                        break;
                    line = new HorGridLine(Bounds, coordinate);
                }
                line.Index = count * indexSign;
                gridLines.Add(line);
                count++;
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
