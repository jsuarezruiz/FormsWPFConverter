using FormsWPFLive.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace FormsWPFLive.ViewModels
{
    public class EditorViewModel : BindableObject
    {
        private string _wpfXaml;
        private string _liveXaml;
        private View _preview;

        public string WpfXaml
        {
            get { return _wpfXaml; }
            set
            {
                _wpfXaml = value;
                var xaml = XamlConverter.Convert(_wpfXaml);
                LiveXaml = xaml;
                OnPropertyChanged();
            }
        }

        public string LiveXaml
        {
            get { return _liveXaml; }
            set
            {
                _liveXaml = value;
                OnPropertyChanged();
                PreviewXaml(_liveXaml);
            }
        }

        public View Preview
        {
            get { return _preview; }
            set
            {
                _preview = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReloadCommand => new Command(Reload);

        private async void PreviewXaml(string xaml)
        {
            var contentPage = new ContentPage();

            try
            {
                if (string.IsNullOrEmpty(xaml))
                    return;

                string contentPageXaml = $"<?xml version='1.0' encoding='utf-8' ?><ContentPage xmlns='http://xamarin.com/schemas/2014/forms' xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml' x:Class ='FormsWPFLive.XamlPage'>{xaml}</ContentPage>";

                await Live.UpdatePageFromXamlAsync(contentPage, contentPageXaml);
            }
            catch (Exception exception)
            {
                // Error 
                Debug.WriteLine(exception.Message);
                var xamlException = Live.GetXamlException(exception);
                await Live.UpdatePageFromXamlAsync(contentPage, xamlException);
            }

            Preview = contentPage.Content;
        }

        private void Reload()
        {
            PreviewXaml(LiveXaml);
        }
    }
}