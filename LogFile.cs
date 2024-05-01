using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup
{
    /// <summary>
    /// ログファイル
    /// </summary>
    class LogFile
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        public static LogFile Instance { get; }= new LogFile();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private LogFile() 
        {
            if( File.Exists(_logFileName)==true)
            {
                File.Delete(_logFileName); 
            }
        }

        /// <summary>
        /// ログファイル名
        /// </summary>
        private string _logFileName = Directory.GetCurrentDirectory() + @"\log.txt";

        /// <summary>
        /// 作成処理
        /// </summary>
        public void Create()
        {
            var stream = new System.IO.StreamWriter(_logFileName);
            stream.AutoFlush = true;
            Console.SetOut(stream);
            //
        }
    }
}
