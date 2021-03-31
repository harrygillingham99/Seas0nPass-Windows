////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;
using Seas0nPass.Interfaces;
using System.Net;
using System.IO;
using System.Threading;
using System.ComponentModel;
using Seas0nPass.Utils;

namespace Seas0nPass.Models
{
    public class DownloadModel : IDownloadModel
    {
        private readonly string _fileName = Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.DOWNLOADED_FILE_PATH);
        private readonly WebClient _webClient;
        private IFirmwareVersionModel _firmwareVersionModel;

        public event EventHandler ProgressChanged;
        public event EventHandler DownloadCompleted;
        public event EventHandler DownloadFailed;
        public event EventHandler DownloadCanceled;

        public int Percentage { get; private set; }

        public DownloadModel()
        {
            _webClient = new WebClient();
            _webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
        }

        private void PerformStart()
        {            
            if (SafeFile.Exists(_firmwareVersionModel.ExistingFirmwarePath) &&
                MiscUtils.ComputeMd5(_firmwareVersionModel.ExistingFirmwarePath) == _firmwareVersionModel.CorrectFirmwareMd5)
            {
                LogUtil.LogEvent("Original firmware found on disk");

                SafeFile.Copy(_firmwareVersionModel.ExistingFirmwarePath, Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.DOWNLOADED_FILE_PATH), true);
                if (DownloadCompleted != null)
                    DownloadCompleted(this, EventArgs.Empty);
                return;
            }

            LogUtil.LogEvent("Starting download");

            _webClient.DownloadFileAsync(new Uri(_firmwareVersionModel.DownloadUri), _fileName);
        }

        public void StartDownload()
        {            
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((sender, e) => PerformStart());
            worker.RunWorkerAsync();
        }

        private void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                LogUtil.LogEvent(string.Format("Download failed:\n{0}", e.Error));

                Percentage = 0;
                if (ProgressChanged != null)
                    ProgressChanged(sender, e);

                if (!e.Cancelled)
                {
                    if (DownloadFailed != null)
                        DownloadFailed(sender, e);
                    return;
                }
                else
                {
                    LogUtil.LogEvent("Download was cancelled by the user");
                    if (DownloadCanceled != null)
                        DownloadCanceled(sender, e);
                    return;
                }
            }

            LogUtil.LogEvent("Download completed");

            SafeFile.Copy(Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.DOWNLOADED_FILE_PATH), _firmwareVersionModel.ExistingFirmwarePath, true);

            LogUtil.LogEvent("Downloaded file copied to Documents folder");

            if (DownloadCompleted != null)
                DownloadCompleted(sender, e);
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                Percentage = e.ProgressPercentage;
                ProgressChanged(sender, e);
            }

        }

        public void CancelDownload()
        {
            if (!_webClient.IsBusy) // already canceled or used ExistingFirmwarePath
                return;

            LogUtil.LogEvent("Cancelling download");
            _webClient.CancelAsync();
            while (_webClient.IsBusy)
            {
                Thread.Sleep(50);
            }
            if (SafeFile.Exists(_fileName))
                SafeFile.Delete(_fileName);
        }

        public void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel)
        {
            this._firmwareVersionModel = firmwareVersionModel;
        }
    }
}
