namespace DMVConnect.Helpers
{
    public class GlobalHelpers
    {
        public static string GetPostedTime(DateTime dateCreated)
        {
            TimeSpan timeSpanSincePosted = DateTime.Now.Subtract(dateCreated);
            string displayTime = timeSpanSincePosted switch
            {
                var t when t.TotalDays >= 730 => $"{(int)t.TotalDays / 365} years ago",
                var t when t.TotalDays >= 365 && t.TotalDays < 730 => $"{(int)t.TotalDays / 365} year ago",
                var t when t.TotalDays >= 58 && t.TotalDays <= 365 => $"{(int)t.TotalDays / 29} months ago",
                var t when t.TotalDays >= 29 && t.TotalDays <= 57 => $"{(int)t.TotalDays / 29} month ago",
                var t when t.TotalDays >= 14 && t.TotalDays <= 28 => $"{(int)t.TotalDays / 7} weeks ago",
                var t when t.TotalDays >= 7 && t.TotalDays <= 13 => $"{(int)t.TotalDays / 7} week ago",
                var t when t.TotalDays >= 2 && t.TotalDays < 7 => $"{(int)t.TotalDays} days ago",
                var t when t.TotalDays >= 1 && t.TotalDays < 2 => $"{(int)t.TotalDays} day ago",
                var t when t.TotalHours >= 2 && t.TotalHours < 24 => $"{(int)t.TotalHours} hours ago",
                var t when t.TotalHours >= 1 && t.TotalHours < 2 => $"{(int)t.TotalHours} hour ago",
                var t when t.TotalMinutes >= 2 && t.TotalMinutes < 59 => $"{(int)t.TotalMinutes} minutes ago",
                var t when t.TotalMinutes >= 1 && t.TotalMinutes < 2 => $"{(int)t.TotalMinutes} minute ago",
                var t when t.TotalMinutes < 1 => $"Just now",
                _ => dateCreated.ToString("M/dd/yyyy hh:mm tt")
            };

            return displayTime;
        }
    }
}
