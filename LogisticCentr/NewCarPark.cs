using LogisticCentr.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LogisticCentr
{
    public partial class NewCarPark : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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
                adapter.RowUpdated += new SqlRowUpdatedEventHandler(onUpdate);
                //adapter.RowUpdating += new SqlRowUpdatingEventHandler(onUpdating);
                //adapter.FillError += new FillErrorEventHandler(tt);
                
                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                SetSettingsDataGrid();
            }
        }

        private void SetSettingsDataGrid()
        {
            dataGridView1.Columns["id_car"].ReadOnly = true;
            dataGridView1.Columns["id_car"].HeaderText = "Код машины";
            dataGridView1.Columns["brand"].HeaderText = "Название марки";
            dataGridView1.Columns["year_issue"].HeaderText = "Год выпуска";
            dataGridView1.Columns["tonnage"].HeaderText = "Вместимость(тонн)";
            dataGridView1.Columns["state_number"].HeaderText = "Гос. номер";
            dataGridView1.Columns["body_type"].HeaderText = "Тип кузова";
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
                //adapter = new SqlDataAdapter(sqlMain, connection);
                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
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

                try
                {
                    adapter.Update(ds);
                }
                catch (Exception ex){}
            }
        }

        /// <summary>
        /// Обработчик обновления данных в DATASET'e используется для отлова ошибок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onUpdate(object sender, SqlRowUpdatedEventArgs e)
        {
            if (e.Errors != null)
            {
                var nameColumn = dataGridView1.CurrentCell.OwningColumn.HeaderText;
                switch (nameColumn)
                {
                    case "year_issue":
                        MessageBox.Show($"Значение не должно быть пустым. Значение должно быть целочисленным и не должно превышать {int.MaxValue}" +
                            $"\n{e.Errors.Message}");
                        return;
                    default:
                        MessageBox.Show($"Ошибка в столбце: {nameColumn}" +
                            $"\n" + e.Errors.Message);
                        return;
                }
            }
        }

        //private void onUpdating(object sender, SqlRowUpdatingEventArgs e)
        //{
        //    if (e.Errors != null)
        //    {
        //        MessageBox.Show(e.Errors.Message);
        //        MessageBox.Show("At row with key=" + e.Row["somecolumntoidentifytherow"]);
        //    }
        //}

        /// <summary>
        /// Filtr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            //ToDo сделать валидацию полей фильтров

            string sql = $"SELECT * FROM cars_park Where {GetFilterFromTextBox()} and ({GetSearchFilterTextBox()})";
            SqlHelper.UpdateSelectViewData(adapter, ds, sql);
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
            this.Close();
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

            SqlHelper.UpdateSelectViewData(adapter, ds, sqlMain);
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

        private void button7_Click(object sender, EventArgs e)
        {
            SqlHelper.UpdateSelectViewData(adapter, ds, sqlMain);
        }
    }
}
