using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheyChat
{
    public partial class Form2 : Form
    {
        public Socket socketSend;
        public string name = null;
        public bool isOnline = false;
        public bool followGot = false;
        Dictionary<string, string> follow = new Dictionary<string, string>();
        public Form2()
        {
            InitializeComponent();
            //创建客户端的socket，绑定服务器端ip和端口，准备连接
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32("8080"));
            try
            {
                socketSend.Connect(point);//连接服务器
                ShowMsg("连接成功");
            }
            catch (Exception)
            {
                textBox1.AppendText("服务器未打开" + "\r\n");
            }
            //开启一个新的线程不停的接收服务端发来的消息
            Thread th = new Thread(Recive);
            th.IsBackground = true;
            th.Start();
        }

        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        void ShowMsg(string str)
        {
            textBox1.AppendText(str);
            textBox1.AppendText("\r\n");
        }

        //不停接收服务器消息
        void Recive()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    int r = socketSend.Receive(buffer);
                    //实际接收到的有效字节数
                    if (r == 0)
                    {
                        break;
                    }
                    //表示接收的是文字消息
                    if (buffer[0] == 0)
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string receiveName = null;
                        string s = null;
                        string[] arr = str.Split(' ');

                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (receiveName == null)
                                    receiveName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        ShowMsg("[0]" + sendName + "说:" + s);
                    }
                    if (buffer[0] == 1)//接收的是文件
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string receiveName = null;
                        string s = null;
                        string[] arr = str.Split(' ');

                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (receiveName == null)
                                    receiveName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        ShowMsg("[1]" + sendName + "发送了文件");
                        byte[] namebuffer = Encoding.UTF8.GetBytes(sendName+" "+ receiveName+" ");
                        byte[] filebuffer = Encoding.UTF8.GetBytes(s);
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Title = "请选择要保存的文件";
                        sfd.Filter = "所有文件|*.*";
                        sfd.ShowDialog(this);
                        string path = sfd.FileName;
                        using (FileStream fsWrite = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fsWrite.Write(filebuffer, 0, filebuffer.Length);
                        }
                        MessageBox.Show("保存成功");
                    }
                    if (buffer[0] == 2)//接收的是振动消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string receiveName = null;
                        string s = null;
                        string[] arr = str.Split(' ');

                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (receiveName == null)
                                    receiveName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        ShowMsg("[2]" + sendName + "发送了窗口抖动" );
                        ZD();
                    }
                    if (buffer[0] == 3)//接收的是上线消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string s = null;
                        string[] arr = str.Split(' ');

                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                        }
                        ShowMsg("[3]" + sendName + "上线了");
                        comboBox1.Items.Add(sendName);
                        refreshFollow();
                    }
                    if (buffer[0] == 4)//接收的是下线消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string receiveName = null;
                        string s = null;
                        string[] arr = str.Split(' ');

                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (receiveName == null)
                                    receiveName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        ShowMsg("[4]" + sendName + "下线了");
                        refreshFollow();
                    }
                    if (buffer[0] == 5)//接收的是好友添加消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string followName = null;
                        string followNameClean = null;
                        string s = null;
                        string[] arr = str.Split(' ');
                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (followName == null)
                                    followName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        foreach (char i in followName)
                        {
                            if (i != '\0')
                                followNameClean += i;
                        }
                        textBox1.AppendText("[5]关注" + followName);
                        ShowMsg(s);
                        follow.Add(followNameClean, "");
                        refreshFollow();
                    }
                    if (buffer[0] == 6)//接收的是在线列表
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1).Trim();
                        //清理'\0'
                        string strClean = null;
                        foreach (char i in str)
                        {
                            if (i != '\0')
                                strClean += i;
                        }
                        //按照空格分割
                        string s = null;
                        string[] arr = strClean.Split(' ');
                        foreach (var i in arr)
                        {
                            s += i + " ";
                            if (i != name && i != " ")
                                comboBox1.Items.Add(i + "\0");
                        }
                        //ShowMsg("[6]" + s + "在线");
                    }
                    if (buffer[0] == 7)//接收的是好友删除消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        string sendName = null;
                        string followName = null;
                        string followNameClean = null;
                        string s = null;
                        string[] arr = str.Split(' ');
                        foreach (var i in arr)
                        {
                            if (sendName == null)
                            {
                                sendName = i;
                            }
                            else
                            {
                                if (followName == null)
                                    followName = i;
                                else
                                    s += i + " ";
                            }
                        }
                        foreach (char i in followName)
                        {
                            if (i != '\0')
                                followNameClean += i;
                        }
                        textBox1.AppendText("[7]取关" + followName);
                        ShowMsg(s);
                        follow.Remove(followNameClean);
                        refreshFollow();
                    }
                    if (buffer[0] == 8)//接收的是好友列表
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1).Trim();
                        //清理'\0'
                        string strClean = null;
                        foreach (char i in str)
                        {
                            if (i != '\0')
                                strClean += i;
                        }
                        //按照空格分割
                        string s = null;
                        string[] arr = null;
                        if (strClean != null)
                        {
                            arr = strClean.Split(' ');
                            foreach (var i in arr)
                            {
                                s += i + " ";
                                if (i != "  ")
                                    follow.Add(i , "");
                            }
                        }
                        ShowMsg("[8]关注了" + s );
                        refreshFollow();
                    }
                }
                catch { }

            }
        }

        //对窗口进行振动
        void ZD()
        {
            for (int i = 0; i < 500; i++)
            {
                this.Location = new Point(200, 200);
                this.Location = new Point(200, 210);
            }
        }

        public void sendOffline()
        {
            name = label1.Text;
            string str = get_uft8(name);
            //如果发送的是消息，在字节数组第一位加0以做标识
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            List<byte> list = new List<byte>();
            list.Add(4);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            socketSend.Send(newBuffer);
        }

        public void sendOnline()
        {
            name = label1.Text.Trim();
            if (name != "label1" && !isOnline)
            {
                string str = get_uft8(name);
                //如果发送的是消息，在字节数组第一位加0以做标识
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                List<byte> list = new List<byte>();
                list.Add(3);
                list.AddRange(buffer);
                //将泛型集合转换为数组
                byte[] newBuffer = list.ToArray();
                socketSend.Send(newBuffer);
                textBox1.AppendText("我上线了\r\n");
                isOnline = true;
                timer1.Stop();
            }
            if (!isOnline)
                ShowMsg("offline");
        }
        //发送消息
        private void button3_Click(object sender, EventArgs e)
        {
            //获得用户在下拉框中选中的IP地址
            if (comboBox2.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                string receiveName = comboBox2.SelectedItem.ToString().Trim();
                string receiveNameClean = null;
                foreach (char i in receiveName)
                {
                    if (i != '\0')
                        receiveNameClean += i;
                }
                string content = textBox2.Text.Trim();
                string str = name + " " + receiveNameClean + "\0 " + content;
                if (receiveNameClean != "")
                {
                    str = get_uft8(str);
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    List<byte> list = new List<byte>();
                    list.Add(0);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    socketSend.Send(newBuffer);
                    textBox1.AppendText("我对" + comboBox1.SelectedItem);
                    textBox1.AppendText("说:"+ content + "\r\n");
                }
            }
        }
        //发送文件
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = @"C:\Users\admin\Desktop";
                ofd.Title = "请选择要发送的文件";
                ofd.Filter = "所有文件|*.*";
                ofd.ShowDialog();
                textBox2.Text = ofd.FileName;
                string path = textBox2.Text;
                using (FileStream freRead = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    string receiveName = comboBox2.SelectedItem.ToString().Trim();
                    string receiveNameClean = null;
                    foreach (char i in receiveName)
                    {
                        if (i != '\0')
                            receiveNameClean += i;
                    }
                    string str = name + " " + receiveNameClean + "\0 ";
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] nameBuffer = Encoding.UTF8.GetBytes(str);
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    freRead.Read(buffer, 0, (int)freRead.Length);
                    List<byte> list = new List<byte>();
                    list.Add(1);
                    list.AddRange(nameBuffer);
                    list.AddRange(buffer);
                    byte[] newBuffer = list.ToArray();
                    socketSend.Send(newBuffer, 0, (int)freRead.Length + 1 + nameBuffer.Length, SocketFlags.None);
                }
                textBox1.AppendText("我给"+comboBox1.SelectedItem);
                ShowMsg("发文件");
            }

        }
        //发送抖动
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                string receiveName = comboBox2.SelectedItem.ToString().Trim();
                string receiveNameClean = null;
                foreach (char i in receiveName)
                {
                    if (i != '\0')
                        receiveNameClean += i;
                } 
                string content = textBox2.Text.Trim();
                string str = name + " " + receiveNameClean + "\0 " + content;
                if (receiveNameClean != "")
                {
                    str = get_uft8(str);
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    List<byte> list = new List<byte>();
                    list.Add(2);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    socketSend.Send(newBuffer);
                    textBox1.AppendText("我给" + comboBox1.SelectedItem);
                    textBox1.AppendText("发送了抖动消息\r\n");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //开启一个新的线程发送上线请求
            sendOnline();
        }
        //通知服务器下线
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                sendOffline();
                //断开客户端
                socketSend.Shutdown(SocketShutdown.Both);
                socketSend.Close();
            }
            catch (Exception)
            {

            }
        }
        //添加好友
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                string followName = comboBox1.SelectedItem.ToString().Trim();
                string str = name + " " + followName ;
                if (followName != "")
                {
                    str = get_uft8(str);
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    List<byte> list = new List<byte>();
                    list.Add(5);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    socketSend.Send(newBuffer);
                    textBox1.AppendText("我关注" + comboBox1.SelectedItem);
                    textBox1.AppendText("\r\n");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                string followName = comboBox2.SelectedItem.ToString().Trim();
                string followNameClean = null;
                foreach (char i in followName)
                {
                    if (i != '\0')
                        followNameClean += i;
                }
                string str = name + " " + followNameClean +"\0";
                if (followNameClean != "")
                {
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    List<byte> list = new List<byte>();
                    list.Add(7);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    socketSend.Send(newBuffer);
                    textBox1.AppendText("我取关" + comboBox1.SelectedItem);
                    textBox1.AppendText("\r\n");
                }
            }
        }

        public void refreshFollow()
        {
            comboBox2.Items.Clear();
            foreach (var item in follow)
            {
                string name1 = item.Key;
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    string name2 = comboBox1.GetItemText(comboBox1.Items[i]);
                    string name2Clean = null;
                    foreach (char j in name2)
                    {
                        if (j != '\0')
                            name2Clean += j;
                    }
                    if (name1 == name2Clean)
                    {
                        comboBox2.Items.Add(name2+"\0");
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //开启一个新的线程发送上线请求
            sendForFollow();
        }

        public void sendForFollow()
        {
            name = label1.Text.Trim();
            if (name != "label1" && !followGot)
            {
                string str = get_uft8(name);
                //如果发送的是消息，在字节数组第一位加0以做标识
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                List<byte> list = new List<byte>();
                list.Add(8);
                list.AddRange(buffer);
                //将泛型集合转换为数组
                byte[] newBuffer = list.ToArray();
                socketSend.Send(newBuffer);
                textBox1.AppendText("更新关注表\r\n");
                followGot = true;
                timer2.Stop();
            }
            if (!followGot)
                ShowMsg("offline");
        }
    }
}