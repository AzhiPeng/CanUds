using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestProcessFormsApp.Frm
{
    // 2026-05-20: 待补打标签管理窗口（离线缓存可视化 + 人工补打）
    public class FrmPendingPrintJobs : Form
    {
        private readonly DataGridView dgvJobs = new DataGridView();
        private readonly Label lbCount = new Label();
        private readonly Button btnRefresh = new Button();
        private readonly Button btnRetrySelected = new Button();
        private readonly Button btnRetryAll = new Button();
        private readonly TextBox tbKeyword = new TextBox();
        private readonly Button btnRetryKeyword = new Button();
        private readonly Button btnClose = new Button();

        public FrmPendingPrintJobs()
        {
            InitializeUi();
            Load += (s, e) => RefreshJobs();
        }

        private void InitializeUi()
        {
            Text = "待补打标签管理";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1180, 560);
            MinimizeBox = false;
            MaximizeBox = true;

            lbCount.AutoSize = true;
            lbCount.Location = new Point(12, 14);
            lbCount.Text = "待补打: 0";

            btnRefresh.Text = "刷新";
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Location = new Point(200, 8);
            btnRefresh.Click += (s, e) => RefreshJobs();

            btnRetrySelected.Text = "补打选中";
            btnRetrySelected.Size = new Size(96, 30);
            btnRetrySelected.Location = new Point(288, 8);
            btnRetrySelected.Click += (s, e) => RetrySelected();

            btnRetryAll.Text = "全部补打";
            btnRetryAll.Size = new Size(96, 30);
            btnRetryAll.Location = new Point(392, 8);
            btnRetryAll.Click += (s, e) => RetryAll();

            tbKeyword.Location = new Point(500, 13);
            tbKeyword.Size = new Size(260, 22);

            btnRetryKeyword.Text = "按关键字补打一条";
            btnRetryKeyword.Size = new Size(140, 30);
            btnRetryKeyword.Location = new Point(768, 8);
            btnRetryKeyword.Click += (s, e) => RetryByKeyword();

            btnClose.Text = "关闭";
            btnClose.Size = new Size(80, 30);
            btnClose.Location = new Point(916, 8);
            btnClose.Click += (s, e) => Close();

            dgvJobs.Location = new Point(12, 48);
            dgvJobs.Size = new Size(1156, 500);
            dgvJobs.ReadOnly = true;
            dgvJobs.AllowUserToAddRows = false;
            dgvJobs.AllowUserToDeleteRows = false;
            dgvJobs.AutoGenerateColumns = false;
            dgvJobs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvJobs.MultiSelect = true;
            dgvJobs.RowHeadersVisible = false;
            dgvJobs.DoubleClick += (s, e) => RetrySelected();

            AddColumns();

            Controls.Add(lbCount);
            Controls.Add(btnRefresh);
            Controls.Add(btnRetrySelected);
            Controls.Add(btnRetryAll);
            Controls.Add(tbKeyword);
            Controls.Add(btnRetryKeyword);
            Controls.Add(btnClose);
            Controls.Add(dgvJobs);
        }

        private void AddColumns()
        {
            dgvJobs.Columns.Clear();
            dgvJobs.Columns.Add(MakeTextCol("Id", "ID", 210, false));
            dgvJobs.Columns.Add(MakeTextCol("创建时间", "创建时间", 130));
            dgvJobs.Columns.Add(MakeTextCol("来源", "来源", 160));
            dgvJobs.Columns.Add(MakeTextCol("产品型号", "产品型号", 110));
            dgvJobs.Columns.Add(MakeTextCol("条码", "条码", 140));
            dgvJobs.Columns.Add(MakeTextCol("追溯码", "追溯码", 120));
            dgvJobs.Columns.Add(MakeTextCol("序列号", "序列号", 110));
            dgvJobs.Columns.Add(MakeTextCol("ECUID", "ECUID", 180));
            dgvJobs.Columns.Add(MakeTextCol("重试次数", "重试次数", 70));
            dgvJobs.Columns.Add(MakeTextCol("最后尝试时间", "最后尝试时间", 130));
            dgvJobs.Columns.Add(MakeTextCol("最后错误", "最后错误", 260));
        }

        private static DataGridViewTextBoxColumn MakeTextCol(string prop, string header, int width, bool visible = true)
        {
            return new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Width = width,
                Visible = visible,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private void RefreshJobs()
        {
            List<WinForm.PendingPrintJobView> data = WinForm.GetPendingPrintJobsSnapshot();
            dgvJobs.DataSource = data;
            lbCount.Text = $"待补打: {data.Count}";
        }

        private void RetryAll()
        {
            string msg = WinForm.ManualRetryPendingPrintJobs();
            MessageBox.Show(msg, "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshJobs();
        }

        private void RetryByKeyword()
        {
            string keyword = tbKeyword.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("请输入关键字（条码/追溯码/序列号/ECUID）。", "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string msg = WinForm.ManualRetrySinglePendingPrintJob(keyword);
            MessageBox.Show(msg, "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshJobs();
        }

        private void RetrySelected()
        {
            if (dgvJobs.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中至少一条待补打记录。", "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var ids = new List<string>();
            foreach (DataGridViewRow row in dgvJobs.SelectedRows)
            {
                if (row.DataBoundItem is WinForm.PendingPrintJobView view && !string.IsNullOrWhiteSpace(view.Id))
                    ids.Add(view.Id);
            }
            ids = ids.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (ids.Count == 0)
            {
                MessageBox.Show("未找到有效任务ID。", "标签补打", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int success = 0;
            int failed = 0;
            var failMsgs = new List<string>();
            foreach (string id in ids)
            {
                string msg = WinForm.ManualRetryPendingPrintJobById(id);
                if (msg.Contains("补打成功"))
                {
                    success++;
                }
                else
                {
                    failed++;
                    failMsgs.Add(msg);
                }
            }

            string summary = $"补打选中完成：成功 {success} 条，失败 {failed} 条，剩余待补打 {WinForm.GetPendingPrintCount()} 条。";
            if (failMsgs.Count > 0)
            {
                summary += Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, failMsgs.Take(3));
                if (failMsgs.Count > 3)
                    summary += Environment.NewLine + $"...其余失败 {failMsgs.Count - 3} 条请查看列表“最后错误”。";
            }

            MessageBox.Show(summary, "标签补打", MessageBoxButtons.OK, failed == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            RefreshJobs();
        }
    }
}
