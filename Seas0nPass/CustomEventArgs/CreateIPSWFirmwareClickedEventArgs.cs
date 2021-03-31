////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////
using System;
using Seas0nPass.Models;

namespace Seas0nPass.CustomEventArgs
{
    public class CreateIpswFirmwareClickedEventArgs : EventArgs
    {
        public FirmwareVersion FirmwareVersion { get; private set; }

        public CreateIpswFirmwareClickedEventArgs(FirmwareVersion firmwareVersion)
        {
            FirmwareVersion = firmwareVersion;
        }
    }
}
