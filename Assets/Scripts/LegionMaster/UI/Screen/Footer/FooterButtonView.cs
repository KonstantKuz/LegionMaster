using DG.Tweening;
using LegionMaster.Extension;
using LegionMaster.Tutorial.UI;
using LegionMaster.UI.Components;
using LegionMaster.UI.Screen.Footer.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LegionMaster.UI.Screen.Footer
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(TutorialUiElement))]
    public class FooterButtonView : MonoBehaviour
    {
        [SerializeField] private float _animationTime = 0.5f;
        [SerializeField] private FooterButtonType _type;
        [SerializeField] private ActivatableObject _notification;

        private Tweener _activeAnimation;
        private CompositeDisposable _disposable;
        private Button _button;
        private Animator _animator;
        private static readonly int BlendHash = Animator.StringToHash("Blend");
        private TutorialUiElement _tutorialUiElement;

        public FooterButtonType Type => _type;

        public void Init(FooterButtonModel model)
        {
            _disposable?.Dispose();
            _disposable = new CompositeDisposable();

            BlendValue = model.SelectedButton.Value == _type ? 1.0f : 0.0f;            
            model.SelectedButton.Subscribe(selected => SetSelected(selected == _type)).AddTo(_disposable);
            Button.OnClickAsObservable().Subscribe(it => model.OnClick(_type)).AddTo(_disposable);
            _notification.Init(model.Notification);

            TutorialUiElement.Id = GetTutorialId(model.Type);
;        }

        public static string GetTutorialId(FooterButtonType type)
        {
            return $"FooterButton_{type}";
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void SetSelected(bool isSelected)
        {
            _activeAnimation?.Kill();
            _activeAnimation = DOTween.To(() => BlendValue, value => BlendValue = value, isSelected ? 1.0f : 0.0f, _animationTime);
        }

        private Button Button => _button ??= GetComponent<Button>();

        private Animator Animator => _animator ??= GetComponent<Animator>();

        private float BlendValue
        {
            get => Animator.GetFloat(BlendHash);
            set => Animator.SetFloat(BlendHash, value);
        }

        private TutorialUiElement TutorialUiElement => _tutorialUiElement ??= GetComponent<TutorialUiElement>();
    }
}