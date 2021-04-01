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

namespace Seas0nPass.Interfaces
{
    public interface IMainView
    {
        void ShowDownloadFailedMessage();
        void ShowControl(IView control);
        void ShowTetherMessage(string fwName);
        void ShowNotEnoughFreeSpaceMessage();
        void ShowCantAccessOriginalFirmwareMessage(string fileName);
        void ShowManualRestoreInstructions(string fileName);
        void ShowProgramsWarning(IEnumerable<string> programNames);
        void ShowCrashMessage();


        event EventHandler Loaded;
    }
}
