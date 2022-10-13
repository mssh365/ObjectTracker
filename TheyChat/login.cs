using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheyChat
{
    public partial class login : Form
    {
        string name=null;
        string password=null;
        public login()
        {
            InitializeComponent();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            password = textBox2.Text;
            sql sql = new sql();
            if (name != "" && password != "")
            {
                int admin=sql.login_admin(name,password);
                if (admin == 1)
                {
                    MessageBox.Show("管理员登录成功");
                    Form1 form1 = new Form1();
                    form1.Show();
                }
                else
                {
                    int ds = sql.login(name, password);
                    if (ds == 1)
                    {
                        MessageBox.Show("登录成功");
                        Form2 form2 = new Form2();
                        form2.Show();
                        form2.label1.Text = name;
                    }
                    else
                        MessageBox.Show("用户名或密码错误或重复登录");
                }
            }
            else
            {
                MessageBox.Show("请输入用户名和密码");

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            password = textBox2.Text;
            sql sql = new sql();
            if (name != "" && password != "")
            {
                int ds = sql.regist(name, password);
                if (ds == 1)
                    MessageBox.Show("注册成功");
                else
                    MessageBox.Show("用户名重复");
            }
            else
            {
                MessageBox.Show("请输入用户名和密码");
            }
        }
    }
}
