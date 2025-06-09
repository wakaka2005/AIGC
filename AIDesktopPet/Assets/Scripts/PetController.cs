using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PetUIDragAndClick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalScale;
    private CanvasGroup canvasGroup;

    [Header("拖动动画设置")]
    public float dragScale = 1.2f; // 拖动时的缩放比例
    public float animationDuration = 0.3f; // 动画持续时间
    public Ease scaleEase = Ease.OutBack; // 缩放动画缓动类型

    [Header("旋转动画设置")]
    public bool enableRotation = true;
    public float rotationSpeed = 180f; // 每秒旋转角度

    [Header("透明度设置")]
    public bool enableFade = true;
    public float dragAlpha = 0.8f; // 拖动时的透明度

    [Header("点击动画设置")]
    public bool enableClickAnimation = true;
    public float clickScaleFactor = 1.3f; // 点击时的缩放倍数
    public float clickAnimationDuration = 0.4f; // 点击动画持续时间
    public Ease clickEase = Ease.OutElastic; // 点击动画缓动

    [Header("点击音效")]
    public AudioClip clickSound; // 点击音效
    public float clickVolume = 1f;

    [Header("弹跳效果")]
    public bool enableBounce = true;
    public float bounceIntensity = 0.1f; // 弹跳强度

    // 防止拖动时触发点击
    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private bool hasDragged = false; // 新增：标记是否真的拖动了

    // class 里
    private Quaternion originalRotation;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;

        // —— 新增这行 —— 
        originalRotation = rectTransform.localRotation;


        // 获取或添加 CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && enableFade)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 确保 enableClickAnimation 为 true
        Debug.Log($"📋 enableClickAnimation: {enableClickAnimation}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("🔧 开始拖动宠物");

        isDragging = true;
        hasDragged = false; // 重置拖动标记
        dragStartPosition = eventData.position;

        // 停止所有之前的动画
        rectTransform.DOKill();

        // 缩放动画
        rectTransform.DOScale(originalScale * dragScale, animationDuration)
            .SetEase(scaleEase);

        // 旋转动画
        if (enableRotation)
        {
            rectTransform.DORotate(new Vector3(0, 0, 360), 360f / rotationSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
        }

        // 透明度动画
        if (enableFade && canvasGroup != null)
        {
            canvasGroup.DOFade(dragAlpha, animationDuration * 0.5f);
        }

        // 轻微的弹跳效果
        if (enableBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOPunchScale(Vector3.one * bounceIntensity, 0.2f, 1, 0.5f));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // 标记为真正的拖动
        hasDragged = true;

        // 拖动 Image
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // 可选：根据拖动速度添加额外效果
        float dragSpeed = eventData.delta.magnitude;
        if (dragSpeed > 20f && enableBounce) // 快速拖动时的反馈
        {
            rectTransform.DOPunchScale(Vector3.one * 0.05f, 0.1f, 1, 0.1f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("🔧 结束拖动宠物");

        // 停止所有动画
        rectTransform.DOKill();

        // 恢复缩放 - 使用弹跳效果
        rectTransform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.OutBounce);

        // 停止旋转并平滑回到原位
        if (enableRotation)
        {
            rectTransform.DORotate(Vector3.zero, animationDuration)
                .SetEase(Ease.OutBack);
        }

        // 恢复透明度
        if (enableFade && canvasGroup != null)
        {
            canvasGroup.DOFade(1f, animationDuration);
        }

        // 结束时的小弹跳
        if (enableBounce)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f); // 稍微延迟
            sequence.Append(rectTransform.DOPunchPosition(Vector3.up * -5f, 0.3f, 3, 0.2f));
        }

        // ✅ 延迟一点时间后清除拖动状态（重要！）
        Invoke(nameof(ResetDragState), 0.15f); // 拖动状态不能立即清掉，否则会抢占点击判定
    }


    private void ResetDragState()
    {
        isDragging = false;
        hasDragged = false;
    }


    // 点击事件处理
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("✅ OnPointerClick 触发！");
        Debug.Log($"🔍 isDragging: {isDragging}, hasDragged: {hasDragged}");

        // 防止拖动时触发点击 - 判断点击中鼠标有没有明显移动
        float clickDelta = Vector2.Distance(eventData.pressPosition, eventData.position);
        Debug.Log($"🧭 点击移动距离: {clickDelta}");
        if (clickDelta > 10f)
        {
            Debug.Log("❌ 点击过程中鼠标移动过大，跳过点击动画");
            return;
        }


        Debug.Log("🎯 点击了宠物！");

        // 强制播放点击动画（用于测试）
        if (enableClickAnimation)
        {
            Debug.Log("🎭 开始播放点击动画");
            PlayClickAnimation();
        }
        else
        {
            Debug.Log("❌ 点击动画被禁用");
        }

        // 播放音效
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position, clickVolume);
        }

        // 可以在这里添加其他点击逻辑
        OnPetClicked();
    }

    // 点击动画
    private void PlayClickAnimation()
    {
        Debug.Log("🚀 PlayClickAnimation 开始执行！");

        // 检查组件是否存在
        if (rectTransform == null)
        {
            Debug.LogError("❌ rectTransform 为空！");
            return;
        }

        if (originalScale == Vector3.zero)
        {
            Debug.LogError("❌ originalScale 为零！重新获取...");
            originalScale = rectTransform.localScale;
        }

        Debug.Log($"🔍 当前缩放: {rectTransform.localScale}, 原始缩放: {originalScale}");

        // 停止当前动画
        rectTransform.DOKill();

        // 创建动画序列
        var sequence = DOTween.Sequence();

        // 快速放大
        var scaleUp = rectTransform.DOScale(originalScale * clickScaleFactor, clickAnimationDuration * 0.3f)
            .SetEase(Ease.OutQuad);
        sequence.Append(scaleUp);

        // 弹性回弹
        var scaleDown = rectTransform.DOScale(originalScale, clickAnimationDuration * 0.7f)
            .SetEase(clickEase);
        sequence.Append(scaleDown);

        // 添加一些额外的效果
        var punchRotation = rectTransform.DOPunchRotation(new Vector3(0, 0, 15), clickAnimationDuration, 2, 0.5f);
        sequence.Join(punchRotation);

        // 轻微的位置弹跳
        var punchPosition = rectTransform.DOPunchPosition(Vector3.up * 10f, clickAnimationDuration, 3, 0.3f);
        sequence.Join(punchPosition);

        // 如果有透明度组件，添加闪烁效果
        if (canvasGroup != null)
        {
            var fadeSequence = DOTween.Sequence();
            fadeSequence.Append(canvasGroup.DOFade(0.7f, 0.1f));
            fadeSequence.Append(canvasGroup.DOFade(1f, 0.1f));
            sequence.Join(fadeSequence);
        }

        // 添加完成回调
        sequence.OnComplete(() => {
            Debug.Log("✅ 点击动画播放完成！");
        });

        Debug.Log("🎬 动画序列已创建并开始播放");
    }

    // 自定义点击事件 - 可以被其他脚本重写或监听
    protected virtual void OnPetClicked()
    {
        Debug.Log("🐾 OnPetClicked 执行");

        // 可以在这里添加具体的宠物互动逻辑
        // 比如：增加好感度、播放语音、显示对话等

        // 示例：随机播放一些互动动画
        int randomAnimation = Random.Range(0, 3);
        Debug.Log($"🎲 随机动画索引: {randomAnimation}");

        switch (randomAnimation)
        {
            case 0:
                PlayHappyAnimation();
                break;
            case 1:
                PlayShakeAnimation();
                break;
            case 2:
                PlayJumpAnimation();
                break;
        }
    }

    // 开心动画
    private void PlayHappyAnimation()
    {
        Debug.Log("😊 播放开心动画");
        var sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOScale(originalScale * 1.1f, 0.2f));
        sequence.Append(rectTransform.DOScale(originalScale, 0.2f));
        sequence.Append(rectTransform.DOScale(originalScale * 1.05f, 0.15f));
        sequence.Append(rectTransform.DOScale(originalScale, 0.15f));
    }

    // 摇摆动画
    private void PlayShakeAnimation()
    {
        Debug.Log("🎋 播放摇摆动画，先重置再抖动");

        // 1. 停掉任何未完成的旋转 Tween
        rectTransform.DOKill();

        // 2. 立即强制归位到「最初旋转」
        rectTransform.localRotation = originalRotation;

        // 3. 做一次 PunchRotation，抖完后再把旋转锁回去
        rectTransform
            .DOPunchRotation(new Vector3(0, 0, 20f), 0.5f, 10, 0.5f)
            .OnComplete(() =>
            {
                // 确保抖完后，一定回到最初角度
                rectTransform.localRotation = originalRotation;
            });
    }


    // 跳跃动画
    private void PlayJumpAnimation()
    {
        Debug.Log("🦘 播放跳跃动画");
        var currentY = rectTransform.localPosition.y;
        var sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMoveY(currentY + 30f, 0.3f)
            .SetEase(Ease.OutQuad));
        sequence.Append(rectTransform.DOLocalMoveY(currentY, 0.3f)
            .SetEase(Ease.InQuad));
    }

    // 可以添加一些额外的动画方法
    public void PlayIdleAnimation()
    {
        if (!gameObject.activeInHierarchy) return;

        // 轻微的上下浮动动画
        var currentY = rectTransform.localPosition.y;
        rectTransform.DOLocalMoveY(currentY + 5f, 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopIdleAnimation()
    {
        rectTransform.DOKill();
    }

    // 添加一个测试方法，可以直接调用来测试动画
    [ContextMenu("测试点击动画")]
    public void TestClickAnimation()
    {
        Debug.Log("🧪 手动测试点击动画");
        if (enableClickAnimation)
        {
            PlayClickAnimation();
        }
    }
}