using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.InputSystem;

namespace MausTemple
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        private float _timer;
        private int _chestsCollected;
        private bool _waitingForRestart;

        private void Start()
        {
            SceneManager.LoadScene(2, LoadSceneMode.Additive);

            if (!PlayerPrefs.HasKey("HighscoreTime"))
            {
                PlayerPrefs.SetFloat("HighscoreTime", 0f);
            }
            if (!PlayerPrefs.HasKey("HighscoreText"))
            {
                PlayerPrefs.SetString("HighscoreText", "00:00:00");
            }
        }

        private void Update()
        {
            if (_waitingForRestart) return;

            _timer += Time.deltaTime;

            int min = (int)_timer / 60;
            var minf = min > 9 ? $"{min}" : $"0{min}";

            int sec = (int)_timer % 60;
            var secf = sec > 9 ? $"{sec}" : $"0{sec}";

            int ms = (int)(Math.Round(_timer, 2) * 100) % 100;
            var msf = ms > 9 ? $"{ms}" : $"0{ms}";

            _timerText.text = $"{minf}:{secf}:{msf}";
        }

        public void OnCollect()
        {
            var mouseClicks = 10;
            if (++_chestsCollected == mouseClicks)
            {
                Time.timeScale = 0f;

                if (_timer > PlayerPrefs.GetFloat("HighscoreTime"))
                {
                    PlayerPrefs.SetFloat("HighscoreTime", _timer);
                    PlayerPrefs.SetString("HighscoreText", _timerText.text);

                    _timerText.text += "\nnew highscore!";
                }

                _timerText.text += "\n\npress any key to restart";
                _waitingForRestart = true;
            }
        }

        public void OnExit(InputAction.CallbackContext context)
        {
            Application.Quit();
        }

        public void OnContinue(InputAction.CallbackContext context)
        {
            if (_waitingForRestart && context.canceled)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
