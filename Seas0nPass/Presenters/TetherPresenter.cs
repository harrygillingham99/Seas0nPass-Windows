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
    public class TetherPresenter
    {
        private ITetherModel _model;
        private ITetherView _view;
        public TetherPresenter(ITetherModel model, ITetherView view)
        {
            this._model = model;
            this._view = view;
            model.CurrentMessageChanged += new EventHandler(model_CurrentMessageChanged);
            model.ProgressChanged += new EventHandler(model_ProgressChanged);
            model.ProcessFinished += new EventHandler(model_ProcessFinished);
        }

        void model_ProgressChanged(object sender, EventArgs e)
        {
            _view.UpdateProgress(_model.ProgressPercentage);
        }

        void model_CurrentMessageChanged(object sender, EventArgs e)
        {
            _view.SetMessageText(_model.CurrentMessage);
        }

        void model_ProcessFinished(object sender, EventArgs e)
        {
            if (ProcessFinished != null)
                ProcessFinished(sender, e);

            _view.Clear();
        }

        public event EventHandler ProcessFinished;

        public void StartProcess()
        {
            _model.StartProcess();
        }
    }
}
