﻿using Box.V2.Models;
using BoxNestGroup.Managers;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    public class MembershipGroupNameMailModel: ObservableCollection<MembershipGroupNameMailView>
    {
        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理にデータ追加
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        // public async Task AddBoxGroupMemberShip(string groupId_)
        public async Task AddMemberShipGroupId(string groupId_)
        {
            var list = await BoxManager.Instance.ListMemberIdFromGroup(groupId_);
            foreach (var item in list.Entries)
            {
                //this.Add(new MembershipGroupNameMailView(item.Group.Name,item.User.Address));
                this.Add(new MembershipGroupNameMailView(item.Group.Name, item.User.Login));
            }
        }


        /// <summary>
        ///  グループに所属する人数の取得
        /// </summary>
        /// <param name="groupId_">BoxグループID</param>
        /// <returns>所属人数</returns>
        //public int CountBoxGroupMemberShip(string groupId_)
        public int CountBoxUserInGroupName(string groupName_)
        {
            var list = this.ToList().FindAll((d) => (d.GroupName == groupName_));
            return list.Count;
        }

        /// <summary>
        /// ユーザーが所属するグループ名の一覧取得
        /// </summary>
        /// <param name="userId__">BoxユーザーID</param>
        /// <returns>グループ名の一覧</returns>
        public IList<string> ListGroupNameInUser(string mailAddress_)
        {
            //Debug.WriteLine(string.Format("■ListGroupNameInUser id  [{0}]", userId_));
            var rtn = new List<string>();

            var list = this.ToList().FindAll((d) => (d.UserAddress == mailAddress_));
            foreach (var member in list)
            {
                rtn.Add(member.GroupName);
            }
            return rtn;
        }
        /// <summary>
        /// ユーザーの所属するグループリストの取得
        /// </summary>
        /// <param name="userId_">ユーザーID</param>
        /// <param name="listAdd_">追加したいリストを指定する場合のグループ名一覧</param>
        /// <returns>ユーザーとグループの紐づけ一覧</returns>
        //public IList<BoxGroupMembership> ListGroupMembershipFromUserId(string userId_, IList<string>? listAdd_ = null)
        public IList<MembershipGroupNameMailView> ListGroupMembershipFromUserAddress(string userAddress_, IList<string>? listAdd_ = null)
        {
            //Debug.WriteLine(string.Format("■ListGroupMembershipFromUserId id  [{0}]", userId_));

            var rtn = this.ToList().FindAll((d) => (d.UserAddress == userAddress_));
            if (listAdd_ != null)
            {
                return rtn.FindAll((d) => (listAdd_.Contains(d.GroupName)) == true);
            }

            return rtn;
        }

        public void UpdateGroupName(string oldName_, string newName_)
        {
            this.ToList().FindAll( m => m.GroupName == oldName_)?.ForEach(m => m.GroupName = newName_);  
        }
    }
}