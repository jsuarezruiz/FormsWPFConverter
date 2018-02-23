using FormsWPFLive.Views;
using Xamarin.Forms;

namespace FormsWPFLive
{
    public class AppEditor : Application
    {
        public AppEditor()
        {
            MainPage = new CustomNavigationPage(new EditorView());
        }
    }
}
