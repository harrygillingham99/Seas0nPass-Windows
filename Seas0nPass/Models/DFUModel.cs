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
using Seas0nPass.Interfaces;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using Seas0nPass.Utils;

namespace Seas0nPass.Models
{
    public class DfuModel : IDfuModel
    {
        public event EventHandler ProcessFinished;
        public event EventHandler CurrentMessageChanged;
        public event EventHandler ProgressChanged;

        private string _currentMessage;
        public string CurrentMessage
        {
            get { return _currentMessage; }
        }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
        }

        private IFirmwareVersionModel _firmwareVersionModel;
        public void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel)
        {
            this._firmwareVersionModel = firmwareVersionModel;
        }

        public void StartProcess()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ProcessFinished != null)
                ProcessFinished(sender, e);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            RunDfu();
        }

        private void RestoreDfuFile()
        {
            LogUtil.LogEvent("Restoring DFU file");

            string iBss = Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBSS_FILE_NAME);
            if (SafeFile.Exists(iBss))
                SafeFile.Copy(iBss, Path.Combine(MiscUtils.BIN_DIRECTORY, MiscUtils.IBSS_FILE_NAME), true);

            string iBec = Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBEC_FILE_NAME);
            if (SafeFile.Exists(iBec))
                SafeFile.Copy(iBec, Path.Combine(MiscUtils.BIN_DIRECTORY, MiscUtils.IBEC_FILE_NAME), true);
        }

        private void RunDfu()
        {
            RestoreDfuFile();

            SafeDirectory.SetCurrentDirectory(MiscUtils.BIN_DIRECTORY);

            var files = new List<string>();
            if (SafeFile.Exists(MiscUtils.IBSS_FILE_NAME))
                files.Add(MiscUtils.IBSS_FILE_NAME);
            if (SafeFile.Exists(MiscUtils.IBEC_FILE_NAME))
                files.Add(MiscUtils.IBEC_FILE_NAME);
            string arguments = string.Join(" ", files);

            LogUtil.LogEvent($"DFU process starting for {arguments}");
            RunDfuProcess(arguments);
        }

        private void RunDfuProcess(string arguments)
        {
            var p = WinProcessUtil.StartNewProcess();
            p.StartInfo.FileName = @"dfu.exe";
            p.StartInfo.Arguments = arguments;

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
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
                var errorString =
                    $"Process: {p.StartInfo.FileName}, args: {p.StartInfo.Arguments} exited with non-zero code";
                LogUtil.LogEvent(errorString);
                throw new InvalidOperationException(errorString);
            }
        }

        public void HandleOutputData(string data)
        {
            if (data == null)
                return;

            LogUtil.LogEvent($"Output received: {data}");

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
    }
}
