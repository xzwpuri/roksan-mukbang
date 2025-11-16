using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class ChangeScenes : MonoBehaviour
{
    public void ChangeSceneGame()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
