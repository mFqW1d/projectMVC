using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace arm_repairs_project.Models
{
    public class ManageUserViewModels
    {
        /// <summary>
        /// Модель для отображения списка пользователей
        /// </summary>
        public class MangeUsers
        {
            public IEnumerable<ApplicationUser> Users { get; set; }
        }

        public class User
        {
            public string Id { get; set; }

            [Required]
            [Display(Name = "ФИО")]
            public string Fio { get; set; }

            [Required]
            [Display(Name = "Логин")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Подтверждение Email")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "Руководитель")]
            public bool IsChief { get; set; }

            [Display(Name = "Менеджер")]
            public bool IsManager { get; set; }

            [Display(Name = "Мастер")]
            public bool IsMaster { get; set; }

            [Display(Name = "Пользователь")]
            public bool IsUser { get; set; }
        }

        public class ChangePasswordViewModel
        {
            public string Id { get; set; }
            public string Fio { get; set; }
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
            [DataType(DataType.Password)]
            [Display(Name = "Новый пароль")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение нового пароля")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}