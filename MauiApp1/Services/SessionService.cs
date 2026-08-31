namespace MauiApp1.Services
{
    // Holds who is currently logged in for the lifetime of the app session.
    // Kept intentionally simple (static/in-memory) since this app has no
    // multi-user/multi-window concerns.
    public static class SessionService
    {
        public static string? CurrentStudentId { get; set; }
        public static string? CurrentAdminUsername { get; set; }

        public static bool IsStudentLoggedIn => !string.IsNullOrEmpty(CurrentStudentId);
        public static bool IsAdminLoggedIn => !string.IsNullOrEmpty(CurrentAdminUsername);

        public static void ClearSession()
        {
            CurrentStudentId = null;
            CurrentAdminUsername = null;
        }
    }
}
