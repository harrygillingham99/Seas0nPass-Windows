﻿////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Seas0nPass.Utils
{
    public static class MiscUtils
    {
        public static readonly string DOWNLOADED_FILE_PATH = "firmware.ipsw";
        public static readonly string KERNEL_CACHE_FILE_NAME = @"kernelcache.release.k66";
        public static readonly string OUTPUT_FOLDER_NAME = @"OUTPUT";
        public static readonly string FIRMWARE_FOLDER_NAME = "Firmware";
        public static readonly string IBSS_FILE_NAME = "iBSS.k66ap.RELEASE.dfu";
        public static readonly string IBEC_FILE_NAME = "iBEC.k66ap.RELEASE.dfu";
        public static readonly string DFU_FOLDER_NAME = "dfu";
        public static readonly string OUTPUT_FIRMWARE_NAME = "output.ipsw";        
        public static readonly string WORKING_FOLDER = Path.Combine(Path.GetTempPath(), "Seas0nPass");
        public static readonly string BIN_DIRECTORY = Path.Combine(WORKING_FOLDER, "BIN");
        public static readonly string PATCHES_DIRECTORY = Path.Combine(WORKING_FOLDER, "PATCHES");
        public static readonly string COMMANDS_FILE_NAME = "commands.fc";

        public static readonly string DOCUMENTS_HOME;

        static MiscUtils()
        {
            string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrWhiteSpace(myDocumentsPath)) // currupted MyDocuments path
            {
                myDocumentsPath = Application.StartupPath; // use startup path
            }

            DOCUMENTS_HOME = Path.Combine(myDocumentsPath, "Seas0nPass");
        }

        public static string ComputeMd5(string filePath)
        {
            var sb = new StringBuilder();
            byte[] hash;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                var md5 = new MD5CryptoServiceProvider();
                hash = md5.ComputeHash(fs);
            }
            foreach (byte hex in hash)
                sb.Append(hex.ToString("x2"));
            return sb.ToString();
        }

        public static List<string> GetAllFoldersInFolder(string folder)
        {
            return new List<string>(
                from original in Directory.GetDirectories(folder, "*", SearchOption.AllDirectories)
                select original.Remove(0, (folder + Path.DirectorySeparatorChar).Length)
            );
        }

        public static List<string> GetAllFileInFodler(string folder)
        {
            return new List<string>(
                from original in SafeDirectory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                select original.Remove(0, (folder + Path.DirectorySeparatorChar).Length)
            );
        }

        public static void RecreateDirectory(string dirPath)
        {
            if (SafeDirectory.Exists(dirPath))
            {
                try
                {
                    SafeDirectory.Delete(dirPath, true);
                }
                catch (IOException ex)
                {
                    LogUtil.LogException(ex);
                    if (SafeDirectory.EnumerateFiles(dirPath).Any())
                        throw;
                    return; // Can't delete dir, but it is empty => skip it
                }
            }
            SafeDirectory.CreateDirectory(dirPath);
        }

        public static void CopyDirectory(string src, string dst)
        {
            string[] files;

            if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                dst += Path.DirectorySeparatorChar;
            if (!SafeDirectory.Exists(dst)) SafeDirectory.CreateDirectory(dst);
            files = SafeDirectory.GetFileSystemEntries(src);
            foreach (string element in files)
            {
                // Sub directories

                if (SafeDirectory.Exists(element))
                    CopyDirectory(element, dst + Path.GetFileName(element));
                // Files in directory

                else
                    SafeFile.Copy(element, dst + Path.GetFileName(element), true);
            }
        }

        private static IEnumerable<ProcessStartInfo> ParseResource(string resourceName)
        {
            var lines = ScriptResource.ResourceManager.GetString(resourceName);

            foreach (var line in lines.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var exePath = line.Substring(0, line.IndexOf(' ')).Trim();
                var args = line.Substring(exePath.Length).Trim();
                yield return new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = SafeDirectory.GetCurrentDirectory(),
                };
            }
        }

        public static void OpenExplorerWindow(string fileToSelect)
        {
            LogUtil.LogEvent("opening explorer window");
            if (!SafeFile.Exists(fileToSelect))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument =  string.Format("/select, \"{0}\"",fileToSelect);

            Process.Start("explorer.exe", argument);
        }

        public static void CleanUp()
        {
            SafeDirectory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            LogUtil.LogEvent("clean up");
            try 
            {
                foreach (var name in GetProcessesToKill())
                {
                    foreach (var process in Process.GetProcessesByName(name))
                    {
                        LogUtil.LogEvent(string.Format("{0} process kill", name));
                        try
                        {
                            process.Kill();
                            process.WaitForExit(1000); // wait for exit no longer than 1 second
                        }
                        catch (Win32Exception)
                        {
                        }
                        catch (InvalidOperationException)
                        {
                        }
                    }
                }
                if (SafeDirectory.Exists(WORKING_FOLDER))
                    SafeDirectory.Delete(WORKING_FOLDER, true);
            } 
            catch (Exception ex) 
            {
                LogUtil.LogException(ex);
                // Do nothing
            }
        }

        public static IEnumerable<string> GetProcessesToKill()
        {
            foreach(var line in ScriptResource.ProgramsToKill.Split(new [] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                yield return line;
            }
        }
    }
}
