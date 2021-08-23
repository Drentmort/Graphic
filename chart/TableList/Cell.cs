using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MathChart.TableList
{
    public class Cell : ICloneable, ITableSelectble, IEquatable<Cell>, IComparable<Cell>
    {
        private Control parent = null;

        public object Value { get; set; } = null;
        public Rectangle Area { get; set; }
        public bool IsMarked { get; set; }
     

        public Cell(Control par)
        {
            parent = par;
            Value = null;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Draw(Graphics gr, TableListView source)
        {
            if(IsMarked)
            {
                Rectangle selArea = Area;
                selArea.X--; selArea.Y--;
                selArea.Width++; selArea.Height++;
                gr.FillRectangle(new SolidBrush(source.SelectionCellColor), selArea);
                gr.DrawRectangle(new Pen(source.BorderColor), selArea);
                
            }
            else
            {             
                gr.DrawRectangle(new Pen(source.BorderColor), Area);
                gr.FillRectangle(new SolidBrush(Color.White), Area);
            }

            if (Value != null)
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                gr.DrawString(Value.ToString(), source.HearderFont, source.HearderFontBrush, Area, sf);
            }
        }

        public bool Equals(Cell other)
        {
            if( Value is string )
                return (Value as string).Equals(other.Value as string);
            if (Value is int || Value is double || Value is float)
                return Value == other.Value;
            if (Value == null)
                return false;
            return Value.Equals(other.Value);
        }

        public int CompareTo(Cell other)
        {
            if (other == null)
                return -1;
            if (other.Value == null || Value == null)
                return 0;
            if (Value is float)
                return ((float)Value).CompareTo((float)other.Value);
            if (Value is IComparable)
                return (Value as IComparable).CompareTo(other.Value as IComparable);
            return Value.ToString().CompareTo(other.Value.ToString());
        }

        public ComboBox MakeComboBox(string[] values)
        {
            Value = new ComboBox();
            (Value as ComboBox).Items.AddRange(values);
            parent.Controls.Add(Value as Control);
            return Value as ComboBox;
        }

        public override string ToString()
        {
            if (Value == null)
                return "";
            return Value.ToString();
        }
    }
}
