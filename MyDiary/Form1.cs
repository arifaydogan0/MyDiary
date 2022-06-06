using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDiary
{
    public partial class Form1 : Form
    {
        static int ilkKosegen = 2200;
        Label activeLabel;
        static Random rnd = new Random();
        static Image add = new Bitmap(Application.StartupPath + @"\images\add.png");
        static Image str = new Bitmap(Application.StartupPath + @"\images\star.png");
        public static Image set = new Bitmap(Application.StartupPath + @"\images\settings.png");
        public static Image avatar = new Bitmap(Application.StartupPath + @"\images\avatar.gif");
        public static Image imgY = new Bitmap(Application.StartupPath + @"\images\imgY.png");
        public static Image imgG = new Bitmap(Application.StartupPath + @"\images\imgG.png");
        public static Image imgB = new Bitmap(Application.StartupPath + @"\images\imgB.png");
        public static Image imgP = new Bitmap(Application.StartupPath + @"\images\imgP.png");
        public static Image imgS1 = new Bitmap(Application.StartupPath + @"\images\imgS1.gif");
        public static Image imgS2 = new Bitmap(Application.StartupPath + @"\images\imgS2.gif");
        public static List<string> renkler = Database.GetColors();
        static List<Mission>[] cache = Database.GetAll(DateTime.Now.AddDays(-1)); //anlık tutulan yedi günlük görev listesi
        static int cacheChangeNumb = 0;
        static readonly Screen[] screens = Screen.AllScreens;
        static List<Mission>[] bellek
        {
            get { return cache; }
            set
            {
                cache = value;
                cacheChangeNumb++;
            }
        }
        static List<Image> imgs;
        Image img
        {
            get
            {
                imgs = new List<Image>();
                for (int i = 0; i < renkler.Count; i++)
                {
                    switch (renkler[i])
                    {
                        case "blue": if (!imgs.Contains(imgB)) { imgs.Add(imgB); } break;
                        case "green": if (!imgs.Contains(imgG)) { imgs.Add(imgG); } break;
                        case "yellow": if (!imgs.Contains(imgY)) { imgs.Add(imgY); } break;
                        case "pink": if (!imgs.Contains(imgP)) { imgs.Add(imgP); } break;
                        case "surprise": if (!imgs.Contains(avatar)) { imgs.Add(imgS1); imgs.Add(imgS2); } break;

                        default: break;
                    }
                }
                if (imgs.Count == 0)
                    return null;
                int random = rnd.Next(rnd.Next(rnd.Next())) % imgs.Count;
                return imgs[random]; // -> img1-img2-img3-img4 random seçim
            }
        }


        public void AddVersionNumber()
        {

        }
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateCells();
            SetComboBox();

            textBox.Font = new Font(label1.Font, FontStyle.Regular);
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }   //Form kapandıktan sonra
        private void button1_Click(object sender, EventArgs e)
        {
            Database.SetAll(DateTime.Now.AddDays(-1), bellek);
            Application.Exit();
        }  //x buttonu
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Database.SetAll(DateTime.Now.AddDays(-1), bellek);
        }  //form kapatılırken kaydet.
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cacheChangeNumb > 17)
            {
                Task.Factory.StartNew(() => Database.SetAll(DateTime.Now.AddDays(-1), bellek));
                cacheChangeNumb = 0;
            }
            else if (DateTime.Now.AddSeconds(6).Hour == 23 && DateTime.Now.AddSeconds(6).Minute == 59 && DateTime.Now.AddSeconds(6).Second > 55)
            {
                Thread.Sleep(1111);
                Task.Factory.StartNew(() =>
                {
                    Database.SetAll(DateTime.Now.AddDays(-1), bellek);
                    cache = Database.GetAll(DateTime.Now.AddDays(-1));
                    cacheChangeNumb = 0;
                    UpdateCells();
                    SetComboBox();
                });
            }
        } //tam saatlere yakın yakalarsa kaydet.
        private void label_DoubleClick(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            Control con = label.Parent;
            if (con.BackgroundImage == null)
                return;
            activeLabel = label;
            Control c = label.Parent;
            label.Visible = false;
            textBox.Visible = true;
            textBox.Text = label.Text;
            textBox.Location = label.Location;
            textBox.Multiline = true;
            textBox.Dock = DockStyle.Fill;
            textBox.BackColor = Color.White;
            c.Controls.Add(textBox);
            textBox.Focus();
        }   //Labellerden birine çift tık
        private void label_Click(object sender, EventArgs e)
        {
            if (textBox.Visible)
                textBox_KeyDown(sender, new KeyEventArgs(Keys.Enter));
        }  //Bir labele tek click yapılınca.
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter || e.KeyData == Keys.Escape)
            {
                string input = textBox.Text;
                string ilktext = activeLabel.Text;

                int row = tableLayoutPanel1.GetPositionFromControl(activeLabel.Parent).Row;
                int column = tableLayoutPanel1.GetPositionFromControl(activeLabel.Parent).Column;

                if (input.Length == 0 && ilktext.Length == 0)
                {
                    textBox.Visible = false;
                    activeLabel.Visible = true;
                    return;
                }  //Add ile yazmaya gelindi ama bişe yazılmadan çıkıldı.
                else if (input.Length != 0 && ilktext.Length == 0)
                {
                    activeLabel.Text = input;
                    activeLabel.Parent.BackgroundImage = img;
                    activeLabel.Visible = true;
                    textBox.Clear();
                    textBox.Visible = false;
                    Mission m = new Mission() { mission = input, isComplete = false, missionDate = DateTime.Now.AddDays(column - 1) };
                    bellek[column].Add(m);
                    if (bellek[column].Count != 4) { Control con = tableLayoutPanel1.GetControlFromPosition(column, row + 1); con.BackgroundImage = add; }
                    return;
                }  //Add ile gelindi, boş kutuya bişiler eklendi.
                else if (input.Length == 0 && ilktext.Length != 0)
                {
                    activeLabel.Text = input;
                    textBox.Clear();
                    bellek[column].Remove(bellek[column][row - 1]);
                    for (int i = 0; i < 4; i++)
                    {
                        Control ccc = tableLayoutPanel1.GetControlFromPosition(column, i + 1);
                        foreach (Control item in ccc.Controls)
                        {
                            item.Text = "";
                            //item.Visible = false;
                        }
                    }
                    for (int i = 0; i < bellek[column].Count; i++)
                    {
                        Control conn = tableLayoutPanel1.GetControlFromPosition(column, i + 1);
                        foreach (Control item in conn.Controls)
                        {
                            item.Text = bellek[column][i].mission;
                            item.Visible = true;
                        }
                        conn.BackgroundImage = img;
                    }
                    Control con = tableLayoutPanel1.GetControlFromPosition(column, bellek[column].Count + 1);
                    con.BackgroundImage = add;
                    foreach (Control item in con.Controls)
                        item.Visible = true;

                    if (bellek[column].Count + 1 < 4)
                    {
                        Control com = tableLayoutPanel1.GetControlFromPosition(column, bellek[column].Count + 2);
                        com.BackgroundImage = null;
                    }
                    textBox.Visible = false;
                    activeLabel.Visible = true;
                    return;
                }  //Doluya çift tıklandı, yazı silinip çıkıldı.
                else if (input.Length != 0 && ilktext.Length != 0)
                {
                    activeLabel.Text = input;
                    textBox.Visible = false;
                    activeLabel.Visible = true;
                    textBox.Clear();
                    List<Mission> missions = new List<Mission>();
                    for (int i = 0; i < bellek[column].Count; i++)
                    {
                        if (bellek[column][i].mission == ilktext)
                        {
                            missions.Add(new Mission() { mission = input, isComplete = false, missionDate = bellek[column][i].missionDate });
                        }
                        else
                        {
                            missions.Add(bellek[column][i]);
                        }
                    }
                    return;
                }  //Dolu kutuya gelindi, dolu şekilde çıkıldı(no change veya update)
            }
        }   //Yazma modu açıkken enter basıldı
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indeks = comboBox1.SelectedIndex;
            SetCompanents(SetAppLocationAndSize(screens[indeks]));
            textBox.Font = new Font(textBox.Font.FontFamily, label1.Font.Size);
            int x = (int)(Math.Sqrt((labelDate2.Size.Width * labelDate2.Size.Width) + (labelDate2.Size.Height * labelDate2.Size.Height)) / 6 / Math.Sqrt(2));
            pictureBoxToday.Size = new Size(x, x);

        } //Screen seçimi, farklı monitöre formu atmak için seçim yapılcak.
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            float seviye = (float)hScrollBar1.Value;
            labelSeviye.Text = "%" + seviye.ToString();
            this.Opacity = ((float)seviye) / 100;

        }  //formun opacity ayarı
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.BackColor = Color.Gray;
            pictureBox1.Update();
            Thread.Sleep(50);
            pictureBox1.BorderStyle = BorderStyle.None;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Update();

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    hScrollBar1.Value -= 1;
                    Thread.Sleep(50);
                }
            });
            Program.formSett = new FormSettings();
            Program.formSett.ShowDialog();
            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    hScrollBar1.Value += 1;
                    Thread.Sleep(5);
                }
            });
        }  //Ayarlar butonu


        void UpdateCells()
        {
            pictureBoxToday.Image = str;
            pictureBox1.Image = set;

            /* Tarihlerin güncellenmesi. */
            for (int i = 0; i < 7; i++)
            {
                foreach (Control c in tableLayoutPanel1.GetControlFromPosition(i, 0).Controls)
                    c.Text = c.Name.Contains("Today") ? c.Text : DateTime.Now.AddDays(i - 1).GetDateTimeFormats()[1] + "\n" + DateTime.Now.AddDays(i - 1).DayOfWeek;
            }

            /* Mission tabloları bellekten alınan veriyle güncelleniyor. */
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    foreach (Control c in tableLayoutPanel1.GetControlFromPosition(i, j + 1).Controls)
                    {
                        c.Text = bellek[i].Count > j ? bellek[i][j].mission : "";
                        Panel p = (Panel)c.Parent;
                        p.BackgroundImage = c.Text.Length > 0 ? img : null;
                    }
                }
                if (bellek[i].Count < 4)
                {
                    int misNumb = bellek[i].Count;
                    foreach (Control c in tableLayoutPanel1.GetControlFromPosition(i, bellek[i].Count + 1).Controls)
                    {
                        Panel p = (Panel)c.Parent;
                        p.BackgroundImage = add;
                    }
                }
            }

        }  //Bellek değişkeni dataları alıyor, bellekten tabloya yükleme burada yapılıyor.
        int SetAppLocationAndSize(Screen screen)
        {
            Size appSize = new Size(screen.Bounds.Size.Width * 3 / 4, screen.Bounds.Height * 3 / 4);
            Point middlePoint = new Point(screen.Bounds.Location.X + screen.Bounds.Width / 2, screen.Bounds.Location.Y + screen.Bounds.Height / 2);
            Rectangle appRec = new Rectangle(new Point(middlePoint.X - appSize.Width / 2, middlePoint.Y - appSize.Height / 2), appSize);
            this.Location = appRec.Location;
            this.Size = appRec.Size;
            double x = this.Size.Width;
            double y = this.Size.Height;
            int kosegen = (int)Math.Sqrt((x * x) + (y * y));
            return kosegen;
        }  //Ekrana göre Size vs Boyut ayarları ve ekranı yerleştirme işlemi, yerlestırılen formun kösegenını döner.
        void SetCompanents(int yeniKosegen)
        {
            if (yeniKosegen == ilkKosegen)
                return;

            float change = (float)yeniKosegen / (float)ilkKosegen;

            for (int i = 0; i < 7; i++)
            {
                foreach (Control c in tableLayoutPanel1.GetControlFromPosition(i, 0).Controls)
                    c.Font = new Font(new Font(c.Font.FontFamily, (float)(c.Font.Size * change)), FontStyle.Bold);
            }//tablo üst kısım yeniden boyutlandırma
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    foreach (Control c in tableLayoutPanel1.GetControlFromPosition(i, j + 1).Controls)
                    {
                        c.Font = new Font(new Font(c.Font.FontFamily, (float)(c.Font.Size * change)), FontStyle.Bold);
                        //c.Size = c.Name.Contains("label") ? new Size(c.Parent.Size.Width - 7, c.Parent.Size.Height - 7) : c.Size;
                        //c.Location = c.Name.Contains("label") ? new Point(10,10) : c.Location;
                    }
                }
            }//tablo alt kısım yeniden boyutlandırma

            ilkKosegen = yeniKosegen;
        } //Label yazı vs Boyutlandırmalar
        void SetComboBox()
        {
            foreach (Screen item in screens)
            {
                comboBox1.Items.Add(item.DeviceName);
            }
            //comboBox1.Focus();
            comboBox1.SelectedItem = comboBox1.Items[0];
        }  //Burası combobox olayını tetikleyip screen değişimi yapcak

    }
}
