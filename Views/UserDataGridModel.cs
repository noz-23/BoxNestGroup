﻿using BoxNestGroup.Files;
using System.Collections.ObjectModel;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// ユーザー情報の表示用クラス
    /// </summary>
    public class UserDataGridModel: ObservableCollection<UserDataGridView>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserDataGridModel() 
        {
        }

        /// <summary>
        /// ユーザー名の更新
        /// </summary>
        /// <param name="oldName_">古いグループ</param>
        /// <param name="newName_">新しいグループ</param>
        public void UpdateGroupName(string oldName_, string newName_)
        {
            LogFile.Instance.WriteLine($"[{oldName_}] -> [{newName_}]");

            this?.ToList().ForEach(m => m.UpdateGroupName(oldName_, newName_));
        }
    }
}
