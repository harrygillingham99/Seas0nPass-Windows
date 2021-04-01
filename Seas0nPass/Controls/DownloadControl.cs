﻿////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System;
using System.Windows.Forms;
using Seas0nPass.Interfaces;

namespace Seas0nPass.Controls
{
    public partial class DownloadControl : UserControl, IDownloadView
    {
        public DownloadControl()
        {
            InitializeComponent();
        }

        

        public void SetMessageText(string text)
        {
            label.Text = text;
        }

        public void UpdateProgress(int value)
        {
            Action action = delegate { progressBar.Value = value; progressBar.Refresh(); };

            if (InvokeRequired)
                Invoke(action);
            else
                action();

            
        }

        public event EventHandler ActionButtonClicked;

        private void actionButton_Click(object sender, EventArgs e)
        {
            if (ActionButtonClicked != null)
                ActionButtonClicked(sender, e);
        }      


        public void SetActionButtonText(string text)
        {
            actionButton.Text = text;
        }
        
    }
}
