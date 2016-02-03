# FloatingTextEntry
FloatingTextEntry Forms Control for Android/iOS

##Setup

###iOS

In your AppDelegate just add this:

```
FloatingTextEntryRenderer.Init ();
```

### Android

####1 - Setup your app to support material desing

Look at this tutorial https://blog.xamarin.com/material-design-for-your-xamarin-forms-android-apps/

####2 - Setup the color

In your theme xml add the following nodes:
```
//Accent Color
<item name="colorAccent">@color/accent</item>
//Inactive stroke color
<item name="colorControlNormal">@color/primaryDark</item>
//Inactive hint text color
<item name="android:textColorHint">@color/primaryDark</item>
```

####3 - Setup your app

Add the fallowing code in your Main Activity "OnStart" method:

```
FloatingTextEntryRenderer.Init ();
```

##Usage

```
var passEntry = new FloatingTextEntry ();
passEntry.Placeholder = "Password";
passEntry.AccentColor = Color.FromHex("#FFC107");
passEntry.InactiveAccentColor = Color.FromHex ("#1976D2");
passEntry.TextColor = Color.Purple;
passEntry.Completed += passEntry_Completed;
passEntry.IsPassword = true;

//Custom Error Message/Color/Validator
passEntry.ErrorColor = Color.Red;
passEntry.ErrorText = "Bad Email";
//Put default emailvalidator
passEntry.Validator = FloatingTextEntry.EmailValidator;
//or create your own validator
passEntry.Validator = (string input) => {
  return !string.IsNullOrWhiteSpace (input);
};
```

## Preview
<img src="https://raw.githubusercontent.com/AlejandroRuiz/FloatingTextEntry/master/Images/AndroidTest.gif" Width="240" />
<img src="https://raw.githubusercontent.com/AlejandroRuiz/FloatingTextEntry/master/Images/iOSTest.gif" Width="240" />

### Third Party Code:
iOS Control Based On: https://github.com/enisgayretli/EGFloatingTextField

PureLayout Binding from https://github.com/unhappy224/PureLayoutSharp
