using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization ;

namespace MarkdownUserStories.Services
{
    public static class DateTimeHelpers
    {
        public static DateTimeOffset ToLocalDateTimeOffset(string s)
        {
            // First requirement, convert UTC ISO string to local DateTimeOffset
            // Second requirement, if incoming string doesn't have zone info, assume it's local
            // and convert to UTC.
            // NOTE: There's no way to know, if a user manually enters a datetime without an offset,
            // that it was a mistake. To handle this situation would require always storing offset
            // info, thus always knowing what zone someone was in. Not necessary and in the end more complicated.

            // This automatically converts to local zone.
            // Need this to correctly determine the incoming Kind, if desired. DateTimeOffset.DateTime.Kind is always Unspecified.
            DateTime dt1 = DateTime.Parse(s);
            DateTimeOffset dt = (DateTimeOffset)dt1;
            return dt;
        }

        public static DateTime ToLocalDateTime(string s)
        {
            return ToLocalDateTimeOffset(s).LocalDateTime;
        }

        public static DateTime ToLocalDateTime(DateTimeOffset d)
        {
            return d.LocalDateTime;
        }

        public static string ToUtcIsoString(DateTimeOffset d)
        {
            // First requirement, convert DateTime to UTC if it's not already, and return in UTC format.
            // This should NEVER return a date with an offset. In short, we're not storing time zone information.
            return d.UtcDateTime.ToString(DateTimeFormatInfo.CurrentInfo.UniversalSortableDateTimePattern);
        }
    }
}
