using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;

namespace MathChart.TableList
{
    internal class PanelTable : ScrollableControl
    {
        public event EventHandler ItemClick;
        public event EventHandler ItemDblClick;
        public event EventHandler SelectItem;
        public event Func<string, object> CellSetRule;

        private TableListView BParent;
        private Matrix matrix;
        private ListG.ListG<object> selected;
        private int VScrollWheelSpeed = 40;      

        private TextBox tbEdit;
        private object edittingObj;
        private bool isEdittingObj;

        public PanelTable(TableListView bParent)
        {
            HScroll = true;
            VScroll = true;
            AutoScroll = true;
            BParent = bParent;
            matrix = new Matrix();
            SetStyle(
                ControlStyles.UserPaint | 
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer, true);
            selected = new ListG.ListG<object>();
            selected.ItemAdded += SelectedAdded;
            selected.ItemDeleted += SelectedDeleted;
            BParent.Rows.CollectionChanged += StopEditCell;
        }    

        protected override void OnScroll(ScrollEventArgs se)
        {
            if(se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                matrix.Translate(-matrix.OffsetX, 0, MatrixOrder.Append);
                matrix.Translate(AutoScrollPosition.X, 0, MatrixOrder.Append);
            }
            else
            {
                matrix.Translate(0, -matrix.OffsetY, MatrixOrder.Append);
                matrix.Translate(0, AutoScrollPosition.Y, MatrixOrder.Append);
            }
      
            Invalidate(); 
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if(!isEdittingObj)
                Focus();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!new Rectangle(new Point(), Size).Contains(e.Location) || 
                AutoScrollMinSize.Height - Height < matrix.OffsetY)
                return;
            int sign = e.Delta / Math.Abs(e.Delta);
            if (matrix.OffsetY + VScrollWheelSpeed * sign <= 0 && 
                matrix.OffsetY + VScrollWheelSpeed * sign >= (AutoScrollMinSize.Height - ClientSize.Height)*(-1) && 
                VScroll)
                matrix.Translate(0, VScrollWheelSpeed * sign, MatrixOrder.Append);

            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            selected.Clear();
            object r = GetObjectByMouse(e);
            if (r != null)
                selected.Add(r);
            if(selected.Count>0)
                ItemClick.Invoke(selected.Last(), new EventArgs());

            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            selected.Clear();
            object r = GetObjectByMouse(e);
            if (r != null)
                selected.Add(r);
            StopEditCell();
            RunEditCell(r);
            if (selected.Count > 0)
                ItemDblClick.Invoke(r, new EventArgs());

            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            selected.Clear();
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
                StopEditCell();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Transform = matrix;
            Graphics graf = e.Graphics;
            int maxWidth = 0;
            int maxHeight = 0;

            foreach (Column item in BParent.Columns)
            {
                maxWidth += item.Width;
            }
            //расчитываем высоту
            foreach (Row item in BParent.Rows)
            {
                maxHeight += item.Heigth + 1;
            }
            AutoScrollMinSize = new Size(maxWidth + 100, maxHeight + 100);

            Row lastVisibleRow = null;
            try { lastVisibleRow = BParent.LastVisibleRow(graf); }
            catch { }   
            if (lastVisibleRow != null)
                BParent.RowHeaderWidth = (int)(lastVisibleRow.Index.ToString().Length * BParent.HearderFont.Size + 4);
            else BParent.RowHeaderWidth = (int)(3 * BParent.HearderFont.Size) + 4;

            graf.Clear(BParent.BackColor);
            BParent.Columns.Refresh();
            DrawHeaderRows(graf);           
            DrawCells(graf);
            DrawHeaderColumns(graf);

            if(tbEdit != null && edittingObj != null)
            {
                Rectangle rec = (edittingObj as Cell).Area;
                tbEdit.SetBounds(rec.X + (int)matrix.OffsetX, rec.Y + (int)matrix.OffsetY, rec.Width, rec.Height);
            }
        }
 
