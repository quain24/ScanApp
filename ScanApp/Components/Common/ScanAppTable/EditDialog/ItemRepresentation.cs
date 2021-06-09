using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ScanApp.Components.Common.ScanAppTable.EditDialog
{
    public class ItemRepresentation<TItem>
    {
        public object[] Objects { get; set; }
        public string[] Strings { get; set; }
        public int?[] Ints { get; set; }
        public DateTime?[] DateTimes { get; set; }
        public double?[] Doubles { get; set; }
        public decimal?[] Decimals { get; set; }
        public Type[] Types { get; set; }
        private TItem _item { get; set; }
        private PropertyInfo[] _properties { get; set; }

        public ItemRepresentation(PropertyInfo[] properties, TItem item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item), "Item argument cannot be null");
            _properties = properties ?? throw new ArgumentNullException(nameof(_properties), "Properties argument cannot be null.");

            int size = properties.Count();

            Objects = new object[size];
            Types = new Type[size];
            Strings = new string[size];
            Ints = new int?[size];
            DateTimes = new DateTime?[size];
            Doubles = new double?[size];
            Decimals = new decimal?[size];

            SetTypes(size);
            SetObjects(size);
            MapItemToRepresentation();
        }

        private void SetTypes(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Types[i] = _properties[i].PropertyType;
            }
        }

        private void SetObjects(int size)
        {
            for (int i = 0; i < size; i++)
            {
                Objects[i] = _properties[i].GetValue(_item);
                if (Objects[i] is null)
                {
                    Objects[i] = CreateDefaultInstance(_properties[i]);
                }
            }
        }

        private object CreateDefaultInstance(PropertyInfo propInfo)
        {
            try
            {
                return Activator.CreateInstance(propInfo.PropertyType);
            }
            catch (Exception e)
            {
                if (propInfo.PropertyType == typeof(string))
                {
                    return string.Empty;
                }

                throw new ArgumentNullException(nameof(propInfo),
                    "Could not create a default instance of a property. " +
                    "Creation of new instance is caused by null values passed to the table.");
            }
        }

        private void MapItemToRepresentation()
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] is null)
                {
                    continue;
                }

                switch (Objects[i])
                {
                    case string str:
                        Strings[i] = Objects[i].ToString();
                        break;

                    case int integer:
                        Ints[i] = Convert.ToInt32(Objects[i]);
                        break;

                    case DateTime datetime:
                        DateTimes[i] = Convert.ToDateTime(Objects[i]);
                        break;

                    case decimal dec:
                        Decimals[i] = Convert.ToDecimal(Objects[i]);
                        break;

                    case double dbl:
                        Doubles[i] = Convert.ToDouble(Objects[i]);
                        break;
                }
            }
        }

        public void SaveChanges()
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] is null)
                {
                    continue;
                }

                switch (Objects[i])
                {
                    case string str:
                        Objects[i] = Strings[i];
                        break;

                    case int integer:
                        Objects[i] = Ints[i];
                        break;

                    case DateTime datetime:
                        Objects[i] = DateTimes[i];
                        break;

                    case decimal dec:
                        Objects[i] = Decimals[i];
                        break;

                    case double dbl:
                        Objects[i] = Doubles[i];
                        break;
                }
            }
        }
    }
}