using LogisticCentr.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LogisticCentr
{
    public partial class Route : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = SqlHelper.GetCon();
        string sqlMain = "SELECT * FROM routes";

        public Route()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlMain, connection);
                adapter.RowUpdated += new SqlRowUpdatedEventHandler(onUpdate);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

                SetSettingsDataGrid();
            }
        }

        /// <summary>
        /// Преднастройка таблицы
        /// </summary>
        private void SetSettingsDataGrid()
        {
            dataGridView1.Columns["id_route"].ReadOnly = true;

            dataGridView1.Columns["id_route"].HeaderText = "Код маршрута";
            dataGridView1.Columns["distance"].HeaderText = "Расстояние";
            dataGridView1.Columns["name_route"].HeaderText = "Название маршрута";
            dataGridView1.Columns["price"].HeaderText = "Стоимость маршрута";
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
                string err = ValidateHelper.CheckValuesItemArrayForNull(e.Row, dataGridView1.Columns, new string[] { "distance", "name_route", "price" });


                if (string.IsNullOrEmpty(err))
                    err = e.Errors.Message;
                    
                MessageBox.Show(err);

            }
        }

        /// <summary>
        /// Назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        /// <summary>
        /// кнопка добавления
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);
        }

        /// <summary>
        /// Кнопка сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = new SqlCommand("sp_CreateRoute", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@distance", SqlDbType.Int, 0, "distance"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@name_route", SqlDbType.NText, 0, "name_route"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Money, 0, "price"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_route", SqlDbType.Int, 0, "id_route");
                parameter.Direction = ParameterDirection.Output;

                try
                {
                    adapter.Update(ds);
                }
                catch (Exception ex) { }
            }
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
        /// обновить данные из БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            SqlHelper.UpdateSelectViewData(adapter, ds, sqlMain);
        }

        /// <summary>
        /// -= сделано для того чтобы метод не вызвался n-ое число раз, т.к. событие это по сути список ссылок на методов, 
        /// а их может быть множество, поэтому сначала метод удаляется из события, а затем снова добавляется
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            dataGridView1.EditingControl.KeyPress -= EditingControl_KeyPress;
            dataGridView1.EditingControl.KeyPress += EditingControl_KeyPress;
        }

        /// <summary>
        /// обработчик нажатия клавиж при вводе в ячейках таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            var nameColumn = this.dataGridView1.CurrentCell.OwningColumn.Name;
            if (nameColumn == "distance")
            {
                ValidateHelper.HandleCheckDigit(e);
            }
            else if(nameColumn == "price")
            {
                Control editingControl = (Control)sender;
                ValidateHelper.HandleTypeMoney(e, editingControl.Text);
            }
        }
    }
}
