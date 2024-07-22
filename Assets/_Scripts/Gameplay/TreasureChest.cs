using UnityEngine;

/// <summary>
/// Handles the logic for a treasure chest.
/// </summary>
public class TreasureChest : MonoBehaviour {
    [SerializeField] private GameObject _highlight;
    [SerializeField] private AudioClip _collectSound;
    [Space(5)]
    [SerializeField] private float _interactDistance;

    [Header("Broadcasting on")]
    [SerializeField] private VoidEventChannelSO _channel;

    private Transform _playerTransform;
    private bool _canClick;
    private bool _mouseHover;

    private void Start() {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// Updates the treasure chest's clickable state and highlight based on the player's proximity and mouse hover.
    /// </summary>
    private void LateUpdate() {
        var directionToPlayer = (_playerTransform.position - transform.position).normalized;
        var hit = Physics2D.Raycast(transform.position, directionToPlayer, _interactDistance);

        if (hit)
            _canClick = hit.collider.CompareTag("Player");
        else
            _canClick = false;

        _highlight.SetActive(_canClick && _mouseHover);
    }

    private void OnMouseEnter() {
        _mouseHover = true;
    }

    private void OnMouseExit() {
        _mouseHover = false;
    }

    /// <summary>
    /// Handles the player's interaction with treasure chests.
    /// </summary>
    private void OnMouseDown() {
        // Only execute this method if the treasure chest is currently clickable
        if (!_canClick) return;

        // Raise an event on the specified event channel
        _channel.RaiseEvent();

        // Create a new game object to play the collect sound effect
        var soundObject = new GameObject("SFX");
        var audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(_collectSound);
        Destroy(soundObject, _collectSound.length);

        // Destroy the treasure chest object
        Destroy(gameObject);
    }
}
