namespace TransitWP7.ViewModel
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
