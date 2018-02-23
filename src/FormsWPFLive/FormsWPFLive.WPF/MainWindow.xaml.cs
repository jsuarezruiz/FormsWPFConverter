using Xamarin.Forms.Platform.WPF;

namespace FormsWPFLive.WPF
{
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();
            Xamarin.Forms.Forms.Init();
            LoadApplication(new AppEditor());
        }
    }
}
