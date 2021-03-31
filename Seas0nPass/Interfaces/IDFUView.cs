////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

namespace Seas0nPass.Interfaces
{
    public interface IDfuView : IView
    {
        bool HintVisibility { get; set; }
        void UpdateProgress(int value);
        void SetMessageText(string text);
        void Clear();
    }
}
