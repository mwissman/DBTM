using System.IO;
using System.Xml.Serialization;

namespace DBTM.Domain
{
    public class XMLSerializer : IXMLSerializer
    {
        public void Serialize<T>(string path, T objectToSerialize)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(fileStream, objectToSerialize);
            }
        }

        public T Deserialize<T>(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                object obj = serializer.Deserialize(fileStream);

                return (T) obj;
            }
        }
    }
}