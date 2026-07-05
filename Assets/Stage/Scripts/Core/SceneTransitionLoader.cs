using UnityEngine;
using UnityEngine.SceneManagement;

// 指定したシーンへ転換する(12 物置のドアのシーン転換など)
// 転換先はInspectorで設定し、演出側のイベントからLoad()を呼ぶ
public class SceneTransitionLoader : MonoBehaviour
{
    [Header("転換先")]
    [Tooltip("読み込むシーン名。Build Settingsに登録されている必要がある")]
    [SerializeField] private string sceneName;

    // Inspectorで設定したシーンへ転換する
    public void Load()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneTransitionLoader] シーン名が設定されていません。");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    // 引数でシーン名を指定して転換する
    public void Load(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        SceneManager.LoadScene(name);
    }
}