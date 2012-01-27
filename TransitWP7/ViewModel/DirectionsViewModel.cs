using GalaSoft.MvvmLight;

namespace TransitWP7.ViewModel
{
    public class DirectionsViewModel : ViewModelBase
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
