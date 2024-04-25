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
        public string GroupId { get; private set; } = string.Empty;
        public string GroupName { get; private set; } = string.Empty; // 1行目
        public BoxGroup GroupBox { get; private set; } = null;


        //private bool _loadMemberId = false;

        //public HashSet<string> ListMemberId { get; private set; } = new HashSet<string>(); // Boxから取得

        public List<BoxGroupMembership> ListMember { get; private set; } = null;

        public GroupSettingData(string groupId_, string path_)
        {
            Console.WriteLine("■GroupSettingData [{0}]-[{1}]", groupId_, path_);
            //

            GroupId = groupId_;
            // 既存は読み込む
            var reader = new System.IO.StreamReader(path_);
            GroupName = reader.ReadLine();
            //while (reader.Peek() > -1)
            //{
            //    ListMemberId.Add(reader.ReadLine());
            //}
            //
            reader.Close();
        }

        //public GroupSettingData(string groupId_, string path_, string groupName_)
        //{
        //    Console.WriteLine("■GroupSettingData [{0}]-[{1}]-[{2}]", groupId_, path_, groupName_);
        //    //
        //    // 新規はグループを追加
        //    var writer = new System.IO.StreamWriter(path_, false); // 常に上書き
        //    writer.WriteLine(groupName_);
        //    writer.Close();
        //}
        public GroupSettingData(BoxGroup boxGroup_, string path_)
        {
            Console.WriteLine("■GroupSettingData [{0}]-[{1}]-[{2}]",  path_, boxGroup_.Id, boxGroup_.Name);
            GroupBox = boxGroup_;
            //
            // 新規はグループを追加
            var writer = new System.IO.StreamWriter(path_, false); // 常に上書き
            writer.WriteLine(boxGroup_.Name);
            writer.Close();
        }


        // BoxからグループIDからメンバーの情報を取得
        public async Task<int> LoadGroupMemberId() 
        {
            //var list = await BoxManager.Instance.ListMemberIdFromGroup(GroupId);

            //if(list !=null)
            //{
            //    ListMemberId.Clear();
            //    foreach (var member in list.Entries)
            //    {
            //        ListMemberId.Add(member.User.Id);
            //    }
            //    _loadMemberId = true;
            //    return ListMemberId.Count;
            //}

            if (ListMember == null)
            {
                var list = await BoxManager.Instance.ListMemberIdFromGroup(GroupId);
                ListMember = list.Entries;
            }
            if(ListMember != null) 
            {
                return ListMember.Count;
            }

            return 0;
        }

        public bool ListMemberContains(string userId_)
        {
            foreach (var member in ListMember)
            {
                if (member.User.Id == userId_)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
