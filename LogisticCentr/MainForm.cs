using System;
using System.Data.SqlClient;
using System.Windows.Forms;

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
