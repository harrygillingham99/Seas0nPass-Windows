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
using System.Linq;
using System.Text;
using Seas0nPass.Interfaces;

namespace Seas0nPass.Presenters
{
    public class DfuPresenter
    {

        private IDfuModel _model;
        private IDfuView _view;

        public event EventHandler ProcessFinished;

        public DfuPresenter(IDfuModel model, IDfuView view )
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

        void model_ProcessFinished(object sender, EventArgs e)
        {
            if (ProcessFinished != null)
                ProcessFinished(sender, e);

            _view.Clear();
        }

        void model_CurrentMessageChanged(object sender, EventArgs e)
        {
            if (_model.CurrentMessage == "Found device in DFU mode...")            
                _view.HintVisibility = false;            
            _view.SetMessageText(_model.CurrentMessage);
        }

        public void StartProcess()
        {
            _view.HintVisibility = true;            
            _model.StartProcess();
        }
    }
}
