<p align="center">
  <a href="https://www.microsoft.com/en-us/store/p/denna/9n9c2hwnzcft">
    <img src="https://github.com/MahStudio/WinGo-Maps/raw/WinGoMapsX/WinGoMapsX/Assets/Branding.jpg" width=80 height=80>
  </a>

  <h3 align="center">Wingo Maps</h3>

  <p align="center">
    WinGo Maps is an UNOFFICIAL Google Map client for Universal Windows Platform.
    <br>
    <a href="https://www.microsoft.com/en-us/p/wingo-maps/9nmj42v775gt">Download</a>
    &middot;
    <a href="https://t.me/joinchat/DQwGRhG-DXgBJNDWjGEoZQ">Insiders Group</a>
  &middot;
    <a href="https://github.com/NGame1/UWPGmaps.SDK">Gmaps SDK</a>
    <br>
    
  </p>
</p>

<br>

## Features

- Show and Download Google Maps Tiles in UWP map control
- Offline map download (+Backup/Restore)
- Switch between online and offline maps dynamically 
- Find directions and navigation
- Voice Navigation
- GeoCoding (Converting latitude and longitude to address)
- Reverse GeoCoding (Converting Address to latitude and longitude)
- Search (Nearby / Text / Place Auto Complete)
- Place details, Rate and Reviews, Images and etc.
- Save Favorite places (+Sync between devices)
- Cortana Integration
- Live Tile
- Multilingual support (Arabic, Belarusian, English, German, Itlian, Persian, Portuguese (Brazil) and more)
- Fluent design

### Screenshots

![Screenshot](http://s8.picofile.com/file/8319001034/image_2018_02_09_23_23_01.png)

![Screenshot](http://s8.picofile.com/file/8319001042/image_2018_02_09_23_25_39.png)

![Screenshot](http://s8.picofile.com/file/8319001100/image_2018_02_09_23_28_07.png)

## Getting started with the source code

### Prerequisites

1. Windows 10
2. Visual Studio 2017 (latest build) with universal windows development features installed.
3. GIT for Windows ([install from here](http://gitforwindows.org/))

### Build and running the code

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Right click on the solution file in Visual Studio solution explorer and add a new Class called `AppCore.cs`
Then write this code in this class based on your *Google API information*: 

```csharp
using WinGoMapsX.ViewModel.SettingsView;
using Windows.ApplicationModel.Store;
using Windows.UI.Core;
using Windows.Web.Http;

public class AppCore
{
    public static string ArianaAPIKey { get => "ArianaAPIKey"; }
    private static CoreDispatcher _dispatch;
    public static CoreDispatcher Dispatcher
    {
        get
        {
            if (_dispatch != null) return _dispatch;
            else
            {
                _dispatch = CoreWindow.GetForCurrentThread().Dispatcher;
                return _dispatch;
            }
        }
    }
    private static HttpClient _http;
    public static HttpClient HttpClient { get { if (_http != null) return _http; else { _http = new HttpClient(); return _http; } } }
    public static string OnMapLanguage { get; set; }
    public static string GoogleMapRequestsLanguage { get; set; }
    public static string GoogleMapAPIKey { get; private set; }
    public static string HttpUserAgent { get; private set; }
    static AppCore()
    {
        HttpUserAgent = "MahStudioWinGoMapsX";
        GoogleMapAPIKey = "YouGMapsAPIKey";
        GoogleMapRequestsLanguage = LanguageSettingsSetters.GetAPILanguage();
        OnMapLanguage = LanguageSettingsSetters.GetOnMapLanguage();
    }
}
```

4. Now hit **F5** and run the project :)

## Contributing

We are always welcome to your help. You can join our [Telegram Insiders Group](https://t.me/joinchat/DQwGRhG-DXgBJNDWjGEoZQ) to help us developing and testing this app. Also you can [post issues and feature requests](https://github.com/MahStudio/WinGo-Maps/issues) and help us in development via sending pull requests. 

### Authors

This project is designed, developed, maintained and supported by the community software development team [**Mah Studio**](https://github.com/MahStudio/).
See also the list of [contributors](https://github.com/MahStudio/WinGo-Maps/contributors) who participated in this project.