        private void DrawHeaderColumns(Graphics graf)
        {
            Rectangle corner = new Rectangle(2, 1, BParent.RowHeaderWidth, BParent.ColumnHeaderHeigth);
            graf.DrawRectangle(new Pen(BParent.BorderColor), corner);
            graf.FillRectangle(new SolidBrush(BParent.ColumnHeaderBack), corner);

            foreach (Column item in BParent.Columns)
            {
                Rectangle rect = item.Area;

                if (item.IsMarked)
                {
                    rect.X--; rect.Y--;
                    rect.Width++; rect.Height++;
                    graf.FillRectangle(new SolidBrush(BParent.SelectionHeaderColor), rect);
                    graf.DrawRectangle(new Pen(BParent.BorderColor), rect);
                }
                else
                {
                    graf.DrawRectangle(new Pen(BParent.BorderColor), rect);
                    graf.FillRectangle(new SolidBrush(BParent.ColumnHeaderBack), rect);
                }

                if (item.Caption.Length != 0)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    graf.DrawString(item.Caption, BParent.HearderFont, BParent.HearderFontBrush, rect, sf);
                }
            }
        }

        private void DrawHeaderRows(Graphics graf)
        {
            foreach (Row item in BParent.Rows)
            {
                Rectangle rect = item.Area;
                rect.Width = BParent.RowHeaderWidth;
                item.Area = rect;
                if (item.IsMarked)
                {
                    rect.X--; rect.Y--;
                    rect.Width++; rect.Height++;
                    graf.FillRectangle(new SolidBrush(BParent.SelectionHeaderColor), rect);
                    graf.DrawRectangle(new Pen(BParent.BorderColor), rect);               
                }
                else
                {
                    graf.DrawRectangle(new Pen(BParent.BorderColor), rect);
                    graf.FillRectangle(new SolidBrush(BParent.ColumnHeaderBack), rect);
                }
                
                if (BParent.NumerableRows)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    graf.DrawString(item.Index.ToString(), BParent.HearderFont, BParent.HearderFontBrush, rect, sf);
                }
            }
        }

        private void DrawCells(Graphics graf)
        {
            int x = 2 + BParent.RowHeaderWidth + 1;
            int y = 2 + BParent.ColumnHeaderHeigth;
            Rectangle rect;
            int i = 0;
            foreach (Row itemRow in BParent.Rows)
            {
                foreach (Column itemColumn in BParent.Columns)
                {
                    rect = new Rectangle(x, y, itemColumn.Width, itemRow.Heigth);
                    itemRow.Cells[i].Area = rect;
                    itemRow.Cells[i].Draw(graf, BParent);
                    x += itemColumn.Width + 1;
                    i++;
                }
                i = 0;
                y += itemRow.Heigth + 1;
                x = 2 + BParent.RowHeaderWidth + 1;
            }
        }

        private void SelectedDeleted()
        {
            foreach (var el in selected)
            {
                if (el is Cell)
                    (el as Cell).IsMarked = false;

                if (el is Row)
                {
                    foreach (Cell cell in (el as Row).Cells)
                        cell.IsMarked = false;
                    (el as Row).IsMarked = false;
                }

                if (el is Column)
                {
                    int index = BParent.Columns.IndexOf(selected.Last() as Column);

                    foreach (Row row in BParent.Rows)
                        row.Cells[index].IsMarked = false;

                    (selected.Last() as Column).IsMarked = false;
                }

            }
        }

        private void SelectedAdded()
        {
            if (selected.Last() is Cell)
            {
                Cell tmp = selected.Last() as Cell;
                tmp.IsMarked = true;
                SelectItem.Invoke(tmp, new EventArgs());
            }

            if (selected.Last() is Row)
            {
                (selected.Last() as Row).IsMarked = true;
                 SelectItem.Invoke(selected.Last(), new EventArgs());
                foreach (Cell cell in (selected.Last() as Row).Cells)
                {
                    cell.IsMarked = true;
                    SelectItem.Invoke(cell, new EventArgs());
                }
                   
                
            }

            if (selected.Last() is Column)
            {
                int index = BParent.Columns.IndexOf(selected.Last() as Column);
                SelectItem.Invoke(selected.Last(), new EventArgs());
                foreach (Row row in BParent.Rows)
                {
                    row.Cells[index].IsMarked = true;
                    SelectItem.Invoke(row.Cells[index], new EventArgs());
                }              

                (selected.Last() as Column).IsMarked = true;
            }
        }

        private object GetObjectByMouse(MouseEventArgs e)
        {
            Point location = e.Location;
            location.X -= (int)matrix.OffsetX;
            location.Y -= (int)matrix.OffsetY;

            if (location.Y < BParent.ColumnHeaderHeigth)
                foreach (Column itemColumn in BParent.Columns)
                    if (itemColumn.Area.Contains(location))
                        return itemColumn;

            foreach (Row itemRow in BParent.Rows)
            {
                if (location.Y >= itemRow.Area.Y && location.Y <= itemRow.Area.Y + itemRow.Area.Height)
                {
                    if (itemRow.Area.Contains(location))
                        return itemRow;
                    else
                        foreach (Cell cell in itemRow.Cells)
                            if (cell.Area.Contains(location))
                                return cell;
                }
            }
            return null;
        }

        private void RunEditCell(object obj)
        {
            if (!(obj is Cell) || isEdittingObj)
                return;
            isEdittingObj = true;
            edittingObj = obj;
            tbEdit = new TextBox();
            tbEdit.Text = obj.ToString();
            Rectangle rec = (obj as Cell).Area;
            tbEdit.SetBounds(rec.X + (int)matrix.OffsetX, rec.Y + (int)matrix.OffsetY, rec.Width, rec.Height);
            tbEdit.KeyDown += TbEdit_KeyDown;
            tbEdit.LostFocus += TbEdit_LostFocus;
            Controls.Add(tbEdit);
            tbEdit.Focus();
        }

        private void TbEdit_LostFocus(object sender, EventArgs e)
        {
            StopEditCell();
        }

        private void TbEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                StopEditCell();
        }

        private void StopEditCell()
        {
            if (!isEdittingObj)
                return;
            isEdittingObj = false;
            object val = CellSetRule.Invoke(tbEdit.Text);
            if(val != null)
            {
                (edittingObj as Cell).Value = CellSetRule.Invoke(tbEdit.Text);
                foreach(var cell in BParent.Rows.Last().Cells)
                    if (cell.Value != null)
                    {
                        BParent.Rows.Add(new Row());
                        break;
                    } 
            }
                
            Controls.Remove(tbEdit);
            tbEdit.Dispose();
            tbEdit = null;
        }
    }
}
