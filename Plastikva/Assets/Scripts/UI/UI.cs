using R3;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class UI : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Label _trashLabel;

    [Inject] private TrashView _viewTrash;

    private readonly CompositeDisposable _disposables = new();

    public void Initialize()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        //_trashLabel = _root.Q<Label>("trashLabel");

        _viewTrash.CurrentCount
      .Subscribe(count =>
      {
          Debug.Log(count);

          //_trashLabel.text = $"{count}/{_viewTrash.MaxCount}";

      });

    }
    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
