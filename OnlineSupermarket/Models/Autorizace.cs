﻿using System.ComponentModel.DataAnnotations;

namespace OnlineSupermarket.Models
{
    public class Autorizace
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        public string Heslo { get; set; }
    }
}