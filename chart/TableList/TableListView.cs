using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MathChart.ListG;

namespace MathChart.TableList
{
    public partial class TableListView : UserControl
    {
        public event EventHandler ItemClick;
        public event EventHandler ItemDblClick;
        public event EventHandler SelectItem;
        public event Func<string, object> CellSetRule;

        private int countRow;
        private ListG<Row> rows;
        private ListG<Column> columns;
        private PanelTable panelTable;

        public int CountRow
        {
            get { return countRow; }
            set { SetCountRow(value); }
        }
        public ListG<Row> Rows => rows;
        public ListG<Column> Columns => columns;
        public int ColumnHeaderHeigth { get; set; }
        public int RowHeaderWidth { get; set; }
        public Color ColumnHeaderBack { get; set; }
        public Color BorderColor { get; set; }
        public Color SelectionCellColor { get; set; }
        public Color SelectionHeaderColor { get; set; }
        public bool NumerableRows { get; set; }
        public Font HearderFont { get; set; }
        public Brush HearderFontBrush { get; set; }

        public TableListView()
        {
            InitializeComponent();
            rows = new ListG<Row>();
            columns = new ListG<Column>();
            countRow = 0;
            ColumnHeaderHeigth = 20;
            RowHeaderWidth = 20;
            ColumnHeaderBack = Color.Pink;
            BorderColor = Color.Black;
            NumerableRows = true;

            HearderFont = new Font("Times", 9);
            HearderFontBrush = Brushes.Black;

            rows.CollectionChanged += EditRows;
            rows.CollectionChanged += RowsRebuildRequiest;
            rows.ItemAdded += Rows_ItemAdded;
            rows.ItemChanged += Rows_ItemChanged;
            columns.CollectionChanged += EditColumn;
            columns.ItemAdded += Columns_ItemAdded;
            columns.ItemTaken += Columns_ItemTaken;

            SelectionCellColor = Color.LightGray;
            SelectionCellColor = Color.Gray;

            panelTable = new PanelTable(this);
            panelTable.ItemClick += ItemClickPanel;
            panelTable.ItemDblClick += ItemDblClickPanel;
            panelTable.SelectItem += SelectItemPanel;
            panelTable.CellSetRule += PanelTable_ValueSet;

            BackColor = SystemColors.AppWorkspace;
            
            panelTable.Dock = DockStyle.Fill;
            Controls.Add(panelTable);
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

       

        private object PanelTable_ValueSet(string arg)
        {
            if (CellSetRule != null)
                return CellSetRule.Invoke(arg);
            else return arg;
        }

        private void SelectItemPanel(object sender, EventArgs e)
        {
            if(SelectItem!= null)
                SelectItem.Invoke(sender, e);
        }

        private void ItemDblClickPanel(object sender, EventArgs e)
        {
            if (ItemDblClick != null)
                ItemDblClick.Invoke(sender, e);
        }

        private void ItemClickPanel(object sender, EventArgs e)
        {
            if (ItemClick != null)
                ItemClick.Invoke(sender, e);
        }

        private void RowsRebuildRequiest()
        {
            panelTable.Invalidate();
        }

        private void Columns_ItemAdded()
        {
            if (columns.Count > 1)
            {
                int x = columns[columns.Count - 2].Area.X
                    + columns[columns.Count - 2].Width + 1;
                columns.Last().Area =
                        new Rectangle(x, 1, columns.Last().Width, ColumnHeaderHeigth);
            }

            if (columns.Count == 1)
            {
                columns.Last().Area =
                        new Rectangle(RowHeaderWidth + 3, 1, columns.Last().Width, ColumnHeaderHeigth);
            }
        }

        private void Rows_ItemAdded()
        {
            if (rows.Count > 1)
            {
                rows.Last().Index =
                   rows[rows.Count - 2].Index + 1;
                int y = rows[rows.Count - 2].Area.Y 
                    + rows.Last().Heigth + 1;
                rows.Last().Area =
                        new Rectangle(2, y, RowHeaderWidth, rows.Last().Heigth);
            }
            
            else if(rows.Count == 1)
            {
                rows.Last().Index = 1;
                rows.Last().Area =
                        new Rectangle(2, 2 + ColumnHeaderHeigth, RowHeaderWidth, rows.Last().Heigth);
            }
        }

        private void Rows_ItemChanged(Row row, int index)
        {
            if (index > 0)
            {
                row.Index =
                   rows[index - 1].Index + 1;
                int y = rows[index - 1].Area.Y
                    + row.Heigth + 1;
                row.Area =
                        new Rectangle(2, y, RowHeaderWidth, row.Heigth);
            }

            else if (index == 0)
            {
                row.Index = 1;
                row.Area =
                        new Rectangle(2, 2 + ColumnHeaderHeigth, RowHeaderWidth, row.Heigth);
            }
        }

        private void Columns_ItemTaken(Column arg1, int arg2)
        {
            arg1.Width = arg1.Caption.Length * (int)(HearderFont.Size);
        }

        public void SetCountRow(int value)
        {
            //При увеличении добавляем 
            if (value > countRow)
            {
                int iteration = value - countRow;
                for (int i = 0; i < iteration; i++)
                {
                    rows.Add(new Row());
                }
            }
            //при уменьшении удаляем с конца
            if (value < countRow)
            {
                int iteration = countRow - value;
                for (int i = 0; i < iteration; i++)
                {
                    rows.Remove(rows[rows.Count - 1]);
                }
            }

            countRow = value;
        }

        public void AddColumns(int count)
        {
            for (int i = 0; i < count; i++)
                columns.Add(new Column());
        }

        public void DeleteRows()
        {
            rows.Clear();
            countRow = 0;
        }

        public Row LastVisibleRow(Graphics graf)
        {
            foreach (var item in rows)
                if (item.Area.Y <= panelTable.Bounds.Height - graf.Transform.OffsetY &&
                   item.Area.Y + item.Area.Height >= panelTable.Bounds.Height - graf.Transform.OffsetY)
                    return item;
            if (rows.Last().Area.Y + rows.Last().Area.Height <= panelTable.Bounds.Height - graf.Transform.OffsetY)
                return rows.Last();

            return null;
        }

        public void SortRows(Column column, SortingInfo sorting = SortingInfo.ACS)
        {
            foreach (var col in columns)
                if (!col.Equals(column))
                    col.Sorting = SortingInfo.NONE;

            List<Row> r = new List<Row>(rows);
            int index = columns.IndexOf(column);
            r.Sort(delegate(Row x, Row y)
            {
                int descend = sorting == SortingInfo.DESC ? -1 : 1;
                if (x.Cells[index].Equals(y.Cells[index]))
                    return 0;
                else if (x.Cells[index] == null) return -1 * descend;
                else if (y.Cells[index] == null) return 1 * descend;
                else
                    return x.Cells[index].CompareTo(y.Cells[index]) * descend;
            });

            for(int i = 0; i < rows.Count; i++)
            {
                rows[i] = r[i];
            }
            columns[columns.IndexOf(column)].Sorting = sorting;
        }

        private void EditColumn()
        {
            for(int i = 0; i < columns.Count; i++)
            {
                if (i == 0)
                {
                    columns[i].Area =
                        new Rectangle(RowHeaderWidth + 3, 1, columns[i].Width, ColumnHeaderHeigth);
                    continue;
                }
                    
                int x = columns[i - 1].Area.X
                    + columns[i - 1].Width + 1;
                columns[i].Area = new Rectangle(x, 1, columns[i].Width, ColumnHeaderHeigth);
            }              
        }

        private void EditRows()
        {
            if (countRow < rows.Count)//Увеличение количества строк
            {
                rows[rows.Count - 1].Cells = CreatCells(columns.Count);//Добавление пустых ячеек в строку
                countRow++;
            }
            if (countRow > rows.Count)//уменьшение количества строк
            {
                countRow--;
            }

        }

        private ListG<Cell> CreatCells(int Count)
        {
            ListG<Cell> result = new ListG<Cell>();
            for (int i = 0; i < Count; i++)
            {
                result.Add(new Cell(panelTable));
            }
            return result;
        }

    }
}
