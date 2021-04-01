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
using System.Windows.Forms;
using Seas0nPass.Interfaces;

namespace Seas0nPass
{
    public partial class MainForm : Form, IMainView
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public event EventHandler Loaded;

        public void ShowCantAccessOriginalFirmwareMessage(string filePath)
        {
            if (InvokeRequired)
                Invoke((Action) (() => ShowCantAccessOriginalFirmwareMessage(filePath)));
            else
                MessageBox.Show(
                    this,
                    $@"The firmware file {filePath} is either corrupt or not accessible. 
                    Please check the firmware file and save it to the local disk.",
                    @"Seas0nPass",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
        }


        public void ShowNotEnoughFreeSpaceMessage()
        {
            if (InvokeRequired)
                Invoke((Action) ShowNotEnoughFreeSpaceMessage);
            else
                MessageBox.Show(
                    this,
                    @"Seas0nPass requires at least 3.5GB of free disk space. 
                       Please check to ensure you have enough disk space available and re-run Seas0nPass.",
                    @"Seas0nPass",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
        }

        public void ShowManualRestoreInstructions(string fileName)
        {
            if (InvokeRequired)
            {
                Invoke((Action) (() => ShowManualRestoreInstructions(fileName)));
            }
            else
            {
                var text = "Follow these steps to restore the custom firmware to the AppleTV.\n\n" +
                           "1. Open iTunes and select the AppleTV from the sidebar.\n" +
                           "2. Hold down 'Shift' and click the 'Restore' button.\n" +
                           "3. Select the {0} file located in My Documents";

                MessageBox.Show(this, string.Format(text, fileName), @"Seas0nPass", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


        public void ShowDownloadFailedMessage()
        {
            if (InvokeRequired)
                Invoke((Action) ShowDownloadFailedMessage);
            else
                MessageBox.Show(
                    this,
                    @"Unable to download firmware. Please check your internet connection or firewall settings and try again.",
                    @"Seas0nPass",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
        }

        public void ShowTetherMessage(string fwName)
        {
            if (InvokeRequired)
                Invoke((Action) (() => ShowTetherMessage(fwName)));
            else
                MessageBox.Show(this, $@"The {fwName} firmware is untethered and does not require this process!",
                    @"Untethered Jailbreak");
        }

        public void ShowControl(IView control)
        {
            Action action = delegate
            {
                contentPanel.Controls.Clear();
                var castedControl = (Control) control;
                contentPanel.Controls.Add(castedControl);
                castedControl.Dock = DockStyle.Fill;
            };
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        public void ShowProgramsWarning(IEnumerable<string> programNames)
        {
            var programsString = "";
            var i = 0;
            foreach (var programName in programNames)
            {
                if (programsString != "")
                    programsString += "\n";
                programsString += $"{++i}. {programName}";
            }

            MessageBox.Show(
                this,
                $@"The program(s) listed below may prevent Seas0nPass from running correctly. Please close these program(s) before continuing.
                {programsString}",
                @"Seas0nPass",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public void ShowCrashMessage()
        {
            MessageBox.Show(this,
                @"Seas0nPass has detected critical error(s) and cannot continue.
                Please ensure no conflicting software is running and try again.",
                @"Seas0nPass",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Loaded?.Invoke(this, e);
        }
    }
}