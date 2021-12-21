using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogisticCentr.Helpers
{
    public static class ValidateHelper
    {
        /// <summary>
        /// Проверяем является ли символ числом
        /// </summary>
        /// <returns></returns>
        public static bool IsDigitOrBackspace(KeyPressEventArgs e)
        {
            return char.IsDigit(e.KeyChar) || IsBackSpace(e);
        }

        public static bool IsBackSpace(KeyPressEventArgs e)
        {
            return e.KeyChar == (char)Keys.Back;
        }

        public static bool IsMoneyTypeOrBackspace(KeyPressEventArgs e, string textVal)
        {
            string pattern = @"^[0-9]{1,15}([,][0-9]{1,4})?$";

            string val = e.KeyChar.ToString();

            //Для того чтобы валидация прошла корректно, если пользователь вводит ,
            if (val == ",")
                val += 0;

            return IsBackSpace(e) || Regex.IsMatch(textVal + val, pattern);
        }

        /// <summary>
        /// Метод обрабатывает, действие если клавиша не цифра
        /// </summary>
        /// <param name="e"></param>
        public static void HandleCheckDigit(KeyPressEventArgs e)
        {
            if (!IsDigitOrBackspace(e))
            {
                MessageBox.Show("Вы не можете вводить ничего кроме цифр");
                e.Handled = true;
            }
        }

        /// <summary>
        /// Метод обрабатывает, действие если клавиша не цифра
        /// </summary>
        /// <param name="e"></param>
        public static void HandleTypeMoney(KeyPressEventArgs e, string textVal)
        {
            if (!IsMoneyTypeOrBackspace(e, textVal))
            {
                MessageBox.Show("Вы можете сюда вводить только денежный тип, например: 555,3443 ;  999993,954; 98999");
                e.Handled = true;
            }
        }

        /// <summary>
        /// Валидация значений строки на null
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnCollection"></param>
        /// <param name="arrayNameColumnNotCanNull">список названий столбцов, которые не могут быть null</param>
        /// <returns></returns>
        public static string CheckValuesItemArrayForNull(DataRow row, DataGridViewColumnCollection columnCollection, string[] arrayNameColumnNotCanNull)
        {
            string err = "";

            if(arrayNameColumnNotCanNull != null)
            {
                foreach (var name in arrayNameColumnNotCanNull)
                {
                    if (row[name] == DBNull.Value)
                        err += $"Значение в столбце: {columnCollection[name].HeaderText} не может быть пустым.\n";
                }
            }

            return err;
        }

        /// <summary>
        /// Проверяет значение на null or empty
        /// </summary>
        /// <param name="nameValueField"> ключоч является имя поля, а значение, - это зн-ие поля</param>
        /// <returns></returns>
        public static string CheckStringNotNullOrEmpty(Dictionary<string, string> nameValueField)
        {
            string err = "";

            if (nameValueField != null)
            {
                foreach (var nv in nameValueField)
                {
                    if (string.IsNullOrEmpty(nv.Value))
                        err += $"Значение в поле: {nv.Key} не может быть пустым.\n";
                }
            }

            return err;
        }
    }
}
