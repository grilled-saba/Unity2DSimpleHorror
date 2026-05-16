using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// イベント2で発動するマップ反転演出を管理する。
// カメラのprojectionMatrixのY軸を反転させることで視覚のみを上下逆転させ、物理判定には影響を与えない。
// ランダム360度回転はカメラのZ軸回転で表現する。
public class MapFlipEffect : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("反転・回転を適用するカメラ")]
    [SerializeField] private Camera targetCamera;

    [Tooltip("入力状態を受け取るコンポーネント（IInputStateReceiverを実装したMonoBehaviour）")]
    [SerializeField] private MonoBehaviour inputReceiverObject;

    [Header("回転設定")]
    [Tooltip("ランダム回転にかかる時間（秒）")]
    [Range(0.5f, 5f)]
    [SerializeField] private float rotateDuration = 2.0f;

    [Tooltip("回転の最小角度（度）")]
    [Range(0f, 360f)]
    [SerializeField] private float rotateAngleMin = 0f;

    [Tooltip("回転の最大角度（度）")]
    [Range(0f, 360f)]
    [SerializeField] private float rotateAngleMax = 360f;

    [Header("イベント")]
    [SerializeField] private UnityEvent onFlipActivated;
    [SerializeField] private UnityEvent onFlipDeactivated;
    [SerializeField] private UnityEvent onRotateCompleted;

    private IInputStateReceiver inputReceiver;
    private bool isFlipped = false;
    private bool isRotating = false;

    private Quaternion originalRotation;

    private void Awake()
    {
        inputReceiver = inputReceiverObject as IInputStateReceiver;

        if (inputReceiverObject != null && inputReceiver == null)
            Debug.LogWarning("[MapFlipEffect] inputReceiverObjectがIInputStateReceiverを実装していません。");

        if (targetCamera == null)
            Debug.LogWarning("[MapFlipEffect] targetCameraが設定されていません。");
        else
            originalRotation = targetCamera.transform.localRotation;
    }

    // 上下反転を切り替える。反転中の場合は解除する
    public void TriggerFlip()
    {
        if (isFlipped)
            ResetFlip();
        else
            ApplyFlip();
    }

    // projectionMatrixのY軸を反転させて上下逆転を適用する
    public void ApplyFlip()
    {
        if (targetCamera == null) return;

        isFlipped = true;

        Matrix4x4 mat = targetCamera.projectionMatrix;
        mat.m11 *= -1f;
        targetCamera.projectionMatrix = mat;

        inputReceiver?.SetInputState(InputState.Inverted);
        onFlipActivated?.Invoke();
    }

    // projectionMatrixをリセットして反転を解除する
    public void ResetFlip()
    {
        if (targetCamera == null) return;

        isFlipped = false;

        targetCamera.ResetProjectionMatrix();
        targetCamera.transform.localRotation = originalRotation;

        inputReceiver?.SetInputState(InputState.Normal);
        onFlipDeactivated?.Invoke();
    }

    // ランダムな角度へのZ軸回転演出を開始する
    public void TriggerRandomRotation()
    {
        if (isRotating || targetCamera == null) return;

        float targetAngle = Random.Range(rotateAngleMin, rotateAngleMax);
        StartCoroutine(RotateCoroutine(targetAngle));
    }

    // 指定角度（度）への回転演出を開始する
    public void TriggerRotation(float targetAngleDegrees)
    {
        if (isRotating || targetCamera == null) return;

        StartCoroutine(RotateCoroutine(targetAngleDegrees));
    }

    // 現在のZ軸回転を取得する
    public float GetCurrentRotationZ()
    {
        if (targetCamera == null) return 0f;
        return targetCamera.transform.localEulerAngles.z;
    }

    // 現在の反転状態を取得する
    public bool IsFlipped => isFlipped;

    // 回転アニメーション中かどうかを取得する
    public bool IsRotating => isRotating;

    // SmoothStepを使ってカメラをZ軸方向に目標角度まで回転させるコルーチン
    private IEnumerator RotateCoroutine(float targetAngle)
    {
        isRotating = true;

        float elapsed = 0f;
        float startAngle = targetCamera.transform.localEulerAngles.z;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotateDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, smoothT);

            Vector3 euler = targetCamera.transform.localEulerAngles;
            euler.z = currentAngle;
            targetCamera.transform.localEulerAngles = euler;

            yield return null;
        }

        // 誤差の補正
        Vector3 finalEuler = targetCamera.transform.localEulerAngles;
        finalEuler.z = targetAngle;
        targetCamera.transform.localEulerAngles = finalEuler;

        isRotating = false;
        onRotateCompleted?.Invoke();
    }
}