using System.ComponentModel;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Localization;

[ContentProperty(nameof(Key))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string Key { get; set; } = string.Empty;

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        var localizationService = IPlatformApplication.Current?.Services.GetService<ILocalizationService>()
            ?? throw new InvalidOperationException("ILocalizationService is not registered.");

        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = localizationService
        };

        return binding;
    }

    // Explicit implementation of the non-generic base interface — required because
    // IMarkupExtension<T> inherits from IMarkupExtension, whose ProvideValue returns
    // plain object. This just forwards to the strongly-typed version above.
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}