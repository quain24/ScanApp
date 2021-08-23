using Microsoft.Extensions.DependencyInjection;

namespace ScanApp.Common.Installers
{
    public static class FluentValidationCommonPropertyValidatorsInstaller
    {
        public static IServiceCollection AddCommonFluentValidationPropertyValidators(this IServiceCollection services)
        {
            //services.AddScoped(typeof(EmailValidator<,>), typeof(EmailValidator<,>));
            //services.AddScoped(typeof(IdentityNamingValidator<,>), typeof(IdentityNamingValidator<,>));
            //services.AddScoped(typeof(MustContainOnlyLettersOrAllowedSymbolsValidator<,>), typeof(MustContainOnlyLettersOrAllowedSymbolsValidator<,>));
            //services.AddScoped(typeof(PhoneNumberValidator<,>), typeof(PhoneNumberValidator<,>));

            return services;
        }
    }
}