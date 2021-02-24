using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ScanApp.Areas.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScanApp.Common.Extensions
{
    public static class AuthorizationOptionsExtension
    {
        /// <summary>
        /// This method will scan and apply all custom policies found in <see cref="Policies"/> object
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public static void AddSharedPolicies(this AuthorizationOptions options,
            ILogger logger)
        {
            logger.LogInformation($"{nameof(AuthorizationPolicy)} Configuration started ...");
            var policies = FindPolicies();
            options.TryToAddPolicies(policies, logger);
            logger.LogInformation($"{nameof(AuthorizationPolicy)} Configuration completed.");
        }

        private static IEnumerable<PolicyInformation> FindPolicies()
        {
            var policyProvider = typeof(Policies);

            return policyProvider.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(methodInfo =>
                {
                    var parameterInfo = methodInfo.GetParameters();

                    // The method should configure the policy builder, not return a built policy, so void return type.
                    // The method has to accept the AuthorizationPolicyBuilder, and no other parameter.
                    return methodInfo.ReturnType == typeof(void) &&
                           parameterInfo.Length == 1 &&
                           parameterInfo[0].ParameterType == typeof(AuthorizationPolicyBuilder);
                })
                .Select(mi => new PolicyInformation(mi.Name, mi));
        }

        private static void TryToAddPolicies(this AuthorizationOptions options,
            IEnumerable<PolicyInformation> policies,
            ILogger logger)
        {
            foreach (var policy in policies)
            {
                try
                {
                    options.AddPolicy(policy.Name, builder => policy.Method.Invoke(null, new object[] { builder }));
                    logger.LogInformation($"Policy '{policy.Name}' was configured successfully.");
                }
                catch (Exception e)
                {
                    options.AddPolicy(policy.Name, Policies.PolicyConfigurationFailedFallback);
                    logger.LogCritical(e, $"Failed to configure policy '{policy.Name}'. Using fallback policy instead.");
                }
            }
        }

        private class PolicyInformation
        {
            internal string Name { get; }

            internal MethodInfo Method { get; }

            internal PolicyInformation(string name, MethodInfo method)
            {
                Name = name;
                Method = method;
            }
        }
    }
}