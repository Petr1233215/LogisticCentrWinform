using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LogisticCentr
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=LogicCentr; Integrated Security=True;";
            string sqlExpression = "SELECT * FROM cars_park";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    //Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2));

                    while (reader.Read()) // построчно считываем данные
                    {
                        object id = reader.GetValue(0);
                        object name = reader.GetValue(1);
                        object age = reader.GetValue(2);

                        //Console.WriteLine("{0} \t{1} \t{2}", id, name, age);
                    }
                }

                reader.Close();
            }


            NewCarPark carPark = new NewCarPark();
            carPark.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Driver drivers = new Driver();
            drivers.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var client = new Client();
            client.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var logist = new Logistic();
            logist.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var route = new Route();
            route.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var trans = new Transportation();
            trans.Show();
            this.Hide();
        }
    }
}
