﻿using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    public class GroupDataGridModel : ObservableCollection<GroupDataGridView>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupDataGridModel()
        { 
        }

        /// <summary>
        /// グループIDからグループ名の取得
        /// </summary>
        /// <param name="groupId_">グループID</param>
        /// <returns>グループ名</returns>
        public string GetBoxGroupName(string groupId_)
        {
            return this.ToList().Find((d) => (d.GroupId == groupId_))?.GroupName??string.Empty;
        }

        /// <summary>
        /// グループ名からグループIDの取得
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        /// <returns>グループID</returns>
        public string GetBoxGroupId(string groupName_)
        {
            return this.ToList().Find((d) => (d.GroupName == groupName_))?.GroupId??string.Empty;
        }

        /// <summary>
        /// ネストをして増やしたグループ名一覧
        /// </summary>
        /// <param name="listGroupName_">ネスト前のグループ名一覧</param>
        /// <returns>グループ名一覧</returns>
        public IList<string> ListNestGroupId(IList<string> listGroupName_)
        {
            var rtn = new List<string>();

            var listGroup = this.ToList();
            foreach (var name in listGroupName_)
            {
                var find = listGroup.Find((g) => (g.GroupName == name));

                if (find == null)
                {
                    continue;
                }
                rtn.Add(find.GroupId);
            }
            return rtn;
        }

    }
}
