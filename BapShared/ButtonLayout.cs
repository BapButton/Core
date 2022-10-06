using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public class ButtonLayoutHistory
    {
        public int ButtonLayoutHistoryId { get; set; }
        public int ButtonLayoutId { get; set; }
        public DateTime DateUsed { get; set; }
        public ButtonLayout? ButtonLayout { get; set; }
    }
    public class ButtonLayout
    {
        public string Description => $"{TotalButtons} Buttons in {RowCount} Rows and {ColumnCount} Columns";
        public int ButtonLayoutId { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int TotalButtons { get; set; }
        public List<ButtonPosition> ButtonPositions { get; set; } = default!;
        public List<ButtonLayoutHistory> ButtonLayoutHistories { get; set; } = default!;
    }
    public class ButtonPosition
    {
        public int ButtonPositionId { get; set; }
        public ButtonLayout ButtonLayout { get; set; } = default!;
        public int ButtonLayoutId { get; set; }
        [StringLength(10)]

        public string ButtonId { get; set; } = "";
        public int RowId { get; set; }
        public int ColumnId { get; set; }
    }
}
