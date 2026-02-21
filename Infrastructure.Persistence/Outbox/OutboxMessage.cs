using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Publishing
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Processing — set by RabbitMQ ACK
        public bool IsProcessed { get; set; }
        public DateTime? ProcessedAt { get; set; }

        // Retries
        public int RetryCount { get; set; }
        public string? LastError { get; set; }

        // Dead letter
        public bool IsDead { get; set; }
        public DateTime? DeadAt { get; set; }
    }
}
