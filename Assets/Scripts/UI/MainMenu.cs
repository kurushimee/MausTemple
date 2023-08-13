using UnityEngine;
using UnityEngine.SceneManagement;

namespace MausTemple
{
    public class MainMenu : MonoBehaviour
    {
        public void OnPlay()
        {
            SceneManager.LoadScene(1);
        }

        public void OnLeave()
        {
            Application.Quit();
        }
    }
}
