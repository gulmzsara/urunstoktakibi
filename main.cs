using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;

namespace stok_takip
{
    public partial class anaSayfa : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=.;Initial Catalog = stok; Integrated Security=True");

        public anaSayfa()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form2 frm = new Form2();
            frm.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            stokgiriscıkıs frm = new stokgiriscıkıs();
            frm.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            excelaktar();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        void excelaktar()
        {
            string query = "select p.name as 'Ürün Adı',p.seri_no as 'Seri No',p.barcode as 'Barkod',IIF(a.action=0,'Çıkış','Giriş') as 'İşlem',a.date as 'İşlem Tarihi',a.quantity as 'Adet' from action a left join product p on  p.id =a.product_id ";
            //bunu her yere yazdım çünkü her işlemde datagridview yenileniyor. 
            connect.Open();
            SqlCommand cmd = new SqlCommand(query, connect);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            // single worksheet
            Excel._Worksheet workSheet = excelApp.ActiveSheet;

            // column headings
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                workSheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
            }

            // rows
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                // to do: format datetime values before printing
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    workSheet.Cells[i + 2, j + 1] = dt.Rows[i][j];
                }
            }


            string folderPath = "";
            FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
            if (directchoosedlg.ShowDialog() == DialogResult.OK)
            {
                folderPath = directchoosedlg.SelectedPath;
            }
            workSheet.SaveAs(folderPath+"\\__yedek"+".xlsx");
            excelApp.Quit();

            connect.Close();
        }
        
    

    }
}
