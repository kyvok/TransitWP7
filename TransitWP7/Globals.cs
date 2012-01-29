namespace TransitWP7
{
    public enum MessengerToken
    {
        ErrorPopup,
        MainMapProgressIndicator,
        TransitTripsReady,
        EndpointResolutionPopup,
        SelectedEndpoint,
        SelectedTransitTrip
    }

    public static class Globals
    {
        public const string MyCurrentLocationText = "My location";
    }

    public static class PhonePageUri
    {
        public const string DirectionsView = "/View/DirectionsView.xaml";
        public const string ExceptionView = "/View/ExceptionView.xaml";
        public const string LocationSelectionView = "/View/LocationSelectionView.xaml";
        public const string MainMapView = "/View/MainMapView.xaml";
        public const string SettingsView = "/View/SettingsView.xaml";
    }
}
