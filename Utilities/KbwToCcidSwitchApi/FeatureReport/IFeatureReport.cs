namespace KbwToCcidSwitchApi.FeatureReport
{
    public interface IFeatureReport
    {
        byte[] Report { get; }
        byte ReportId { get; }
        string Description { get; }
        void Send(KeyboardData keyboardData);
    }
}
