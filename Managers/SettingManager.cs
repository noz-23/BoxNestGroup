using Box.V2.Models;
using BoxNestGroup.Managers.Data;
using BoxNestGroup.Views;
using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace BoxNestGroup.Managers
{
    public class SettingManager: INotifyPropertyChanged
    {
        private const int INDEX_NAME = 1;
        private const int INDEX_MAIL = 2;
        private const int INDEX_GROUP = 3;
        private const int INDEX_STRAGE = 4;
        private const int INDEX_COLABO = 5;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// シングルトン
        /// </summary>
        static public SettingManager Instance { get; } = new SettingManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SettingManager()
        {
        }

        ~SettingManager() 
        {
            ListGroupIdName.SavaData();
        }

        /// <summary>
        /// ログインネーム
        /// </summary>
        private string _loginName = "認証前";
        public string LoginName 
        {
            get { return _loginName; }
            set
            { 
                _loginName = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged("LoginName");
            }               
        }

        /// <summary>
        /// 
        /// </summary>
        public DictionaryGroupIdName ListGroupIdName { get; private set; } = new DictionaryGroupIdName();
        
        /// <summary>
        /// グループビュー
        /// </summary>
        public GroupDataGridModel ListGroupDataGridView { get; private set; } = new GroupDataGridModel();

        /// <summary>
        /// ユーザービュー
        /// </summary>
        public UserDataGridModel ListUserDataGridView { get; private set; } = new UserDataGridModel();

        /// <summary>
        ///  Box でのグループとユーザーの紐づけ管理
        ///  グループとユーザーの両方でカウントありのためBoxGroupMembershipを登録
        /// </summary>
        public MembershipGroupNameMailModel ListMembershipGroupNameMail { get; private set; } = new MembershipGroupNameMailModel();

        /// <summary>
        /// エクセルのシート読み込み処理
        /// </summary>
        /// <param name="sheet_">エクセルのシート</param>
        public void LoadExcelSheet(IXLWorksheet sheet_)
        {
            var listGroup =new HashSet<string>();

            // 1行目はヘッダーのため飛ばし
            for (int row = 2; row < sheet_.RowCount()+1; row++)
            {
                var userName = sheet_.Cell(row, INDEX_NAME).Value.ToString();
                var userMail = sheet_.Cell(row, INDEX_MAIL).Value.ToString();
                //
                var group = new List<string>(sheet_.Cell(row, INDEX_GROUP).Value.ToString().Split(";"));
                group.Remove(string.Empty);
                listGroup.UnionWith(group);
                //
                var strage = sheet_.Cell(row, INDEX_STRAGE).Value.ToString();
                var colabo = sheet_.Cell(row, INDEX_COLABO).Value.ToString();
                //
                var add = new UserDataGridView(userName, userMail, group, strage, colabo);

                if (userName == string.Empty)
                {
                    break;
                }
                ListUserDataGridView.Add(add);

                foreach (var groupName in listGroup) 
                {
                    SettingManager.Instance.ListMembershipGroupNameMail.Add(new MembershipGroupNameMailView(groupName, userMail));
                }
            }

            foreach (var groupName in listGroup)
            {
                ListGroupDataGridView.Add(new GroupDataGridView(groupName));
            }
        }

        /// <summary>
        /// エクセルのシートの保存処理
        /// </summary>
        /// <param name="path_"></param>
        public void SaveExcelFile(string path_)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(1);
                int row = 1;

                worksheet.Cell(row, INDEX_NAME).Value = "名前";
                worksheet.Cell(row, INDEX_MAIL).Value = "メール";
                worksheet.Cell(row, INDEX_GROUP).Value = "グループ";
                worksheet.Cell(row, INDEX_STRAGE).Value = "ストレージ";
                worksheet.Cell(row, INDEX_COLABO).Value = "外部コラボレーション制限";
                row++;

                foreach (var user in SettingManager.Instance.ListUserDataGridView)
                {
                    if (user.ListModAllGroup == string.Empty)
                    {
                        continue;
                    }

                    worksheet.Cell(row, INDEX_NAME).Value = user.UserName;
                    worksheet.Cell(row, INDEX_MAIL).Value = user.UserMailAddress;
                    worksheet.Cell(row, INDEX_GROUP).Value = user.ListModAllGroup.Replace("\n", ";");
                    worksheet.Cell(row, INDEX_STRAGE).Value = (user.UserSpaceUsed == UserDataGridView.APP_UNLIMITED) ? UserDataGridView.BOX_UNLIMITED : user.UserSpaceUsed;
                    worksheet.Cell(row, INDEX_COLABO).Value = (user.UserExternalCollaborate == UserDataGridView.APP_ENABLED) ? UserDataGridView.BOX_ENABLED : UserDataGridView.BOX_DISABLED;
                    row++;

                }

                workbook.SaveAs(path_);
            }
        }
    }
}
