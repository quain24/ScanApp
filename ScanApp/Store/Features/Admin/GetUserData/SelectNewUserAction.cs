namespace ScanApp.Store.Features.Admin.GetUserData
{
    public class GetUserDataAction
    {
        public string Name { get; }

        public GetUserDataAction(string name)
        {
            Name = name;
        }
    }
}