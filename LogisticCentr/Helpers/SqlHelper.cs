using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticCentr.Helpers
{
    public static class SqlHelper
    {
        /// <summary>
        /// Оборачивает строку в sql like pattern
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="str"></param>
        /// <param name="isSearchNullable">Флаг отвечает, за то учитывать null при поиске или нет(тру учитывать, фалс нет)</param>
        /// <returns></returns>
        public static string GetStringLikePattern(string columnName, string str, string orAnd = "", bool isSearchNullable = true)
        {
            if (!isSearchNullable && string.IsNullOrEmpty(str))
                return "";

            return string.IsNullOrEmpty(str)
                ? $"{orAnd} ({columnName} like '%%' or {columnName} is NULL )" 
                : $"{orAnd} {columnName} like '%{str}%'";
        }

        /// <summary>
        /// Преобразование даты с По в запрос битвин
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="orAnd"></param>
        /// <param name="formatDate"></param>
        /// <returns></returns>
        public static string GetDateBetweenPattern(string columnName, DateTime dateFrom, DateTime dateTo, string orAnd = "", string formatDate = "yyyy/MM/dd hh:mm:ss")
        {
            return $" {orAnd} {columnName} between '{dateFrom.ToString(formatDate)}' and '{dateTo.ToString(formatDate)}' ";

        }

        /// <summary>
        /// Возвращает запрос лайк, если вал пустой или запрос equal, если val не пустой
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="val">Это какое то число</param>
        /// <param name="isInt">тру означает INT, false означает string</param>
        /// <returns></returns>
        public static string GetStringEqualOrLikeForINT(string columnName, string val)
        {
            return string.IsNullOrEmpty(val) ? $"{GetStringLikePattern(columnName, val)}" : $"{columnName} = {val}";  
        }

        /// <summary>
        /// Обновляет видимы данные в датаГРиде
        /// </summary>
        public static void UpdateSelectViewData(SqlDataAdapter adapter, DataSet ds, string sqlQuery, SqlConnection connection)
        {
            adapter.SelectCommand = new SqlCommand(sqlQuery, connection);

            ds.Clear();
            adapter.Fill(ds);
        }

        /// <summary>
        /// позволяет передать Action и выполнить его в SqlConnection
        /// </summary>
        public static void ActionWorkWithSqlConnection(Action<SqlConnection> callback)
        {
            var connectionString = GetCon();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                callback(connection);
            }
        }

        /// <summary>
        /// Получение строки подключения к БД
        /// </summary>
        /// <returns></returns>
        public static string GetCon()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}
