using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Content.Shared.TelecrystalExchange;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Utility;

namespace Content.Client.TelecrystalExchange
{
    public sealed class TelecrystalExchangeWindow : DefaultWindow
    {
        [Dependency] private readonly IEntityManager _entity = default!;

        private ItemSlot DiskSlot => GetChild<ItemSlot>("DiskSlot");
        private Button ExchangeButton => GetChild<Button>("ExchangeButton");
        private Label MessageLabel => GetChild<Label>("MessageLabel");

        public TelecrystalExchangeWindow()
        {
            IoCManager.InjectDependencies(this);
            RobustXamlLoader.Load(this);

            ExchangeButton.OnPressed += _ => TryExchange();

            DiskSlot.OnItemChanged += args => UpdateMessage();
        }

        private void TryExchange()
        {
            var item = DiskSlot.Item?.Owner;
            var message = new TelecrystalExchangeMessage(
                _entity.GetEntity(OwnerUid),
                item);

            UserInterfaceManager.SendUiMessage(this, message);
        }

        private void UpdateMessage()
        {
            if (DiskSlot.Item == null)
            {
                MessageLabel.Text = Loc.GetString("telecrystal-exchange-insert-disk");
                MessageLabel.FontColorOverride = Color.White;
            }
        }

        public void UpdateState(TelecrystalExchangeBoundUserInterfaceState state)
        {
            if (state.Message != null)
            {
                MessageLabel.Text = Loc.GetString(state.Message);
                MessageLabel.FontColorOverride = state.Message.Contains("error") ? Color.Red : Color.Green;
            }
        }
    }
}
