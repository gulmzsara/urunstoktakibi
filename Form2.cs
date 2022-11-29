using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Configuration;


namespace stok_takip
{
    public partial class Form2 : Form
    {

        #region değişken tanımlamaları
        SqlConnection connect = new SqlConnection(@"Data Source=.;Initial Catalog = stok; Integrated Security=True");
        private int id = 0;
        private string resimyolu = "";
        private string yeniresimyolu = "";
        private string dbresimyolu = "";
        private string projeKlasoru = "";

        //buradaki değişkenleri her yerde kullanıyoruz, içini de boş yapıyoruz yani "" yapıyoruz.
        //çoğu kodda if in içinde "" değerine eşitse diye seçenek var boşsa işlem yapmıyor. 
        //private özel demek, yani bu dosyanın yani bu class dışında bi yerde kullanılamaz 
        #endregion
        public Form2()
        {
            InitializeComponent();
        }
        
        private void Form2_Load(object sender, EventArgs e)
        {

            projeKlasoru = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int donendeger_Baslangic = projeKlasoru.IndexOf("\\bin\\");
            int donendeger_Bitis = projeKlasoru.Length - donendeger_Baslangic;
            projeKlasoru = projeKlasoru.Remove((donendeger_Baslangic), (donendeger_Bitis));
            //projenin bulunduğu klasörü alıyoruz.
            //bin\debug klasörüne yönlendiriyo ama biz o kısmını siliyoruz sonrasında resimleri ekliyoruz.
            //bin debug klasöründe exe dosyası var bunun o yüzden orayı açıyo.

            veri_cek("");

        }
        
        private void button4_Click(object sender, EventArgs e)
        {


            // burası insert kodu 
            connect.Open();

       
            String query = "INSERT INTO product (name,seri_no,barcode,brand,description,create_date,image) VALUES (@name,@seri_no,@barcode,@brand,@description,@create_date,@image)";

            SqlCommand command = new SqlCommand(query,connect);

            command.Parameters.AddWithValue("@name", textBox1.Text);
            command.Parameters.AddWithValue("@seri_no", textBox3.Text);
            command.Parameters.AddWithValue("@barcode", textBox4.Text);
            command.Parameters.AddWithValue("@brand", textBox2.Text);
            command.Parameters.AddWithValue("@description", "");
            command.Parameters.AddWithValue("@create_date", DateTime.Now);
            command.Parameters.AddWithValue("@image", dbresimyolu);

            //resim ekleme yeri burası da
            resimkaydet();


            command.ExecuteNonQuery();
            connect.Close();
            MessageBox.Show("Başarılı! Ürün başarıyla eklendi!","İşlem Sonucu");

            veri_cek("");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //resim yolu vs burada doluyor. 
            //burası karışık biraz

            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            //sadece bu dosya türleri görüntülensin istiyoruz o yüzden


            resimyolu = openFileDialog1.FileName;
            dbresimyolu = "\\resimler\\" + Path.GetFileName(openFileDialog1.FileName);
            yeniresimyolu = projeKlasoru + dbresimyolu;
            //dbye resimler klasörünün altında gibi kaydediyoruz ama gösterirken proje klasörünü alıyoruz
            resimgetir(openFileDialog1.FileName);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            // geri butonu
            this.Hide();
            anaSayfa sayfa = new anaSayfa();
            sayfa.Show();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ///burası da datagridde seçtiğimiz satırdaki ürünün id vs bilgisi.
            id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            textBox1.Text = dataGridView1.CurrentRow.Cells["Ürün Adı"].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells["Seri No"].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells["Barkod"].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells["Marka"].Value.ToString();
            //textboxlara bilgileri getiriyo seçince
            connect.Open();
            SqlCommand cmd = new SqlCommand("select image from product where id="+id,connect);
            SqlDataReader okuyucu = cmd.ExecuteReader();
            //burdaki sorguda sadece o idye ait olan kaydın resmini getiriyo
            while (okuyucu.Read())
            {
                dbresimyolu = okuyucu["image"].ToString();
            }
                if (dbresimyolu != null && dbresimyolu != "")
                    resimgetir(projeKlasoru + dbresimyolu);//resimgetir fonksiyonunu anlatmıştım
                else
                    resimgetir("");//resim yoksa eğer "" koyuyoruz bak boş getiriyo o zaman
            okuyucu.Close();
            connect.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            if(id!=0)
            {
                //burası update kodu

                string query = "update product set ";
                query += "name='" + textBox1.Text + "', ";
                query += "brand='" + textBox2.Text + "', ";
                query += "barcode='" + textBox3.Text + "', ";
                query += "seri_no='" + textBox4.Text + "', ";
                query += "image='" + dbresimyolu + "'";
                query += "where id=" + id;
                //burada resmi kaydediyo ama resimyolu ve yeniresimyolu değişkenleri başka yerde doluyo
                //
                resimkaydet();


                connect.Open();
                SqlCommand cmd = new SqlCommand(query, connect);
                cmd.ExecuteNonQuery();
                connect.Close();
                MessageBox.Show("Başarılı! Ürün bilgisi güncellendi!", "İşlem Sonucu");

            }
            veri_cek("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(id!=0)//id yi yukarda 0 yaptık, seçili değilse bişey 0 kalcak 0 sa silemeyecek.
            {
            DialogResult dialogResult = MessageBox.Show("Uyarı!", "Ürünü Siliyor musunuz?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
            string query = "delete from product where id=" + id;
            connect.Open();
            SqlCommand cmd = new SqlCommand(query, connect);
            cmd.ExecuteNonQuery();
            connect.Close();
            MessageBox.Show( "İşlem Sonucu", "Başarılı! Ürün başarıyla silindi!");
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
            veri_cek("");

            }

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            veri_cek(textBox5.Text);
            //her yazı değiştiğinde o kelimeyi aratan yer, bak sadece 1 satır kod ile yapıyoruz :)
        }


        #region bizim yazdığımız fonksiyonlar

        void resimkaydet()
        {
            if (resimyolu != "")
                if (File.Exists(yeniresimyolu) == false)//dosya var mı diye kontrol ediyo varsa kaydetmiyor
                    File.Copy(resimyolu, yeniresimyolu);
        }
        void resimgetir(string resimyolu)
        {
            //burda resmi 100px genişliğine göre ayarlayıp gösteriyor.
            //her tıkladığında bunu çalıştırıyor ve resim değişiyor

            int w, h, new_w, new_h;
            if (resimyolu != "")
            {

                Bitmap bmp = new Bitmap(resimyolu);
                w = bmp.Width;
                h = bmp.Height;
                new_w = w / 100;
                new_h = h / new_w;
                Size s = new Size(100, new_h);
                pictureBox1.Image = new Bitmap(bmp, s);
                pictureBox1.Size = s;
            }
            else
            {
                pictureBox1.Image = null;
            }
        }
        void veri_cek(string kelime)
        {
            //bunu her yere yazdım çünkü her işlemde datagridview yenileniyor. 
            connect.Open();
            string where_sartlari = " where name like '%" + kelime + "%' or barcode like '%" + kelime + "%' or seri_no like '%" + kelime + "%'";
            SqlCommand cmd = new SqlCommand("select id as ID,barcode as 'Barkod',seri_no as 'Seri No',brand as 'Marka', name 'Ürün Adı',create_date as 'Kayıt Tarihi' from product" + where_sartlari, connect);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

            connect.Close();

        }
        #endregion

    }
}
