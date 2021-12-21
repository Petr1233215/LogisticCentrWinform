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

                SetComboBoxes(connection, ds.Tables[0].Rows.Count);

                SetDateTimePicker();
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
        /// Заполняем КомбоБоксы значениями
        /// </summary>
        /// <param name="connection"></param>
        private void SetComboBoxes(SqlConnection connection, int rowsCount)
        {
            if (rowsCount <= 0)
                return;

            ControlHelper.FillCombobox(new Dictionary<string, ComboBox>() { { "brand", comboBoxCarParkView }, { "id_car", comboBoxCarParkHideId } },
                                        "SELECT * FROM cars_park", connection);

            ControlHelper.FillCombobox(new Dictionary<string, ComboBox>() { { "name_logistic", comboBoxLogistView }, { "id_logist", comboBoxLogistId } },
                "SELECT id_logist, first_name + ' ' + second_name + ' ' + COALESCE(last_name, '') AS name_logistic FROM logistic", connection);

            ControlHelper.FillCombobox(new Dictionary<string, ComboBox>() { { "name_client", comboBoxClientView }, { "id_client", comboBoxClientId } },
                "SELECT * FROM clients", connection);

            ControlHelper.FillCombobox(new Dictionary<string, ComboBox>() { { "name_route", comboBoxRouteView }, { "id_route", comboBoxRouteId } },
                "SELECT * FROM routes", connection);

            ControlHelper.FillCombobox(new Dictionary<string, ComboBox>() { { "driver_name", comboBoxDriverView }, { "id_driver", comboBoxDriverId } },
                "SELECT id_driver, first_name + ' ' + second_name + ' ' + COALESCE(last_name, '') AS driver_name FROM drivers", connection);

            //Выбираем первое зн-ие в списках, чтобы долго не прокликивать
            comboBoxCarParkView.SelectedIndex = 0;
            comboBoxLogistView.SelectedIndex = 0;
            comboBoxClientView.SelectedIndex = 0;
            comboBoxRouteView.SelectedIndex = 0;
            comboBoxDriverView.SelectedIndex = 0;
        }

        private void SetDateTimePicker()
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "MM/dd/yyyy hh:mm:ss";
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
                string err = ValidateHelper.CheckValuesItemArrayForNull(e.Row, dataGridView1.Columns, null);

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
        /// Вернет Тру, если есть пустые текст боксы
        /// </summary>
        /// <returns></returns>
        private bool CheckTextBoxesValueNull()
        {
            var err = ValidateHelper.CheckStringNotNullOrEmpty(new Dictionary<string, string> {
                {label6.Text, textBox1.Text },
                {label7.Text, textBox2.Text },
            });

            if(!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(err);
                return true;
            }
            return false;
        }

        /// <summary>
        /// кнопка добавления
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckTextBoxesValueNull())
                return;

            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                adapter.InsertCommand = new SqlCommand("sp_CreateTransportation", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_route", comboBoxRouteId.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_logist", comboBoxLogistId.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_client", comboBoxClientId.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_driver", comboBoxDriverId.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@id_car", comboBoxCarParkHideId.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@point_shipment", textBox1.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@point_arrival", textBox2.Text));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@datetime_shipment", dateTimePicker1.Value));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@datetime_arrival", dateTimePicker2.Value));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@id_transportation", SqlDbType.Int, 0, "id_transportation");
                parameter.Direction = ParameterDirection.Output;

                adapter.Update(ds);
                SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, connection);

                //После добавления очищакем выбранные строки, скролим вниз и выбираем последнюю добавленную
                dataGridView1.ClearSelection();
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                dataGridView1.Rows[dataGridView1.RowCount - 1].Selected = true;
            }
        }

        /// <summary>
        /// Изменить данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (CheckTextBoxesValueNull())
                return;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                adapter.SelectCommand = new SqlCommand(sqlMain, connection);
                commandBuilder = new SqlCommandBuilder(adapter);

                var row = ds.Tables[0].Rows[dataGridView1.CurrentRow.Index];

                row["id_car"] = comboBoxCarParkHideId.Text;
                row["id_route"] = comboBoxRouteId.Text;
                row["id_client"] = comboBoxClientId.Text;
                row["id_driver"] = comboBoxDriverId.Text;
                row["id_logist"] = comboBoxLogistId.Text;
                row["point_shipment"] = textBox1.Text;
                row["point_arrival"] = textBox2.Text;
                row["datetime_shipment"] = dateTimePicker1.Value;
                row["datetime_arrival"] = dateTimePicker2.Value;

                try
                {
                    adapter.Update(ds);
                    SqlHelper.UpdateSelectViewData(adapter, ds, sqlMainQueryView, connection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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

        private void comboBoxCarParkView_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxCarParkHideId.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        private void comboBoxDriverView_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxDriverId.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        private void comboBoxClientView_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxClientId.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        private void comboBoxLogistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxLogistId.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        private void comboBoxRouteView_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxRouteId.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        /// <summary>
        /// Событие обработки фокуса на строке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var row = ds.Tables[0].Rows[e.RowIndex];

            comboBoxCarParkView.SelectedIndex = comboBoxCarParkHideId.SelectedIndex = comboBoxCarParkHideId.FindStringExact(row["id_car"].ToString());

            comboBoxRouteView.SelectedIndex = comboBoxRouteId.SelectedIndex = comboBoxRouteId.FindStringExact(row["id_route"].ToString());

            comboBoxClientView.SelectedIndex = comboBoxClientId.SelectedIndex = comboBoxClientId.FindStringExact(row["id_client"].ToString());

            comboBoxDriverView.SelectedIndex = comboBoxDriverId.SelectedIndex = comboBoxDriverId.FindStringExact(row["id_driver"].ToString());

            comboBoxLogistView.SelectedIndex = comboBoxLogistId.SelectedIndex = comboBoxLogistId.FindStringExact(row["id_logist"].ToString());

            textBox1.Text = row["point_shipment"].ToString();
            textBox2.Text = row["point_arrival"].ToString();

            dateTimePicker1.Value = DateTime.TryParse(row["datetime_shipment"].ToString(), out DateTime dt) ? dt : DateTime.Now;
            dateTimePicker2.Value = DateTime.TryParse(row["datetime_arrival"].ToString(), out DateTime dt2) ? dt2 : DateTime.Now;
        }
    }
}
