using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinGoMapsX.ViewModel.PlacesControls;
using WinGoMapsX.ViewModel.SettingsView;

namespace WinGoMapsX
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (!ClassProInfo.SystemVersion.StartsWith("10.0.14393"))
                this.InitializeComponent();
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
            this.Suspending += OnSuspending;
            //XBOX Improvements
            //if(ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.XBOX)
            //{
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode
            //(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
            //this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            //this.FocusVisualKind = FocusVisualKind.Reveal;
            //}
            this.UnhandledException += App_UnhandledException;
            try
            {
                var English = new System.Globalization.CultureInfo("en-us");
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator = English.NumberFormat.NumberDecimalSeparator;
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = English.NumberFormat.CurrencyDecimalSeparator;
                System.Globalization.CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator = English.NumberFormat.PercentDecimalSeparator;
            }
            catch { }
            try
            {
                var index = SettingsSetters.GetThemeIndex();
                if (index == 0) this.RequestedTheme = ApplicationTheme.Dark;
                if (index == 1) this.RequestedTheme = ApplicationTheme.Light;
            }
            catch
            {
                this.RequestedTheme = ApplicationTheme.Light;
            }
            SettingsSetters.SetRotationControlsVisible(Windows.UI.Xaml.Controls.Maps.MapInteractionMode.GestureOnly);
        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            //var Usage = MemoryManager.AppMemoryUsage;
            //var Limit = MemoryManager.AppMemoryUsageLimit;
            var Lvl = MemoryManager.AppMemoryUsageLevel;
            if (Lvl == AppMemoryUsageLevel.High)
                GC.Collect();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (ClassProInfo.SystemVersion.StartsWith("10.0.14393"))
            {
                #region Initialize Page
                Application.LoadComponent(this, new Uri("ms-appx:///App.AuSupport.xaml", UriKind.RelativeOrAbsolute), ComponentResourceLocation.Application);
                #endregion
            }
            try
            {
                var index = SettingsSetters.GetThemeIndex();
                if (index == 1)
                    StatusBar.GetForCurrentView().ForegroundColor = Colors.Black;
                else
                    StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }
            catch { }
            Frame rootFrame = Window.Current.Content as Frame;
            Resources["ToggleButtonBackgroundChecked"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlHighlightListAccentLowBrush"] = Color.FromArgb(255, 96, 165, 255);
            Resources["ToggleSwitchFillOn"] = Color.FromArgb(255, 96, 165, 255);
            Resources["HyperlinkButtonForeground"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlBackgroundAccentBrush"] = Color.FromArgb(255, 96, 165, 255);
            GMapsUWP.Initializer.Initialize(AppCore.GoogleMapAPIKey, AppCore.GoogleMapRequestsLanguage);
            StartFluent();

            //if (rootFrame == null)
            //{
            //    rootFrame = new Frame();
            //    rootFrame.NavigationFailed += OnNavigationFailed;
            //    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //    {
            //    }
            //    Window.Current.Content = rootFrame;
            //}
            //if (e.PrelaunchActivated == false)
            //{
            //    if (rootFrame.Content == null)
            //    {
            //        rootFrame.Navigate(typeof(MainPage), e.Arguments);
            //    }
            //    Window.Current.Activate();
            //}
            SplashScreen splashScreen = e.SplashScreen;
            ExtendedSplashScreen eSplash = null;
            if (e.Arguments != "")
            {
                var sp = e.Arguments.Split(',');
                eSplash = new ExtendedSplashScreen(splashScreen, new Uri($"https://google.com/maps/@?api=1&map_action=map&center={sp[0]},{sp[1]}&zoom=17", UriKind.RelativeOrAbsolute));
            }
            else
            {
                eSplash = new ExtendedSplashScreen(splashScreen);
            }
            // Register an event handler to be executed when the splash screen has been dismissed.
            Window.Current.Content = eSplash;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            #region Start app

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            #endregion
            try
            {
                var index = SettingsSetters.GetThemeIndex();
                if (index == 1)
                    StatusBar.GetForCurrentView().ForegroundColor = Colors.Black;
                else
                    StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }
            catch { }
            Resources["ToggleButtonBackgroundChecked"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlHighlightListAccentLowBrush"] = Color.FromArgb(255, 96, 165, 255);
            Resources["ToggleSwitchFillOn"] = Color.FromArgb(255, 96, 165, 255);
            Resources["HyperlinkButtonForeground"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlBackgroundAccentBrush"] = Color.FromArgb(255, 96, 165, 255);
            GMapsUWP.Initializer.Initialize(AppCore.GoogleMapAPIKey, AppCore.GoogleMapRequestsLanguage);
            StartFluent();
            SplashScreen splashScreen = args.SplashScreen;
            ExtendedSplashScreen eSplash = null;

            if (args.Kind == ActivationKind.VoiceCommand)
            {
                var voiceArgs = (VoiceCommandActivatedEventArgs)args;
                var Rule = voiceArgs.Result.RulePath.FirstOrDefault();
                var input = voiceArgs.Result.SemanticInterpretation.Properties.Where(x => x.Key == "UserInput").FirstOrDefault().Value.FirstOrDefault();
                if (Rule == "DirectionsCommand")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen, new Uri("https://google.com/maps/@searchplace=" + input, UriKind.RelativeOrAbsolute));
                }
                if (Rule == "FindPlace")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen, new Uri("https://google.com/maps/@searchplace=" + input, UriKind.RelativeOrAbsolute));
                }
                if (Rule == "WhereAmI")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen);
                }
            }
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = (ProtocolActivatedEventArgs)args;
                var x = protocolArgs.Uri.ToString();

                // Register an event handler to be executed when the splash screen has been dismissed.
                //Type deepLinkPageType = typeof(PageLogin);
                if (rootFrame.Content == null)
                {
                    try
                    {
                        switch (protocolArgs.Uri.AbsolutePath.ToLower().Replace("/maps", string.Empty).Split('/')[1].ToString())
                        {
                            case "search":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            case "dir":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            case "@":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            default:
                                eSplash = new ExtendedSplashScreen(splashScreen);
                                break;
                        }
                    }
                    catch
                    {
                        eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                    }
                }
            }
            Window.Current.Content = eSplash;
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog($"{e.Message}{Environment.NewLine}{e.Exception.StackTrace}").ShowAsync();
        }

        private void StartFluent()
        {
            if (ApiInformation.IsMethodPresent("Windows.UI.Xaml.Hosting.ElementCompositionPreview", "SetElementChildVisual"))
            {
                ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
                formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
                formattableTitleBar.ButtonForegroundColor = Colors.Black;
                formattableTitleBar.InactiveForegroundColor = Colors.Black;
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
            }
        }
        public async void InstallVCD()
        {
            try
            {
                var r = ApplicationData.Current.LocalSettings.Values["VCDV10"].ToString();
            }
            catch (Exception)
            {
                try
                {
                    var vcdStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///VoiceCommands.xml", UriKind.RelativeOrAbsolute));
                    await Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                    ApplicationData.Current.LocalSettings.Values["VCDV10"] = "";
                }
                catch
                {
                    ApplicationData.Current.LocalSettings.Values["VCDV10"] = null;
                }
            }
        }
        public async void UpdateJumpList()
        {
            try
            {
                var listjump = await JumpList.LoadCurrentAsync();
                listjump.Items.Clear();
                foreach (var Place in SavedPlacesVM.GetSavedPlaces())
                {
                    listjump.SystemGroupKind = JumpListSystemGroupKind.None;
                    listjump.Items.Add(JumpListItem.CreateWithArguments($"{Place.Latitude},{Place.Longitude}", Place.PlaceName));
                }
                await listjump.SaveAsync();
            }
            catch { }
        }
    }

}
