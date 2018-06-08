using System.Collections;
using UnityEngine;

public class SorcererHand : Senso.Hand
{
    public Transform Palm;

    private Quaternion palmInitialRotation;

    public Transform[] thumbBones;
    public Transform[] indexBones;
    public Transform[] middleBones;
    public Transform[] thirdBones;
    public Transform[] littleBones;

    private Quaternion[][] fingerInitialRotations;

    // Use this for initialization
    public new void Start ()
    {
        base.Start();
        palmInitialRotation = Palm.localRotation;
        fingerInitialRotations = new Quaternion[5][];

        for (int i = 0; i < 5; i++)
        {
            Transform[] arr;
            switch (i)
            {
                case 1: arr = indexBones; break;
                case 2: arr = middleBones; break;
                case 3: arr = thirdBones; break;
                case 4: arr = littleBones; break;
                default: arr = thumbBones; break;
            }
            fingerInitialRotations[i] = new Quaternion[arr.Length];
            for (int j = 0; j < arr.Length; ++j)
                fingerInitialRotations[i][j] = arr[j].localRotation;
        }
    }

	public override void SetSensoPose(Senso.HandData data)
    {
        base.SetSensoPose(data);

        Palm.localPosition = transformVector3(data.PalmPosition);
        Palm.localRotation = palmInitialRotation * data.PalmRotation;

        if (data.AdvancedThumb)
        {
            //Quaternion thumbQ = new Quaternion(aData.ThumbQuaternion.y / 3.0f, aData.ThumbQuaternion.x, -aData.ThumbQuaternion.z, aData.ThumbQuaternion.w);
            Quaternion thumbQ = new Quaternion(data.ThumbQuaternion.x, data.ThumbQuaternion.y, -data.ThumbQuaternion.z / 3.0f, data.ThumbQuaternion.w);
            thumbBones[0].localRotation = fingerInitialRotations[0][0] * thumbQ;
            thumbBones[1].localRotation = fingerInitialRotations[0][1] * Quaternion.Euler(0.0f, 0.0f, -data.ThumbQuaternion.x / 3.0f);
            thumbBones[2].localRotation = fingerInitialRotations[0][2] * Quaternion.Euler(0.0f, 0.0f, -data.ThumbBend * Mathf.Rad2Deg);
        }

        setFingerBones(ref indexBones, data.IndexAngles, ref fingerInitialRotations[1]);
        setFingerBones(ref middleBones, data.MiddleAngles, ref fingerInitialRotations[2]);
        setFingerBones(ref thirdBones, data.ThirdAngles, ref fingerInitialRotations[3]);
        setFingerBones(ref littleBones, data.LittleAngles, ref fingerInitialRotations[4]);

		//setThumbBones(ref ThumbFingerJoints, data.Fingers[0].Angles, thumbInitialRotations);
    }

	private Quaternion transformQuaternion(Quaternion q)
	{
		Quaternion newQ;
		if (HandType != Senso.EPositionType.RightHand)
			newQ = new Quaternion(q.y, q.x, -q.z, q.w);
		else
			newQ = new Quaternion(-q.y, -q.x, -q.z, q.w);
		return newQ;
	}

	private Vector3 transformVector3(Vector3 vec)
	{
		Vector3 newVec = new Vector3(vec.x, vec.z, vec.y);
		return newVec;
	}

    /**
     * Sets finger angles
     */
    private void setFingerBones(ref Transform[] bones, Vector2 angles, ref Quaternion[] initialRotations)
    {
        if (angles.y < 0.0f)
        {
            var q = Quaternion.Euler(0.0f, angles.x, -angles.y);
            bones[0].localRotation = initialRotations[0] * q;
        }
        else
        {
            angles.y /= 2.0f;
            var q = Quaternion.Euler(0.0f, angles.x, -angles.y);
            for (int j = 0; j < bones.Length; ++j)
            {
                bones[j].localRotation = initialRotations[j] * q;
                if (j != 0) q = Quaternion.Euler(0.0f, 0.0f, -angles.y);
            }
        }
    }

	private static void setThumbBones(ref Transform[] bones, Vector2 angles, Quaternion[] initialRotations)
	{
		bones[0].localRotation = initialRotations[0];
		Vector3 a = new Vector3(0.0f, -angles.y, angles.x);
		a.z += 30.0f;
		bones[0].Rotate(angles);
	}


}
