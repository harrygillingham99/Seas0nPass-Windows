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
    public interface ITetherModel
    {
        void StartProcess();
        event EventHandler ProcessFinished;
        event EventHandler CurrentMessageChanged;

        string CurrentMessage { get; }

        int ProgressPercentage { get; }
        event EventHandler ProgressChanged;

        void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel);

    }
}
