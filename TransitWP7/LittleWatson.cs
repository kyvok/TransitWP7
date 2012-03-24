using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace TransitWP7
{
    using System.Threading;

    using GalaSoft.MvvmLight.Threading;

    using TransitWP7.Resources;

    // More info about this error reporting class here:
    // http://blogs.msdn.com/b/andypennell/archive/2010/11/01/error-reporting-on-windows-phone-7.aspx
    // Can also be transformed as a http reporting service here:
    // http://bjorn.kuiper.nu/2011/10/02/wp7-littlewatson-extended-error-reporting-to-http-endpoint/
    public class LittleWatson
    {
        private const string ErrorReportFileName = "LittleWatson.txt";

        internal static void ReportException(Exception ex, string extra)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    SafeDeleteFile(store);

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

                        SafeDeleteFile(store);
                    }
                }

                if (contents != null)
                {
                    ThreadPool.QueueUserWorkItem(_ => DispatcherHelper.UIDispatcher.BeginInvoke(() =>
                        {
                            var result = MessageBox.Show(SR.LittleWatsonDialogDesc, SR.LittleWatsonDialogTitle, MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                var email = new EmailComposeTask();
                                email.To = Globals.SupportEmailAddress;
                                email.Subject = string.Format("Transitive v{0} crash report", System.Reflection.Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0]);
                                email.Body = contents;
                                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
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
                SafeDeleteFile(IsolatedStorageFile.GetUserStoreForApplication());
            }
        }

        private static void SafeDeleteFile(IsolatedStorageFile store)
        {
            try
            {
                store.DeleteFile(ErrorReportFileName);
            }
            catch (Exception)
            {
            }
        }
    }
}