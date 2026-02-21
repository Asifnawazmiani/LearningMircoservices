namespace Shared.Common.Correlation;

public class CorrelationContext : ICorrelationContext
{
    private string _correlationId = string.Empty;
    private string _requestId = string.Empty;

    public string CorrelationId => _correlationId;
    public string RequestId => _requestId;

    public void Generate()
    {
        // Background job generates its own ✅
        _correlationId = Guid.CreateVersion7().ToString();
        _requestId = Guid.CreateVersion7().ToString();
    }
}