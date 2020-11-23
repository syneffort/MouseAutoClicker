using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseClicker
{
    public partial class AddPlanDialog : Form
    {
        private int _posX;
        private int _posY;

        public PPlan Plan { get; set; }

        public AddPlanDialog()
        {
            InitializeComponent();

            InitInstance();
        }

        private void InitInstance()
        {
            dtpDay.Value = dtpTime.Value = DateTime.Now;
            txtXPos.Text = txtYPos.Text = "0";
            txtInterval.Text = "1000";
        }

        private bool CheckIntegrity()
        {
            if (txtXPos.Text == String.Empty || !int.TryParse(txtXPos.Text, out _posX) ||
                txtYPos.Text == String.Empty || !int.TryParse(txtYPos.Text, out _posY))
                return false;

            return true;
        }

        private void btnRecorPosition_Click(object sender, EventArgs e)
        {
            btnRecorPosition.Text = "Ctrl+R";

            tmrMain.Enabled = true;
            tmrMain.Start();
        }

        private void tmrMain_Tick(object sender, EventArgs e)
        {
            txtXPos.Text = Cursor.Position.X.ToString();
            txtYPos.Text = Cursor.Position.Y.ToString();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.R))
            {
                btnRecorPosition.Text = "좌표지정";

                tmrMain.Stop();
                tmrMain.Enabled = false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckIntegrity())
            {
                MessageBox.Show("잘못된 데이터가 입력되었습니다.");
                return;
            }

            PPlan newPlan = new PPlan()
            {
                ReserveTime = new DateTime(dtpDay.Value.Year, dtpDay.Value.Month, dtpDay.Value.Day, dtpTime.Value.Hour, dtpTime.Value.Minute, dtpTime.Value.Second),
                PosX = _posX,
                PosY = _posY,
                Interval = int.Parse(txtInterval.Text),
                Count = int.Parse(txtCount.Text),
                IgnoreDay = chkRepeat.Checked,
                Message = txtMessage.Text
            };

            this.Plan = newPlan;
            this.DialogResult = DialogResult.OK;
        }
    }
}
