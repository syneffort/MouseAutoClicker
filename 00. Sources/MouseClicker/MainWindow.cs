using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseClicker
{
    public partial class MainWindow : Form
    {
        private List<PPlan> _planList = new List<PPlan>();

        public MainWindow()
        {
            InitializeComponent();
            InitInstance();
        }

        private void InitInstance()
        {
            tmrMain.Start();
        }

        private void CheckAuth()
        {
            if (DateTime.Now.Year > 2020)
            {
                tmrMain.Stop();

                MessageBox.Show("사용 가능 기간이 초과되었습니다. 개발자에게 문의하세요");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.ShowIcon = false;

                icoNoti.Visible = true;
            }
        }

        private void RefreshBinding()
        {
            bndPlan.DataSource = null;
            bndPlan.DataSource = _planList;
        }

        private void CheckPlans()
        {
            foreach (PPlan plan in _planList)
            {
                if (plan.IsNeedAction(DateTime.Now))
                    plan.Act();
            }
        }

        private void icoNoti_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowIcon = true;

            icoNoti.Visible = false;

            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            this.TopMost = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddPlanDialog Dlg = new AddPlanDialog();
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                _planList.Add(Dlg.Plan);
                RefreshBinding();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bndPlan.Current == null)
                return;

            PPlan objItem = bndPlan.Current as PPlan;
            if (objItem == null)
                return;

            _planList.Remove(objItem);
            RefreshBinding();
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            CheckAuth();

            lblNow.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CheckPlans();
        }
    }
}
