using System;
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
using System.Collections;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        TcpListener Server;
        Socket Client;
        Thread Th_Svr;
        Thread Th_Clt;
        Hashtable HT = new Hashtable();
        string name;
        bool data_flag = false;
        int datasize = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void ServiceOn()
        {
            Server = new TcpListener(IPAddress.Parse(button1.Text), 2021);
            Server.Start();
            while (true)
            {
                try
                {
                    Client = Server.AcceptSocket();
                    Th_Clt = new Thread(Listen);
                    Th_Clt.IsBackground = true;
                    Th_Clt.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Listen()
        {
            Socket Sck = Client;
            Thread Th = Th_Clt;
            while (true)
            {
                try
                {
                    byte[] B = new byte[Sck.ReceiveBufferSize];
                    int len = Sck.Receive(B);
                    Console.WriteLine("Receive B ");

                    string Msg = Encoding.Default.GetString(B, 0, len);
                    string flag = Msg.Substring(0, 1);
                    string str = Msg.Substring(1);

                    if (flag == "0")//建立新用戶
                    {
                        HT.Add(str, Sck);
                        listBox1.Items.Add(str);
                    }

                    foreach (string n in HT.Keys)
                    {
                        if (HT[n] == Sck) name = n;
                    }

                    if (flag == "1")//接收+傳送訊息
                    {
                        foreach (string n in HT.Keys)
                        { 
                            SendTo("0" + name, n);
                            SendTo("1" + str, n);
                            Console.WriteLine(name + ':' + str);
                        }
                    }

                    if (data_flag == true)
                    {
                        foreach (string n in HT.Keys)
                        {
                            Socket s = (Socket)HT[n];
                            s.Send(B, 0, B.Length, SocketFlags.None);
                            Console.WriteLine("Sever has send a file.");
                        }
                        data_flag = false;
                    }

                    if (flag == "8")//副檔名
                    {
                        foreach (string n in HT.Keys)
                        {
                            SendTo("8" + str, n);
                        }
                        data_flag = true;
                        Console.WriteLine("data_flag has been set");
                    }

                    if (flag == "9")//刪除用戶
                    {
                        listBox1.Items.Remove(name);
                        HT.Remove(name);
                        Th.Abort();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private void SendTo(string Str, string user)
        {
            byte[] B = Encoding.Default.GetBytes(Str);
            Socket Sck = (Socket)HT[user];
            Sck.Send(B, 0, B.Length, SocketFlags.None);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            if(Server != null) Server.Stop();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            IPAddress[] ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress it in ip)
            {
                if (it.AddressFamily == AddressFamily.InterNetwork)
                    button1.Text = it.ToString();
                else button1.Text = "";
            }

            Th_Svr = new Thread(ServiceOn);
            Th_Svr.IsBackground = true;
            Th_Svr.Start();
            button1.Enabled = false;
        }

    }
}
