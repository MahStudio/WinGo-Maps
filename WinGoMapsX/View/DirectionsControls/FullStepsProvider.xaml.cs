using GMapsUWP;
using GMapsUWP.Directions;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinGoMapsX.View.DirectionsControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FullStepsProvider : Page
    {
        private class Instruction
        {
            public string Distance { get; set; }
            public string Text { get; set; }
            public BitmapImage Image { get; set; }
        }
        public FullStepsProvider(DirectionsHelper.Route CurrentRoute)
        {
            this.InitializeComponent();
            var inst = new List<Instruction>();
            foreach (var Leg in CurrentRoute.Legs)
            {
                foreach (var CurrentStep in Leg.Steps)
                {
                    inst.Add(new Instruction()
                    {
                        Text = CurrentStep.HtmlInstructions.NoHTMLString(),
                        Distance = CurrentStep.Distance.Text,
                        Image = new BitmapImage(new Uri("ms-appx:///Assets/DirectionsIcons/" + CurrentStep.Maneuver + ".png", UriKind.RelativeOrAbsolute))
                    });
                }
            }
            DataContext = inst;
        }
    }
}
