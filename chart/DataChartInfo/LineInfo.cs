using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathChart.Chart;
using MathChart.Interpol;
using MathChart.TableList;
using MathChart.ListG;
using MathChart.Calculus;
using System.Drawing;
using System.Windows.Forms;

namespace MathChart.DataChartInfo
{
    public enum LineType { EMPIRIC, THEORETICAL }
    class LineInfo
    {
        private int pointCount;
        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private string calling;
        private string xUnits;
        private string yUnits;
        private Color lineColor;
        private ChartData data;
        private LineType lineType;
        private IMathModel model;
        private IInterpol interpol;

        public LineInfo()
        {
            pointCount = 0;
            xMin = -1; xMax = 1; yMin = -1; yMax = 1;
            calling = "Новая линия";
            xUnits = ""; yUnits = "";
            lineColor = Color.Red;
            data = new ChartData();
            lineType = LineType.EMPIRIC;
            model = null;
            interpol = null;
        }

        public void FillRow(Row row)
        {
            if (row.Cells.Count != 11)
                return;
            row.Cells[0].Value = calling;
            row.Cells[1].Value = calling;

            //row.Cells[2].MakeComboBox()
            row.Cells[3].Value = calling;
            row.Cells[4].Value = calling;
            row.Cells[5].Value = calling;
            row.Cells[6].Value = calling;
            row.Cells[7].Value = calling;
            row.Cells[8].Value = calling;
            row.Cells[9].Value = calling;
            row.Cells[10].Value = calling;

        }

    }
}
