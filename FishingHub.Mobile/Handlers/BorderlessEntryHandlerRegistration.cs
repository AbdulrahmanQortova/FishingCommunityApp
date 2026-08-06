#if ANDROID
using Android.Graphics.Drawables;
#elif IOS || MACCATALYST
using UIKit;
#endif
using FishingHub.Mobile.Controls;
using Microsoft.Maui.Handlers;

namespace FishingHub.Mobile.Handlers;

public static class BorderlessEntryHandlerRegistration
{
    public static void Apply()
    {
        EntryHandler.Mapper.AppendToMapping<BorderlessEntry, IEntryHandler>("Borderless", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            var textColor = handler.PlatformView.CurrentTextColor;
            SetAndroidCursorColor(handler.PlatformView, textColor);
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UITextBorderStyle.None;
            handler.PlatformView.TintColor = handler.PlatformView.TextColor;
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });
    }

#if ANDROID
    private static void SetAndroidCursorColor(
        Android.Widget.EditText editText,
        int color)
    {
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var density = editText.Context?.Resources?.DisplayMetrics?.Density ?? 1f;

                var drawable = new GradientDrawable();
                drawable.SetColor(color);
                drawable.SetSize(4, (int)(density * 20));

                editText.TextCursorDrawable = drawable;
            }
        }
        catch
        {
        }
    }
#endif
}