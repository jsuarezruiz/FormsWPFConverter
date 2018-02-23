using FormsWPFLive.ViewModels;
using Xamarin.Forms;

namespace FormsWPFLive.Views
{
    public partial class EditorView : ContentPage
    {
        public EditorView()
        {
            InitializeComponent();

            BindingContext = new EditorViewModel();
        }
    }
}