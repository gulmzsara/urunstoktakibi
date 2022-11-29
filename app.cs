using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace stok_takip
{
    class app
    {
        public void connect()
        {
            SqlConnection connect = new SqlConnection(@"Data Source=.;Initial Catalog = stok; Integrated Security=True");
        }


    }
}
