using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
[RequireComponent(typeof(Button))]
public class BoutonEnter : MonoBehaviour
{
    private Button button;
    public string sceneToLoad = "Level1";
 
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TaskOnClip);
    }
 
    private void TaskOnClip()
    {
        Debug.Log("You have clicked the button!");
        SceneManager.LoadScene(sceneToLoad);
       
    }
}