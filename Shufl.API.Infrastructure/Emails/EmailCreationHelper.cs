using Shufl.API.Infrastructure.Emails.ViewModels;
using System;
using System.IO;
using System.Text;

namespace Shufl.API.Infrastructure.Emails
{
    public static class EmailCreationHelper
    {
        private static string FetchEmailTemplateString(string templateUri)
        {
            var htmlString = File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Emails/Templates/{templateUri}"),
                Encoding.UTF8);

            return htmlString;
        }

        private static string FormatHtmlString(string htmlString, params string[] arguments)
        {
            var argIndex = 0;
            foreach (var arg in arguments)
            {
                htmlString = htmlString.Replace("{" + argIndex + "}", arg);

                argIndex++;
            }

            return htmlString;
        }

        public static string CreateWelcomeVerificationEmailString(LinkEmailViewModel welcomeVerificationViewModel)
        {
            return CreateLinkEmailString("UserWelcomeVerification.html", welcomeVerificationViewModel);
        }

        public static string CreateVerificationEmailString(LinkEmailViewModel verificationViewModel)
        {
            return CreateLinkEmailString("UserVerification.html", verificationViewModel);
        }

        public static string CreatePasswordResetEmailString(LinkEmailViewModel passwordResetViewModel)
        {
            return CreateLinkEmailString("UserPasswordReset.html", passwordResetViewModel);
        }

        private static string CreateLinkEmailString(string htmlFile, LinkEmailViewModel linkEmailViewModel)
        {
            var htmlString = FetchEmailTemplateString(htmlFile);
            var formattedHtmlString = FormatHtmlString(
                htmlString,
                linkEmailViewModel.FullName,
                linkEmailViewModel.Link);

            return formattedHtmlString;
        }
    }
}
