#if UNITY_2019_4_OR_NEWER
using System;

namespace YooAsset.Editor
{
    public class SingleValueCell : ITableCell, IComparable
    {
        public object CellValue { set; get; }
        public string HeaderTitle { private set; get; }
        public double SingleValue
        {
            get
            {
                return (double)CellValue;
            }
        }

        public SingleValueCell(string headerTitle, object cellValue)
        {
            HeaderTitle = headerTitle;
            CellValue = cellValue;
        }
        public object GetDisplayObject()
        {
            return CellValue.ToString();
        }
        public int CompareTo(object other)
        {
            if (other is SingleValueCell cell)
            {
                return this.SingleValue.CompareTo(cell.SingleValue);
            }
            else
            {
                return 1;
            }
        }
    }
}
#endif