using System;

namespace ReleaseTools
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}