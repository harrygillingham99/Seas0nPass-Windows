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

namespace Seas0nPass.Presenters
{
    public class DownloadPresenter
    {
        public enum ProcessResult
        {
            Completed,
            Cancelled,
            Failed
        }

        private ProcessResult _result = ProcessResult.Completed;
        public ProcessResult Result
        {
            get { return _result; }
        }

        private IDownloadModel _model;
        private IDownloadView _view;

        public event EventHandler ProcessFinished;

        private IFirmwareVersionModel _firmwareVersionModel;

        public void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel)
        {
            this._firmwareVersionModel = firmwareVersionModel;
        }

        public DownloadPresenter(IDownloadModel model, IDownloadView view)
        {
            this._model = model;
            this._view = view;

            model.ProgressChanged += model_ProgressChanged;
            view.ActionButtonClicked += view_ActionButtonClicked;
            model.DownloadCompleted += model_DownloadFinished;
            model.DownloadFailed += model_DownloadFailed;
            model.DownloadCanceled += model_DownloadCanceled;
        }

        private void model_DownloadFailed(object sender, EventArgs e)
        {
            _result = ProcessResult.Failed;
            if (ProcessFinished != null)
                ProcessFinished(sender, e);
        }

        private void model_DownloadFinished(object sender, EventArgs e)
        {
            _result = ProcessResult.Completed;
            if (ProcessFinished != null)
                ProcessFinished(sender, e);
        }

        private void model_DownloadCanceled(object sender, EventArgs e)
        {
            _result = ProcessResult.Cancelled;
            if (ProcessFinished != null)
                ProcessFinished(sender, e);
        }

        public void StartProcess()
        {
            _view.SetMessageText($"Downloading {Path.GetFileName(_firmwareVersionModel.ExistingFirmwarePath)}...");
            _view.SetActionButtonText("Cancel");
            _model.StartDownload();
        }

        private void view_ActionButtonClicked(object sender, EventArgs e)
        {
            _model.CancelDownload();
        }

        private void model_ProgressChanged(object sender, EventArgs e)
        {
            _view.UpdateProgress(_model.Percentage);
        }
    }
}
