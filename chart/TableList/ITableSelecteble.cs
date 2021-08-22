using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathChart.TableList
{
    interface ITableSelectble
    {
        Rectangle Area { get; set; }
        bool IsMarked { get; set; }
    }
}
