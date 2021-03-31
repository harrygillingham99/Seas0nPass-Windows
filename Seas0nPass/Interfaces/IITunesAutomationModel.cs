using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Seas0nPass.Interfaces
{
    public interface IITunesAutomationModel
    {
        SynchronizationContext SyncContext { get; set; }
        IFirmwareVersionModel FirmwareVersionModel { get; set; }
        IITunesInfoProvider TunesInfoProvider { get; set; }
        void Run();
    }
}
