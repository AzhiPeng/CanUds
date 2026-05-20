using CCWin;
using JLRScan;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using TestProcessFormsApp.Communication.CAN;
using TestProcessFormsApp.Public;
using Message = TestProcessFormsApp.Communication.CAN.Message;

namespace TestProcessFormsApp.Frm
{
    public partial class FrmCanNode : Skin_VS
    {
        public BindingList<CANUDPClient> Clients { get; set; } = new BindingList<CANUDPClient>();
        public FrmCanNode()
        {
            InitializeComponent();
            InitializeControls();
            SetupEventHandlers();
        }
        private void InitializeControls()
        {
            // 配置TreeView
            treeView.Dock = DockStyle.Fill;
            treeView.LabelEdit = true;  // 允许节点文本编辑
            treeView.AllowDrop = true;  // 启用拖放功能
            treeView.ImageList = new ImageList();
            treeView.ImageList.Images.Add("client", SystemIcons.Shield);
            treeView.ImageList.Images.Add("message", SystemIcons.Question);
            treeView.ImageList.Images.Add("signal", SystemIcons.Information); // 使用信息图标，可替换为其他图标
            // 右键菜单
            var ctxMenu = new ContextMenuStrip();
            ctxMenu.Opening += (s, e) =>
            {
                ctxMenu.Items.Clear();
                var node = treeView.SelectedNode;

                if (node?.Tag is CANUDPClient)
                {
                    ctxMenu.Items.Add("添加消息", null, (a, b) => AddMessageToClient(node.Tag as CANUDPClient));
                    ctxMenu.Items.Add("删除客户端", null, (a, b) => DeleteClient(node.Tag as CANUDPClient));
                    ctxMenu.Items.Add("新建客户端", null, (a, b) => AddNewClient());
                }
                else if (node?.Tag is Message)
                {
                    ctxMenu.Items.Add("添加 Signal", null, (a, b) => AddSignalToMessage(node.Tag as Message));
                    ctxMenu.Items.Add("删除消息", null, (a, b) => DeleteMessage(node.Tag as Message));
                }
                else if (node?.Tag is Signal)
                {
                    ctxMenu.Items.Add("复制", null, (a, b) => CopySignal(node.Tag as Signal));
                    ctxMenu.Items.Add("删除 Signal", null, (a, b) => DeleteSignal(node.Tag as Signal));
                }
                else
                {
                    ctxMenu.Items.Add("新建客户端", null, (a, b) => AddNewClient());
                }
            };

            treeView.ContextMenuStrip = ctxMenu;
            // 配置PropertyGrid
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.ToolbarVisible = false;  // 隐藏工具栏
            propertyGrid.HelpVisible = false;     // 隐藏帮助区域
            propertyGrid.PropertySort = PropertySort.Categorized;

       
        }
        private void SetupEventHandlers()
        {
            // 数据绑定
            Clients.ListChanged += (s, e) => RefreshTree();

            // 节点选择事件
            treeView.AfterSelect += (s, e) =>
            {
                propertyGrid.SelectedObject = e.Node?.Tag;
            };
        }
        #region 数据操作
        private void AddNewClient()
        {
            var client = new CANUDPClient
            {
                Name = $"Client_{Clients.Count + 1}"
            };
            Clients.Add(client);
        }

        private void AddMessageToClient(CANUDPClient client)
        {
            var msg = new Message
            {
                Id = $"0x{client.Messages.Count + 1:X8}",
                Name = $"0x{client.Messages.Count :X4}"
            };
            client.Messages.Add(msg);
        }

        private void DeleteClient(CANUDPClient client)
        {
            Clients.Remove(client);
        }

        private void DeleteMessage(Message msg)
        {
            foreach (var client in Clients)
            {
                if (client.Messages.Contains(msg))
                {
                    client.Messages.Remove(msg);
                    break;
                }
            }
        }
        private void AddSignalToMessage(Message message)
        {
            var signal = new Signal
            {
                Name = $"Signal_{message.Signals.Count + 1}",
                StartBit = 0,
                Length = 8,
                // 其他属性的默认值
            };
            message.Signals.Add(signal);
        }

