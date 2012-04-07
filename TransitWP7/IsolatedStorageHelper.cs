namespace TransitWP7
{
    using System;
    using System.IO.IsolatedStorage;

    public static class IsolatedStorageHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Must never fail deleting a file. Best effort only.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must never fail deleting a file. Best effort only.")]
        public static void SafeDeleteFile(IsolatedStorageFile storage, string fileName)
        {
            try
            {
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
