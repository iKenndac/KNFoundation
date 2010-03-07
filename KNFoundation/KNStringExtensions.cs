using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    static class KNStringExtensions {

        public static bool CharacterAtIndexIsEscapedWithCharacter(this string s, int index, char escapeCharacter) {

            if (index == 0) {
                return false;
            } else {
                return s.ToCharArray()[index - 1] == escapeCharacter;
            }
        }

        public static string DeEscapedString(this string s) {
            // TODO: Give this a less stupid name

            string newString = s;

            newString = newString.Replace("\\\"", "\"");
            newString = newString.Replace("\\n", Environment.NewLine);
            newString = newString.Replace("\\\\", "\\");

            return newString;
        }
    }

