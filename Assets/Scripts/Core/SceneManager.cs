using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private string nextLevelName; // The name of the next level to load

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
