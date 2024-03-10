using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxNestGroup
{
    class LogFile
    {
        public static LogFile Instance { get; }= new LogFile();

        private string _logFileName = Directory.GetCurrentDirectory() + @"\log.txt";
        private LogFile() 
        {
            if( File.Exists(_logFileName)==true)
            {
                File.Delete(_logFileName); 
            }
        }

        public void Create()
        {
            var stream = new System.IO.StreamWriter(_logFileName);
            stream.AutoFlush = true;
            Console.SetOut(stream);
            //
        }
    }
}
