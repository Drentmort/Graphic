using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MathChart.TableList
{
    public enum SortingInfo { NONE,ACS,DESC}
    public class Column:ITableSelectble,IEquatable<Column>
    {
        public string Caption { get; set; } 
        public int Width { get; set; }
        public Rectangle Area { get; set; }
        public Color Back { get; set; }
        public bool IsMarked { get; set; }
        public SortingInfo Sorting { get; set; }

        public Column()
        {
            Caption  = "CaptionColumn";//Текст заголовка
            Width = 100;
            Back = Color.White;
        }

        public bool Equals(Column other)
        {
            bool result = true;
            result &= Caption.Equals(other.Caption);
            result &= Area.Equals(other.Area);
            result &= IsMarked.Equals(other.IsMarked);
            return result;

        }
    }
}
