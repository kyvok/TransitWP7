namespace BingApisLib.BingMapsRestApi
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/ff701707.aspx
    /// </summary>
    public enum AuthenticationResultCode
    {
        ValidCredentials,
        InvalidCredentials,
        CredentialsExpired,
        NotAuthorized,
        NoCredentials,
        None
    }
}