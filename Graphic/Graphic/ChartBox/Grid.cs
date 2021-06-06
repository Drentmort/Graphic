using Graphic.MyList;
using System.Drawing;

namespace Graphic.ChartBox
{
    class Grid:MyList<GridLine>
    {
        #region Common params
        private Rectangle bounds;
        private Point chartZero;
        private Point dataZero;
        #endregion

        #region Horizontal lines params
        private int horLineHalfCount;
        private int horLineStep;
        private double horScale;
        private double horShift;
        #endregion

        #region Vertical lines params
        private int verLineHalfCount;
        private int verStepOrigin;
        private double verStepScaled;
        private double verScale;
        private double verShift;
        #endregion

       
        public Grid()
        {
            horLineHalfCount = 3;
            horScale = 1;
            horShift = 0;

            verLineHalfCount = 3;
            verScale = 1;
            verShift = 0;

            ItemAdded += LineUpdate;
            ItemChanged += LineUpdate;
            ItemDeleted += LineDeleted;
        }
        
        public Grid(Rectangle bounds):this()
        {
            this.bounds = bounds;
            horLineStep = bounds.Height / (horLineHalfCount * 2 + 2);
            verStepOrigin = bounds.Width / (verLineHalfCount * 2 + 2);
            verStepScaled = verStepOrigin * verScale;

            dataZero = new Point(bounds.Width / 2, bounds.Height / 2);
            chartZero = new Point(bounds.Width / 2, bounds.Height / 2);
                    

            for (int i = -verLineHalfCount-1; i <= verLineHalfCount+1; i++)
                Add(new VerGridLine(i * verStepOrigin + chartZero.X, bounds));

            //for (int i = -horLineHalfCount - 1; i <= horLineHalfCount + 1; i++)
               // Add(new HorGridLine(i * horLineStep + chartZero.Y, bounds));
        }

        public void Shift(Point offset)
        {
            verShift = offset.X;
            horShift = offset.Y;
            dataZero.X += offset.X;
            dataZero.Y += offset.Y;
            chartZero.X = dataZero.X % verStepOrigin;
            chartZero.Y = dataZero.Y % horLineStep;
            Update();
        }

        public void Scale(double xScale, double yScale, Point location)
        {
            Clear();
            verScale *= xScale;
            verStepScaled = verStepOrigin * verScale;
            int diff = location.X % (int)verStepScaled;
            //diff = diff > verStepScaled / 2 ? diff : -diff;

            int count = 0; 
            int newVerCount = 0;
            while (true)
            {
                int verLocationPosit = location.X + (int)(verStepScaled * count) + diff;
                if (verLocationPosit <= bounds.Width)
                {
                    Add(new VerGridLine(verLocationPosit, bounds));
                    newVerCount++;
                }
                int verLocationNeg = location.X - (int)(verStepScaled * count) + diff - (int)verStepScaled;
                if (verLocationNeg >=0)
                {
                    Add(new VerGridLine(verLocationNeg, bounds));
                    newVerCount++;
                }
                if (verLocationPosit > bounds.Width && verLocationNeg < 0)
                    break;

                count++;
            }

        }
        
        public void Draw(Graphics e)
        {
            foreach (GridLine line in this)
            {
                line.Draw(e);
            }
        }

        private void LineDeleted(GridLine obj)
        {
            obj.Dispose();
        }    

        private void LineUpdate(GridLine obj)
        {
            //obj.Scale(verScale, horScale);
            if (obj is HorGridLine)
                obj.Shift(horShift,horLineStep);
            if (obj is VerGridLine)
                obj.Shift(verShift,verStepOrigin);
        }
    }
}
