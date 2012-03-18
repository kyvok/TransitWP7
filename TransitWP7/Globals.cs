namespace TransitWP7
{
    public enum MessengerToken
    {
        ErrorPopup,
        MainMapProgressIndicator,
        TransitTripsReady,
        EndpointResolutionPopup,
        SelectedEndpoint,
        SelectedTransitTrip,
        LockUiIndicator,
        EnableLocationButtonIndicator
    }

    public static class Globals
    {
        public const string MyCurrentLocationText = "My location";
        public const double LocateMeZoomLevel = 17;
    }

    public static class PhonePageUri
    {
        public const string DirectionsView = "/View/DirectionsView.xaml";
        public const string LocationSelectionView = "/View/LocationSelectionView.xaml";
        public const string MainMapView = "/View/MainMapView.xaml";
        public const string SettingsView = "/View/SettingsView.xaml";
    }
}
