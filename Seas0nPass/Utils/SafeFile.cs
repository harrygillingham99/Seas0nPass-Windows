using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Seas0nPass.Utils
{
    public static class SafeFile
    {       

        public static void Copy(string from, string to)
        {
            BaseIoUtils.RepeatActionWithDelay(() => File.Copy(from, to));
        }

        public static void Copy(string from, string to, bool @override)
        {
            BaseIoUtils.RepeatActionWithDelay(() => File.Copy(from, to, @override));
        }



        public static FileStream Create(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<FileStream>(() => File.Create(name));
        }

        public static FileStream OpenRead(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<FileStream>(() => File.OpenRead(name));
        }

        public static bool Exists(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<bool>(() => File.Exists(name));
        }

        public static void Delete(string name)
        {
            BaseIoUtils.RepeatActionWithDelay(() => File.Delete(name));
        }

        public static void Move(string from, string to)
        {
            BaseIoUtils.RepeatActionWithDelay(() => File.Move(from, to));
        }
        
    
    }
}
