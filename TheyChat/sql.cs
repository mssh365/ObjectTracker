using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheyChat
{
    internal class sql
    {
        public string server = "127.0.0.1";
        public string user = "root";
        public string pwd = "chen";
        public string sqlstr = "";
        public string conn = "";
        public string connstr = "";
        public string sqlname = "chatsql";
        //search
        //查询数据库sqlname中的tabelname表,返回一个DataSet类型的数据
        public int login(string name,string pwd)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "select * from user where name = "+ name + " and password = "+ pwd + " and status = 0";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();//执行ExecuteReader()返回一个MySqlDataReader对象

                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    return 1;//"userid"是数据库对应的列名，推荐这种方式
                }
            }
            catch (MySqlException ex){}
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            } 
            return 0;
        }
        public int login_admin(string name, string pwd)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "select * from user where name = " + name + " and password = " + pwd + " and admin = "+1;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();//执行ExecuteReader()返回一个MySqlDataReader对象

                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    return 1;//"userid"是数据库对应的列名，推荐这种方式
                }
            }
            catch (MySqlException ex) { }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public int regist(string name, string pwd)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "insert into user value(" + name + "," + pwd + ",0,0)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public int online(string name)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "update user set status = 1 where name ="+ name;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public int offline(string name)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "update user set status = 0 where name =" + name;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public int follow(string name,string follow)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "insert into fan value( " + name + " , " + follow + " )";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public int unfollow(string name, string follow)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);

            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "delete from fan where name ="+ name+" and follow ="+follow;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return 0;
        }
        public string getfollow(string name)
        {
            string connstr = "server = 127.0.0.1; port = 3306; user = root ; password = chen; database = chatsql";
            MySqlConnection conn = new MySqlConnection(connstr);
            string str = null;
            try
            {
                //可能出现异常
                conn.Open();
                //return"链接成功!";
                string sql = "select * from fan where name = " + name;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();//执行ExecuteReader()返回一个MySqlDataReader对象

                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    str += reader.GetString(1)+" ";
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    //"userid"是数据库对应的列名，推荐这种方式
                }
            }
            catch (MySqlException ex) { }
            finally
            {
                //务必关闭MysqlConnection
                conn.Close();
            }
            return str;
        }
    }
}
