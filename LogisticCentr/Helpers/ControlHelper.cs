using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LogisticCentr.Helpers
{
    public static class ControlHelper
    {
        public static void FillCombobox(Dictionary<string, ComboBox> comboBoxWithColumnNames, string sql, SqlConnection connection)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                foreach(var cb in comboBoxWithColumnNames)
                {
                    cb.Value.Items.Add(row[cb.Key].ToString());
                }
            }
        }
    }
}
