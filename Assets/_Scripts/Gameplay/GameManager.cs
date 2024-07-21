using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject continueText;
    [SerializeField] private PlayerInput playerInput;

    private int _chestsCollected;
    private InputActionMap _gameplayActionMap;
    private float _timer;
    private InputActionMap _uiActionMap;
    private bool _waitingForRestart;

    private void Start() {
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

        if (!PlayerPrefs.HasKey("HighscoreTime")) PlayerPrefs.SetFloat("HighscoreTime", 0f);

        if (!PlayerPrefs.HasKey("HighscoreText")) PlayerPrefs.SetString("HighscoreText", "00:00:00");

        _gameplayActionMap = playerInput.actions.FindActionMap("Gameplay");
        _uiActionMap = playerInput.actions.FindActionMap("UI");

        _gameplayActionMap.Enable();
        _uiActionMap.Disable();
    }

    private void Update() {
        if (_waitingForRestart) return;

        _timer += Time.deltaTime;

        var min = (int)_timer / 60;
        var minf = min > 9 ? $"{min}" : $"0{min}";

        var sec = (int)_timer % 60;
        var secf = sec > 9 ? $"{sec}" : $"0{sec}";

        var ms = (int)(Math.Round(_timer, 2) * 100) % 100;
        var msf = ms > 9 ? $"{ms}" : $"0{ms}";

        timerText.text = $"{minf}:{secf}:{msf}";
    }

    private void EndGame() {
        Time.timeScale = 0f;

        if (_timer > PlayerPrefs.GetFloat("HighscoreTime")) {
            PlayerPrefs.SetFloat("HighscoreTime", _timer);
            PlayerPrefs.SetString("HighscoreText", timerText.text);

            timerText.text += "\nnew highscore!";
        }

        continueText.SetActive(true);
        _waitingForRestart = true;
        _gameplayActionMap.Disable();
        _uiActionMap.Enable();
    }

    public void OnCollect() {
#if UNITY_EDITOR
        const int mouseClicks = 1;
#else
            const int mouseClicks = 10;
#endif
        if (++_chestsCollected == mouseClicks) EndGame();
    }

    public void OnExit(InputAction.CallbackContext context) {
        Application.Quit();
    }

    public void OnContinue(InputAction.CallbackContext context) {
        if (!context.canceled) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
