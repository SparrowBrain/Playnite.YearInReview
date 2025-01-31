using System;

namespace ReleaseTools
{
    internal class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}