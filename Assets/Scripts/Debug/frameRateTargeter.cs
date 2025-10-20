using UnityEngine;

public class frameRateTargeter : MonoBehaviour
{
    public int targetFramerate = 60;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFramerate;
    }

    // Update is called once per frame
    void Update()
    {
        if(targetFramerate != Application.targetFrameRate)
            Application.targetFrameRate = targetFramerate;
    }
}
