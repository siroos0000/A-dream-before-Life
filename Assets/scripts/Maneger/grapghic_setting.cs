using UnityEngine;

public class grapghic_setting : MonoBehaviour
{
    public int fps_lock = 60;
    int vSynv;
    private void Awake()
    {
        vSynv = QualitySettings.vSyncCount;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps_lock;
        QualitySettings.vSyncCount = vSynv;
    }
}
