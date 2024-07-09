using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.scripts.Helpers.Utility
{
    public static class LabelConverter
    {
        /// <summary>
        /// Convert camelcase words to readable text and capitalises first letter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertCamelCaseToWord(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            //Insert a space before each uppercase letter that is preceded by a lowercase letter or another uppercase letter
            string result = Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");

            //Capitalize the first letter of the result
            if (char.IsLower(result[0]))
            {
                result = char.ToUpper(result[0]) + result.Substring(1);
            }

            //Remove (Clone) text from object copy logic
            if (result.Contains("(Clone)"))
            {
                result = result.Replace("(Clone)", "");
            }

            return result;
        }
    }
}
