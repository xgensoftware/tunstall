using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace TunstallBL
{
    public static class ExtensionMethods
    {
        #region Serialization
        public static string ToXML<T>(this T value)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (StringWriter sw = new StringWriter(sb))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T), null, new Type[0], null, null);
                    serializer.Serialize(sw, value);
                }
            }
            catch { }
            return sb.ToString();
        }

        public static T FromXML<T>(this string value)
        {
            T returnXMLClass = default(T);

            try
            {
                using (TextReader reader = new StringReader(value))
                {
                    try
                    {
                        returnXMLClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
            catch { }

            return returnXMLClass;
        }

        public static T FromJson<T>(this string value)
        {
            T returnObject = default(T);

            try
            {
                returnObject = JsonConvert.DeserializeObject<T>(value);
            }
            catch { }

            return returnObject;
        }

        public static string ToJson<T>(this T value)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append(JsonConvert.SerializeObject(value,Newtonsoft.Json.Formatting.Indented));
            }
            catch { }

            return sb.ToString();
        }

        #endregion

        #region CSV

        public static void ToCsvFile<T>(this IEnumerable<T> items, string separator, string fileName)
        where T : class
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            string header = String.Join(separator, properties.Select(f => f.Name).ToArray());
            using (FileStream file = File.Create(fileName))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine(header);

                    foreach (var o in items)
                    {
                        string line = string.Join(separator, properties.Select(p => p.GetValue(o, null).ToCsvValue()).ToArray());
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private static string ToCsvValue<T>(this T item)
        {
            if (item == null) return "\"\"";

            if (item is string)
            {
                return string.Format("\"{0}\"", item.ToString().Replace("\"", "\\\""));
            }
            double dummy;
            if (double.TryParse(item.ToString(), out dummy))
            {
                return string.Format("{0}", item);
            }
            return string.Format("\"{0}\"", item);
        }
        #endregion
    }
}
