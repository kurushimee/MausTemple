using UnityEngine;

namespace MausTemple
{
    public class TreasureChest : MonoBehaviour
    {
        [SerializeField] private GameObject _highlight;
        [SerializeField] private AudioClip _collectSound;

        [Header("Broadcasting on")]
        [SerializeField] private VoidEventChannelSO _channel = default;

        private Transform _playerTransform;
        private bool _canClick;
        private bool _mouseHover;

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void LateUpdate()
        {
            var interactDistance = 5f;
            _canClick = Vector3.Distance(transform.position, _playerTransform.position) <= interactDistance;
            _highlight.SetActive(_canClick && _mouseHover);
        }

        private void OnMouseEnter()
        {
            _mouseHover = true;
        }

        private void OnMouseExit()
        {
            _mouseHover = false;
        }

        private void OnMouseDown()
        {
            if (_canClick)
            {
                _channel.RaiseEvent();

                var soundObject = new GameObject("SFX");
                var audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.PlayOneShot(_collectSound);
                Destroy(soundObject, _collectSound.length);

                Destroy(gameObject);
            }
        }
    }
}
