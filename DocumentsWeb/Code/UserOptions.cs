namespace DocumentsWeb
{
    public class UserOptions
    {
        public UserOptions()
        {
            
        }
        /// <summary>
        /// ������� ���� ������������
        /// </summary>
        public string CurrentTheme { get; set; }
        /// <summary>
        /// ��������� �������� ��������� ����� �����
        /// </summary>
        public string StartPage { get; set; }

        public static UserOptions GetUserOptions()
        {
            return new UserOptions();
        }
    }
}