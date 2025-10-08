using UnityEngine;
using TMPro;
using System.Collections;

public class GemGoalUI : MonoBehaviour
{
    public TMP_Text textUI;

    void Awake()
    {
        if (!textUI) textUI = GetComponent<TMP_Text>();
        if (textUI) textUI.text = "Gems : 0 / 0"; // valeur par défaut visible
    }

    void OnEnable()
    {
        StartCoroutine(WaitAndHook()); // robuste même si le manager est en retard
    }

    IEnumerator WaitAndHook()
    {
        // attend que le manager existe (1 frame suffit en général)
        while (GemGoalManager.Instance == null) yield return null;

        GemGoalManager.Instance.OnCountChanged += UpdateText;
        // push initial
        UpdateText(GemGoalManager.Instance.Current, GemGoalManager.Instance.requiredGems);
    }

    void OnDisable()
    {
        if (GemGoalManager.Instance)
            GemGoalManager.Instance.OnCountChanged -= UpdateText;
    }

    void UpdateText(int current, int required)
    {
        if (textUI)
            textUI.text = $"Gems : {current} / {required}";
    }
}
