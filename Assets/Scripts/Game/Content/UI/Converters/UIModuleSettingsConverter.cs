namespace Game.Content.UI
{
    public class UIModuleSettingsConverter : ContentConverter<UIDatabaseScrobject, UIModuleSettings>
    {
        protected sealed override UIModuleSettings SourceToSettings(UIDatabaseScrobject source)
        {
            var settings = new UIModuleSettings();

            settings.negativeColor = source.negativeColor;
            settings.neutralColor = source.neutralColor;
            settings.positiveColor = source.positiveColor;

            settings.lightNegativeColor = source.lightNegativeColor;
            settings.lightNeutralColor = source.lightNeutralColor;
            settings.lightPositiveColor = source.lightPositiveColor;

            settings.moneyNegativeColor = source.moneyNegativeColor;
            settings.moneyPositiveColor = source.moneyPositiveColor;

            return settings;
        }
    }
}