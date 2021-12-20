using System;
using System.Collections.Generic;
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
        /// <returns></returns>
        public static string GetStringLikePattern(string columnName, string str)
        {
            return string.IsNullOrEmpty(str)
                ? $"({columnName} like '%%' or {columnName} is NULL )" 
                : $"{columnName} like '%{str}%'";
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
    }
}
