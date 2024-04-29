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
    /// <summary>
    /// 保存しているグループ名の呼び出し
    /// 　基本は上位のフォルダ名の比較用
    /// </summary>
    internal class GroupSettingData
    {
        /// <summary>
        /// 基本グループ設定 フォルダパス
        /// </summary>
        private static readonly string _commonGroupSettingPath = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupSetting;
        /// <summary>
        /// ファイル名 : グループ名ID_Setting.txt
        /// </summary>
        private const string _settingFile = "_Setting.txt";

        /// <summary>
        /// 全て設定ファイルの取得
        /// </summary>
        /// <returns>全て設定ファイル名リスト</returns>
        public static string [] ListAllGroupSettingData()
        {
            return Directory.GetFiles(_commonGroupSettingPath, "*" + _settingFile);
        }
        /// <summary>
        /// グループIDからの設定ファイルのパス取得
        /// </summary>
        /// <param name="id_">グループID</param>
        /// <returns>ファイルへのパス</returns>
        public static string PathGroupSetting(string groupId_)
        {
            return _commonGroupSettingPath + @"\" + groupId_ + _settingFile;
        }
        /// <summary>
        /// パス(ファイル名)からグループIDの取得
        /// </summary>
        /// <param name="path_">設定ファイルのパス</param>
        /// <returns>グループID</returns>
        public static string GetGroupId(string path_)
        {
            return path_.Replace(_commonGroupSettingPath, string.Empty).Replace(_settingFile, string.Empty).Replace(@"\", string.Empty);
        }

        /// <summary>
        /// 設定ファイルの呼び出し
        /// </summary>
        /// <param name="path_">パス</param>
        /// <returns>グルー名</returns>
        public static string ReadGroupName(string path_)
        {
            Console.WriteLine("■ReadGroupName [{0}]",  path_);
            //
            // 既存は読み込む
            var reader = new System.IO.StreamReader(path_);
            var  name = reader.ReadLine();
            reader.Close();

            return name;
        }

        /// <summary>
        /// 設定ファイルの保存
        /// </summary>
        /// <param name="path_">パス</param>
        /// <param name="groupName_">グループ名</param>
        public static void WriteGroupName(string path_, string groupName_)
        {
            Console.WriteLine("■WriteGroupName [{0}]", path_);
            //

            var writer = new System.IO.StreamWriter(path_, false); // 常に上書き
            writer.WriteLine(groupName_);
            writer.Close();
        }
    }
}
