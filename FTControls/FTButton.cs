using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTControls
{
    public partial class FTButton : UserControl
    {
        /// <summary>
        /// 边长
        /// </summary>
        private int mDefaultSideLength = 15;
        const string category = "FTControls";
        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        [Description("单击按钮时"), Category(category)]
        public event EventHandler TFClick;
        /// <summary>
        /// 背景颜色
        /// </summary>
        private Color mBackColorEx = Color.Transparent;
        [Description("背景颜色"), Category(category)]
        public Color BackColorEx
        {
            get
            {
                return mBackColorEx;
            }
            set
            {
                mBackColorEx = value;
                label.BackColor = mBackColorEx;
            }
        }
        /// <summary>
        /// 鼠标悬停在到控件上时的背景颜色
        /// </summary>
        private Color mBackColorHover = Color.FromArgb(225, 128, 0);
        [Description("鼠标悬停在到控件上时的背景颜色"), Category(category)]
        public Color BackColorHover
        {
            get
            {
                return mBackColorHover;
            }
            set
            {
                mBackColorHover = value;
            }
        }
        /// <summary>
        /// 鼠标离开控件时的背景颜色
        /// </summary>
        private Color mBackColorLeave = Color.Transparent;
        [Description("鼠标离开控件时的背景颜色"), Category(category)]
        public Color BackColorLeave
        {
            get
            {
                return mBackColorLeave;
            }
            set
            {
                mBackColorLeave = value;
            }
        }
        /// <summary>
        /// 文本颜色
        /// </summary>
        private Color mTextColor = Color.Black;
        [Description("文本颜色"), Category(category)]
        public Color TextColor
        {
            get
            {
                return mTextColor;
            }
            set
            {
                mTextColor = value;
                //label.ForeColor = mTextColor;
                DrawSign();
            }
        }
        /// <summary>
        /// 文本字体
        /// </summary>
        private bool mIsPlus = true;
        [Description("加号或者减号"), Category(category)]
        public bool IsPlus
        {
            get
            {
                return mIsPlus;
            }
            set
            {
                mIsPlus = value;
                DrawSign();
            }
        }

        public FTButton()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 控件缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTResize(object sender, EventArgs e)
        {
            // 界面设置成等距
            var ctrl = (Control)sender;
            // 这里再改改（只能拖动一边的大小）
            var side = ctrl.Size.Width > this.Size.Width ? ctrl.Size.Width : ctrl.Size.Height;
            this.Size = new Size(side, side);
            label.Size = new Size(side, side);
            DrawSign();
        }
        /// <summary>
        /// 鼠标键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTMouseDown(object sender, MouseEventArgs e)
        {
            // 显示边框
            this.BorderStyle = BorderStyle.Fixed3D;
            // 设置背景颜色
            this.BackColor = mBackColorHover;
            if (TFClick != null)
            {
                // 触发单击事件
                TFClick(sender, e);
            }
            this.Update();
        }
        /// <summary>
        /// 鼠标键抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTMouseUp(object sender, MouseEventArgs e)
        {
            // 去掉边框
            this.BorderStyle = BorderStyle.None;
            this.Update();
        }
        /// <summary>
        /// 光标悬停在控件上时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTMouseHover(object sender, EventArgs e)
        {
            // 设置背景颜色
            base.BackColor = mBackColorHover;
            this.Update();
        }
        /// <summary>
        /// 光标离开控件后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FTMouseLeave(object sender, EventArgs e)
        {
            // 设置背景颜色
            base.BackColor = mBackColorLeave;
            // 去掉边框
            this.BorderStyle = BorderStyle.None;
            this.Update();
        }
        /// <summary>
        /// 绘制正负号
        /// </summary>
        private void DrawSign()
        {

            label.Image = new Bitmap(Size.Width, Size.Height);
            var graph = Graphics.FromImage(label.Image);
            // 绘制横线
            float side = Size.Width; // 边长
            float width = side * 0.1f; // 线宽
            float gap = width * 0.5f; // 中心偏移量
            float length = side - 2 * width; // 线长
            float halfSide = side * 0.5f; // 半边长
            graph.FillRectangle(new SolidBrush(mTextColor),
                width,
                halfSide - gap,
                length,
                width);
            if (mIsPlus)
            {
                // 绘制竖线
                graph.FillRectangle(new SolidBrush(mTextColor),
                halfSide - gap,
                width,
                width,
                length);
            }
            graph.Flush();
            label.Update();
        }
    }
}
