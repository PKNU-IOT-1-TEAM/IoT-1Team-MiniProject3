using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneChanger : MonoBehaviour
{
    private string activeScene = "Inside_Scene";
    public void Scene_Change()
    {

        // 씬을 로드. LoadSceneMode.Single은 이전 씬을 언로드하고 새로운 씬만 로드
        if (activeScene == "Inside_Scene")
        {
            // INSIDE_SC 씬을 언로드합니다.
            SceneManager.UnloadSceneAsync("Inside_Scene");
            // OUTSIDE_SC 씬 로드
            SceneManager.LoadScene("Outside_Scene", LoadSceneMode.Additive);
            activeScene = "Outside_Scene";
        }
        else if (activeScene == "Outside_Scene")
        {
            // OUTSIDE_SC 씬을 언로드합니다.
            SceneManager.UnloadSceneAsync("Outside_Scene");
            // OUTSIDE_SC 씬 로드
            SceneManager.LoadScene("Inside_Scene", LoadSceneMode.Additive);
            activeScene = "Inside_Scene";
        }
    }
}
