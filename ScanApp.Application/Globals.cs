// ReSharper disable once CheckNamespace
namespace Globals
{
    /// <summary>
    /// Provides custom strong-typed claim types to be used in policies and similar.
    /// </summary>
    public static class ClaimTypes
    {
        public static string Location { get; } = "Location";
        public static string IgnoreLocation { get; } = "IgnoreLocation";
        public static string CanAdd { get; } = "CanAdd";
        public static string CanEdit { get; } = "CanEdit";
        public static string CanDelete { get; } = "CanDelete";
    }

    public static class ModuleNames
    {
        public static string AdminModule { get; } = "Admin";
        public static string SparePartsModule { get; } = "SpareParts";
    }
}