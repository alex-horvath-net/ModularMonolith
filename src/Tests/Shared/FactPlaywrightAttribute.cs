namespace Tests.Shared;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal sealed class FactPlaywrightAttribute : FactAttribute
{
    public FactPlaywrightAttribute()
    {
        if (PlaywrightFixture.Skip.Value)
        {
            Skip = "Playwright tests skipped per configuration.";
        }
    }
}
