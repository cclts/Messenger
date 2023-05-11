using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing.Drawing2D;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket s;
        Thread Th;
        string ip = "172.20.10.4";
        int port = 2021;
        string myname;
        string name;

        bool data_flag = false;
        string extension;

        int height = 0;
        int count = 0;
        int have_shown = 0;
        string[] name_ = new string[50];
        string[] content_ = new string[50];

        Label[] l1 = new Label[50];
        Label[] l2 = new Label[50];
        FlowLayoutPanel[] p = new FlowLayoutPanel[50];
        public Form1()
        {
            InitializeComponent();
            

            for (int i = 0; i < 50; i++)
            {
                l1[i] = new Label();
                l1[i].Name = "name" + splitContainer1.Panel1.Controls.Count;
                l1[i].AutoSize = true;
                l1[i].Visible = true;
                l1[i].Font = new Font("微軟正黑體", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
                l1[i].ForeColor = Color.Black;
                l1[i].Location = new Point(10, 0);
                l1[i].Margin = new Padding(10, 0, 3, 0);
                //l1[i].Text = name + ":";

                l2[i] = new Label();
                l2[i].Name = "text" + splitContainer1.Panel1.Controls.Count;
                l2[i].AutoSize = true;
                l2[i].Visible = true;
                l2[i].BackColor = Color.WhiteSmoke;
                l2[i].Dock = DockStyle.Top;
                l2[i].Font = new Font("微軟正黑體", 14F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                l2[i].ForeColor = Color.Black;
                l2[i].Location = new Point(10, 24);
                l2[i].Margin = new Padding(10, 3, 3, 3);
                //l2[i].Text = str;

                p[i] = new FlowLayoutPanel();
                p[i].Name = "panel" + splitContainer1.Panel1.Controls.Count;
                p[i].AutoSize = true;
                p[i].Visible = true;
                p[i].FlowDirection = FlowDirection.TopDown;
                //p[i].Location = new Point(3, (Int32)height);
                p[i].MaximumSize = new Size(320, 0);
                p[i].Controls.Add(l1[i]);
                p[i].Controls.Add(l2[i]);
                splitContainer1.Panel1.Controls.Add(p[i]);

            }
        }
    
        private void init_chat()
        {
            textBox1.Enabled = true;
            textBox1.Visible = true;
            button2.Enabled = true;
            button2.Visible = true;
            button3.Enabled = true;
            button3.Visible = true;
            splitContainer1.Panel2.Enabled = true;
            splitContainer1.Panel1.HorizontalScroll.Maximum = 0;
            splitContainer1.Panel1.AutoScroll = false;
            splitContainer1.Panel1.VerticalScroll.Visible = false;
            splitContainer1.Panel1.AutoScroll = true;
        }
        private void Listen()
        {
            EndPoint ServerEP = (EndPoint)s.RemoteEndPoint;
            int len = 0;
            string Msg;
            string flag;
            string str;
            while (true)
            {
                try
                {
                    byte[] B = new byte[s.ReceiveBufferSize];
                    len = s.ReceiveFrom(B, ref ServerEP);

                    Msg = Encoding.Default.GetString(B, 0, len);
                    flag = Msg.Substring(0, 1);
                    str = Msg.Substring(1);

                    if (flag == "0")//對方名字
                    {
                        name = str;
                    }
                    if (flag == "1")//對方訊息
                    {
                        name_[count] = name;
                        content_[count] = str;
                        Console.WriteLine(name_[count] + ": " + content_[count]);
                        count++;
                    }

                    if (data_flag == true)
                    {
                        string filename = null;
                        foreach (char c in str.Substring(20, 25))
                        {
                            filename += Convert.ToInt32(c);
                        }
                        File.WriteAllBytes("D:\\joyli\\Downloads\\Lisa\\" + filename + extension, B);
                        data_flag = false;
                    }   

                    if (flag == "8")//副檔名
                    {
                        data_flag = true;
                        extension = str;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    MessageBox.Show("伺服器斷線");
                    flowLayoutPanel0.Visible = true;
                    flowLayoutPanel0.Enabled = true;
                    button1.Enabled = true;
                    button1.Text = "重新連線";

                    s.Close();
                    Th.Abort();
                }
                
            }
        }
        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            s.Send(B, 0, B.Length, SocketFlags.None);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            if (textBox2.Text == "" || textBox2.ForeColor == Color.DimGray) return;
            try
            {
                IPEndPoint EP = new IPEndPoint(IPAddress.Parse(ip), port);
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(EP);
                Th = new Thread(Listen);
                Th.IsBackground = true;
                Th.Start();
               
                Send("0" + textBox2.Text);
                myname = textBox2.Text;
                flowLayoutPanel0.Visible = false;
                flowLayoutPanel0.Enabled = false;
                init_chat();
            }
            catch (Exception)
            {
                button1.Text = "重新連線";
                label1.Visible = true;
                label1.Text = "連線失敗，請重新連線";
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            Send("1" + textBox1.Text);
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\Users\\joyli\\OneDrive\\圖片";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();
                    MemoryStream ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    
                    byte[] file = ms.ToArray();
                    //Console.WriteLine(file.Length);
                    Send("8" + Path.GetExtension(filePath));
                    s.Send(file, 0, file.Length, SocketFlags.None);
                    Console.WriteLine("file has sent");
                    ms.Close();
                    fileStream.Close();
                }
            }
        }


        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.ForeColor=Color.Black;
            textBox2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Send("9");
            s.Close();
            if(Th != null) Th.Abort();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = have_shown; i < count; i++)
            {
                l1[i].Text = name_[i] + ":";
                l2[i].Text = content_[i];
                p[i].Location = new Point(3, (Int32)height);
                height += p[i].Height;
                have_shown++;

                if (name_[i] == myname)
                {
                    l1[i].Visible = false;
                    l2[i].BackColor = Color.DodgerBlue;
                    l2[i].ForeColor = Color.White;
                    int Width = splitContainer1.Panel1.Width - l2[i].Width - 40;
                    p[i].Location = new Point(Width, (Int32)height);
                }

                Console.WriteLine("i = " + i + ",p[i].Location = " + p[i].Location);
            }
        }
    }
}
