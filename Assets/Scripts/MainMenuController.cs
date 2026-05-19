using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var startButton = root.Q<Button>("StartButton");
        if (startButton != null)
        {
            startButton.clicked += () => SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogError("Кнопка");
        }

        var quitButton = root.Q<Button>("QuitButton");
        if (quitButton != null)
        {
            quitButton.clicked += () =>{UnityEditor.EditorApplication.isPlaying = false;};
        }
        else
        {
            Debug.LogError("Кнопка");
        }
    }
    void Update()
    {

    }
}
