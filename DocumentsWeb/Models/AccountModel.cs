using System.ComponentModel.DataAnnotations;

namespace DocumentsWeb.Models
{
    public class LogOnModel
    {
        public LogOnModel()
        {
            RememberMe = false;
        }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Имя пользователя обязателено")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage="Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Запомнить меня
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Ошибка входа
        /// </summary>
        public bool LoginError { get; set; }
    }

    public class RestoreModel
    {
        public string Email { get; set; }
    }
}