////
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

namespace Seas0nPass
{
    public partial class TetherControl : UserControl, ITetherView
    {
        public TetherControl()
        {
            InitializeComponent();
        }

        

        public void SetMessageText(string text)
        {
            Action action = delegate { this.label.Text = text; };

            if (InvokeRequired)
                Invoke(action);
            else
                action();

        }

        public void UpdateProgress(int value)
        {
            Action action = delegate 
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = value; 
                progressBar.Refresh(); 
            };

            if (InvokeRequired)
                Invoke(action);
            else
                action();
           
        }

        public void Clear()
        {
            Action action = delegate { progressBar.Style = ProgressBarStyle.Marquee; label.Text = ""; };
            if (InvokeRequired)
                Invoke(action);
            else
                action();

        }

        
    }
}
