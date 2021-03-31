////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System;
using System.Drawing;
using System.Windows.Forms;
using Seas0nPass.Interfaces;

namespace Seas0nPass.Controls
{
    public partial class DfuControl : UserControl, IDfuView
    {
        private readonly Image _dfuImage;
        private readonly Image _iTVimage;
        public DfuControl()
        {
            InitializeComponent();
            _dfuImage = pictureBox1.BackgroundImage;
            _iTVimage = pictureBox1.Image;
            pictureBox1.Image = null;
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

        public bool HintVisibility
        {
            get
            { 
               return hintLabel.Visible;

            }
            set
            {
                Action impl = () =>
                {
                    hintLabel.Visible = value;
                    pictureBox1.BackgroundImage = value ? _dfuImage : _iTVimage;
                };
                if (InvokeRequired)
                    Invoke(impl);
                else
                    impl();
            }
        }
    }
}
