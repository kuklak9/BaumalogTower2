using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPUtilities
{
    public class StringFormatCheck
    {
        public static string StringFormat(string baseString, object param1)
        {
            try
            {
                return String.Format(baseString, param1);
            }
            catch
            {
                return "";
            }
        }

        public static bool LessThan(String text, int value, bool onlyPositive)
        {
            if (text.Length > 0 && !(text.Length == 1 && !onlyPositive && text[0] == '-'))
            {
                try
                {
                    int a = Convert.ToInt32(text);
                    if (a < value && (onlyPositive && a >= 0 || !onlyPositive))
                        return true;
                    else
                        return false;

                }
                catch
                {
                    return false;
                }
            }
            else
                return true;

        }

        public static bool GreaterThan(String text, int value, bool onlyPositive)
        {
            if (text.Length > 0 && !(text.Length == 1 && !onlyPositive && text[0] == '-'))
            {
                try
                {
                    int a = Convert.ToInt32(text);
                    if (a > value && (onlyPositive && a >= 0 || !onlyPositive))
                        return true;
                    else
                        return false;

                }
                catch
                {
                    return false;
                }
            }
            else
                return true;

        }


        public static bool Int(String text)
        {
            if (text.Length > 0)
            {
                try
                {
                    int a = Convert.ToInt32(text);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }


        public static bool PositiveInt(String text)
        {
            if (text.Length > 0)
            {
                try
                {
                    int a = Convert.ToInt32(text);
                    if (a >= 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            else
                return true;
        }
    }

    public static class ExtensionMethods
    {
        public static String GetPrefixForLog(this String source)
        {
            try
            {
                if (source[source.Length - 1] == '.')
                {
                    return source.Remove(source.Length - 1, 1).ToUpper();
                }
                else
                {
                    return source.ToUpper();
                }
            }
            catch
            {
                return source;
            }
        }

        public static String GetPrefixForLogFromFullVariableName(this String source)
        {
            int firstDotPosition = source.IndexOf('.');

            if (firstDotPosition < 0)
                throw new ArgumentException();

            return source.Substring(0, firstDotPosition).ToUpper();
        }

        public static bool EqualsIgnoreCase(this string source, string compared)
        {
            return source.Equals(compared, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool EqualsIgnoreCaseAndPrefixIndex(this string source, string programPrefix, string variableName)
        {
            if (programPrefix.Length < 2)
                throw new ArgumentException();

            return source.StartsWith(programPrefix.Substring(0, programPrefix.Length - 2), StringComparison.InvariantCultureIgnoreCase)
                && source.EndsWith(variableName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
