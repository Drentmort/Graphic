using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathChart.Chart;
using MathChart.TableList;
using MathChart.ListG;

namespace MathChart
{
    public partial class mainForm : Form   
    {
        private ListG<ChartData> datas;
        
        public mainForm()
        {
            InitializeComponent();
            DataTablePoints.NumerableRows = true;
            DataTablePoints.AddColumns(2);
            DataTablePoints.Columns[0].Caption = "X coordinate";
            DataTablePoints.Columns[1].Caption = "Y coordinate";        
            DataTablePoints.ItemDblClick += DataTablePoints_Sorting;
            DataLinesInfo.AddColumns(11);
            DataLinesInfo.Columns[0].Caption = "Название кривой";
            DataLinesInfo.Columns[1].Caption = "Тип кривой";
            DataLinesInfo.Columns[2].Caption = "Тип интерполяции точек";
            DataLinesInfo.Columns[3].Caption = "Цвет кривой";
            DataLinesInfo.Columns[4].Caption = "Количество точек";
            DataLinesInfo.Columns[5].Caption = "Закон";
            DataLinesInfo.Columns[6].Caption = "Временные данные";
            DataLinesInfo.Columns[7].Caption = "Пределы вдоль оси абсцисс";
            DataLinesInfo.Columns[8].Caption = "Пределы вдоль оси ординат";
            DataLinesInfo.Columns[9].Caption = "Единицы измерения по оси абсцисс";
            DataLinesInfo.Columns[10].Caption = "Единицы измерения по оси оридинат";
            datas = new ListG<ChartData>();

        }

        private void DataTablePoints_Sorting(object sender, EventArgs e)
        {
            if (sender is Column)
            {
                if((sender as Column).Sorting == SortingInfo.ACS)
                    DataTablePoints.SortRows(sender as Column, SortingInfo.DESC);
                else if((sender as Column).Sorting == SortingInfo.DESC)
                    DataTablePoints.SortRows(sender as Column, SortingInfo.ACS);
                else DataTablePoints.SortRows(sender as Column);
            }               
        }

        private void AddData_click(object sender, EventArgs e)
        {
            TimeData timeData1 = new TimeData();
            for (float i = -100; i <= 100; i+=0.1f)
                timeData1.AddPoint(i, Math.Sin(i));

            MainChart.SetData(timeData1);
            timeData1.AddInfoToTable(DataTablePoints);
            DataTablePoints.CellSetRule += delegate(string data) 
            { 
                float output = 0;
                if (float.TryParse(data, out output))
                    return output;
                return null;
            };
        }

        private void bAddLine_Click(object sender, EventArgs e)
        {

        }
    }
}
