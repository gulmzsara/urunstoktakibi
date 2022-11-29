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

namespace stok_takip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection connect = new SqlConnection(@"Data Source=.;Initial Catalog = stok; Integrated Security=True");

        private void button1_Click(object sender, EventArgs e)
        {
            anaSayfa mainpage = new anaSayfa();

            string k_adi, parola, query;
            k_adi = textBox1.Text;
            parola = textBox2.Text;
            connect.Open();
            query = "select * from [user] where name=@k_adi and password=@parola";
            SqlParameter p1 = new SqlParameter("@k_adi", k_adi);
            SqlParameter p2 = new SqlParameter("@parola", parola);
            SqlCommand cmd = new SqlCommand(query,connect);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            if(dt.Rows.Count>0)
            {
                this.Hide();
                mainpage.Show();
            }
            else
            {
                MessageBox.Show(k_adi+ " isminde kullanıcı yok");
            }
            connect.Close();
        }
    }
}
