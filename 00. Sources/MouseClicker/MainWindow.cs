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
        private PPlanManager _planManager;

        public MainWindow()
        {
            InitializeComponent();
            InitInstance();
        }

        private void InitInstance()
        {
            _planManager = PPlanManager.Instance;
            if (_planManager.LoadPlans())
                RefreshBinding();

            tmrMain.Start();
        }

        private void CheckAuth()
        {
            //if (DateTime.Now.Year > 2020)
            //{
            //    tmrMain.Stop();

            //    MessageBox.Show("사용 가능 기간이 초과되었습니다. 개발자에게 문의하세요");
            //    Process.GetCurrentProcess().Kill();
            //}
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
            bndPlan.DataSource = _planManager.Plans;
        }

        private void CheckPlans()
        {
            foreach (PPlan plan in _planManager.Plans)
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
            PlanDialog Dlg = new PlanDialog();
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                _planManager.Plans.Add(Dlg.Plan);
                RefreshBinding();
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (bndPlan.Current == null)
                return;

            PPlan plan = bndPlan.Current as PPlan;
            if (plan == null)
                return;

            PPlan originPlan = _planManager.Plans.Find(x => x.ReserveTime == plan.ReserveTime && x.PosX == plan.PosX && x.PosY == plan.PosY);
            if (originPlan == null)
                return;

            int index = _planManager.Plans.IndexOf(originPlan);
            PlanDialog Dlg = new PlanDialog(plan);
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                _planManager.Plans[index] = Dlg.Plan;
                RefreshBinding();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bndPlan.Current == null)
                return;

            PPlan plan = bndPlan.Current as PPlan;
            if (plan == null)
                return;

            _planManager.Plans.Remove(plan);
            RefreshBinding();
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            CheckAuth();

            lblNow.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CheckPlans();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _planManager.SavePlans();
        }
    }
}
