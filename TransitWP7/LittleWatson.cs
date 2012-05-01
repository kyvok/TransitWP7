namespace TransitWP7
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Threading;
    using System.Windows;
    using GalaSoft.MvvmLight.Threading;
    using Microsoft.Phone.Tasks;
    using TransitWP7.Resources;

    // More info about this error reporting class here:
    // http://blogs.msdn.com/b/andypennell/archive/2010/11/01/error-reporting-on-windows-phone-7.aspx
    // Can also be transformed as a http reporting service here:
    // http://bjorn.kuiper.nu/2011/10/02/wp7-littlewatson-extended-error-reporting-to-http-endpoint/
    public static class LittleWatson
    {
        private const string ErrorReportFileName = "LittleWatson.txt";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must never fail the error reporting tool.")]
        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageHelper.SafeDeleteFile(store, ErrorReportFileName);

                    using (var output = new StreamWriter(store.CreateFile(ErrorReportFileName)))
                    {
                        output.WriteLine(extra);
                        output.WriteLine();
                        output.WriteLine("Top level exception");
                        output.WriteLine();
                        output.WriteLine(ex.Message);
                        output.WriteLine(ex.StackTrace);
                        var ex1 = ex.GetBaseException();
                        if (ex1 != ex)
                        {
                            output.WriteLine();
                            output.WriteLine("Base exception");
                            output.WriteLine();
                            output.WriteLine(ex1.Message);
                            output.WriteLine(ex1.StackTrace);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must never fail the error reporting tool.")]
        internal static void CheckForPreviousException()
        {
            try
            {
                string contents = null;
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(ErrorReportFileName))
                    {
                        using (var reader = new StreamReader(store.OpenFile(ErrorReportFileName, FileMode.Open, FileAccess.Read, FileShare.None)))
                        {
                            contents = reader.ReadToEnd();
                        }

                        IsolatedStorageHelper.SafeDeleteFile(store, ErrorReportFileName);
                    }
                }

                if (contents != null)
                {
                    ThreadPool.QueueUserWorkItem(_ => DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var result = MessageBox.Show(SR.LittleWatsonDialogDesc, SR.LittleWatsonDialogTitle, MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                var email = new EmailComposeTask();
                                email.To = Globals.SupportEmailAddress;
                                email.Subject = string.Format(CultureInfo.InvariantCulture, "Transitive v{0} crash report", System.Reflection.Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0]);
                                email.Body = contents;
                                IsolatedStorageHelper.SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication(), ErrorReportFileName);
                                email.Show();
                            }
                        }));
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsolatedStorageHelper.SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication(), ErrorReportFileName);
            }
        }
    }
}