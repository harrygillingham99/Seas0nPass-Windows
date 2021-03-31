using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Seas0nPass.Utils
{
    public static class SafeDirectory
    {
        public static void CreateDirectory(string name)
        {
            BaseIoUtils.RepeatActionWithDelay(() => Directory.CreateDirectory(name));
        }

        public static bool Exists(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<bool>(() => Directory.Exists(name));
        }

        public static void Delete(string name, bool recursive)
        {
            BaseIoUtils.RepeatActionWithDelay(() => Directory.Delete(name, recursive));
        }

        public static void SetCurrentDirectory(string name)
        {
            BaseIoUtils.RepeatActionWithDelay(() => Directory.SetCurrentDirectory(name));
        }

        public static string[] GetDirectories(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<string[]>(() => Directory.GetDirectories(name));
        }

        public static string GetCurrentDirectory()
        {
            return BaseIoUtils.RepeatActionWithDelay<string>(() => Directory.GetCurrentDirectory());
        }

        public static string[] GetFiles(string name, string filter, SearchOption option)
        {
            return BaseIoUtils.RepeatActionWithDelay<string[]>(() => Directory.GetFiles(name, filter, option));
        }

        public static string[] GetFileSystemEntries(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<string[]>(() => Directory.GetFileSystemEntries(name));
        }

        public static IEnumerable<string> EnumerateFiles(string name)
        {
            return BaseIoUtils.RepeatActionWithDelay<IEnumerable<string>>(() => Directory.EnumerateFiles(name));
        }


        
        
    }
}
