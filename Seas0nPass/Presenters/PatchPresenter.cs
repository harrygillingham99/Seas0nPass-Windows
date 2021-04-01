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

namespace Seas0nPass.Presenters
{
    public class PatchPresenter
    {
        private IPatchView _view;
        private IPatchModel _model;
        public PatchPresenter(IPatchView view, IPatchModel model)
        {
            this._view = view;
            this._model = model;            
            model.ProgressUpdated += model_ProgressUpdated;
            model.CurrentMessageChanged += model_CurrentMessageChanged;
            model.Finished += model_Finished;

        }

        void model_CurrentMessageChanged(object sender, EventArgs e)
        {
            _view.SetMessageText(_model.CurrentMessage);
        }

        void model_Finished(object sender, EventArgs e)
        {
            if (Finished != null)
                Finished(sender, e);
        }

        void model_ProgressUpdated(object sender, EventArgs e)
        {
            _view.UpdateProgress(_model.CurrentProgress);
        }

        public event EventHandler Finished;

        public void StartPatch()
        {            
            _view.SetActionButtonText("Cancel");
            _model.StartProcess();
        }
    }
}
