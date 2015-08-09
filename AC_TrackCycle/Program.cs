﻿using acPlugins4net.helpers;
using System;
using System.Windows.Forms;

namespace AC_TrackCycle
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrackCyclerForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(FileLogWriter.GetExceptionString(ex));
            }
        }
    }
}