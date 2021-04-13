using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable
{
    public class ItemRepresentation
    {
        public object[] Objects { get; set; }
        public string[] Strings { get; set; }
        public int?[] Ints { get; set; }
        public DateTime?[] DateTimes { get; set; }
        public double?[] Doubles { get; set; }
        public decimal?[] Decimals { get; set; }
        private PropertyInfo[] _properties { get; set; }

        public ItemRepresentation(PropertyInfo[] properties)
        {
            _properties = properties;
            int size = properties.Count();
            Objects = new object[size];
            Strings = new string[size];
            Ints = new int?[size];
            DateTimes = new DateTime?[size];
            Doubles = new double?[size];
            Decimals = new decimal?[size];
        }
        public void MapItemToArrays()
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] is null)
                {
                    continue;
                }
                if (Objects[i].GetType() == typeof(string))
                {
                    Strings[i] = Objects[i].ToString();
                }
                else if (Objects[i].GetType() == typeof(int))
                {
                    Ints[i] = Convert.ToInt32(Objects[i]);
                }
                else if (Objects[i].GetType() == typeof(DateTime))
                {
                    DateTimes[i] = Convert.ToDateTime(Objects[i]);
                }
                else if (Objects[i].GetType() == typeof(decimal))
                {
                    Decimals[i] = Convert.ToDecimal(Objects[i]);
                }
                else if (Objects[i].GetType() == typeof(double))
                {
                    Doubles[i] = Convert.ToDouble(Objects[i]);
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
                if (Objects[i].GetType() == typeof(string))
                {
                    Objects[i] = Strings[i];
                }
                else if (Objects[i].GetType() == typeof(int))
                {
                    Objects[i] = Ints[i];
                }
                else if (Objects[i].GetType() == typeof(DateTime))
                {
                    Objects[i] = DateTimes[i];
                }
                else if (Objects[i].GetType() == typeof(decimal))
                {
                    Objects[i] = Decimals[i];
                }
                else if (Objects[i].GetType() == typeof(double))
                {
                    Objects[i] = Doubles[i];
                }
            }
        }
    }
}
