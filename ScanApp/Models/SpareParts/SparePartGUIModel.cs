using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Models.SpareParts
{
    public class SparePartGUIModel
    {

        public SparePartGUIModel()
        {
            Name = String.Empty;
            Amount = 0;
        }
        public SparePartGUIModel(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }

        public string Name { get; set; }
        public int Amount { get; set; }
    }
}
