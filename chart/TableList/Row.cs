using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathChart.ListG;

namespace MathChart.TableList
{
    public class Row: ITableSelectble
    {
        public int Heigth { get; set; }// Высота строки
        public ListG<Cell> Cells { get; set; }//список ячеек
        public Rectangle Area { get; set; }
        public int Index { get; set; }
        public bool IsMarked { get; set; }

        public Row()
        {
            Heigth = 20;
            Cells = new ListG<Cell>();
        }

        public Row(ListG<Cell> cells):this()
        {
            Cells = cells;
        }
    }
}
