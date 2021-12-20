using LogisticCentr.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogisticCentr
{
    public partial class Drivers : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string sqlMain = "SELECT * FROM drivers";

        public Drivers()
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

        private void SetSettingsDataGrid()
        {
            dataGridView1.Columns["id_driver"].ReadOnly = true;
            dataGridView1.Columns["birth_date"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dataGridView1.Columns["person_hired"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";

            dataGridView1.Columns["id_driver"].HeaderText = "Код водителя";
            dataGridView1.Columns["first_name"].HeaderText = "Имя";
            dataGridView1.Columns["second_name"].HeaderText = "Фамилия";
            dataGridView1.Columns["last_name"].HeaderText = "Отчество";
            dataGridView1.Columns["birth_date"].HeaderText = "Дата рождения";
            dataGridView1.Columns["person_hired"].HeaderText = "Дата приема на работу";
        }

        /// <summary>
        /// Назад
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.Show();
            this.Close();
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
        /// кнопка добавления
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.InsertCommand = new SqlCommand("sp_CreateDriver", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@first_name", SqlDbType.NVarChar, 255, "first_name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@second_name", SqlDbType.NVarChar, 255, "second_name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@last_name", SqlDbType.NVarChar, 255, "last_name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@birth_date", SqlDbType.DateTime, 0, "birth_date"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@person_hired", SqlDbType.DateTime, 0, "person_hired"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_driver", SqlDbType.Int, 0, "id_driver");
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
    }
}
