using AutoMapper;
using InventorySystem_API.User.Models;
using InventorySystem_API.User.Repositories;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text.RegularExpressions;

namespace InventorySystem_API.User.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IHasher _bCryptHasher;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly EmailSettingOptions _emailData;

        public PasswordResetService
            (
            IPasswordResetRepository passwordResetRepository,
            IHasher bCryptHasher,
            IUserRepository userRepository,
            IOptions<EmailSettingOptions> emailData
            )
        {
            _passwordResetRepository = passwordResetRepository;
            _bCryptHasher = bCryptHasher;
            _userRepository = userRepository;
            _emailData = emailData.Value;
        }

        public async Task ChangePassword(string email, string newPassword)
        {
            if (!Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_])[A-Za-z\d@$!%*?&_]{8,}$"))
                throw new ArgumentException("Парль має містити " +
                    "принаймні одну велику та малу англійську літеру," +
                    " спеціальний символ та" +
                    " мати довжину мінімум вісім символів"
                );

            var user = await _userRepository.GetByEmail(email);

            if(user is null)
                throw new Exception($"Не знайдено коритувача із вказаною поштою: {email}");

            user.PasswordHash = _bCryptHasher.HashPassword(newPassword);

            await _userRepository.Update(user);

            await _passwordResetRepository.Delete(email);

        }

        public async Task<bool> CheckCode(string email, string code)
        {
            var resetModel = await _passwordResetRepository.Get(email);

            if (resetModel is null)
                throw new KeyNotFoundException("Час вийшов, зробіть новий запит");

            return _bCryptHasher.VerifyPassword(code, resetModel.CodeHash);
        }

        public async Task GenerateResetCode(string email)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("InventorySystem", _emailData.From));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Код для відновлення паролю";

            var random = new Random();
            message.Body = new TextPart("plain")
            {
                Text = $"Ваш код для відновлення паролю: {random.Next(1000000,9999999)}"
            };

            var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailData.From, _emailData.AppPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
