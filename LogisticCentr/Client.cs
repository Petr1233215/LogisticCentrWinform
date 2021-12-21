using LogisticCentr.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LogisticCentr
{
    public partial class Client : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = SqlHelper.GetCon();
        string sqlMain = "SELECT * FROM clients";

        public Client()
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
            dataGridView1.Columns["id_client"].ReadOnly = true;

            dataGridView1.Columns["id_client"].HeaderText = "Код клиента";
            dataGridView1.Columns["name_client"].HeaderText = "Название клиента(организации)";
            dataGridView1.Columns["contact_person"].HeaderText = "Контактное лицо";
            dataGridView1.Columns["phone"].HeaderText = "Телефон";
            dataGridView1.Columns["email"].HeaderText = "Эл. почта";
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
                    default:
                        MessageBox.Show($"Ошибка в столбце: {nameColumn}" +
                            $"\n" + e.Errors.Message);
                        return;
                }
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
                adapter.InsertCommand = new SqlCommand("sp_CreateClient", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@name_client", SqlDbType.NVarChar, 255, "name_client"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@contact_person", SqlDbType.NVarChar, 255, "contact_person"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 50, "phone"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 255, "email"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_client", SqlDbType.Int, 0, "id_client");
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
        private void button4_Click(object sender, EventArgs e)
        {
            SqlHelper.ActionWorkWithSqlConnection((con) => SqlHelper.UpdateSelectViewData(adapter, ds, sqlMain, con));
        }

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
            if(nameColumn == "phone")
            {
                ValidateHelper.HandleCheckDigit(e);
            }
        }
    }
}
