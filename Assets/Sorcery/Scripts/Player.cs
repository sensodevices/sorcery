using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject BloodBlur;
    private bool isShielded = false;

	// Use this for initialization
	void Start ()
    {
        BloodBlur.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetShielded(bool toggle)
    {
        isShielded = toggle;
    }

    public void Hit()
    {
        if (!isShielded)
        {
            StartCoroutine(StartHitEffect());
        }
    }

    IEnumerator StartHitEffect()
    {
        BloodBlur.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        BloodBlur.SetActive(false);
    }
}
