using Box.V2.Models;
using BoxNestGroup.Files;
using BoxNestGroup.Managers;
using System.Runtime.CompilerServices;

namespace BoxNestGroup.Views
{
    /// <summary>
    /// グループの表示データビュー
    /// </summary>
    public class GroupDataGridView : BaseView
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GroupDataGridView():base()
        {
            _flgInital = true;
        }

        /// <summary>
        /// オフラインや新規作成時のコンストラクタ
        /// </summary>
        /// <param name="groupName_">グループ名</param>
        public GroupDataGridView(string groupName_)
        {
            LogFile.Instance.WriteLine($"{groupName_}");
            //
            GroupName = groupName_;
            OldGroupName = groupName_;

            GroupId = SettingManager.Instance.ListXmlGroupTreeView.FindGroupId(groupName_);
            GroupId =(string.IsNullOrEmpty(GroupId) == true )?(Properties.Resource.ID_NAME_OFFLINE) :( GroupId);

            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            string name = string.Empty;

            // 設定がない場合は作る
            if (SettingManager.Instance.ListXmlGroupTreeView.ContainsName(GroupName) == false)
            {
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, null));
            }
            _flgInital = true;
        }

        /// <summary>
        /// オンライン時のコンストラクタ
        /// </summary>
        /// <param name="group_">Boxグループ</param>
        public GroupDataGridView(BoxGroup group_)
        {
            LogFile.Instance.WriteLine($"{group_.Id} {group_.Name}");
            //
            GroupName = group_.Name;
            OldGroupName = group_.Name;
            GroupId = group_.Id;

            // Boxから取得
            UserCount = SettingManager.Instance.ListMembershipGroupNameLogin.CountBoxUserInGroupName(GroupName);

            // グループ名の変更された場合の処理
            string name = string.Empty;

            var listTreeView = SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupId(GroupId);
            listTreeView?.ToList().ForEach(view_ => 
            {
                if (view_.GroupName != GroupName)
                {
                    view_.GroupName = GroupName;
                }
            });

            // 設定がない場合は作る
            if (SettingManager.Instance.ListXmlGroupTreeView.ContainsId(GroupId) == false)
            {
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, GroupId, null));
            }

            var listAllGroupName = SettingManager.Instance.ListXmlGroupTreeView.FindAllGroupName(GroupName);
            listAllGroupName?.ToList().ForEach(view_ =>
            {
                // TreeView にグループIDがない場合は追加する
                if (string.IsNullOrEmpty(view_.GroupId) == true)
                {
                    view_.GroupId = GroupId;
                }
            });

            if (listAllGroupName == null)
            {
                // ない場合は新規作成
                SettingManager.Instance.ListXmlGroupTreeView.Add(new XmlGroupTreeView(GroupName, GroupId, null));
            }

            _flgInital = true;
        }

        /// <summary>
        /// コンストラクタの場合、状態更新しない
        /// </summary>
        private bool _flgInital = false;

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// グループ名変更判定
        /// </summary>
        public bool IsSameOldGroupName { get => GroupName == OldGroupName; }

        /// <summary>
        /// 旧グループ名
        /// </summary>
        public string OldGroupName { get; private set; } = string.Empty;

        /// <summary>
        /// グループID
        /// </summary>
        public string GroupId { get; set; } = string.Empty;
        /// <summary>
        /// 最大のネスト最大数(基本並び替え用)
        /// </summary>
        public int MaxNestCount { get => SettingManager.Instance.ListXmlGroupTreeView.MaxNestCount(GroupName); }


        /// <summary>
        /// フォルダの数
        /// </summary>
        public int FolderCount { get => SettingManager.Instance.ListXmlGroupTreeView.NameCount(GroupName); }

        /// <summary>
        /// ユーザーの数
        /// </summary>
        public int UserCount { get; private set; } = 0;

        /// <summary>
        /// 通知の変更
        /// </summary>
        /// <param name="propertyName_"></param>
        protected override void _NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            if (_flgInital == true )
            {
                _StatudData = (string.IsNullOrEmpty(GroupId) == true) ? Status.NEW : Status.MOD;
                base._NotifyPropertyChanged("StatusName");
            }
            base._NotifyPropertyChanged(propertyName_);
        }
    }
}
