using System.Text.Json;
using CRM.Shared.Audit;

namespace CRM.Application.Common.Audit;

public static class AuditMetadataParser
{
    public static AuditMetadata? Parse(string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<AuditMetadata>(
                metadataJson);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}