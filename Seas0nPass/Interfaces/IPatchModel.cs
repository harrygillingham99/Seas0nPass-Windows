////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;

namespace Seas0nPass.Interfaces
{
    public interface IPatchModel
    {
        void StartProcess();
        void Cancel();
        event EventHandler ProgressUpdated;
        event EventHandler Finished;
        int CurrentProgress { get; }
        string CurrentMessage { get; }
        event EventHandler CurrentMessageChanged;

        void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel);

    }
}
