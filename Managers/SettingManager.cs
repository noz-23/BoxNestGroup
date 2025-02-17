﻿using BoxNestGroup.Files;
using BoxNestGroup.Views;
using ClosedXML.Excel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using static BoxNestGroup.Views.BaseView;

namespace BoxNestGroup.Managers
{
    /// <summary>
    /// 設定管理クラス
    /// </summary>
    public class SettingManager: INotifyPropertyChanged
    {
        private const int INDEX_NAME = 1;
        private const int INDEX_MAIL = 2;
        private const int INDEX_GROUP = 3;
        private const int INDEX_STORAGE = 4;
        private const int INDEX_COLABO = 5;

        /// <summary>
        /// シングルトン
        /// </summary>
        static public SettingManager Instance { get; private set; } = new SettingManager();
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private SettingManager()
        {
        }

        ~SettingManager() 
        {
        }

        public void Create()
        {
            ListGroupDataGridView.Clear();
            ListUserDataGridView.Clear();
            ListMembershipGroupNameLogin.Clear();
            ListXmlGroupTreeView.Open();
        }

        /// <summary>
        /// 更新通知
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName_ = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName_));
        }

        /// <summary>
        /// ログインネーム
        /// </summary>
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
        private string _loginName = Properties.Resource.LOGIN_BEFOUR;

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
        public MembershipGroupNameLoginModel ListMembershipGroupNameLogin { get; private set; } = new MembershipGroupNameLoginModel();

        /// <summary>
        /// フォルダのツリービュー管理
        /// </summary>
        public XmlGroupTreeModel ListXmlGroupTreeView { get; private set; } = new XmlGroupTreeModel();


        /// <summary>
        /// エクセルのシート読み込み処理
        /// </summary>
        /// <param name="sheet_">エクセルのシート</param>
        public void LoadExcelSheet(IXLWorksheet sheet_)
        {
            LogFile.Instance.WriteLine($"Load RowCount{sheet_.RowCount()}");

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
                var strage = sheet_.Cell(row, INDEX_STORAGE).Value.ToString();
                var colabo = sheet_.Cell(row, INDEX_COLABO).Value.ToString();
                //
                var add = new UserDataGridView(userName, userMail, group, strage, colabo);

                if (userName == string.Empty)
                {
                    break;
                }
                App.Current.Dispatcher.Invoke(() => {
                    ListUserDataGridView.Add(add);
                    listGroup?.ToList().ForEach(groupName => ListMembershipGroupNameLogin.Add(new MembershipGroupNameLoginView(groupName, userMail)));
                });
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                listGroup?.ToList().ForEach(groupName => ListGroupDataGridView.Add(new GroupDataGridView(groupName)));
            });
        }

        /// <summary>
        /// エクセルのシートの保存処理
        /// </summary>
        /// <param name="path_"></param>
        public void SaveExcelFile(string path_)
        {
            LogFile.Instance.WriteLine($"Save RowCount{path_}");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet(1);
                int row = 1;

                worksheet.Cell(row, INDEX_NAME).Value = Properties.Resource.EXCEL_USER_NAME;
                worksheet.Cell(row, INDEX_MAIL).Value = Properties.Resource.EXCEL_USER_MAIL;
                worksheet.Cell(row, INDEX_GROUP).Value = Properties.Resource.EXCEL_USER_GROUP;
                worksheet.Cell(row, INDEX_STORAGE).Value = Properties.Resource.EXCEL_USER_STORAGE;
                worksheet.Cell(row, INDEX_COLABO).Value = Properties.Resource.EXCEL_USER_COLABO;
                row++;

                // 変更
                var allWrite =(System.Windows.MessageBox.Show("変更箇所だけ書き出すか?","確認",MessageBoxButton.YesNo) ==MessageBoxResult.Yes);

                SettingManager.Instance.ListUserDataGridView?.ToList().ForEach(view_ =>
                {
                    if (allWrite == false && view_.StatudData != Status.NONE)
                    {
                        return;
                    }

                    worksheet.Cell(row, INDEX_NAME).Value = view_.UserName;
                    worksheet.Cell(row, INDEX_MAIL).Value = view_.UserLogin;
                    worksheet.Cell(row, INDEX_GROUP).Value = string.Join(";",view_.ListNowAllGroup);
                    worksheet.Cell(row, INDEX_STORAGE).Value = (view_.UserSpaceUsed == Properties.Resource.BOX_USER_DISK_UNLIMITED) ? UserDataGridView.BOX_UNLIMITED : view_.UserSpaceUsed;
                    worksheet.Cell(row, INDEX_COLABO).Value = (view_.UserExternalCollaborate == Properties.Resource.BOX_USER_LIMIT_ENABLED) ? UserDataGridView.BOX_ENABLED : UserDataGridView.BOX_DISABLED;
                    row++;
                });

                workbook.SaveAs(path_);
            }
        }


        /// <summary>
        /// グループ名リストからグループIDリストへの変換
        /// </summary>
        /// <param name="listGroupName_"></param>
        /// <returns></returns>
        public IList<string> ConvertGroupNameToId(IList<string> listGroupName_)
        {
            LogFile.Instance.WriteLine($"[{string.Join(",", listGroupName_)}]");

            var rtn = new List<string>();

            // グループ名からグループIDに変換
            listGroupName_?.ToList().ForEach(groupName_ =>
            {
                var groupId = ListGroupDataGridView.GetBoxGroupName(groupName_);
                if (string.IsNullOrEmpty(groupId) == true)
                {
                    return;
                }
                rtn.Add(groupId);
            });

            return rtn;
        }

    }
}
