using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Senso {
	
	public class GestureFramework : MonoBehaviour {

		public Hand[] hands;
		
		public Transform effectsParent;
		public Transform pistolEffect;
		public Transform openPalmEffect;
		public Transform fistEffect;
		public Transform shieldEffect;
        public Transform handFireball;

        // TODO: make arrays or array of structures
        private bool pistolFired = false;
		private float prevAcc;
		private bool accelerating = false;
		private bool quickStop = false;
				
		private DateTime t_lastFired = DateTime.Now;
		private TimeSpan fireEvery;

        private TimeSpan t_palmOpened;
        private Transform handFireballInst = null;

        public Player aPlayer;

        private Coroutine FireballSpawnCoroutine;
		
		// Use this for initialization
		void Start () 
		{
			fireEvery = TimeSpan.FromSeconds(1);
		}
		
		// Update is called once per frame
		void Update () {
			quickStop = false;
            bool palmWaitFireball;
			
			int len = hands.Length;
			int i = 0;
			for (i = 0; i < len; ++i)
			{
				if (hands[i].Pose != null) 
				{
                    palmWaitFireball = false;
					var p = hands[i].Pose;
					bool indexBend = IsFingerBound(p.IndexAngles);
					bool middleBend = IsFingerBound(p.MiddleAngles);
					bool thirdBend = IsFingerBound(p.ThirdAngles);
					bool littleBend = IsFingerBound(p.LittleAngles);

					var now = DateTime.Now;
					/////////////////////////// Acceleration handling ////////////////////////
					if (p.PalmAccelerometer.magnitude < prevAcc) {
						if (accelerating == true) {
							accelerating = false;
							if (p.PalmAccelerometer.magnitude > 15 && now - t_lastFired > fireEvery) { // quick stop
								quickStop = true;
								t_lastFired = now;
							}
						}
					} else {
						if (accelerating == false) {
							accelerating = true;
						}
					}
					prevAcc = p.PalmAccelerometer.magnitude;
                    //////////////////////////////////////////////////////////////////////////

                    // Fist
                    if (IsFistFinger(p.IndexAngles) && IsFistFinger(p.MiddleAngles) && IsFistFinger(p.ThirdAngles) && IsFistFinger(p.LittleAngles)) {
                        if (handFireballInst != null)
                        {
                            UnityEngine.Object.Destroy(handFireballInst.gameObject, 0.0f);
                            handFireballInst = null;
                        }
                        else
                        {
                            if (quickStop)
                            {
                                hands[i].VibrateFinger(EFingerType.Thumb, 1000, 10);
                                hands[i].VibrateFinger(EFingerType.Index, 1000, 10);
                                hands[i].VibrateFinger(EFingerType.Middle, 1000, 10);
                                hands[i].VibrateFinger(EFingerType.Third, 1000, 10);
                                hands[i].VibrateFinger(EFingerType.Little, 1000, 10);
                                var obj = spawnEffect(fistEffect, hands[i].transform.position, hands[i].transform.rotation);
                                obj.Rotate(Vector3.forward * -45, Space.Self);
                            }
                        }
					}
                    // Open palm
                    else if (!indexBend && !middleBend && !thirdBend && !littleBend) { 
						if (quickStop) {
                            if (handFireballInst != null)
                            {
                                //UnityEngine.Object.Destroy(handFireballInst.gameObject, 0.0f);
                                var rigidBody = handFireballInst.GetComponent<Rigidbody>();
                                rigidBody.isKinematic = false;
                                rigidBody.AddForce(p.PalmAccelerometer, ForceMode.VelocityChange);
                                handFireballInst = null;
                                // TODO: register collision
                            }
                            else
                            {
                                var q = p.PalmRotation * Quaternion.Inverse(p.WristRotation);
                                var dx = q.eulerAngles.x; if (dx > 180.0f) dx -= 360.0f;
                                if (Mathf.Abs(dx) > 25)
                                {
                                    hands[i].VibrateFinger(EFingerType.Index, 100, 20);
                                    hands[i].VibrateFinger(EFingerType.Little, 100, 20);
                                    var palmDirection = FindObjectWithTag(hands[i].transform, "PalmDirection");
                                    var obj = spawnEffect(openPalmEffect, hands[i].transform.position, palmDirection.rotation);
                                    obj.Rotate(Vector3.forward * -45, Space.Self);
                                }
                            }
						} else
                        {
                            // shield
                            if (p.PalmSpeed.sqrMagnitude > 5 && p.PalmSpeed.y > 1.5f)
                            {
                                if (now - t_lastFired > fireEvery)
                                {
                                    StartCoroutine(ShieldSpawn(hands[i]));
                                    var camera = FindObjectWithTag(transform.parent, "MainCamera");
                                    var obj = UnityEngine.Object.Instantiate(shieldEffect, camera.transform.position - Vector3.up, Quaternion.identity);
                                    aPlayer.SetShielded(true);
                                    UnityEngine.Object.Destroy(obj.gameObject, 10.0f);
                                    t_lastFired = now;
                                }
                            } else
                            {
                                /*if (handFireballInst == null && p.PalmRotation.eulerAngles.z > 165 && p.PalmRotation.eulerAngles.z < 195)
                                {
                                    palmWaitFireball = true;
                                    if (FireballSpawnCoroutine == null)
                                        FireballSpawnCoroutine = StartCoroutine(vibrateCircle(hands[i], 2));
                                    t_palmOpened = t_palmOpened.Add(TimeSpan.FromSeconds(Time.deltaTime));
                                    if (t_palmOpened.TotalSeconds > 1.5f && handFireballInst == null)
                                    {
                                        var palmDirection = FindObjectWithTag(hands[i].transform, "PalmDirection");
                                        handFireballInst = UnityEngine.Object.Instantiate(handFireball, palmDirection, false);
                                        handFireballInst.localPosition = Vector3.forward * 0.1f;
                                    }
                                }*/
                            }
                        }
                    } else {
                        // Pistol!
                        if (!indexBend && IsFistFinger(p.MiddleAngles) && IsFistFinger(p.ThirdAngles) && IsFistFinger(p.LittleAngles)) {
							if (p.ThumbBend > 0.25) {
								if (!pistolFired) {
                                    hands[i].VibrateFinger(EFingerType.Index, 200, 60);
									pistolFired = true;
									var tip = FindObjectWithTag(hands[i].transform, "IndexTip");
									spawnEffect(pistolEffect, tip.position, hands[i].transform.rotation);
								}
							} else {
								if (pistolFired) pistolFired = false;
							}
						}
					}
                    /*if (!palmWaitFireball && t_palmOpened != TimeSpan.Zero)
                    {
                        t_palmOpened = TimeSpan.Zero;
                        if (FireballSpawnCoroutine != null)
                        {
                            StopCoroutine(FireballSpawnCoroutine);
                            FireballSpawnCoroutine = null;
                        }
                    }*/
				}
			}
		}
		
		bool IsFingerBound(Vector2 angles)
		{
			return angles.y > 30;
		}
		bool IsFistFinger(Vector2 angles)
		{
			return angles.y > 100;
		}
		
		Transform FindObjectWithTag(Transform parent, string _tag)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (child.tag == _tag)
				{
					return child.transform;
				}
				if (child.childCount > 0)
				{
					var obj = FindObjectWithTag(child, _tag);
					if (null != obj) return obj;
				}
			}
			return null;
		}
		
		Transform spawnEffect(UnityEngine.Transform effect, Vector3 position, Quaternion rotation)
		{
			var obj = UnityEngine.Object.Instantiate(effect, position, rotation);
			var comp = obj.GetComponentInChildren<RFX1_TransformMotion>(); 
			comp.CollisionEnter += onEffectCollision;
            return obj;
		}
		
		void onEffectCollision(object sender, RFX1_TransformMotion.RFX1_CollisionInfo i)
		{
			var comp = sender as RFX1_TransformMotion;
			if (comp != null) {
                var _parent = comp.transform.parent;
                var projectile = _parent.GetComponent<Projectile>();
                var damage = 30.0f;
                if (projectile != null)
                {
                    damage = projectile.damage;
                }
                var enemy = i.Hit.transform.GetComponent<Enemy>();
                if (enemy != null) // Enemy was hit
                {
                    enemy.Hit(damage);
                }
			}
		}

        IEnumerator ShieldSpawn(Senso.Hand hand)
        {
            for (int i = 0; i < 10; ++i)
            {
                hand.VibrateFinger((EFingerType)(i % 5), 500, 10);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(5.0f);
            aPlayer.SetShielded(false);
        }

        IEnumerator vibrateCircle(Senso.Hand hand, int cicles)
        {
            for (int k = 0; k < cicles; ++k)
            {
                for (int i = 0; i < 5; ++i)
                {
                    hand.VibrateFinger((EFingerType)i, 150, 10);
                    yield return new WaitForSeconds(0.15f);
                }
            }
        }
    }
}