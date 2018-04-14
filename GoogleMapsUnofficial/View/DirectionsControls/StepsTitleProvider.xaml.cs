using GoogleMapsUnofficial.Extension;
using GoogleMapsUnofficial.ViewModel.VoiceNavigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static GoogleMapsUnofficial.ViewModel.DirectionsControls.DirectionsHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.DirectionsControls
{
    public sealed partial class StepsTitleProvider : UserControl
    {
        private Step _currentstep;
        public Step CurrentStep
        {
            get
            {
                return _currentstep;
            }
            set { _currentstep = value; UpdateStep(); }
        }
        public static StepsTitleProvider Provider { get; set; }
        public StepsTitleProvider()
        {
            this.InitializeComponent();
            Provider = this;
        }

        async void UpdateStep()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                ComandInstructions.Text = CurrentStep.html_instructions.NoHTMLString();
                ComandImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/DirectionsIcons/" + CurrentStep.maneuver + ".png", UriKind.RelativeOrAbsolute));
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (VoiceHelper.Route == null) return;
            List<Step> Steps = new List<Step>();
            foreach (var RouteLeg in VoiceHelper.Route.legs)
            {
                foreach (var LegStep in RouteLeg.steps)
                {
                    Steps.Add(LegStep);
                }
            }
            try
            {
                var i = Steps.FindIndex(x => x == CurrentStep);
                var nextstep = Steps[++i];
                CurrentStep = nextstep;
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (VoiceHelper.Route == null) return;
            List<Step> Steps = new List<Step>();
            foreach (var RouteLeg in VoiceHelper.Route.legs)
            {
                foreach (var LegStep in RouteLeg.steps)
                {
                    Steps.Add(LegStep);
                }
            }
            try
            {
                var i = Steps.FindIndex(x => x == CurrentStep);
                var nextstep = Steps[--i];
                CurrentStep = nextstep;
            }
            catch { }
        }
    }
}
