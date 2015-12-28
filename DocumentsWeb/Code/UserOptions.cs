namespace DocumentsWeb
{
    public class UserOptions
    {
        public UserOptions()
        {
            
        }
        /// <summary>
        /// Текущая тема пользователя
        /// </summary>
        public string CurrentTheme { get; set; }
        /// <summary>
        /// Стартовая страница навигации после входа
        /// </summary>
        public string StartPage { get; set; }

        public static UserOptions GetUserOptions()
        {
            return new UserOptions();
        }
    }
}