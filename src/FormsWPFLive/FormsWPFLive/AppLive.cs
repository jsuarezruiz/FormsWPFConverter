using FormsWPFLive.Views;
using Xamarin.Forms;

namespace FormsWPFLive
{
    public class AppLive : Application
    {
        public AppLive()
        {
            MainPage = new LiveView();
        }
    }
}
