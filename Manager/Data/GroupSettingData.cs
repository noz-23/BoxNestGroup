using Box.V2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace BoxNestGroup.Manager.Data
{
    internal class GroupSettingData
    {
        public string GroupName = string.Empty; // 1行目
        public HashSet<string> ListMemberId = new HashSet<string>(); // 2行目以降

        public GroupSettingData(string path_)
        {
            // 既存は読み込む
            var reader = new System.IO.StreamReader(path_);
            GroupName = reader.ReadLine();
            while (reader.Peek() > -1)
            {
                ListMemberId.Add(reader.ReadLine());
            }
            //
            reader.Close();
        }

        public GroupSettingData(string path_, string groupName_):this(path_, groupName_, new HashSet<string>())
        {
            Console.WriteLine("GroupSettingData [{0}]-[{1}]", groupName_, path_);
        }
        public GroupSettingData(string path_, string groupName_, HashSet<string> listMember_)
        {
            // 新規はグループを追加
            var writer = new System.IO.StreamWriter(path_, false); // 常に上書き
            writer.WriteLine(groupName_);
            GroupName = groupName_;
            //

            foreach (var member in listMember_)
            {
                writer.WriteLine(member);
            }

            writer.Close();

            Console.WriteLine("GroupSettingData [{0} -{1}]-[{2}]", groupName_, listMember_.Count, path_);
        }

    }
}
