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
using System.IO;
using System.ComponentModel;
using Seas0nPass.Utils;

namespace Seas0nPass.Models
{
    public class PatchModel : IPatchModel
    {
        private IPatch _patch;
        private IFirmwareVersionModel _firmwareVersionModel;
        private int _currentProgress;
        private string _currentMessage;

        public event EventHandler CurrentMessageChanged;
        public event EventHandler ProgressUpdated;
        public event EventHandler Finished;

        public int CurrentProgress
        {
            get { return _currentProgress; }
        }

        public string CurrentMessage
        {
            get { return _currentMessage; }
        }

        public void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel)
        {
            this._firmwareVersionModel = firmwareVersionModel;
        }

        public void StartProcess()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += (sender, args) => { if (args.Error != null) throw args.Error; };
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformPatch();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private void UpdateProgress(int value)
        {
            _currentProgress = value;
            if (ProgressUpdated != null)
                ProgressUpdated(this, EventArgs.Empty);
        }

        private void UpdateCurrentMessage(string message)
        {
            LogUtil.LogEvent(string.Format("Patching message changed to: {0}", message));

            _currentMessage = message;
            if (CurrentMessageChanged != null)
                CurrentMessageChanged(this, EventArgs.Empty);
        }

        private void SaveDfuAndTetherFiles()
        {
            LogUtil.LogEvent(string.Format("Saving {0} and {1} files", MiscUtils.KERNEL_CACHE_FILE_NAME, MiscUtils.IBSS_FILE_NAME));

            MiscUtils.RecreateDirectory(_firmwareVersionModel.AppDataFolder);

            LogUtil.LogEvent(string.Format("Directory {0} recreated successfully", _firmwareVersionModel.AppDataFolder));

            string kernelcache = Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.OUTPUT_FOLDER_NAME, MiscUtils.KERNEL_CACHE_FILE_NAME);
            if (SafeFile.Exists(kernelcache))
            {
                SafeFile.Copy(kernelcache, Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.KERNEL_CACHE_FILE_NAME), true);
                LogUtil.LogEvent(string.Format("{0} file copied successfully", MiscUtils.KERNEL_CACHE_FILE_NAME));
            }

            string iBss = Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.OUTPUT_FOLDER_NAME, MiscUtils.FIRMWARE_FOLDER_NAME, MiscUtils.DFU_FOLDER_NAME, MiscUtils.IBSS_FILE_NAME);
            if (SafeFile.Exists(iBss))
            {
                SafeFile.Copy(iBss, Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBSS_FILE_NAME), true);
                LogUtil.LogEvent(string.Format("{0} file copied successfully", MiscUtils.IBSS_FILE_NAME));
            }

            string iBec = Path.Combine(MiscUtils.WORKING_FOLDER, MiscUtils.OUTPUT_FOLDER_NAME, MiscUtils.FIRMWARE_FOLDER_NAME, MiscUtils.DFU_FOLDER_NAME, MiscUtils.IBEC_FILE_NAME);
            if (_firmwareVersionModel.SelectedVersion.SaveIBec && SafeFile.Exists(iBec))
            {
                SafeFile.Copy(iBec, Path.Combine(_firmwareVersionModel.AppDataFolder, MiscUtils.IBEC_FILE_NAME), true);
                LogUtil.LogEvent(string.Format("{0} file copied successfully", MiscUtils.IBEC_FILE_NAME));
            }
        }

        private IPatch GetPatch()
        {
            if (_firmwareVersionModel.SelectedVersion != null)
                return new UniversalPatch(_firmwareVersionModel.SelectedVersion.CommandsText);
            throw new InvalidOperationException("Unknown firmware version");
        }

     

        private void PerformPatch()
        {
            _patch = GetPatch();

            _patch.CurrentMessageChanged += patch_CurrentMessageChanged;
            _patch.CurrentProgressChanged += patch_CurrentProgressChanged;

            string resultFile = _patch.PerformPatch();

            _patch.CurrentMessageChanged -= patch_CurrentMessageChanged;
            _patch.CurrentProgressChanged -= patch_CurrentProgressChanged;
            _patch = null;

            SaveDfuAndTetherFiles();

            SafeFile.Copy(resultFile, _firmwareVersionModel.PatchedFirmwarePath, true);           

            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }

        private void patch_CurrentMessageChanged(object sender, EventArgs args)
        {
            UpdateCurrentMessage(_patch.CurrentMessage);
        }

        private void patch_CurrentProgressChanged(object sender, EventArgs args)
        {
            UpdateProgress(_patch.CurrentProgress);
        }
    }
}
