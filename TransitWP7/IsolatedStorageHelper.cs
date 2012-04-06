namespace TransitWP7
{
    using System;
    using System.IO.IsolatedStorage;

    public static class IsolatedStorageHelper
    {
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
