using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup.Managers.Data
{
    /// <summary>
    /// グループIDと名前の対応リスト
    /// </summary>
    public class DictionaryGroupIdName: Dictionary<string, string>
    {
        /// <summary>
        /// 基本グループ設定 フォルダパス
        /// </summary>
        private static readonly string _commonGroupSettingPath = Directory.GetCurrentDirectory() + @"\" + Settings.Default.CommonGroupSetting+ @"\SettingGroupIdName.txt";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DictionaryGroupIdName():base()
        {
            _loadData();
        }

        /// <summary>
        /// データを読み込む
        /// </summary>
        private void _loadData()
        {
            if (File.Exists(_commonGroupSettingPath) == true)
            {
                string data = File.ReadAllText(_commonGroupSettingPath).Replace("\r",string.Empty);
                var list =data.Split('\n');

                foreach (var item in list)
                {
                    if (item.Contains(",") == false)
                    {
                        continue;
                    }
                    var csv = item.Split(',');
                    this[csv[0]] = csv[1];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SavaData()
        {
            using (var fs = File.CreateText(_commonGroupSettingPath))
            {
                foreach (var key in this.Keys)
                {
                    fs.WriteLine(key+","+this[key]);
                }
                fs.Close();
            }
        }
    }
}
