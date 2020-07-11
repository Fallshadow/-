
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class FuncEditor : MonoBehaviour
{
    [MenuItem("Func/2.相机边移动边看向")]
    public static void Check()
    {
        Camera.main.transform.DOLookAt(new Vector3(0, 0, 0), 5);
        Camera.main.transform.DOMove(new Vector3(10, 10, 10), 5);
    }

    [MenuItem("Func/3.magnitude sqrmagnitude")]
    public static void CheckMagnitude()
    {
        Vector3 vectorN1 = new Vector3(1,1,1);
        Vector3 vectorN2 = new Vector3(2,2,2);
        Debug.Log((vectorN1 - vectorN2).magnitude);
        Debug.Log((vectorN1 - vectorN2).sqrMagnitude);
    }

    [MenuItem("Func/4.pow")]
    public static void CheckPow()
    {
        Debug.Log(Mathf.Pow(3,4));
    }

    [MenuItem("Func/5.resetToBase")]
    public static void resetToBase()
    {
        Camera.main.GetComponent<T2_CameraTrackControl>().ChangeTo(CameraPointType.Base);
    }
    [MenuItem("Func/5.resetToFace")]
    public static void resetToFace()
    {
        Camera.main.GetComponent<T2_CameraTrackControl>().ChangeTo(CameraPointType.Face);
    }
    [MenuItem("Func/5.resetToFeature")]
    public static void resetToFeature()
    {
        Camera.main.GetComponent<T2_CameraTrackControl>().ChangeTo(CameraPointType.Feature);
    }
}
