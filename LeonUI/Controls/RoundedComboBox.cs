﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Threading;
using System.Runtime.InteropServices;

namespace LeonUI.Controls
{
    [ToolboxBitmap(typeof(ComboBox))]
    public partial class RoundedComboBox : UserControl
    {

        private Rectangle CenterRectangle = new Rectangle(17,16,60,1);

        public int SelectedIndex
        {
            get => ItemsListBox.SelectedIndex;
            set => ItemsListBox.SelectedIndex=value;
        }

        public new event EventHandler TextChanged
        {
            add => InnerTextBox.TextChanged += value;
            remove => InnerTextBox.TextChanged -= value;
        }

        public event EventHandler DropDown;

        public event EventHandler SelectedIndexChanged
        {
            add {ItemsListBox.SelectedIndexChanged += value;}
            remove { ItemsListBox.SelectedIndexChanged -= value; }
        }

        /// <summary>
        /// 下拉项目列表
        /// </summary>
        private ListBox ItemsListBox = null;

        /// <summary>
        /// 下拉项目浮动容器
        /// </summary>
        private ToolStripDropDown toolStripDropDown = null;

        /// <summary>
        /// 下拉项目容器
        /// </summary>
        private ToolStripControlHost toolStripControlHost = null;

        [Browsable(true)]
        // System.Windows.Forms.ListBox
        /// <summary>Gets the items of the <see cref="T:System.Windows.Forms.ListBox" />.</summary>
        /// <returns>An <see cref="T:System.Windows.Forms.ListBox.ObjectCollection" /> representing the items in the <see cref="T:System.Windows.Forms.ListBox" />.</returns>
        // Token: 0x1700096F RID: 2415
        // (get) Token: 0x06002751 RID: 10065 RVA: 0x000BA7A0 File Offset: 0x000B89A0
        [Category("CatData"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true), Description("ListBoxItemsDescr"), Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]

        public ListBox.ObjectCollection Items
        {
            get => (ItemsListBox?.Items)??null;
            set
            {
                Text = "";
                ItemsListBox.Items.Clear();
                foreach (string s in value)
                    ItemsListBox.Items.Add(s);
            }
        }

        ComboBoxStyle dropDownStyle = ComboBoxStyle.DropDown;
        /// <summary>
        /// 设置 ComboBox 显示效果类型
        /// </summary>
        public ComboBoxStyle DropDownStyle {
            get => dropDownStyle;
            set
            {
                dropDownStyle = value;

                switch (value)
                {
                    case ComboBoxStyle.DropDown:
                        {
                            InnerLabel.Hide();
                            InnerTextBox.Width = this.Width - 42;
                            InnerTextBox.Top=(this.Height- InnerTextBox.Height)/2;
                            InnerTextBox.Show();
                            CenterRectangle.Width = 60;
                            StaticBGImage = UnityResource.ComboBoxBGI;
                            break;
                        }
                    case ComboBoxStyle.DropDownList:
                        {
                            InnerTextBox.Hide();
                            InnerLabel.Width = this.Width - 42;
                            InnerTextBox.Top = (this.Height - InnerTextBox.Height) / 2;
                            InnerLabel.Show();
                            CenterRectangle.Width = 60;
                            StaticBGImage = UnityResource.ComboBoxBGI;
                            break;
                        }
                    case ComboBoxStyle.Simple:
                        {
                            InnerLabel.Hide();
                            InnerTextBox.Width = this.Width - 34;
                            InnerTextBox.Top = (this.Height - InnerTextBox.Height) / 2;
                            InnerTextBox.Show();
                            CenterRectangle.Width = 70;
                            StaticBGImage = UnityResource.DefaultButton_0;
                            break;
                        }
                }

                if (this.Size.Equals(BGImage?.Size)) return;

                this.BackgroundImage = null;
                BitmapProcessor.RenderBGI(StaticBGImage, this.Size, CenterRectangle, ref BGImage);
                this.BackgroundImage = BGImage;
            }
        }

        /// <summary>
        /// ComboBox 背景图资源
        /// </summary>
        static Bitmap StaticBGImage = UnityResource.ComboBoxBGI;
        
