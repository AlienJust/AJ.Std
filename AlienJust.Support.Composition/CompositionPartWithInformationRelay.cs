using AlienJust.Support.Composition.Contracts;

namespace AlienJust.Support.Composition
{
    public class CompositionPartWithInformationRelay : ICompositionPartWithInformation
    {
        public CompositionPartWithInformationRelay(ICompositionPart relayPart)
        {
            CompositionPart = relayPart;
            IsInitComplete = false;
        }

        public void SetCompositionRoot(ICompositionRoot root)
        {
            CompositionPart.SetCompositionRoot(root);
        }

        public bool IsInitComplete { get; set; }

        public ICompositionPart CompositionPart { get; private set; }
    }
}