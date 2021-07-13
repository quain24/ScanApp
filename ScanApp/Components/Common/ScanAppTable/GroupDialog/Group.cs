using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// <see cref="string"/> representing key of a group.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// List of <see cref="TItem"/> representing items which are binded to a specific <see cref="Key"/>
        /// </summary>
        public List<TItem> ItemGroup { get; set; } = new List<TItem>();
    }
}