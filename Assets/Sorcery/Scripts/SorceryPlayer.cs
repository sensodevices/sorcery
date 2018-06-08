using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SorceryPlayer : MonoBehaviour
{
    private HitScreenSettings m_hitScreenSettings;

    void Start ()
    {
        var ppgo = GameObject.FindGameObjectWithTag("PostProcessing");
        if (ppgo != null)
        {
            var ppv = ppgo.GetComponent<PostProcessVolume>();
            ppv.profile.TryGetSettings(out m_hitScreenSettings);
            if (m_hitScreenSettings) m_hitScreenSettings.enabled.value = false;
        } else
        {
            Debug.LogError("Postprocessing stack was not found");
        }
    }
	
    void Update () {
		
    }

    public void Hit()
    {
        if (m_hitScreenSettings != null)
        {
            m_hitScreenSettings.enabled.value = true;
            StartCoroutine(resetHitEffect());
        }
    }

    IEnumerator resetHitEffect()
    {
        yield return new WaitForSeconds(0.2f);
        m_hitScreenSettings.enabled.value = false;
    }
}
