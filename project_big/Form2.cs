using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_big
{
    public partial class Form2 : Form
    {
        private int hours;
        private int mins;
        private int seconds;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numMin.Value < 1 && numHour.Value < 1)
            {
                MessageBox.Show("Bạn chưa chọn thời gian.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                hours = (int)numHour.Value;
                mins = (int)numMin.Value;

                timer2.Start();
                timer1.Enabled = true;
                btnStart.Enabled = false;
                btnCancel.Enabled = true;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            cbbMethod.SelectedIndex = 0;           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimeNow.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
        private string formatHour(int s)
        {
            string t = s.ToString();
            return s < 10 ? "0" + t : t;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (hours > 0 | mins > 0 | seconds > 0)
            {
                if (mins == 0 && hours > 0) { mins = 60; hours = hours - 1; }
                if (seconds == 0 && mins > 0) { seconds = 60; mins = mins - 1; }
                seconds = seconds - 1;
            }
            numHour.Value = hours;
            numMin.Value = mins;
            lblTimeLeft.Text = string.Format("{0}:{1}:{2}", formatHour(hours), formatHour(mins), formatHour(seconds));

            if (cbAlert.Checked)
            {
                string msg = string.Format("Máy tính của bạn sẽ {0} sau {1} phút nữa. Vui lòng lưu các tài liệu đang làm dở, và đóng hết các chương trình lại.", cbbMethod.Text, mins);
                if (mins == numTimeAlert.Value && seconds == 0) MessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (hours == 0 && mins == 0 && seconds == 0)
            {
                timer2.Stop();
                int select = cbbMethod.SelectedIndex;
                switch (select)
                {
                    default:
                    case 0:
                        ExitWindows.LogOff(true);
                        break;
                    case 1:
                        ExitWindows.Reboot(true);
                        break;
                    case 2:
                        ExitWindows.Shutdown(true);
                        break;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            btnStart.Enabled = true;
            btnCancel.Enabled = false;
            lblTimeLeft.Text = "00:00:00";
        }

        private void cbAlert_CheckedChanged(object sender, EventArgs e)
        {
            numTimeAlert.Visible = cbAlert.Checked;
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Menu Time:\n\nMenu Time là một ứng dụng nhỏ dùng để hẹn giờ tắt hoặc khởi động lại máy tính.","About",
                MessageBoxButtons.OK,MessageBoxIcon.Information);            
        }
    }
}
