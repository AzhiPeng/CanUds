using CCWin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace HMIPLC
{
    internal partial class ParmsInputDialogue : Skin_VS
    {
        internal static GridItem selectedItemGlobal;
        internal string SelectedString;
        internal bool changed = false;
        Array itmmem;

        int GWL_WNDPROC = -4;
        int WM_KEYUP = 0x101;
        int VK_RETURN = 0xD;
        int VK_ESCAPE = 0x1B;
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, WindowProcDelegate dwNewLong);
        [DllImport("user32.dll")]
        static extern bool EnumChildWindows(IntPtr hWndParent, ChildWindowDelegate lpEnumFunc, string wndCaption);
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(int lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        delegate bool ChildWindowDelegate(IntPtr hwndChild, string wndCaption);
        delegate IntPtr WindowProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        private bool EnumChildProc(IntPtr hwndChild, string wndCaption)
        {
            StringBuilder windowName = new StringBuilder(255);
            GetWindowText(hwndChild, windowName, 255);
            if (windowName.ToString().Equals(wndCaption))
            {
                expectedChild = hwndChild;
                return false;
            }
            else
            {
                return true;
            }
        }
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_KEYUP)
            {
                if (wParam.ToInt32() == VK_RETURN)
                {
                    Console.WriteLine("enter key is pressed");
                }
                else if (wParam.ToInt32() == VK_ESCAPE)
                {
                    Console.WriteLine("escape key is pressed");
                }
            }
            // let the default procedure handle the Windows messages
            IntPtr result = CallWindowProc(defaultProc, expectedChild, msg, wParam, lParam);
            return result;
        }
        int defaultProc;
        IntPtr expectedChild;

        internal ParmsInputDialogue(string title, object obj)
        {
            InitializeComponent();
            //this.Icon = Tofine.CamExplorer.Properties.Resources.camera;
            this.Text = title;
          //  splitContainer1.Panel2Collapsed = true;
            //propertyGrid1.SelectedObject = obj.Clone();
            //if (obj is PlcInterface[])
            //{
            //    itmmem = obj as Array;
            //    comboBox1.Visible = true;
            //    for (int i = 0; i < itmmem.Length; i++)
            //    {
            //        comboBox1.Items.Add(i.ToString());
            //    }
            //    if (comboBox1.Items.Count > 0)
            //        comboBox1.SelectedIndex = 0;
            //}
            //else
            {
                propertyGrid1.SelectedObject = obj;
                //comboBox1.Visible = false;
            }

            //bool result = EnumChildWindows(this.propertyGrid1.Handle, new ChildWindowDelegate(EnumChildProc), "PropertyGridView");
            //if (result == false)
            //{
            //    defaultProc = GetWindowLong(expectedChild, GWL_WNDPROC);
            //    SetWindowLong(expectedChild, GWL_WNDPROC, new WindowProcDelegate(WindowProc));
            //}
        }

        private void PropertyGrid1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        internal object SelectedObject
        {
            get { return propertyGrid1.SelectedObject; }
        }

        private void ParmsInputDialogue_Load(object sender, EventArgs e)
        {
        }
        private void ParmsInputDialogue_Shown(object sender, EventArgs e)
        {
            MoveSplitterTo(propertyGrid1);
            Application.DoEvents();
            Thread.Sleep(1);
            this.ShowIcon = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Type tp = propertyGrid1.SelectedObject.GetType();
                propertyGrid1.SelectedObject = Activator.CreateInstance(tp);
            }
            catch (System.Exception ex)
            {
              //  Program.ErrHdl(ex);
            }
        }

        internal static string GetLabel(GridItem gi)
        {
            if (gi == null)
                return null;
            Stack<GridItem> stk = new Stack<GridItem>();
            while (gi.Parent != null)
            {
                stk.Push(gi);
                gi = gi.Parent;
            }
            StringBuilder sb = new StringBuilder();
            while (stk.Count > 0)
            {
                gi = stk.Pop();
                sb.Append(gi.Label).Append('_');
            }
            if (sb.Length > 0)
                return sb.ToString(0, sb.Length - 1);
            else
                return null;
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (SelectedString != null)
                if (e.ChangedItem.Value == null)
                {
                  //  if (e.OldValue != null)
                    //    Program.Loginfo("{3}-{0}：{1} -> {2}".FormatWith(GetLabel(e.ChangedItem), e.OldValue, e.ChangedItem.Value, SelectedString));
                }
             //   else if (e.ChangedItem.Value.Equals(e.OldValue) == false)
                 //   Program.Loginfo("{3}-{0}：{1} -> {2}".FormatWith(GetLabel(e.ChangedItem), e.OldValue, e.ChangedItem.Value, SelectedString));
            changed = true;
            propertyGrid1.Refresh();
        }

        internal static void MoveSplitterTo(System.Windows.Forms.PropertyGrid grid, int add = 50)
        {
            Type tp = typeof(PropertyGrid);
            FieldInfo fi = tp.GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic);
            object ob = fi.GetValue(grid);
            tp = ob.GetType();
            MethodInfo mi = tp.GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo f2 = tp.GetField("allGridEntries", BindingFlags.Instance | BindingFlags.NonPublic);
            GridItemCollection grids = f2.GetValue(ob) as GridItemCollection;

            if (grids == null)
                return;
            float len = 5;
            System.Drawing.Graphics graphics = grid.CreateGraphics();
            foreach (GridItem item in grids)
            {
                SizeF sizeF = graphics.MeasureString(item.Label, grid.Font);

                //if (item.Label.Length > len)
                //len = item.Label.Length;
                if (sizeF.Width > len)
                    len = sizeF.Width;
            }
            if (len > grid.Width / 2)
                len = grid.Width / 2;
            mi.Invoke(ob, new object[] { (int)(len) + 50 });
            //mi.Invoke(ob, new object[] { (int)(len * propertyGrid1.Font.Size * rate) });
        }

        DateTime SelectedGridItemChangedTime = DateTime.MinValue;
        GridItem selectedItem;
        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if (e.NewSelection.Label != "状态")
                SelectedGridItemChangedTime = DateTime.Now;
            selectedItem = e.NewSelection;
            selectedItemGlobal = selectedItem;
            //if (selectedItem != null && (e.NewSelection.Value is Cognex.VisionPro.Block1Settings == false))
            //    selectedItem = null;

            if (selectedItem != null)
                propertyGrid1.ContextMenuStrip = contextMenuStrip1;
            else
                propertyGrid1.ContextMenuStrip = null;

            if (e.NewSelection.PropertyDescriptor != null)
            {
                if (string.IsNullOrEmpty(e.NewSelection.PropertyDescriptor.Description))
                    propertyGrid1.HelpVisible = false;
                else
                    propertyGrid1.HelpVisible = true;
            }

            if (refreshFlag)
            {
                refreshFlag = false;
                propertyGrid1.Refresh();
            }
        }

        private void ParmsInputDialogue_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form1.HideKeyBoard();
            //if (changed && DialogResult != DialogResult.OK)
            //    MessageBox.Show("参数已改变但未保存，重启程序可还原最后一次保存时的参数");
        }
        internal void RefreshArrayHdl(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
            {
                if (propertyGrid1.SelectedObject == sender)
                    if (this.InvokeRequired)
                        this.BeginInvoke(new Action(propertyGrid1.Refresh));
                    else
                        propertyGrid1.Refresh();
            }
        }
        internal void RefreshHdl(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized && (DateTime.Now - SelectedGridItemChangedTime).TotalSeconds > 3)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(new Action(propertyGrid1.Refresh));
                else
                    propertyGrid1.Refresh();
            }
            else
                refreshFlag = true;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void moveupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void movedownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
            }
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public bool refreshFlag = false;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var it = comboBox1.SelectedIndex;
            //if (it >= 0 && itmmem != null && itmmem.Length > it)
            //    propertyGrid1.SelectedObject = itmmem.GetValue(it);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
                SelectedGridItemChangedTime = DateTime.MinValue;
            else
                SelectedGridItemChangedTime = DateTime.Now;
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
    [AttributeUsage(AttributeTargets.All)]
    public class TooltipAttribute : Attribute
    {
        string desc;
        public TooltipAttribute(string description)
        {
            desc = description;
        }

        public string TooltipText { get { return desc; } }
    }
}
