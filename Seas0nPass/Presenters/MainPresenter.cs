////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;
using System.Linq;
using Seas0nPass.Interfaces;
using Seas0nPass.CustomEventArgs;
using System.IO;
using System.Threading;
using Seas0nPass.Utils;


namespace Seas0nPass.Presenters
{
    public class MainPresenter
    {
        private DfuPresenter _dfuPresenter;
        private PatchPresenter _patchPresetner;
        private TetherPresenter _tetherPresenter;
        private IMainView _view;
        public MainPresenter(IMainView view)
        {
            this._view = view;
        }

        private IStartView _startControl;
        private IDownloadModel _downloadModel;
        private IDownloadView _downloadView;
        private IPatchView _patchControl;
        private IPatchModel _patchModel;
        private IDfuView _dfuControl;
        private IDfuModel _dfuModel;
        private IMainModel _mainModel;

        private DownloadPresenter _downloadPresenter;

        private IDfuSuccessControl _dfuSuccessControl;
        private ITetherSuccessControl _tetherSuccessControl;
        private IFirmwareVersionModel _firmwareVersionModel;
        private IFirmwareVersionDetector _firmwareVersionDetector;

        private ITetherView _tetherView;
        private ITetherModel _tetherModel;
        private IITunesAutomationModel _iTunesAutomationModel;


        private IFreeSpaceModel _freeSpaceModel;
        private IITunesInfoProvider _iTunesInfoProvider;
        private SynchronizationContext _syncContext;

        private void InstantiateModelsAndViews()
        {
            _startControl = IoC.Resolve<IStartView>();
            _syncContext = _startControl.SyncContext;
            _downloadModel = IoC.Resolve<IDownloadModel>();
            _downloadView = IoC.Resolve<IDownloadView>();
            _patchControl = IoC.Resolve<IPatchView>();
            _patchModel = IoC.Resolve<IPatchModel>();
            _dfuControl = IoC.Resolve<IDfuView>();
            _dfuModel = IoC.Resolve<IDfuModel>();
            _dfuSuccessControl = IoC.Resolve<IDfuSuccessControl>();
            _tetherSuccessControl = IoC.Resolve<ITetherSuccessControl>();
            _mainModel = IoC.Resolve<IMainModel>();
            _firmwareVersionModel = IoC.Resolve<IFirmwareVersionModel>();
            _tetherView = IoC.Resolve<ITetherView>();
            _tetherModel = IoC.Resolve<ITetherModel>();
            _firmwareVersionDetector = IoC.Resolve<IFirmwareVersionDetector>();
            _freeSpaceModel = IoC.Resolve<IFreeSpaceModel>();
            _iTunesInfoProvider = IoC.Resolve<IITunesInfoProvider>();

            _iTunesAutomationModel = IoC.Resolve<IITunesAutomationModel>();
            _iTunesAutomationModel.FirmwareVersionModel = _firmwareVersionModel;
            _iTunesAutomationModel.SyncContext = _syncContext;
            _iTunesAutomationModel.TunesInfoProvider = _iTunesInfoProvider;

            _mainModel.SetFirmwareVersionModel(_firmwareVersionModel);
            _downloadModel.SetFirmwareVersionModel(_firmwareVersionModel);
            _patchModel.SetFirmwareVersionModel(_firmwareVersionModel);
            _dfuModel.SetFirmwareVersionModel(_firmwareVersionModel);
            _tetherModel.SetFirmwareVersionModel(_firmwareVersionModel);

            _tetherPresenter = new TetherPresenter(_tetherModel, _tetherView);
            _tetherPresenter.ProcessFinished += tetherPresenter_ProcessFinished;
            _patchPresetner = new PatchPresenter(_patchControl, _patchModel);
            _patchPresetner.Finished += patchPresetner_Finished;
            _dfuPresenter = new DfuPresenter(_dfuModel, _dfuControl);
            _dfuPresenter.ProcessFinished += dfuPresenter_ProcessFinished;
            _downloadPresenter = new DownloadPresenter(_downloadModel, _downloadView);
            _downloadPresenter.ProcessFinished += downloadPresenter_ProcessFinished;
        }

        private void ShowStartPage()
        {
            var patchedVersion = _firmwareVersionDetector.Version;

            if (patchedVersion == null)
                _startControl.DisableTether();
            else if (patchedVersion.NeedTether)
                _startControl.EnableTether();
            else
                _startControl.SetTetherNotRequiredState();

            _startControl.ResetState();

            _view.ShowControl(_startControl);
        }

        private void CheckForProgramsToWarn()
        {
            var programsToWarn = _mainModel.GetProgramsToWarnNames();
            if (!programsToWarn.Any())
                return;
            _view.ShowProgramsWarning(programsToWarn);
        }

        public void HandleCrash()
        {
            _view.ShowCrashMessage();
            ShowStartPage();
        }

