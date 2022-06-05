using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDiary
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Form1.set;
            pictureBox2.Image = Form1.avatar;
            labelWelcome.Text = "Hoşgeldin Şef.\nHadi bir kaç ayar yapalım!!";

            this.Location = new Point(Program.formMain.Location.X, Program.formMain.Location.Y + Program.formMain.Height + 1);
            Task.Run(() => efectGel());

            SetComponents();
        }
        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<string> colors = new List<string>();
            if (checkBoxB.Checked)
                colors.Add("blue");
            if (checkBoxG.Checked)
                colors.Add("green");
            if (checkBoxY.Checked)
                colors.Add("yellow");
            if (checkBoxP.Checked)
                colors.Add("pink");
            if (checkBoxS.Checked)
                colors.Add("surprise");

            if(Form1.renkler != colors)
            {
                Database.SetColors(colors);
                Form1.renkler = colors;
            }
        }
        private void button1_Click(object sender, EventArgs e)  //kaybol moko
        {
            labelWelcome.Text = "Bak Giderim,\nVe bir daha dönüp arkama bile bakmam...";
            labelWelcome.Refresh();
            Thread.Sleep(2222);
            labelWelcome.Text = "İyi o zaman sen bilirsin.\n Gidiyorum o zaman kendine iyi bak.";
            labelWelcome.Refresh();
            Thread.Sleep(2222);
            labelWelcome.Text = "Moko Gidiyor..";
            labelWelcome.Refresh();
            Thread.Sleep(1111);

            Task.Run(() =>
            {
                efectGit();
            }).Wait();

            this.Close();
        }
        void SetComponents()
        {
            List<string> colors = Database.GetColors();
            if (colors.Contains("blue"))
                checkBoxB.Checked = true;
            if (colors.Contains("green"))
                checkBoxG.Checked = true;
            if (colors.Contains("yellow"))
                checkBoxY.Checked = true;
            if (colors.Contains("pink"))
                checkBoxP.Checked = true;
            if (colors.Contains("surprise"))
                checkBoxS.Checked = true;
        }
        void efectGel()
        {
            foreach (Control item in panelMain.Controls)
            {
                item.Visible = item.Name != pictureBox2.Name ? false : true;
            }
            pictureBox2.Dock = DockStyle.Fill;

            for (int i = 0; i <= 400; i++)
            {
                this.Size = new Size(i, i);
                this.Location = new Point(this.Location.X, this.Location.Y - 1);
                Thread.Sleep(3);
            }

            pictureBox2.Dock = DockStyle.None;
            foreach (Control item in panelMain.Controls)
            {
                item.Visible = item.Name == pictureBox2.Name ? false : true;
            }
        }
        void efectGit()
        {

            foreach (Control item in panelMain.Controls)
            {
                item.Visible = item.Name != pictureBox2.Name ? false : true;
            }
            pictureBox2.Dock = DockStyle.Fill;

            for (int i = 0; i <= 400; i++)
            {
                this.Size = new Size(400 - i, 400 - i);
                this.Location = new Point(this.Location.X, this.Location.Y + 1);
                Thread.Sleep(3);
            }

            pictureBox2.Dock = DockStyle.None;
            foreach (Control item in panelMain.Controls)
            {
                item.Visible = item.Name == pictureBox2.Name ? false : true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Form1.imgS2;
            Task.Run(() =>
            {
                Thread.Sleep(7000);
                pictureBox1.Image = Form1.set;
            });
        }
    }
}
