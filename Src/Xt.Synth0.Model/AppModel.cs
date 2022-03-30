namespace Xt.Synth0.Model
{
    public enum ThemeGroup
    {
        Env,
        Amp,
        Unit,
        Plot,
        Delay,
        Master,
        Pattern,
        Control,
        Settings,
        VoiceLfo,
        GlobalLfo,
        VoiceFilter,
        GlobalFilter
    }

    public sealed class AppModel
    {
        public TrackModel Track { get; } = new();
        public SettingsModel Settings { get; } = new();
        public StreamModel Stream { get; } = new(true);
    }
}