        public bool Init()
        {
            MiscUtils.CleanUp();

            InstantiateModelsAndViews();

            _view.Loaded += new EventHandler(view_Loaded);

            TunesInfo iTunesInfo = _iTunesInfoProvider.CheckITunesVersion();
            if (!iTunesInfo.IsCompatible)
            {
                _view.ShowCompatibleITunesVersionIsNotInstalled(iTunesInfo.RequiredVersion, iTunesInfo.InstalledVersion);
                return false;
            }
          

            _startControl.InitFirmwaresList(_firmwareVersionModel.KnownVersions.ToArray());

            _startControl.CreateIpswClicked += startControl_CreateIPSWClicked;
            _startControl.CreateIpswFwVersionClicked += startControl_CreateIPSW_fwVersion_Clicked;
            _startControl.TetherClicked += startControl_TetherClicked;
            _dfuSuccessControl.ButtonClicked += dfuSuccessControl_ButtonClicked;
            _tetherSuccessControl.ButtonClicked += tetherSuccessControl_ButtonClicked;
            ShowStartPage();
            
            return true;
        }

        void view_Loaded(object sender, EventArgs e)
        {
            CheckForProgramsToWarn();
        }

        private void startControl_CreateIPSW_fwVersion_Clicked(object sender, CreateIpswFirmwareClickedEventArgs e)
        {
            if (!_freeSpaceModel.IsEnoughFreeSpace())
            {
                _view.ShowNotEnoughFreeSpaceMessage();
                return;
            }

            _firmwareVersionModel.SelectedVersion = e.FirmwareVersion;
            DoDownload();
        }

        private void tetherSuccessControl_ButtonClicked(object sender, EventArgs e)
        {
            ShowStartPage();
        }

        private void dfuSuccessControl_ButtonClicked(object sender, EventArgs e)
        {
            ShowStartPage();
        }

        private void StartTetherProcess()
        {
            _view.ShowControl(_tetherView);
            _tetherPresenter.StartProcess();
        }

        private void startControl_TetherClicked(object sender, EventArgs e)
        {
            var detectedVersion = _firmwareVersionDetector.Version;
            if (detectedVersion == null)
                return;

            _firmwareVersionModel.SelectedVersion = detectedVersion;

            if (detectedVersion.NeedTether)
                StartTetherProcess();
            else
                _view.ShowTetherMessage(detectedVersion.Folder);
        }

        private void tetherPresenter_ProcessFinished(object sender, EventArgs e)
        {
            _view.ShowControl(_tetherSuccessControl);
        }

        private void DoDownload()
        {
            _downloadPresenter.SetFirmwareVersionModel(_firmwareVersionModel);
            _view.ShowControl(_downloadView);
            _downloadPresenter.StartProcess();
        }

        private void startControl_CreateIPSWClicked(object sender, CreateIpswClickedEventArgs e)
        {
            if (!_freeSpaceModel.IsEnoughFreeSpace())
            {
                _view.ShowNotEnoughFreeSpaceMessage();
                return;
            }

            var fileName = e.FileName;

            if (!String.IsNullOrEmpty(fileName))
            {
                LogUtil.LogEvent($"User has manually selected original firmware path: {fileName}");
                try
                {
                    _firmwareVersionModel.CheckVersion(fileName);
                }
                catch (IOException ex)
                {
                    LogUtil.LogEvent(string.Format("IOException during firmware MD5 check", fileName));
                    LogUtil.LogException(ex);
                    _view.ShowCantAccessOriginalFirmwareMessage(fileName);
                    return;
                }

                if (_firmwareVersionModel.SelectedVersion == null)
                    return;
                _firmwareVersionModel.ExistingFirmwarePath = fileName;
            }

            DoDownload();
        }

        private void downloadPresenter_ProcessFinished(object sender, EventArgs e)
        {
            if (_downloadPresenter.Result == DownloadPresenter.ProcessResult.Failed)
            {
                _view.ShowDownloadFailedMessage();
                ShowStartPage();
                return;
            }

            if (_downloadPresenter.Result == DownloadPresenter.ProcessResult.Cancelled)
            {
                ShowStartPage();
                return;
            }

            _view.ShowControl(_patchControl);
            _patchPresetner.StartPatch();
        }

        private void patchPresetner_Finished(object sender, EventArgs e)
        {
            _firmwareVersionDetector.SaveState(_firmwareVersionModel.SelectedVersion);

            _view.ShowControl(_dfuControl);
            _dfuPresenter.StartProcess();
        }

        private void dfuPresenter_ProcessFinished(object sender, EventArgs e)
        {
            _view.ShowControl(_dfuSuccessControl);

            if (_view.ConfirmITunesAutomation())
                _iTunesAutomationModel.Run();
            else
            {
                MiscUtils.OpenExplorerWindow(_firmwareVersionModel.PatchedFirmwarePath);
                _view.ShowManualRestoreInstructions(Path.GetFileName(_firmwareVersionModel.PatchedFirmwarePath));
            }
        }
    }
}
