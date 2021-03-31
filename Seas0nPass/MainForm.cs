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

        public void ShowCompatibleITunesVersionIsNotInstalled(string requiredITunesVersion, string installedITunesVersion)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => ShowCompatibleITunesVersionIsNotInstalled(requiredITunesVersion, installedITunesVersion)));
            }
            else
            {
                string messageText = string.IsNullOrEmpty(installedITunesVersion) ? $"Seas0nPass requires iTunes version {requiredITunesVersion} or later. " +
                                                                                    "Please install the latest iTunes version from the Apple website (www.apple.com)."
                    : $"Seas0nPass requires iTunes version {requiredITunesVersion} or later. " +
                      $"You have iTunes version {installedITunesVersion} installed. " +
                      "Please open Help -> Check For Updates in iTunes to install a more recent version.";
                MessageBox.Show(
                    owner: this,
                    text: messageText,
                    caption: "Seas0nPass",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Exclamation
                );
            }
        }


        public void ShowCantAccessOriginalFirmwareMessage(string filePath)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => ShowCantAccessOriginalFirmwareMessage(filePath)));
            }
            else
            {
                MessageBox.Show(
                    owner: this,
                    text: $"The firmware file {filePath} is either corrupt or not accessible. " +
                          "Please check the firmware file and save it to the local disk.",
                    caption: "Seas0nPass",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Exclamation
                );
            }
        }


        public void ShowNotEnoughFreeSpaceMessage()
        {
            if (InvokeRequired)
            {
                Invoke((Action)ShowNotEnoughFreeSpaceMessage);
            }
            else
            {
                MessageBox.Show(
                    owner: this,
                    text: "Seas0nPass requires at least 3.5GB of free disk space. Please check to ensure you have enough disk space available and re-run Seas0nPass.",
                    caption: "Seas0nPass",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Exclamation
                );
            }
        }

        public bool ConfirmITunesAutomation()
        {
            if (InvokeRequired)
            {
                return (bool)Invoke((Func<bool>)ConfirmITunesAutomation);
            }
            return
                MessageBox.Show(this, "Would you like iTunes to restore the firmware automatically?", "Seas0nPass", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;
        }

        public void ShowManualRestoreInstructions(string fileName)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => ShowManualRestoreInstructions(fileName)));
            }
            else
            {
                var text = "Follow these steps to restore the custom firmware to the AppleTV.\n\n" +
                           "1. Open iTunes and select the AppleTV from the sidebar.\n" +
                           "2. Hold down 'Shift' and click the 'Restore' button.\n" +
                           "3. Select the {0} file located in My Documents";

                MessageBox.Show(this, string.Format(text, fileName), "Seas0nPass", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        public void ShowDownloadFailedMessage()
        {
            if (InvokeRequired)
            {
                Invoke((Action)ShowDownloadFailedMessage);
            }
            else
            {
                MessageBox.Show(
                    owner: this,
                    text: "Unable to download firmware. Please check your internet connection or firewall settings and try again.",
                    caption: "Seas0nPass",
                    buttons: MessageBoxButtons.OK,
                    icon: MessageBoxIcon.Exclamation
                    );
            }
        }

        public void ShowTetherMessage(string fwName)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => ShowTetherMessage(fwName)));
            }
            else
            {
                MessageBox.Show(this, $"The {fwName} firmware is untethered and does not require this process!",
                    "Untethered Jailbreak");
            }
        }

        public void ShowControl(IView control)
        {
            Action action = delegate
            {
                contentPanel.Controls.Clear();
                var castedControl = (Control)control;
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
                $"The program(s) listed below may prevent Seas0nPass from running correctly. Please close these program(s) before continuing.\n{programsString}",
                "Seas0nPass", 
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public void ShowCrashMessage()
        {
            MessageBox.Show(this, 
                "Seas0nPass has detected critical error(s) and cannot continue.\nPlease ensure no conflicting software is running and try again.",
                "Seas0nPass",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Loaded(this, e);
        }
    }
}
