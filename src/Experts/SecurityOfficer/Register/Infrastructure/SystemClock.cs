using System;

namespace Experts.SecurityOfficer.Register.Infrastructure;

public sealed class SystemClock : UserStory.IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
