using UnityEngine;

// プレイヤーのアニメーション状態を管理するコンポーネント。
// IAnimationStateNotifierのイベントを購読し、Animatorパラメータに反映する。
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("参照")]
    [Tooltip("IAnimationStateNotifierを実装したコンポーネントを設定する")]
    [SerializeField] private MonoBehaviour animationStateNotifier;

    private Animator animator;
    private IAnimationStateNotifier notifier;

    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        notifier = animationStateNotifier as IAnimationStateNotifier;
    }

    private void OnEnable()
    {
        notifier.OnRunningChanged += HandleRunningChanged;
        notifier.OnJumpingChanged += HandleJumpingChanged;
    }

    private void OnDisable()
    {
        notifier.OnRunningChanged -= HandleRunningChanged;
        notifier.OnJumpingChanged -= HandleJumpingChanged;
    }

    private void HandleRunningChanged(bool value)
    {
        animator.SetBool(IsRunning, value);
    }

    private void HandleJumpingChanged(bool value)
    {
        animator.SetBool(IsJumping, value);
    }
}