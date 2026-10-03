using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonFuncs : MonoBehaviour
{
    [Header("audio")]
    public AudioClip[] AUDclick;
    public float AUDclickV;
    public AudioSource ClickADU;
    public void Resume()
    {
        ClickADU.Play();    
        GameManger.Instance.StartUnPause();
        ClickADU.PlayOneShot(AUDclick[Random.Range(0, AUDclick.Length)], AUDclickV);

    }


    public void Restart()
    {
        ClickADU.PlayOneShot(AUDclick[Random.Range(0, AUDclick.Length)], AUDclickV);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManger.Instance.StartUnPause();

    }

    public void Quit()
    {
        ClickADU.PlayOneShot(AUDclick[Random.Range(0, AUDclick.Length)], AUDclickV);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}
    

