using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Movement_Sound_Manager : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private float firstPersonVolume = 1f;
   // [SerializeField] private float thirdPersonVolume = 0.7f;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("References")]
    [SerializeField] private walk playerMovement;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip sprintSound;

    private AudioSource audioSource;
    private bool isMoving;
    private bool isSprinting;
    private float targetVolume;
    private AudioClip currentClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (playerMovement == null || !playerMovement.enabled) return;

        // فقط اگر اول شخص هستیم پردازش کن
        if (playerMovement.thirdperson) return;

        UpdateMovementState();
        UpdateAudioClip();
        UpdateVolume();
    }

    private void UpdateMovementState()
    {
        // تشخیص حرکت (با استفاده از Input یا بهتر: از خود playerMovement)
        bool verticalInput = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
        bool horizontalInput = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        isMoving = verticalInput || horizontalInput;

        // تشخیص دویدن
        isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
    }

    private void UpdateAudioClip()
    {
        AudioClip desiredClip = isMoving ? (isSprinting ? sprintSound : walkSound) : null;

        // فقط اگر کلیپ تغییر کرد، عوضش کن (جلوگیری از پرش صدا)
        if (desiredClip != null && desiredClip != currentClip)
        {
            if (audioSource.isPlaying)
            {
                // انتقال نرم: کلیپ جدید رو ست کن و بذار لوپ ادامه بده
                audioSource.clip = desiredClip;
                currentClip = desiredClip;
                // نکته: برای انتقال حرفه‌ای‌تر می‌تونید از AudioMixer یا crossfade استفاده کنید
            }
            else
            {
                audioSource.clip = desiredClip;
                currentClip = desiredClip;
                audioSource.Play();
            }
        }
        else if (desiredClip == null && audioSource.isPlaying)
        {
            // اگر حرکت متوقف شد، صدا رو فید اوت کن
            targetVolume = 0f;
        }
    }

    private void UpdateVolume()
    {
        // تنظیم حجم هدف بر اساس حالت
        if (!isMoving)
        {
            targetVolume = 0f;
        }
        else
        {
            targetVolume = firstPersonVolume;
            // اینجا می‌تونید لاجیک volume بر اساس سرعت هم اضافه کنید
        }

        // فید نرم حجم صدا
        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );

        // پاز کردن وقتی حجم صفر شد (بهینه‌سازی)
        if (audioSource.volume <= 0.01f && !isMoving)
        {
            audioSource.Pause();
        }
        else if (audioSource.volume > 0.01f && !audioSource.isPlaying && isMoving)
        {
            audioSource.UnPause();
        }
    }

    // متد عمومی برای تغییر حالت اول/سوم شخص
    public void OnPerspectiveChanged(bool isThirdPerson)
    {
        if (isThirdPerson)
        {
            targetVolume = 0f; // یا thirdPersonVolume اگر صدای سوم شخص دارید
        }
        else
        {
            targetVolume = isMoving ? firstPersonVolume : 0f;
        }
    }
}