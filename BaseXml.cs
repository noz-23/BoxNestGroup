using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Vml.Office;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace BoxNestGroup
{
    public class BaseXml<T> where T : class
    {
        public void  Open(string path_)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sr =new StreamReader(path_,Encoding.UTF8))
            {
                var xml = serializer.Deserialize(sr) as T;

                var listPropert =typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var p in listPropert)
                {
                    typeof(T)?.GetProperty(p.Name)?.SetValue(this, p.GetValue(xml));
                }
            }
        }

        public void Save(string path_)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sw = new StreamWriter(path_, false, Encoding.UTF8))
            {
                serializer.Serialize(sw, this);
            }
        }
    }
}
