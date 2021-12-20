using LogisticCentr.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogisticCentr
{
    public partial class CarPark : Form
    {
        public CarPark()
        {
            InitializeComponent();
        }

        private void CarPark_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'logicCentrDataSet.cars_park' table. You can move, or remove it, as needed.
            this.cars_parkTableAdapter.Fill(this.logicCentrDataSet.cars_park);

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Validate();

            carsparkBindingSource.EndEdit();

            tableAdapterManager.UpdateAll(logicCentrDataSet);
        }

        private void bindingNavigatorDeleteItem1_Click(object sender, EventArgs e)
        {
            carsparkBindingSource.EndEdit();
            carsparkBindingSource.RemoveCurrent();
            tableAdapterManager.UpdateAll(logicCentrDataSet);
        }

        private void cars_parkDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cars_parkDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if(e.Exception.GetType().ToString().Equals("System.FormatException"))
            {
                //Таким образом будем обрабатывать неверные зн-ия
                if(sender is DataGridView dg)
                {
                    var n = dg.Columns[e.ColumnIndex].DataPropertyName;
                }
                MessageBox.Show("FormatException");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LogicCentr; Integrated Security=True;";
            //string sql = $"SELECT * FROM cars_park Where id_car like {SqlHelper.GetStringLikePattern(textBox1.Text)} or brand like {SqlHelper.GetStringLikePattern(textBox1.Text)}";
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();

            //    //SqlCommand command = new SqlCommand(sql, connection);
            //    //SqlDataReader reader = command.ExecuteReader();

            //    SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, connection);

            //    //tableAdapterManager.(logicCentrDataSet);
            //    tableAdapterManager.UpdateAll(logicCentrDataSet);
            //    //reader.Close();
            //}
        }
    }
}
