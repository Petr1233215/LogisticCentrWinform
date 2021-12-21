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
    public partial class Transportation : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        SqlCommandBuilder commandBuilder;
        string connectionString = SqlHelper.GetCon();
        string sqlMainQueryView = @"SELECT        transportation.id_transportation, transportation.id_route, routes.name_route, transportation.id_logist, logistic.first_name + ' ' + logistic.second_name + ' ' + COALESCE(logistic.last_name, '') as name_logistic, 
        						transportation.id_client, clients.name_client, transportation.id_driver, drivers.first_name + ' ' + drivers.second_name + ' ' + COALESCE(drivers.last_name, '') AS name_driver, 
                                 transportation.id_car, cars_park.brand, transportation.point_shipment, transportation.point_arrival,
        						 transportation.datetime_shipment, transportation.datetime_arrival
        FROM            transportation INNER JOIN
                                 cars_park ON transportation.id_car = cars_park.id_car INNER JOIN
                                 clients ON transportation.id_client = clients.id_client INNER JOIN
                                 drivers ON transportation.id_driver = drivers.id_driver INNER JOIN
                                 logistic ON transportation.id_logist = logistic.id_logist INNER JOIN
                                 routes ON transportation.id_route = routes.id_route ";

        string sqlMain = @"SELECT * FROM transportation";

        public Transportation()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sqlMainQueryView, connection);
                adapter.RowUpdated += new SqlRowUpdatedEventHandler(onUpdate);

                ds = new DataSet();
                adapter.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];

                SetSettingsDataGrid();


                SqlDataAdapter ad2 = new SqlDataAdapter("SELECT * FROM cars_park", connection);
                DataSet ds2 = new DataSet();
                ad2.Fill(ds2);
                comboBox1.DataSource = ds2;
                comboBox1.DisplayMember = "brand";
                //comboBox1.ValueMember = "id_transportation";
            }
        }

        /// <summary>
        /// Преднастройка таблицы
        /// </summary>
        private void SetSettingsDataGrid()
        {
            dataGridView1.Columns["id_transportation"].ReadOnly = true;
            dataGridView1.Columns["id_route"].ReadOnly = true;
            dataGridView1.Columns["id_logist"].ReadOnly = true;
            dataGridView1.Columns["id_client"].ReadOnly = true;
            dataGridView1.Columns["id_driver"].ReadOnly = true;
            dataGridView1.Columns["id_car"].ReadOnly = true;

            dataGridView1.Columns["id_transportation"].HeaderText = "Код перевозки";
            dataGridView1.Columns["id_route"].HeaderText = "Код маршрута";
            dataGridView1.Columns["id_logist"].HeaderText = "Код логиста";
            dataGridView1.Columns["id_client"].HeaderText = "Код клиента";
            dataGridView1.Columns["id_driver"].HeaderText = "Код водитея";
            dataGridView1.Columns["id_car"].HeaderText = "Код машины";

            dataGridView1.Columns["name_route"].HeaderText = "Название маршрута";
            dataGridView1.Columns["name_logistic"].HeaderText = "Имя логиста";
            dataGridView1.Columns["name_client"].HeaderText = "Имя клиента";
            dataGridView1.Columns["name_driver"].HeaderText = "Имя водителя";
            dataGridView1.Columns["brand"].HeaderText = "Название марки авто";
            dataGridView1.Columns["point_shipment"].HeaderText = "Пункт отправления";
            dataGridView1.Columns["point_arrival"].HeaderText = "Пункт прибытия";
            dataGridView1.Columns["datetime_shipment"].HeaderText = "Дата и время отправки";
            dataGridView1.Columns["datetime_arrival"].HeaderText = "Дата и время прибытия";
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
                string err = ValidateHelper.CheckValuesItemArrayForNull(e.Row, dataGridView1.Columns, new string[] { });

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
        /// обновить данные из БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            SqlHelper.ActionWorkWithSqlConnection((con) => SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, con));
        }

        /// <summary>
        /// кнопка удаления
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("Выберите элементы для удаления");

            var res = MessageBox.Show("Вы действительно хотите удалить выбранные элементы?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    dataGridView1.Rows.Remove(row);

                SaveData();
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
            else if (nameColumn == "price")
            {
                Control editingControl = (Control)sender;
                ValidateHelper.HandleTypeMoney(e, editingControl.Text);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            dataGridView1.EditingControl.KeyPress -= EditingControl_KeyPress;
            dataGridView1.EditingControl.KeyPress += EditingControl_KeyPress;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                adapter.InsertCommand = new SqlCommand("sp_CreateTransportation", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_route", 1));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_logist", 1));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_client", 1));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_driver", 3));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_car", 2));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@point_shipment", "dsf"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@point_arrival", "sdf"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@datetime_shipment", DateTime.Now));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@datetime_arrival", DateTime.Now));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_transportation", SqlDbType.Int, 0, "id_transportation");
                parameter.Direction = ParameterDirection.Output;

                adapter.Update(ds);

                SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, connection);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);

                var row = ds.Tables[0].Rows[dataGridView1.CurrentRow.Index];
                row["point_shipment"] = Guid.NewGuid().ToString();

                try
                {
                    adapter.Update(ds);
                    //SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, connection);
                }
                catch (Exception ex)
                {//Сделать логирование 
                    MessageBox.Show("123");
                }
            }
        }

        /// <summary>
        /// Сохраняем данные в БД
        /// </summary>
        private void SaveData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);

                try
                {
                    adapter.Update(ds);
                    SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, connection);
                }
                catch (Exception ex) { }
            }
        }
    }
}
