using System;
using System.Reflection;

namespace ScanApp.Components.Common.ScanAppTable.EditDialog
{
    public static class ItemCloner<TItem>
    {
        public static TItem Clone(TItem item, PropertyInfo[] properties)
        {
            TItem itemClone = (TItem)Activator.CreateInstance(item.GetType());

            foreach (var property in properties)
            {
                property.SetValue(itemClone, property.GetValue(item));
            }

            return itemClone;
        }
    }
}