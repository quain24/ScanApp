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

    public static class ModuleNames
    {
        public const string AdminModule = "Admin";
        public const string SparePartsModule = "SpareParts";
    }
}