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

namespace Seas0nPass.Controls
{
    public partial class DfuSuccessControl : UserControl, IDfuSuccessControl
    {
        public DfuSuccessControl()
        {
            InitializeComponent();
            _successMessage = label1.Text;
        }

        private string _successMessage;

        public event EventHandler ButtonClicked;

        private void button_Click(object sender, EventArgs e)
        {
            if (ButtonClicked != null)
                ButtonClicked(sender, e);
        }       
    }
}
