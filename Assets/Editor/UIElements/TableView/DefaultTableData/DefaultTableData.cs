#if UNITY_2019_4_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    public class DefaultTableData : ITableData
    {
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { set; get; } = true;

        /// <summary>
        /// 单元格集合
        /// </summary>
        public IList<ITableCell> Cells { set; get; } = new List<ITableCell>();

        #region 添加默认的单元格数据
        public void AddButtonCell()
        {
            var cell = new ButtonCell();
            Cells.Add(cell);
        }
        public void AddAssetPathCell(string headerTitle, string path)
        {
            var cell = new AssetPathCell(headerTitle, path);
            Cells.Add(cell);
        }
        public void AddStringValueCell(string headerTitle, string value)
        {
            var cell = new StringValueCell(headerTitle, value);
            Cells.Add(cell);
        }
        public void AddIntegerValueCell(string headerTitle, long value)
        {
            var cell = new IntegerValueCell(headerTitle, value);
            Cells.Add(cell);
        }
        public void AddSingleValueCell(string headerTitle, double value)
        {
            var cell = new SingleValueCell(headerTitle, value);
            Cells.Add(cell);
        }
        #endregion
    }
}
#endif