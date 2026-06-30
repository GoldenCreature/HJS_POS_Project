namespace HJS_POS_Project.Database
{
    // 로그인한 사용자 정보를 앱 전체에서 공유하기 위한 정적 클래스
    public static class CurrentUser
    {
        // 로그인한 사용자의 아이디
        public static string Username { get; set; }

        // 로그인한 사용자의 권한 (admin / staff)
        public static string Role { get; set; }

        // 관리자인지 확인하는 편의 속성
        public static bool IsAdmin
        {
            get { return Role == "admin"; }
        }
    }
}