using Fluxor;

namespace ScanApp.Store.Features.Admin
{
    public class AdminFeature : Feature<AdminState>
    {
        public override string GetName() => "User Names state";

        protected override AdminState GetInitialState()
        {
            return new AdminState();
        }
    }
}