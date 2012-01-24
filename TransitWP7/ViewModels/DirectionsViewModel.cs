namespace TransitWP7.ViewModels
{
    public class DirectionsViewModel
    {
        public TransitRequestContext Context
        {
            get
            {
                return TransitRequestContext.Current;
            }
        }
    }
}
