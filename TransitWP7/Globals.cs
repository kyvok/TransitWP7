namespace TransitWP7
{
    public enum MessengerToken
    {
        EnableLocationButtonIndicator,
        EndpointResolutionPopup,
        ErrorPopup,
        LockUiIndicator,
        MainMapProgressIndicator,
        SelectedEndpoint,
        SelectedTransitTrip,
        TransitTripsReady,
        TripStepSelection
    }

    public static class Globals
    {
        public const double LocateMeZoomLevel = 17;
        public const string SupportEmailAddress = "TransitiveWP7@live.com";
        public const int MovementThreshold = 20; // recommended to be 20m to avoid noise.
    }

    public static class PhonePageUri
    {
        public const string DirectionsView = "/View/DirectionsView.xaml";
        public const string LocationSelectionView = "/View/LocationSelectionView.xaml";
        public const string MainMapView = "/View/MainMapView.xaml";
        public const string SettingsView = "/View/SettingsView.xaml";
    }
}
