# UWPGmaps 
Unofficial Google Map client for Universal Windows Platform 

[Telegram Messenger Insiders Group](https://t.me/joinchat/DQwGRhG-DXgBJNDWjGEoZQ)

[GMaps Class Library](https://github.com/NGame1/UWPGmaps.SDK)

NuGet Library to work with Google Map APIs will be available soon too.

# What is supported in project till now 
-Show and Download Google Maps Tiles in UWP map control

-Offline map download (+Backup/Restore)

-Find directions and navigation

-Voice Navigation

-GeoCoding (Converting latitude and longitude to address)

-Reverse GeoCoding (Converting Address to latitude and longitude)

-Search (Nearby / Text / Place Auto Complete)

-Place Helper

-Save Favorite places (+Sync between devices)

-Cortana Integration

# Screenshots
![Screenshot](http://s9.picofile.com/file/8319001000/image_2018_02_09_23_23_27.png)

![Screenshot](http://s8.picofile.com/file/8319001034/image_2018_02_09_23_23_01.png)

![Screenshot](http://s8.picofile.com/file/8319001042/image_2018_02_09_23_25_39.png)

![Screenshot](http://s8.picofile.com/file/8319001100/image_2018_02_09_23_28_07.png)

# Building Solution
Right click on the solution file in Visual Studio solution explorer and add a new Class called AppCore.cs

write this code in the class and save it . 
```
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
