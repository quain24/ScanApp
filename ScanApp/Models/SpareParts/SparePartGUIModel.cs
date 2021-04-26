namespace ScanApp.Models.SpareParts
{
    public class SparePartGUIModel
    {
        public SparePartGUIModel()
        {
        }

        public SparePartGUIModel(string name)
        {
            Name = name;
        }

        public SparePartGUIModel(string name, int amount) : this(name)
        {
            Amount = amount;
        }

        public string Name { get; set; }
        public int Amount { get; set; }
    }
}