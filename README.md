# WinGo Maps 

Win Go Maps is an UNOFFICIAL Google Map client for Universal Windows Platform.

[Telegram Messenger Insiders Group](https://t.me/joinchat/DQwGRhG-DXgBJNDWjGEoZQ)

[GMaps Class Library](https://github.com/NGame1/UWPGmaps.SDK)

[SDK NuGet package](https://www.nuget.org/packages/GMapsUWPSDK)

## Features

- Show and Download Google Maps Tiles in UWP map control
- Offline map download (+Backup/Restore)
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
![Screenshot](http://s9.picofile.com/file/8319001000/image_2018_02_09_23_23_27.png)

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
public class AppCore
{
    public static string OnMapLanguage { get; set; }
    public static string GoogleMapRequestsLanguage { get; set; }
    public static string GoogleMapAPIKey { get; private set; }
    public static string HttpUserAgent { get; private set; }
    static AppCore()
    {
        HttpUserAgent = "WindowsUniversalGoogleMapsV2ALPHA";
        GoogleMapAPIKey = "Your GoogleMap API Key";
        GoogleMapRequestsLanguage = LanguageSettingsSetters.GetAPILanguage();
        OnMapLanguage = LanguageSettingsSetters.GetOnMapLanguage();
    }
}
```

4. Now hit **F5** and run the project :)

## Contributing

We are always welcome to your help. You can join our [Telegram Insiders Group](https://t.me/joinchat/DQwGRhG-DXgBJNDWjGEoZQ) to help us developing and testing this app. Also you can [post issues and feature requests](https://github.com/MahStudio/WinGo-Maps/issues) and help us in development via sending pull requests. 

### Authors

This project is designed, developed, maintained and supported by the community software development team **Mah Studio**.
See also the list of [contributors](https://github.com/MahStudio/WinGo-Maps/contributors) who participated in this project.
