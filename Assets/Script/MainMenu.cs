using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Nom de la scène de jeu")]
    public string gameSceneName = "Level1"; // Nom de ta scène de jeu (ou Level1)

    // --- Bouton Jouer ---
    public void OnPlay()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
        else
            Debug.LogError("[MainMenu] Le nom de la scène de jeu est vide !");
    }

    // --- Bouton Quitter ---
    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // pour arrêter le mode Play dans l’éditeur
#else
        Application.Quit();  // pour quitter le jeu après build
#endif
    }
}
