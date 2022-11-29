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
    public partial class stokgiriscıkıs : Form
    {
        int id = 0;
        int quantity = 0;
        SqlConnection connect = new SqlConnection(@"Data Source=.;Initial Catalog = stok; Integrated Security=True");

        public stokgiriscıkıs()
        {
            InitializeComponent();
        }
        

        void veri_cek(string kelime)
        {
            string adet_giris,adet_cikis,adet;
            string sorgu="";
            //bunu her yere yazdım çünkü her işlemde datagridview yenileniyor. 
            connect.Open();
            string where_sartlari = " where name like '%" + kelime + "%' or barcode like '%" + kelime + "%' or seri_no like '%" + kelime + "%'";
            SqlCommand cmd = new SqlCommand("select id as ID,name 'Ürün Adı',barcode as 'Barkod',seri_no as 'Seri No',create_date as 'Kayıt Tarihi' from product" + where_sartlari, connect);
            SqlDataReader okuyucu = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            DataColumn dc;
            dc = new DataColumn("ID", typeof(String));
            dt.Columns.Add(dc);
            dc = new DataColumn("Ürün Adı", typeof(String));
            dt.Columns.Add(dc);
            dc = new DataColumn("Barkod", typeof(String));
            dt.Columns.Add(dc);
            dc = new DataColumn("Seri No", typeof(String));
            dt.Columns.Add(dc);
            dc = new DataColumn("Adet", typeof(String));
            dt.Columns.Add(dc);
            //burdaki sorguda sadece o idye ait olan kaydın resmini getiriyo
            string[,] data = new string[1000,5];
            int i = 0;
            while (okuyucu.Read())
            {

                data[i, 0] = okuyucu[0].ToString();
                data[i, 1] = okuyucu[1].ToString();
                data[i, 2] = okuyucu[2].ToString();
                data[i, 3] = okuyucu[3].ToString();
                i++;

            }
            okuyucu.Close();
            for(int j=0; j<i; j++)
            {
                DataRow dr = dt.NewRow();
                sorgu = "select sum(quantity) as adet  from action where product_id = " + data[j, 0] + " and action = 1 group by product_id";
                SqlCommand komut = new SqlCommand(sorgu, connect);
                SqlDataReader readData = komut.ExecuteReader();

                adet_giris = "0";
                if (readData.HasRows == true)
                {
                    while (readData.Read())
                    {
                        adet_giris = readData["adet"].ToString();

                    }
                }

                readData.Close();
                sorgu = "select sum(quantity) as adet  from action where product_id = " + data[j, 0] + " and action = 0 group by product_id";
                 komut = new SqlCommand(sorgu, connect);
                 readData = komut.ExecuteReader();

                adet_cikis = "0";
                if (readData.HasRows == true)
                {
                    while (readData.Read())
                    {
                        adet_cikis = readData["adet"].ToString();

                    }
                }

                readData.Close();
                adet = (Convert.ToInt32(adet_giris) - Convert.ToInt32(adet_cikis)).ToString();

                dr[0] = data[j, 0];
                dr[1] = data[j, 1];
                dr[2] = data[j, 2];
                dr[3] = data[j, 3];
                dr[4] = adet;
                dt.Rows.Add(dr);
               
            }
            dataGridView1.DataSource = dt;
            connect.Close();
        }

        private void stokgiriscıkıs_Load(object sender, EventArgs e)
        {
            veri_cek("");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string action ="1";
            if (id==0)
            {
                MessageBox.Show("Ürün Seçiniz","Uyarı");
            }
            else
            {
                DateTime date = DateTime.Now;
                if (radioButton1.Checked == true)
                    action = "1";
                else if (radioButton2.Checked == true)
                    action = "0";

                int qty = Convert.ToInt32(textBox1.Text);
                if(action=="0" && (quantity-qty)<0)
                {
                    MessageBox.Show("İçeriye girilmiş bu kadar ürün yok. Lütfen "+ qty.ToString()+"'den daha az adet giriniz.", "Uyarı!!");
                }
                else
                {
                string query = "insert into action (date,product_id,action,quantity) values (@date,@product_id,@action,@quantity)";
                connect.Open();
                SqlCommand cmd = new SqlCommand(query,connect);
                
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@product_id", id);
                cmd.Parameters.AddWithValue("@action", action);
                cmd.Parameters.AddWithValue("@quantity", textBox1.Text);
                cmd.ExecuteNonQuery();
                connect.Close();
                MessageBox.Show("Başarılı!", "İşlem Sonucu");

                }
                veri_cek("");
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            quantity = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Adet"].Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            anaSayfa sayfa = new anaSayfa();
            sayfa.Show();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            veri_cek(textBox3.Text);
        }
    }
}
