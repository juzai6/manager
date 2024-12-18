using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;

namespace 新用户管理系统
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //零、清空文本框
        protected void ClearTextbox()
        {
            text_Name.Text="";
            text_Company.Text="";
            text_Telephone.Text="";
            text_Adress.Text="";
            rbtn_Sex1.Checked=true;
            nudown_Age.Value=1;
            lbl_status.Text="添加";
            customerid = "";
        }

        //一、添加客户信息
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string name = text_Name.Text.Trim();
            string company = text_Company.Text.Trim();
            string telephone = text_Telephone.Text.Trim();
            string address = text_Adress.Text.Trim();
            string sex = rbtn_Sex1.Checked ? "男" : "女";
            string age = nudown_Age.Value.ToString();

            if (name == " ")
            {
                lbl_Note.Text = "姓名不能为空";
                lbl_Note.ForeColor = Color.Red;
                text_Name.Focus();
            }
            else if (lbl_status.Text == "添加")
            {
                SqlConnection con = new SqlConnection("Data Source=juzai66;database=wang;Integrated Security=SSPI");
                con.Open();
                string str = string.Format("insert into wmc2 values('{0}','{1}','{2}','{3}','{4}','{5}')", name, company, sex, age, telephone, address);
                SqlCommand cmd = new SqlCommand(str, con);
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (i > 0)
                {
                    lbl_Note.Text = "添加成功";
                    lbl_Note.ForeColor = Color.Blue;
                    ClearTextbox();
                    DataBind_Customer();
                }
                else
                {
                    lbl_Note.Text = "添加失败";
                    lbl_Note.ForeColor = Color.Red;
                }

            }
            else    //修改操作
            {
                //1、创建数据库连接
                SqlConnection comm = new SqlConnection("Data Source=juzai66; database=wang; Integrated Security=SSPI");
                //2、打开连接
                comm.Open();
                //3、数据修改语句
                string str = string.Format("update wmc2 set CustomerName='{0}',Company='{1}',Sex='{2}',Age={3},Telephone='{4}',Address='{5}' Where CustomerID={6}", name, company, sex, age, telephone, address, customerid);
                //4、创建执行对象
                SqlCommand cmd = new SqlCommand(str, comm);
                //5、执行操作，返回受影响的行数
                int i = cmd.ExecuteNonQuery();
                //6、关闭连接
                comm.Close();
                //7、处理结果
                if (i > 0)
                {
                    lbl_Note.Text = "恭喜您，客户信息修改成功！";
                    lbl_Note.ForeColor = Color.Blue;
                    ClearTextbox();    //调用函数，清空各控件
                    DataBind_Customer();    //重新加载客户信息
                }
                else
                {
                    lbl_Note.Text = "对不起，客户信息修改失败！";
                    lbl_Note.ForeColor = Color.Red;
                }
            }
        }
        //二、加载客户信息
        protected void DataBind_Customer()
        {
            //1、创建并打开数据库连接
            SqlConnection conn = new SqlConnection("Data Source=juzai66;database=wang;Integrated Security=SSPI");
            conn.Open();
            //2、客户端发出请求：数据查询语句
            string str = "select * from wmc2";
            //3、创建执行对象
            SqlDataAdapter da = new SqlDataAdapter(str, conn);
            //4、创建临时数据表
            DataTable dt = new DataTable();
            //5、执行查询，返回结果，填充到临时表中
            da.Fill(dt);
            //6、关闭连接
            conn.Close();
            //7、显示结果
            lv_Customer.Items.Clear(); //先清空列表框视图控件中现有行
            foreach (DataRow dr in dt.Rows)
            {
                ListViewItem myitem = new ListViewItem(dr["Customerid"].ToString()); //创建一个ListViewItem项，并为第1列赋值，客户编号
                myitem.SubItems.Add(dr["name"].ToString()); //第2列，姓名
                myitem.SubItems.Add(dr["company"].ToString()); //第3列，单位
                myitem.SubItems.Add(dr["sex"].ToString()); //第4列，性别
                myitem.SubItems.Add(dr["age"].ToString()); //第5列，年龄
                myitem.SubItems.Add(dr["telephone"].ToString()); //第6列，电话
                myitem.SubItems.Add(dr["address"].ToString()); //第7列，地址
                lv_Customer.Items.Add(myitem); //将新建项添加到ListView控件中
            }
        }

        //三、窗体加载事件
        private void Form1_Load(object sender, EventArgs e)
        {
            DataBind_Customer(); //加载客户信息
        }

        string customerid = "";  // 定义全局变量，用于存储客户编号



        // 五、删除客户信息
        private void btn_Del_Click(object sender, EventArgs e)
        {
            if (customerid == "")    //如果没有选中要删除的客户信息
            {
                MessageBox.Show("请先选择要删除的客户信息！");
            }
            else
            {
                //弹出删除确认提示框
                DialogResult result = MessageBox.Show("确定要删除选中的客户信息？", "删除提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)    //如果确定删除
                {
                    //1、创建数据库连接
                    SqlConnection comm = new SqlConnection("Data Source=juzai66; database=wang; Integrated Security=SSPI");
                    //2、打开连接
                    comm.Open();
                    //3、数据修改语句
                    string str = string.Format("delete from wmc2 where Customerid={0}", customerid);
                    //4、创建执行对象
                    SqlCommand cmd = new SqlCommand(str, comm);            //5、执行操作，返回受影响的行数
                    int i = cmd.ExecuteNonQuery();
                    //6、关闭连接
                    comm.Close();
                    //7、处理结果
                    if (i > 0)
                    {
                        lbl_Note.Text = "恭喜您，客户信息删除成功！";
                        lbl_Note.ForeColor = Color.Blue;
                        ClearTextbox();    //调用函数，清空各控件
                        DataBind_Customer();    //重新加载客户信息
                    }
                    else
                    {
                        lbl_Note.Text = "对不起，客户信息删除失败！";
                        lbl_Note.ForeColor = Color.Red;
                    }
                }
            }
        }



        //四、【客户信息】选中事件
        private void lv_Customer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_Customer.SelectedItems.Count > 0)  // 如果有选中项
            {
                ListViewItem myitem = lv_Customer.SelectedItems[0];  // 获取选中的第一行（一次只能选一行）

                customerid = myitem.SubItems[0].Text;  // 将选中行第1列的值赋值给全局变量，客户编号
                text_Name.Text = myitem.SubItems[1].Text;  // 选中行第2列，姓名
                text_Company.Text = myitem.SubItems[2].Text;  // 选中行第3列，单位
                rbtn_Sex1.Checked = myitem.SubItems[3].Text == "男" ? true : false;  // 性别
                rbtn_Sex2.Checked = myitem.SubItems[3].Text == "女" ? true : false;  // 性别
                nudown_Age.Value = decimal.Parse(myitem.SubItems[4].Text);  // 年龄
                text_Telephone.Text = myitem.SubItems[5].Text;  // 电话
                text_Adress.Text = myitem.SubItems[6].Text;  // 地址
                lbl_status.Text = "修改";  // 当前状态
            }
        }

        //六、取消事件
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ClearTextbox();    //调用函数，清空各控件
            lbl_Note.Text = "";
        }
    }
}

