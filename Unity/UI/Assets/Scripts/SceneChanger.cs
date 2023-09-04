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

        // ���� �ε�. LoadSceneMode.Single�� ���� ���� ��ε��ϰ� ���ο� ���� �ε�
        if (activeScene == "Inside_Scene")
        {
            // INSIDE_SC ���� ��ε��մϴ�.
            SceneManager.UnloadSceneAsync("Inside_Scene");
            // OUTSIDE_SC �� �ε�
            SceneManager.LoadScene("Outside_Scene", LoadSceneMode.Additive);
            activeScene = "Outside_Scene";
        }
        else if (activeScene == "Outside_Scene")
        {
            // OUTSIDE_SC ���� ��ε��մϴ�.
            SceneManager.UnloadSceneAsync("Outside_Scene");
            // OUTSIDE_SC �� �ε�
            SceneManager.LoadScene("Inside_Scene", LoadSceneMode.Additive);
            activeScene = "Inside_Scene";
        }
    }
}
