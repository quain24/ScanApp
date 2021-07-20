// ReSharper disable once CheckNamespace
namespace Globals
{
    /// <summary>
    /// Provides custom strong-typed claim types to be used in policies and similar.
    /// </summary>
    public static class ClaimTypes
    {
        public const string Location = "Location";
        public const string IgnoreLocation = "IgnoreLocation";
        public const string CanAdd = "CanAdd";
        public const string CanEdit = "CanEdit";
        public const string CanDelete = "CanDelete";
    }

    /// <summary>
    /// Provides names of modules, to be used throughout application.<br/>
    /// For example they can be used when setting up a custom policy.
    /// </summary>
    public static class ModuleNames
    {
        public const string AdminModule = "Admin";
        public const string SparePartsModule = "SpareParts";
        public const string Depots = "Depots";
    }

    /// <summary>
    /// Provides hard-coded names of special accounts.
    /// </summary>
    public static class AccountNames
    {
        public const string Administrator = "Administrator";
    }

    /// <summary>
    /// Provides hard-coded names of special user roles.
    /// </summary>
    public static class RoleNames
    {
        public const string Administrator = "Administrator";
    }
}