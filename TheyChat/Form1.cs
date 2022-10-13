using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Timers;

namespace TheyChat
{
    public partial class Form1 : Form
    {
        Socket socketSend;
        public Form1()
        {
            InitializeComponent();
            //创建服务器端的Socket
            Socket tcpserivce = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //将服务器端的Socket绑定Ip和端口号
            IPAddress ip = IPAddress.Parse("127.0.0.1");//将ip转换为对应的格式
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32("8080"));//将IP和端口号组成point类
            tcpserivce.Bind(point);//服务器绑定
            ShowMsg("监听成功");
            //允许连接总数
            tcpserivce.Listen(10);
            Thread th = new Thread(Listen);//开启线程监听客户端的连接情况
            th.IsBackground = true;
            th.Start(tcpserivce);
        }

        /// <summary>
        /// 监听客户端连接函数
        /// </summary>
        /// <param name="t"></param>
        public void Listen(object t)
        {
            //获取服务器端的Socket
            Socket tcpserivce = t as Socket;
            while (true)//循环，保证一直可以接收客户端的连接
            {
                //接收到客户端的连接，创建新的socket与之通信
                socketSend = tcpserivce.Accept();
                //将远程连接的客户端的IP地址和Socket存入集合中
                dicSocket.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
                //显示连接成功
                ShowMsg(socketSend.RemoteEndPoint.ToString() + "连接成功");
                //开启线程接收客户端的消息
                Thread th1 = new Thread(Recive);
                th1.IsBackground = true;
                th1.Start(socketSend);
            }
        }
        //将远程连接的客户端的IP地址和Socket存入集合中
        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();
        //将远程连接的客户端的name和IP地址存入集合中
        Dictionary<string, string> dicName = new Dictionary<string, string>();
        //为timer1调用设置name
        //接收客户端的消息
        public void Recive(object o)
        {
            //获取到客户端的socket
            Socket socketSend = o as Socket;
            while (true)
            {
                //判断该客户端是否断开连接
                if (socketSend.Poll(10, SelectMode.SelectRead))
                {
                    break;
                }
                byte[] buffer = new byte[1024 * 1024 * 3];
                //实际接受到的有效字节数
                int r = socketSend.Receive(buffer);
                //将byte数组转换为string类型
                string str = Encoding.UTF8.GetString(buffer, 0, r);
                ShowMsg(socketSend.RemoteEndPoint + "说:" + str);
                if (r == 0)
                {
                    break;
                }                
                //表示接收的是客户端上线
                if (buffer[0] == 3)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
                    ShowMsg("[3]上线:"+ str);
                    dicName.Add(str, socketSend.RemoteEndPoint.ToString());
                    comboBox1.Items.Add(str);

                    sendAllOnline(str);
                    sendOnlineList(str);
                    //同时发送太多socket会串
                    //sendFollowList(str);


                    sql sql = new sql();
                    sql.online(str);
                }
                //表示接收的是客户端断开
                if (buffer[0] == 4)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
                    ShowMsg("[4]下线:" + str);

                    dicSocket.Remove(socketSend.RemoteEndPoint.ToString());
                    dicName.Remove(str);
                    comboBox1.Items.Remove(str);

                    sendAllOffline(str);

                    sql sql = new sql();
                    sql.offline(str);
                }
                //表示接收的是客户端添加好友
                if (buffer[0] == 5)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
                    string sendName = null;
                    string followName = null;
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
                        }
                    }
                    ShowMsg("[" + buffer[0] + "]" + sendName + "关注" + followName.Substring(0, followName.Length - 1));
                    sql sql = new sql();
                    int result = sql.follow(sendName, followName.Substring(0, followName.Length - 2));
                    string returnStr;
                    if (result == 1)
                        returnStr = sendName + " " + followName + " 成功";
                    else
                        returnStr = sendName + " " + followName + " 失败";
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer2 = System.Text.Encoding.UTF8.GetBytes(returnStr);
                    List<byte> list = new List<byte>();
                    list.Add(5);
                    list.AddRange(buffer2);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    //获取要发送的客户端
                    string ip = socketSend.RemoteEndPoint.ToString();
                    dicSocket[ip].Send(newBuffer);
                }
                //6留给了发送在线名单，7给取消关注，8给发送好友名单
                if (buffer[0] == 7)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
                    string sendName = null;
                    string followName = null;
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
                        }
                    }
                    ShowMsg("[" + buffer[0] + "]" + sendName + "取关" + followName.Substring(0, followName.Length - 1));
                    sql sql = new sql();
                    int result = sql.unfollow(sendName, followName.Substring(0, followName.Length - 2));
                    string returnStr;
                    if (result == 1)
                        returnStr = sendName + " " + followName + " 成功";
                    else
                        returnStr = sendName + " " + followName + " 失败";
                    //如果发送的是消息，在字节数组第一位加0以做标识
                    byte[] buffer2 = System.Text.Encoding.UTF8.GetBytes(returnStr);
                    List<byte> list = new List<byte>();
                    list.Add(7);
                    list.AddRange(buffer2);
                    //将泛型集合转换为数组
                    byte[] newBuffer = list.ToArray();
                    //获取要发送的客户端
                    string ip = socketSend.RemoteEndPoint.ToString();
                    dicSocket[ip].Send(newBuffer);
                }
                //表示接收的是客户端转发消息请求
                if (buffer[0] == 0|| buffer[0] == 1|| buffer[0] == 2)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
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
                    ShowMsg("["+ buffer[0] + "]" + sendName + "发给" + receiveName.Substring(0, receiveName.Length - 1)+ ":" + s);
                    //获取要发送的客户端
                    string name = receiveName;
                    string ip = dicName[name];
                    //dicSocket[ip].Send(newBuffer);
                    dicSocket[ip].Send(buffer);
                }
                //请求好友名单
                if (buffer[0] == 8)
                {
                    str = Encoding.UTF8.GetString(buffer, 1, r);
                    string sendName = str;
                    sendFollowList(sendName);
                }
            }
        }
        //通知所有人上线消息
        public void sendAllOnline(string str)
        {
            //如果发送的是消息，在字节数组第一位加0以做标识
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            List<byte> list = new List<byte>();
            list.Add(3);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            foreach (var item in dicSocket)
            {
                item.Value.Send(newBuffer);
            }
        }
        //通知所有人下线消息
        public void sendAllOffline(string str)
        {
            //如果发送的是消息，在字节数组第一位加0以做标识
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            List<byte> list = new List<byte>();
            list.Add(4);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            foreach (var item in dicSocket)
            {
                item.Value.Send(newBuffer);
            }
        }
        public void sendOnlineList(string name)
        {
            string sendName = null;
            foreach (var item in dicName)
            {
                sendName += (item.Key + " ");
            }
            //如果发送的是消息，在字节数组第一位加0以做标识
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sendName);
            List<byte> list = new List<byte>();
            list.Add(6);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            //获取要发送的客户端
            string ip = dicName[name];
            dicSocket[ip].Send(newBuffer);
            textBox1.AppendText("给" + name);
            textBox1.AppendText("发送在线列表\r\n");
        }
        public void sendFollowList(string name)
        {
            byte[] buffer=null;
            byte[] newBuffer=null;
            sql sql = new sql();
            string sendName = sql.getfollow(name);
            if(sendName != null)
            {
                buffer = System.Text.Encoding.UTF8.GetBytes(sendName);
                List<byte> list = new List<byte>();
                list.Add(8);
                list.AddRange(buffer);
                //将泛型集合转换为数组
                newBuffer = list.ToArray();
            }
            else
            {
                List<byte> list = new List<byte>();
                list.Add(8);
                //将泛型集合转换为数组
                newBuffer = list.ToArray();
            }
            //获取要发送的客户端
            string ip = dicName[name];
            dicSocket[ip].Send(newBuffer);
            textBox1.AppendText("给" + name);
            textBox1.AppendText("发送好友列表\r\n");
        }
        //消息框追加消息
        void ShowMsg(string str)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            textBox1.AppendText(str);
            textBox1.AppendText("\r\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string str = textBox2.Text;
            //如果发送的是消息，在字节数组第一位加0以做标识
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            List<byte> list = new List<byte>();
            list.Add(0);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            //获得用户在下拉框中选中的IP地址
            if (comboBox1.SelectedItem == null)
            {
                textBox1.AppendText("没有选择发送的客户端" + "\r\n");
            }
            else
            {
                //获取要发送的客户端
                string name = comboBox1.SelectedItem.ToString();
                string ip = dicName[name];
                dicSocket[ip].Send(newBuffer);
                textBox1.AppendText("我说"+textBox2.Text + "\r\n");
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                sql sql = new sql();
                foreach (var item in dicName)
                {
                    string name = item.Key.Trim();
                    sql.offline(name);
                }
                socketSend.Shutdown(SocketShutdown.Both);
                socketSend.Close();
            }
            catch (Exception)
            {

            }
        }

    }
}