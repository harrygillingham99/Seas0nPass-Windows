////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using Seas0nPass.Models;

namespace Seas0nPass.Interfaces
{
    public interface IFirmwareVersionDetector
    {
        FirmwareVersion Version { get; }

        void SaveState(FirmwareVersion version);
    }
}
