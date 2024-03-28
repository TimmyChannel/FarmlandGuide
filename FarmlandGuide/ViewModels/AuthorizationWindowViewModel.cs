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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private string _login;
        private string _password;
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
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (ctx.Employees.Any(e => e.EmployeeName == login))
                return ValidationResult.Success;
            return new ValidationResult("Нет пользователя с таким логином");
        }
        public static ValidationResult ValidatePassword(string password, ValidationContext context)
        {
            AuthorizationWindowViewModel instance = (AuthorizationWindowViewModel)context.ObjectInstance;
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            if (ctx.Employees.Where(e => e.EmployeeName == instance.Login).Count() == 1)
            {
                var employee = ctx.Employees.First(e => e.EmployeeName == instance.Login);
                var passwordSalt = employee.PasswordSalt;
                var currentPasswordHash = PasswordManager.HashPassword(password, passwordSalt);
                if (currentPasswordHash == employee.PasswordHash)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Пароль неверный");

            }
            if (ctx.Employees.Where(e => e.EmployeeName == instance.Login).Count() > 1)
                return new ValidationResult("Ошибка! Несколько пользователей с таким логином");
            return ValidationResult.Success;
        }
        #endregion
        [RelayCommand]
        private void OnLoginToApplication()
        {
            _logger.Debug("Login attempt initiated");
            ValidateAllProperties();
            if (HasErrors)
            {
                _logger.Info("Login attempt is failed");
                return;
            }
            using var ctx = new ApplicationDbContext();
            ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var employee = ctx.Employees.Include(e => e.Role).First(e => e.EmployeeName == Login).Copy();
            WeakReferenceMessenger.Default.Send(new LoggedUserMessage(employee.Copy()));
            _logger.Info("Success login. Employee: {0}", Login);
            Password = "";
            Login = "";
            ClearErrors();
        }
    }
}
