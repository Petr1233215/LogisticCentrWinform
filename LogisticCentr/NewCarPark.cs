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
    public partial class NewCarPark : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LogicCentr; Integrated Security=True;";
        string sqlMain = "SELECT * FROM cars_park";

        public NewCarPark()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlMain, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns["id_car"].ReadOnly = true;
            }
        }

        private void NewCarPark_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// кнопка добавления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);
        }

        /// <summary>
        /// кнопка удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // удаляем выделенные строки из dataGridView1
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        /// <summary>
        /// сохраняем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = new SqlCommand("sp_CreateCarPark", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@brand", SqlDbType.NVarChar, 50, "brand"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@year_issue", SqlDbType.Int, 0, "year_issue"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@tonnage", SqlDbType.Int, 0, "tonnage"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@state_number", SqlDbType.NVarChar, 10, "state_number"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@body_type", SqlDbType.NVarChar, 100000, "body_type"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_car", SqlDbType.Int, 0, "id_car");
                parameter.Direction = ParameterDirection.Output;

                adapter.Update(ds);
            }
        }

        /// <summary>
        /// Filtr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            //string sqlStr = $"SELECT * FROM cars_park Where id_car like {SqlHelper.GetStringLikePattern(textBox1.Text)} or brand like {SqlHelper.GetStringLikePattern(textBox1.Text)}";

            string sql = $"SELECT * FROM cars_park Where {GetFilterFromTextBox()} and ({GetSearchFilterTextBox()})";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                adapter.SelectCommand = new SqlCommand(sql, connection);

                ds.Clear();
                adapter.Fill(ds);
            }
        }


        /// <summary>
        /// назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            MainForm main = new MainForm();
            main.Show();
            this.Hide();
        }

        /// <summary>
        /// Clear textBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBoxBrand.Clear();
            textBoxIdCar.Clear();
            textBoxStateNumber.Clear();
            textBoxTypebody.Clear();
            textBoxYear.Clear();
            textBoxBrand.Clear();
            textBoxtonnage.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                adapter.SelectCommand = new SqlCommand(sqlMain, connection);

                ds.Clear();
                adapter.Fill(ds);
            }
        }

        /// <summary>
        /// Возвращает составной запрос на фильтры
        /// </summary>
        /// <returns></returns>
        private string GetFilterFromTextBox()
        {
            return $"{SqlHelper.GetStringLikePattern("brand", textBoxBrand.Text)} and {SqlHelper.GetStringEqualOrLikeForINT("year_issue", textBoxYear.Text)} " +
                $"and {SqlHelper.GetStringEqualOrLikeForINT("tonnage", textBoxtonnage.Text)} and {SqlHelper.GetStringLikePattern("state_number", textBoxStateNumber.Text)} " +
                $"and {SqlHelper.GetStringLikePattern("body_type", textBoxTypebody.Text)} and {SqlHelper.GetStringEqualOrLikeForINT("id_car", textBoxIdCar.Text)}";
        }

        private string GetSearchFilterTextBox() 
        {
             return $"{SqlHelper.GetStringLikePattern("brand", textBox1.Text)} or {SqlHelper.GetStringLikePattern("year_issue", textBox1.Text)} " +
                $"or {SqlHelper.GetStringLikePattern("tonnage", textBox1.Text)} or {SqlHelper.GetStringLikePattern("state_number", textBox1.Text)} " +
                $"or {SqlHelper.GetStringLikePattern("body_type", textBox1.Text)} or {SqlHelper.GetStringLikePattern("id_car", textBox1.Text)}";
        }
    }
}
