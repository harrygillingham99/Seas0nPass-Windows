////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System.Collections.Generic;

namespace Seas0nPass.Interfaces
{
    public interface IMainModel
    {
        bool IsTetherPossible();
        void SetFirmwareVersionModel(IFirmwareVersionModel firmwareVersionModel);

        IEnumerable<string> GetProgramsToWarnNames();

    }
}
