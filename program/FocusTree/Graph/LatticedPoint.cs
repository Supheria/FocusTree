using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Graph
{
    /// <summary>
    /// 栅格化坐标
    /// </summary>
    public struct LatticedPoint
    {
        /// <summary>
        /// 所在栅格列数
        /// </summary>
        int ColNumber;
        /// <summary>
        /// 所在栅格行数
        /// </summary>
        int RowNumber;
        /// <summary>
        /// 所在栅格列数
        /// </summary>
        public int Col { get => ColNumber; set => ColNumber = value; }
        /// <summary>
        /// 所在栅格行数
        /// </summary>
        public int Row { get => RowNumber; set => RowNumber = value; }
        public LatticedPoint()
        {
            ColNumber = 0;
            RowNumber = 0;
        }
        public LatticedPoint(int col, int row)
        {
            ColNumber = col;
            RowNumber = row;
        }
        /// <summary>
        /// 使用真实坐标创建，将坐标转换为栅格化坐标
        /// </summary>
        /// <param name="cursor"></param>
        public LatticedPoint(Point realPoint)
        {
            var widthDiff = realPoint.X - Lattice.OriginLeft;
            var heightDiff = realPoint.Y - Lattice.OriginTop;
            ColNumber = widthDiff / LatticeCell.Width;
            RowNumber = heightDiff / LatticeCell.Height;
            if (widthDiff < 0) { ColNumber--; }
            if (heightDiff < 0) { RowNumber--; }
        }
        /// <summary>
        /// 行列数是否都相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator ==(LatticedPoint lhd, LatticedPoint rhd) => lhd.Col == rhd.Col && lhd.Row == rhd.Row;
        /// <summary>
        /// 行列数是否有任一不相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator !=(LatticedPoint lhd, LatticedPoint rhd) => lhd.Col != rhd.Col || lhd.Row != rhd.Row;
        /// <summary>
        /// 尚未重写 Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 已重写 GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(ColNumber, RowNumber);
    }
}
