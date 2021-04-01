﻿////
//
//  Seas0nPass
//
//  Copyright 2011 FireCore, LLC. All rights reserved.
//  http://firecore.com
//
////

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Seas0nPass.Presenters;
using Seas0nPass.Utils;

namespace Seas0nPass
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);            
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;               

            new HookResolver();           

            InitDocumentsHome();

            LogUtil.Init();
            LogUtil.LogEvent("Application start");
            LogUtil.LogEvent($"{Application.ProductName} {Application.ProductVersion}");
            
            Application.ApplicationExit += Application_ApplicationExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new MainForm();
            _mainPresenter = new MainPresenter(form);
            if (_mainPresenter.Init())
            {
                Application.Run(form);
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Trace.WriteLine("!!! Unhandled Exception caught in Application_ThreadException !!!");
            if (ex != null)
                LogUtil.LogException(ex);

            _mainPresenter.HandleCrash();
        }

        private static MainPresenter _mainPresenter;

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Trace.WriteLine("!!! Unhandled Exception caught in AppDomain.CurrentDomain.UnhandledException !!!");
            if (ex != null)
                LogUtil.LogException(ex);

            Environment.Exit(0);            
        }

        private static void InitDocumentsHome()
        {
            if (!SafeDirectory.Exists(MiscUtils.DOCUMENTS_HOME))
                SafeDirectory.CreateDirectory(MiscUtils.DOCUMENTS_HOME);

            var downloadsFolder = Path.Combine(MiscUtils.DOCUMENTS_HOME, "Downloads");
            if (!SafeDirectory.Exists(downloadsFolder))
                SafeDirectory.CreateDirectory(downloadsFolder);
        }

        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            LogUtil.LogEvent("Application Exit");
            WinProcessUtil.KillAllProcesses();
            MiscUtils.CleanUp();
        }
    }
}
