using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(XRSocketInteractor))]
public class ChefHatSocket : MonoBehaviour {
    private XRSocketInteractor SocketInteractor;
    private ChefHatSocketSelectFilter SelectFilter;
    private Character Character;

    protected virtual void Reset() {
        Init();
    }

    protected virtual void Awake() {
        Init();

        SelectFilter = new ChefHatSocketSelectFilter(this);
    }

    private void Init() {
        SocketInteractor = SocketInteractor == null ? GetComponent<XRSocketInteractor>() : SocketInteractor;
        Character = Character == null ? GetComponentInParent<Character>() : Character;
    }

    protected virtual void OnEnable() {
        SocketInteractor.selectEntered.AddListener(OnSelectEntered);
        SocketInteractor.selectExited.AddListener(OnSelectExited);
        SocketInteractor.selectFilters.Add(SelectFilter);
    }
    private void OnDestroy() {
        SocketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        SocketInteractor.selectExited.RemoveListener(OnSelectExited);
        SocketInteractor.selectFilters.Remove(SelectFilter);
    }

    private void OnSelectEntered(SelectEnterEventArgs args) {
        Character.SetRole(CharacterRole.Player);
    }

    private void OnSelectExited(SelectExitEventArgs args) {
        Character.SetRole(CharacterRole.Spectator);
    }

    private class ChefHatSocketSelectFilter : IXRSelectFilter {
        public ChefHatSocket ChefHatSocket { get; }
        public bool canProcess => true;

        public ChefHatSocketSelectFilter(ChefHatSocket chefHatSocket) {
            ChefHatSocket = chefHatSocket;
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
            return ChefHatSocket.SocketInteractor.interactablesSelected.Count == 0 && ChefHatSocket.Character.transform.IsParentOf(interactor.transform);
        }
    }
}
