////
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
using System.Globalization;
using System.IO;
using Seas0nPass.Interfaces;
using Seas0nPass.Utils;

namespace Seas0nPass.Models
{
    public class TetherModel : ITetherModel
    {
        private string _currentMessage;

        public void StartProcess()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ProcessFinished != null)
                ProcessFinished(sender, e);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RunTether();
        }

        private void RestoreDfuAndTetherFiles()
        {
            LogUtil.LogEvent("Restoring DFU and Tether file");

            string kernelCache = Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.KERNEL_CACHE_FILE_NAME);
            if (SafeFile.Exists(kernelCache))
                SafeFile.Copy(kernelCache, Path.Combine(MiscUtils.BIN_DIRECTORY, MiscUtils.KERNEL_CACHE_FILE_NAME), true);

            string iBss = Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBSS_FILE_NAME);
            if (SafeFile.Exists(iBss))
                SafeFile.Copy(iBss, Path.Combine(MiscUtils.BIN_DIRECTORY, MiscUtils.IBSS_FILE_NAME), true);

            string iBec = Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBEC_FILE_NAME);
            if (SafeFile.Exists(iBec))
                SafeFile.Copy(iBec, Path.Combine(MiscUtils.BIN_DIRECTORY, MiscUtils.IBEC_FILE_NAME), true);
        }

        private void RunTether()
        {
            RestoreDfuAndTetherFiles();

            SafeDirectory.SetCurrentDirectory(MiscUtils.BIN_DIRECTORY);

            LogUtil.LogEvent("Tether process starting");

            var p = WinProcessUtil.StartNewProcess();
            p.StartInfo.FileName = @"tether.exe";

            var files = new List<string>();
            if (File.Exists(MiscUtils.IBSS_FILE_NAME))
                files.Add(MiscUtils.IBSS_FILE_NAME);
            if (File.Exists(MiscUtils.IBEC_FILE_NAME))
                files.Add(MiscUtils.IBEC_FILE_NAME);
            if (File.Exists(MiscUtils.KERNEL_CACHE_FILE_NAME))
                files.Add(MiscUtils.KERNEL_CACHE_FILE_NAME);
            string arguments = string.Join(" ", files);

            p.StartInfo.Arguments = arguments;

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WorkingDirectory = SafeDirectory.GetCurrentDirectory();

            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += (sender, e) => HandleOutputData(e.Data);
            p.ErrorDataReceived += (sender, e) => HandleOutputData(e.Data);

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                var errorString = string.Format("Process: {0}, args: {1} exited with non-zero code", p.StartInfo.FileName, p.StartInfo.Arguments);
                LogUtil.LogEvent(errorString);
                throw new InvalidOperationException(errorString);
            }
        }

        public void HandleOutputData(string data)
        {
            if (data == null)
                return;

            LogUtil.LogEvent(string.Format("Output received: {0}", data));

            if (data.StartsWith("::"))
            {

                _currentMessage = data.Substring(2);
                if (CurrentMessageChanged != null)
                    CurrentMessageChanged(this, EventArgs.Empty);
            }

            if (data.StartsWith("##"))
            {
                var percentString = data.Substring(3, data.Length - 4);
                var info = new CultureInfo("en-US");
                _progressPercentage = Convert.ToInt32(double.Parse(percentString, info), info);

                if (ProgressChanged != null)
                    ProgressChanged(this, EventArgs.Empty);
            }

        }

        public event EventHandler ProgressChanged;
        public event EventHandler ProcessFinished;
        public event EventHandler CurrentMessageChanged;

        public string CurrentMessage => _currentMessage;

        private int _progressPercentage;
        public int ProgressPercentage => _progressPercentage;

        private IFirmwareVersionModel _firmwareVersionModel;
        public void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel)
        {
            _firmwareVersionModel = firmwareVersionModel;
        }
    }
}
