using System;

namespace BookingService.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = default!;
        public string PerformedBy { get; set; } = default!;
        public string TargetUser { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
