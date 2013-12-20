using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class StringEx
    {
        public static string Truncate(this string source, int length)
        {
            if (source != null && source.Length > length)
            {
                source = source.Substring(0, length);
            }
            return source;
        }

        /// <summary>
        /// Removes non-ascii characters
        /// </summary>
        public static string CleanString(this string inString)
        {
            if (inString == null)
                return inString;

            return Regex.Replace(inString, @"[^\u0000-\u007F]", "");
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return String.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static string Format(this string str, params object[] args)
        {
            return String.Format(str, args);
        }

        public static bool ContainsIgnoreCase(this string original, string value)
        {
            return Contains(original, value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(this string original, string value, StringComparison comparisionType)
        {
            return original.IndexOf(value, comparisionType) >= 0;
        }

        public static string ToTitleCase(this string text)
        {
            if (text == null) 
                return text;

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            // TextInfo.ToTitleCase only operates on the string if is all lower case, otherwise it returns the string unchanged.
            return textInfo.ToTitleCase(text.ToLower());
        }

        public static string ToSnakeCase(this string word)
        {
            if (String.IsNullOrWhiteSpace(word))
                return word;
            return String.Format("{0}", word.Substring(0, 1).ToLower() + word.Substring(1));
        }

        /// <summary>
        /// Returns true if the string equals one of the values.
        /// </summary>
        public static bool Match(this string original, string[] values)
        {
            return original.Match(values, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns true if the string equals one of the values.
        /// </summary>
        public static bool Match(this string original, string[] values, StringComparison comparisionType)
        {
            foreach (var value in values)
            {
                if (original.Equals(value, comparisionType))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Splits a string into a split of 'words' based on capitalization.
        /// e.g. AnExampleString => [An, Example, String]
        /// </summary>
        public static string[] SplitIntoWords(this string str)
        {
            List<string> words = new List<string>();

            if (!String.IsNullOrWhiteSpace(str))
            {
                StringBuilder word = new StringBuilder();
                for (int i = 0; i < str.Length; i++)
                {
                    char character = str[i];

                    // A 'word' starts with a capital letter.
                    if (Char.IsUpper(character))
                    {
                        if (word.Length > 0)
                        {
                            words.Add(word.ToString());
                            word.Clear();
                        }
                    }

                    word.Append(character);
                }

                if (word.Length > 0)
                    words.Add(word.ToString());
            }

            return words.ToArray();
        }
    }
}
