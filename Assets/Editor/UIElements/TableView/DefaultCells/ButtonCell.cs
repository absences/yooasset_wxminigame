#if UNITY_2019_4_OR_NEWER
using System;

namespace YooAsset.Editor
{
    public class ButtonCell : ITableCell, IComparable
    {
        public object CellValue { set; get; }

        public object GetDisplayObject()
        {
            return string.Empty;
        }
        public int CompareTo(object other)
        {
            return 1;
        }
    }
}
#endif