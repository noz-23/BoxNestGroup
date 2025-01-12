using Box.V2.Models;
using BoxNestGroup.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BoxNestGroup.Views
{
/*
    public class ListBoxMembership : List<BoxGroupMembership>
    {

        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理にデータ追加
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        public async Task AddBoxGroupMemberShip(string groupId_)
        {
            var list = await BoxManager.Instance.ListMemberIdFromGroup(groupId_);
            this.AddRange(list.Entries);
        }


        /// <summary>
        ///  グループに所属する人数の取得
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        /// <returns>所属人数</returns>
        public int CountBoxGroupMemberShip(string groupId_)
        {
            var list = this.FindAll((d) => (d.Group.Id == groupId_));
            return list.Count;
        }

        /// <summary>
        /// ユーザーが所属するグループ名の一覧取得
        /// </summary>
        /// <param name="userId__">BoxユーザーID</param>
        /// <returns>グループ名の一覧</returns>
        public IList<string> ListGroupNameInUser(string userId_)
        {
            //Debug.WriteLine(string.Format("■ListGroupNameInUser id  [{0}]", userId_));
            var rtn = new List<string>();

            var list = this.FindAll((d) => (d.User.Id == userId_));
            foreach (var member in list)
            {
                rtn.Add(member.Group.Name);
            }
            return rtn;
        }
        /// <summary>
        /// ユーザーの所属するグループリストの取得
        /// </summary>
        /// <param name="userId_">ユーザーID</param>
        /// <param name="listAdd_">追加したいリストを指定する場合のグループ名一覧</param>
        /// <returns>ユーザーとグループの紐づけ一覧</returns>
        public IList<BoxGroupMembership> ListGroupMembershipFromUserId(string userId_, IList<string>? listAdd_ = null)
        {
            //Debug.WriteLine(string.Format("■ListGroupMembershipFromUserId id  [{0}]", userId_));

            var rtn = this.FindAll((d) => (d.User.Id == userId_));
            if (listAdd_ != null)
            {
                return rtn.FindAll((d) => (listAdd_.Contains(d.Group.Name)) == true);
            }

            return rtn;
        }
    }
*/
}
