using System.Text.Json;

namespace ScanApp.Components.Common.ScanAppTable.EditDialog
{
    public static class ItemCloner
    {
        public static TItem Clone<TItem>(TItem item)
        {
            var serialized = JsonSerializer.Serialize(item);
            return JsonSerializer.Deserialize<TItem>(serialized);
        }
    }
}