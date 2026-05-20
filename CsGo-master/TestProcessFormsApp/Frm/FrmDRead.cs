using CCWin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using TestProcessFormsApp.Communication;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmDRead : Skin_VS
    {
        private DataGridView dataGridView;
        private BindingList<DictionaryEntry> bindingList;
        private ConcurrentDictionary<ushort, ushort> dList;

        public FrmDRead(ConcurrentDictionary<ushort, ushort> keyValuePairs)
        {
            InitializeComponent();

            dList = new ConcurrentDictionary<ushort, ushort>();
            bindingList = new BindingList<DictionaryEntry>();
            foreach (var temp in CAEA008TCPModbus.Instance.DList)
            {
                if (CAEA008TCPModbus.Instance.DList.ContainsKey(temp.Key))
                {
                    // 更新或添加项
                    bindingList.Add(new DictionaryEntry(temp.Key, temp.Value));
                }
            }
            dataGridView1.DataSource = bindingList;
        }

     
        // 同步更新BindingList
        public void UpdateBindingList(ushort key, ushort value)
        {
            // 检查键是否存在
            if (CAEA008TCPModbus.Instance.DList.ContainsKey(key))
            {
                // 更新或添加项
                bindingList.Add(new DictionaryEntry(key, value));
            }
            //else
            //{
            //    // 移除项
            //    bindingList.Remove(new DictionaryEntry(key, dList[key]));
            //    dList.AddOrUpdate(key, value, (k, oldValue) => value);
            //    bindingList.Add(new DictionaryEntry(key, value));
            //}
        }
 
       
    }
}
