using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace BoxNestGroup.Files
{
    /// <summary>
    /// XML 読み書きのBase
    /// </summary>
    /// <typeparam name="T">利用しやすいので親のテンプレート</typeparam>
    public class BaseXml<T> where T : class
    {
        /// <summary>
        /// XMLファイルを読み込む
        /// </summary>
        /// <param name="path_"></param>
        public void Open(string path_)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var sr = new StreamReader(path_, Encoding.UTF8))
            {
                var xml = serializer.Deserialize(sr) as T;

                var listPropert = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                listPropert?.ToList().ForEach(p_ => typeof(T)?.GetProperty(p_.Name)?.SetValue(this, p_.GetValue(xml)));
            }
        }

        /// <summary>
        /// XMLファイルに保存する
        /// </summary>
        /// <param name="path_"></param>
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
