using UnityEngine;

namespace LegionMaster.Location.Arena
{
    public class ArenaRewardChest : MonoBehaviour
    {
        private const string OPENED_STATE = "ChestOpen";
        private const string CLOSED_STATE = "None";

        [SerializeField] private Animator _animator;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Open()
        {
            _animator.Play(OPENED_STATE);
        }

        public void Close()
        {
            _animator.Play(CLOSED_STATE);
        }
    }
}