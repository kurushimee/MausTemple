using UnityEngine;

namespace MausTemple
{
    public class TreasureChest : MonoBehaviour
    {
        [SerializeField] private GameObject _highlight;
        [SerializeField] private AudioClip _collectSound;
        [Space(5)]
        [SerializeField] private float _interactDistance;

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
            var directionToPlayer = (_playerTransform.position - transform.position).normalized;
            var hit = Physics2D.Raycast(transform.position, directionToPlayer, _interactDistance);

            if (hit)
            {
                _canClick = hit.collider.CompareTag("Player");
            }
            else
            {
                _canClick = false;
            }

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
