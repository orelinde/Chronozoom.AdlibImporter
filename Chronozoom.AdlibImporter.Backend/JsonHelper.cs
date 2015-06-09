using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Chronozoom.AdlibImporter.Backend
{
    public class JsonHelper
    {
        public static string PropertyToJson(string key, object value)
        {
            return PropertyToJson(key, value, false);
        }

        public static string PropertyToJson(string key, object value, bool lastProperty)
        {
            
            StringBuilder builder = new StringBuilder();
            builder.Append("\"").Append(key).Append("\" : ");
            if (value == null) return builder.Append("\"\"").Append(lastProperty ? "" : ",").ToString();
            if (IsNumeric(value))
            {
                builder.Append(value);
                builder.Append(lastProperty ? "" : ",");
            }
            else
            {
                builder.Append("\"").Append(value).Append("\"");
                builder.Append(lastProperty ? "" : ",");
            }
           
            return builder.ToString();
        }

        private static bool IsNumeric(object value)
        {
            var type = value.GetType();
            if (type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(long))
            {
                return true;
            }
            return false;
        }

        public static string BeginObject()
        {
            return "{";
        }

        public static string EndObject()
        {
            return "}";
        }

        public static string ArrayToJson(string key, List<string> pictureUrLs, bool lastitem)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\"").Append(key).Append("\" : [");
            if (pictureUrLs != null)
            {
                for (int i = 0; i < pictureUrLs.Count; i++)
                {
                    builder.Append(i == pictureUrLs.Count
                        ? AppendLineToArray(pictureUrLs[i], true)
                        : AppendLineToArray(pictureUrLs[i], false));
                }
            }
            builder.Append("]");
            builder.Append(lastitem ? "" : ",");
            return builder.ToString();
        }

        private static string AppendLineToArray(string line, bool isLast)
        {
            return isLast ? String.Format("\"{0}\"", line) : String.Format("\"{0}\",", line);
        }
    }
}