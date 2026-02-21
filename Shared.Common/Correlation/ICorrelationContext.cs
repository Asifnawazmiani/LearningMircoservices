namespace Shared.Common.Correlation;

public interface ICorrelationContext
{
    string CorrelationId { get; }
    string RequestId { get; }
    void Generate();
}