        /// <summary>
        /// 背景图像
        /// </summary>
        private Bitmap BGImage = null;

        //用于把属性显示在属性面板中，并在代码生成器中储存属性的值
        [Browsable(true)]
        [Description("ComBox 控件样式"), Category("自定义属性卡"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => InnerLabel.Text;
            set
            {
                InnerLabel.Text = value;
                InnerTextBox.Text = value;
            }
        }

        /// <summary>
        /// 文字水平方向
        /// </summary>
        public HorizontalAlignment TextAlign
        {
            get => InnerTextBox.TextAlign;
            set
            {
                InnerTextBox.TextAlign = value;
                if (value == HorizontalAlignment.Left) InnerLabel.TextAlign = ContentAlignment.MiddleLeft;
                else if (value == HorizontalAlignment.Center) InnerLabel.TextAlign = ContentAlignment.MiddleCenter;
                else if (value == HorizontalAlignment.Right) InnerLabel.TextAlign = ContentAlignment.MiddleRight;
            }
        }

        /// <summary>
        /// 设置控件字体
        /// </summary>
        public new Font Font
        {
            get => InnerTextBox.Font;
            set
            {
                InnerLabel.Font = value;
                InnerTextBox.Font=value;
                InnerLabel.Size = InnerTextBox.Size;

                this.MinimumSize = new Size(45, InnerTextBox.Height + 12);

                DropDownStyle = dropDownStyle;
            }
        }

        /// <summary>
        /// 设置控件字体颜色
        /// </summary>
        public new Color ForeColor
        {
            get => InnerTextBox.ForeColor;
            set
            {
                InnerLabel.ForeColor = value;
                InnerTextBox.ForeColor=value;
            }
        }

        public RoundedComboBox()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            this.OnResize(null);
            CreateDropDownHost();

            this.MinimumSize = new Size(45, this.Height);

            InnerLabel.Click += new EventHandler(ComboBox_Click);
            InnerTextBox.TextChanged += new EventHandler((s,e)=>InnerLabel.Text=InnerTextBox.Text);
        }

        private void ComboBox_Resize(object sender, EventArgs e)
        {
            DropDownStyle = dropDownStyle;
        }

        private void CreateDropDownHost()
        {
            toolStripDropDown = new ToolStripDropDown()
            {
                AutoClose = true,
                DropShadowEnabled = true,
                Opacity = 0.9,
                AllowTransparency = true,
                Padding = new Padding(1,0,1,0),
                ShowItemToolTips=false
            };

            ItemsListBox = new ListBox()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = InnerLabel.Font,
                ForeColor = InnerLabel.ForeColor,
                IntegralHeight = false,
            };

            ItemsListBox.SelectedIndexChanged += new EventHandler((s,e)=> 
            {
                this.Text = ItemsListBox.SelectedItem.ToString();
                toolStripDropDown.Hide();
            });

            toolStripControlHost = new ToolStripControlHost(ItemsListBox);
            toolStripDropDown.Items.Add(toolStripControlHost);
        }

        public void DropDownList()
        {
            DropDown?.Invoke(this, new EventArgs());

            ItemsListBox.MinimumSize = new Size(this.Width,0);
            ItemsListBox.MaximumSize = new Size(0, 200);

            Point locationOnClient = LocationOnClient(this);
            Point locationOnScreen= new Point(PointToScreen(locationOnClient).X, PointToScreen(locationOnClient).Y);
            locationOnScreen.Offset(-locationOnClient.X,-locationOnClient.Y);
            locationOnScreen.Offset(0, this.Height);

            toolStripDropDown.Show(locationOnScreen);
        }

        private Point LocationOnClient(Control c)
        {
            Point retval = new Point(0, 0);
            for (; c.Parent != null; c = c.Parent)
            { retval.Offset(c.Location); }
            return retval;
        }

        private void ComboBox_Click(object sender, EventArgs e)
        {
            if(dropDownStyle!= ComboBoxStyle.Simple)
                DropDownList();
        }

    }

}
