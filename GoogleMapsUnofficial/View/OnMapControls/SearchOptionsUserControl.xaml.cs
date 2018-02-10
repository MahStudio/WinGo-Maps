﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GoogleMapsUnofficial.View.OnMapControls
{
    public sealed partial class SearchOptionsUserControl : UserControl
    {

        public string SearchText
        {
            get
            {
                return (string)GetValue(SearchTextProperty);
            }
            set
            {
                SetValue(SearchTextProperty, value);
                TextSearchProviderUserControl.SearchText = value;
                NearbySearchProviderUserControl.SearchText = value;
            }
        }
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
         "SearchText",
         typeof(string),
         typeof(SearchOptionsUserControl),
         new PropertyMetadata(null)
        );
        public bool PopUP
        {
            get
            {
                return (bool)GetValue(PopUPProperty);
            }
            set
            {
                SetValue(PopUPProperty, value);
                if (value)
                    SearchBTN.Flyout.ShowAt(SearchBTN);
            }
        }
        public static readonly DependencyProperty PopUPProperty = DependencyProperty.Register(
         "PopUP",
         typeof(bool),
         typeof(SearchOptionsUserControl),
         new PropertyMetadata(null)
        );
        public SearchOptionsUserControl()
        {
            this.InitializeComponent();
        }
    }
}
