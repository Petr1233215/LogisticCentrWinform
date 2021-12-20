using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back;
        }

        /// <summary>
        /// Метод обрабатывает, действие если клавиша не цифра
        /// </summary>
        /// <param name="e"></param>
        public static void HandleCheckDigit(KeyPressEventArgs e)
        {
            if (!IsDigitOrBackspace(e))
            {
                MessageBox.Show("Вы не можете вводить сюда ничего кроме цифр");
                e.Handled = true;
            }
        }
    }
}
