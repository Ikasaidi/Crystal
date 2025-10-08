using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(-100)] // s’exécute AVANT l’UI et les gemmes
public class GemGoalManager : MonoBehaviour
{
    public static GemGoalManager Instance { get; private set; }

    [Header("Objectif")]
    public int requiredGems = 3;
    public int Current { get; private set; }

    public event Action<int,int> OnCountChanged;

    // remet à zéro les statics quand on (re)charge une scène sans domain reload
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Instance = null;
    }

    void Awake()
    {
        // singleton de scène
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Current = 0; // <- toujours repartir à 0 au lancement de la scène
    }

    void OnEnable()
    {
        // pousse une valeur initiale pour l’UI dès que possible
        OnCountChanged?.Invoke(Current, requiredGems);

        // si tu veux aussi reset à chaque changement de scène dans le même Play
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        Current = 0;
        OnCountChanged?.Invoke(Current, requiredGems);
    }

    public void AddGem(int amount = 1)
    {
        Current += amount;
        OnCountChanged?.Invoke(Current, requiredGems);
    }
}
