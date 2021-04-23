﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.GroupDialog
{
    public class Group<TItem>
    {
        public Group(string key, List<TItem> itemList)
        {
            Key = key;
            ItemGroup = itemList;
        }

        public Group(string key, IGrouping<object, TItem> itemList)
        {
            Key = key;
            ItemGroup = itemList.ToList();
        }

        public string Key { get; set; }
        public List<TItem> ItemGroup { get; set; } = new List<TItem>();
    }
}