        private void DeleteSignal(Signal signal)
        {
            foreach (var client in Clients)
            {
                foreach (var msg in client.Messages)
                {
                    if (msg.Signals.Contains(signal))
                    {
                        msg.Signals.Remove(signal);
                        return;
                    }
                }
            }
        }
        private TreeNode CreateSignalNode(Signal signal)
        {
            var signalNode = new TreeNode(signal.Name)
            {
                Tag = signal,
                ImageKey = "signal",
                SelectedImageKey = "signal"
            };
            signal.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Signal.Name))
                {
                    signalNode.Text = signal.Name;
                }
            };
            return signalNode;
        }
        private void CopySignal(Signal originalSignal)
        {
            // 获取原 Signal 所在的 Message（通过父节点）
            var messageNode = treeView.SelectedNode?.Parent;

            if (messageNode?.Tag is Message parentMessage)
            {
                // 创建新 Signal 并复制属性
                var newSignal = new Signal
                {
                    Name = $"{originalSignal.Name}_副本", // 自动添加后缀避免重复
                    StartBit = originalSignal.StartBit,
                    Length = originalSignal.Length,
                    IsMotorolaByteOrder = originalSignal.IsMotorolaByteOrder,
                    Factor = originalSignal.Factor,
                    Offset = originalSignal.Offset,
                    Unit = originalSignal.Unit,
                    Minimum = originalSignal.Minimum,
                    Maximum = originalSignal.Maximum,
                    IsSigned = originalSignal.IsSigned
                };

                // 将新 Signal 添加到当前 Message
                parentMessage.Signals.Add(newSignal);
            }else
            {
                MessageBox.Show("无法找到父消息节点！");
                return;
            }
        }
        #endregion
        #region TreeView刷新
        private void RefreshTree()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            foreach (var client in Clients)
            {
                var clientNode = new TreeNode(client.Name)
                {
                    Tag = client,
                    ImageKey = "client",
                    SelectedImageKey = "client"
                };

                // 绑定消息集合变化
                client.Messages.CollectionChanged += (s, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (Message msg in e.NewItems)
                        {
                            clientNode.Nodes.Add(CreateMessageNode(msg));
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (Message msg in e.OldItems)
                        {
                            var nodes = clientNode.Nodes
                                .Cast<TreeNode>()
                                .Where(n => n.Tag == msg)
                                .ToArray();

                            foreach (var n in nodes)
                                clientNode.Nodes.Remove(n);
                        }
                    }
                };

                // 初始化现有消息
                foreach (var msg in client.Messages)
                    clientNode.Nodes.Add(CreateMessageNode(msg));

                treeView.Nodes.Add(clientNode);
            }
            treeView.EndUpdate();
        }
     
        private TreeNode CreateMessageNode(Message msg)
        {
            var node = new TreeNode($"{msg.Id} ({msg.Name})")
            {
                Tag = msg,
                ImageKey = "message",
                SelectedImageKey = "message"
            };
            // 初始化现有 Signal 节点
            foreach (var signal in msg.Signals)
            {
                node.Nodes.Add(CreateSignalNode(signal));
            }
            // 监听 Signals 集合变化
            msg.Signals.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (Signal signal in e.NewItems)
                    {
                        node.Nodes.Add(CreateSignalNode(signal));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (Signal signal in e.OldItems)
                    {
                        var nodesToRemove = node.Nodes.Cast<TreeNode>()
                            .Where(n => n.Tag == signal).ToList();
                        foreach (var n in nodesToRemove)
                        {
                            node.Nodes.Remove(n);
                        }
                    }
                }
                // 其他动作如 Reset 可根据需要处理
            };
            // 绑定属性变化
            msg.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Message.Id) ||
                    e.PropertyName == nameof(Message.Name))
                {
                    node.Text = $"{msg.Id} ({msg.Name})";
                }
            };

            return node;
        }
        #endregion

        private void btn_Save_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                //sfd.Filter = "JSON配置|*.json";
             //   if (sfd.ShowDialog() == DialogResult.OK)
                try
                {
                    var settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    string json = JsonConvert.SerializeObject(Clients, settings);
                   // File.WriteAllText(sfd.FileName, json);
                    File.WriteAllText($"{LocalSetting.SystemDIR}\\产品报文配置\\" + $"\\0Y4G40AAEWWACanNode.json", json, Encoding.UTF8);
                    MessageBox.Show("配置保存成功！");
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void FrmCanNode_Load(object sender, EventArgs e)
        {
            try
            {
                string json = File.ReadAllText($"{LocalSetting.SystemDIR}\\产品报文配置\\"  + $"\\0Y4G40AAEWWACanNode.json");
                var loadedClients = JsonConvert.DeserializeObject<BindingList<CANUDPClient>>(json);

                Clients.Clear();
                foreach (var client in loadedClients)
                    Clients.Add(client);

                MessageBox.Show("配置加载成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败: {ex.Message}");
            } 
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}