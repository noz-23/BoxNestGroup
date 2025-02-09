using BoxNestGroup.Files;
using System.Collections.ObjectModel;

namespace BoxNestGroup.Views
{
    public class MembershipGroupNameLoginModel: ObservableCollection<MembershipGroupNameLoginView>
    {
        /// <summary>
        ///  グループに所属する人数の取得
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        /// <returns>所属人数</returns>
        //public int CountBoxGroupMemberShip(string groupId_)
        public int CountBoxUserInGroupName(string groupName_)
        {
            if (string.IsNullOrEmpty(groupName_) == true)
            {
                return 0;
            }

            return this.ToList().FindAll(view_ => (view_.GroupName == groupName_))?.Count ?? 0;
        }

        /// <summary>
        /// ユーザーが所属するグループ名の一覧取得
        /// </summary>
        /// <param name="userId__">BoxユーザーID</param>
        /// <returns>グループ名の一覧</returns>
        public IList<string> ListGroupNameInUser(string userLogin_)
        {
            var rtn = new List<string>();

            if (string.IsNullOrEmpty(userLogin_) == true)
            {
                return rtn;
            }

            this.ToList().FindAll((d) => (d.UserLogin == userLogin_))?.ForEach(member_=> rtn.Add(member_.GroupName));

            return rtn;
        }
        /// <summary>
        /// ユーザーの所属するグループリストの取得
        /// </summary>
        /// <param name="userId_">ユーザーID</param>
        /// <param name="listAdd_">追加したいリストを指定する場合のグループ名一覧</param>
        /// <returns>ユーザーとグループの紐づけ一覧</returns>
        public IList<MembershipGroupNameLoginView> ListGroupMembershipFromUserAddress(string userAddress_, IList<string>? listAdd_ = null)
        {
            var rtn = this.ToList().FindAll(view_ => (view_.UserLogin == userAddress_));
            if (listAdd_ != null)
            {
                return rtn.FindAll(view_ => (listAdd_.Contains(view_.GroupName)) == true);
            }

            return rtn;
        }

        /// <summary>
        /// グループ名更新
        /// </summary>
        /// <param name="oldName_">古いグループ名</param>
        /// <param name="newName_">新しいグループ名</param>
        public void UpdateGroupName(string oldName_, string newName_)
        {
            LogFile.Instance.WriteLine($"[{oldName_}] -> [{newName_}]");

            this.ToList().FindAll(view_ => view_.GroupName == oldName_)?.ForEach(view_ => view_.GroupName = newName_);  
        }
    }
}
