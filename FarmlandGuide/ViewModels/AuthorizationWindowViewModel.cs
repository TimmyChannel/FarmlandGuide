using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FarmlandGuide.Helpers;
using FarmlandGuide.Helpers.Messages;
using FarmlandGuide.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmlandGuide.ViewModels
{
    public partial class AuthorizationWindowViewModel : ObservableValidator
    {
        string _login;
        string _password;
        [CustomValidation(typeof(AuthorizationWindowViewModel), nameof(ValidateLogin))]
        public string Login
        {
            get { return _login; }
            set
            {
                SetProperty(ref _login, value, true);
            }
        }
        [CustomValidation(typeof(AuthorizationWindowViewModel), nameof(ValidatePassword))]
        public string Password
        {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value, true);
            }
        }
        #region Validators

        public static ValidationResult ValidateLogin(string login, ValidationContext context)
        {
            AuthorizationWindowViewModel instance = (AuthorizationWindowViewModel)context.ObjectInstance;
            using var ctx = new ApplicationDbContext();
            if (ctx.Employees.Any(e => e.EmployeeName == login))
                return ValidationResult.Success;
            return new("Нет пользователя с таким логином");
        }
        public static ValidationResult ValidatePassword(string password, ValidationContext context)
        {
            AuthorizationWindowViewModel instance = (AuthorizationWindowViewModel)context.ObjectInstance;
            using var ctx = new ApplicationDbContext();
            if (ctx.Employees.Where(e => e.EmployeeName == instance.Login).Count() == 1)
            {
                var employee = ctx.Employees.First(e => e.EmployeeName == instance.Login);
                var passwordSalt = employee.PasswordSalt;
                var currentPasswordHash = PasswordManager.HashPassword(password, passwordSalt);
                if (currentPasswordHash == employee.PasswordHash)
                    return ValidationResult.Success;
                else
                    return new("Пароль неверный");

            }
            if (ctx.Employees.Where(e => e.EmployeeName == instance.Login).Count() > 1)
                return new("Ошибка! Несколько пользователей с таким логином");
            return ValidationResult.Success;
        }
        #endregion
        [RelayCommand]
        private void OnLoginToApplication()
        {
            ValidateAllProperties();
            if (HasErrors) return;
            using var ctx = new ApplicationDbContext();
            var employee = ctx.Employees.Include(e=>e.Role).First(e => e.EmployeeName == Login).Copy();
            WeakReferenceMessenger.Default.Send(new LoggedUserMessage(employee.Copy()));
            Password = "";
            Login = "";
            ClearErrors();
        }
    }
}
