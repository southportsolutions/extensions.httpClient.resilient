using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Southport.Extensions.HttpClient.Resilient.Converters
{
    public static class XmlConvert
    {
        public static string SerializeObject<T>(T data)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(stream, data);
                stream.Position = 0;
                var writer = new StringWriter();
                var xmlDoc = XDocument.Load(stream, LoadOptions.None);
                xmlDoc.Save(writer);
                return OalmXmlWhiteSpaceCleaner.CleanXml(writer.ToString());
            }
        }

        public static T DeserializeObject<T>(string data)
        {
            using (var reader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                var xmlDoc = XDocument.Load(reader);
                var stream = new MemoryStream();
                xmlDoc.Save(stream);
                stream.Position = 0;
                return (T)serializer.Deserialize(stream);
            }
        }
    }

    public static class OalmXmlWhiteSpaceCleaner
    {
        public static string CleanXml(string dirtyXml)
        {
            var regex = new Regex(@">\s*<");

            var cleanedXml = regex.Replace(dirtyXml, "><");
            return cleanedXml;
        }
    }
}