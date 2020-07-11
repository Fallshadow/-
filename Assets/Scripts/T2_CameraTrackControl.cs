using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public enum CameraPointType
{
    Base = 0,
    Face = 1,
    Feature = 2,
}

public enum LimitBounary
{
    InsideLimit = 1,
    OutsideLimit = -1,
    NoneLimit = 0,
}

[System.Serializable]
public class T2_CameraTrackPoint
{
    public Vector3 PointPos;
    public Vector3 LookAtPos;

    public float Distance()
    {
        return (PointPos - LookAtPos).magnitude;
    }
}
public class T2_CameraTrackControl : MonoBehaviour
{
    public T2_CameraTrackPoint[] AllPointOnTrack;
    public T2_CameraTrackPoint CurCameraTrackPoint;
    public Cinemachine.CinemachineVirtualCamera[] cinemachineVirtualCameras;
    public CameraPointType CurType = CameraPointType.Base;

    public Vector3 curLookPoint;

    public float LerpSpeed = 1;
    public float mouseSpeed = 10;
    public float wheelSpeed = 10;
    public float wheelTime = 2;
    public float ChangeSpeed = 2;

    public float ErrorRange = 0.01f;
    public bool IsAutoMove = false;

    public Color PointColor = Color.blue;
    public float PointRadius = 0.1f;
    public Color CameraLookColor = Color.green;
    public Color PointCheckColor = Color.red;
    public Color DisColor = Color.yellow;

    private Rigidbody body;

    private bool justEnterCirle = false;
    private bool justExitCirle = false;
    //在转换之前自身坐标点
    private Vector3 beforeChangePoint;
    //在转换之前lookat点
    private Vector3 lateLookAtPoint;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        ResetPos();
    }

    public void ResetPos()
    {
        CurCameraTrackPoint = AllPointOnTrack[(int)CurType];
        transform.position = CurCameraTrackPoint.PointPos;
        curLookPoint = CurCameraTrackPoint.LookAtPos;
        justEnterCirle = true;
        justExitCirle = false;
        judgeDir();
    }


    public void ChangeTo(CameraPointType type)
    {
        if(CurType == type)
        {
            return;
        }
        CurType = type;
        CurCameraTrackPoint = AllPointOnTrack[(int)CurType];

        beforeChangePoint = transform.position;
        
        IsAutoMove = true;
        lateLookAtPoint = curLookPoint;
    }
    private float GetChangeCurPointRatio()
    {
        //  总距离  我们当时到改变的最终点距离
        //  现距离  我们目前到改变的最终点距离
        float twoTrackDis = Vector3.Distance(beforeChangePoint, CurCameraTrackPoint.PointPos);
        float nowDis = DistanceOfPoint(CurCameraTrackPoint.PointPos);
        return nowDis / twoTrackDis;
    }

    private float GetPointRatio()
    {
        float totalDis = Vector3.Distance(startPoint,endPoint);
        float curDis = DistanceOfPoint(endPoint);
        return curDis / totalDis;
    }

    private Vector3 startPoint;
    private Vector3 endPoint;

    private Vector3 GetAnyDirWhenRotate()
    {
        if(CurType == CameraPointType.Feature)
        {
            return transform.forward;
        }

        Vector3 endShadow = new Vector3(transform.position.x, AllPointOnTrack[(int)CurType + 1].PointPos.y, transform.position.z);
        Vector3 endDir = (endShadow - AllPointOnTrack[(int)CurType + 1].LookAtPos).normalized;
        Vector3 end = endDir * AllPointOnTrack[(int)CurType + 1].Distance() + AllPointOnTrack[(int)CurType + 1].LookAtPos;
        endPoint = end;

        Vector3 vecS3 = Vector3.Project((end - CurCameraTrackPoint.LookAtPos), transform.position - end);
        float disS1 = (end - CurCameraTrackPoint.LookAtPos).sqrMagnitude;
        float disS2 = disS1 - vecS3.sqrMagnitude;
        float disX = Mathf.Sqrt(Mathf.Pow(CurCameraTrackPoint.Distance(), 2) - disS2) - vecS3.magnitude;
        Vector3 start = (transform.position - end).normalized * disX + end;

        startPoint = start;

        return end - start;
    }

    private Vector3 GetAnyDirWhenExitCircle()
    {
        if(CurType == CameraPointType.Feature)
        {
            return transform.forward;
        }

        //退出时投影在当前范围中的边际点为起点
        Vector3 startShadow = new Vector3(transform.position.x, CurCameraTrackPoint.PointPos.y, transform.position.z);
        Vector3 startDir = (startShadow - CurCameraTrackPoint.LookAtPos).normalized;
        Vector3 start = startDir * CurCameraTrackPoint.Distance() + CurCameraTrackPoint.LookAtPos;

        Vector3 end = transform.position;

        endPoint = end;
        startPoint = start;
        return end - start;
    }

    private Vector3 GetAnyDirWhenEnterCircle()
    {
        if(CurType == CameraPointType.Feature)
        {
            return transform.forward;
        }
        Vector3 start = transform.position;

        Vector3 endShadow = new Vector3(transform.position.x, AllPointOnTrack[(int)CurType + 1].PointPos.y, transform.position.z);
        Vector3 endDir = (endShadow - AllPointOnTrack[(int)CurType + 1].LookAtPos).normalized;
        Vector3 end = endDir * AllPointOnTrack[(int)CurType + 1].Distance() + AllPointOnTrack[(int)CurType + 1].LookAtPos;
        endPoint = end;

        startPoint = start;

        return end - start;
    }

    private void FixedUpdate()
    {

        int limitInside = 1;
        int limitOutside = 1;
        LimitBounary limitBounary;
        CheckOutBoundary(out limitBounary);
        switch(limitBounary)
        {
            case LimitBounary.InsideLimit:
                limitInside = 0;
                body.velocity = Vector3.zero;
                ChangeTo(CameraPointType.Feature);
                break;
            case LimitBounary.OutsideLimit:
                limitOutside = 0;
                body.velocity = Vector3.zero;
                ChangeTo(CameraPointType.Base);
                break;
            case LimitBounary.NoneLimit:
                limitInside = 1;
                limitOutside = 1;
                break;
            default:
                break;
        }
        if(moveDir == Vector3.zero)
        {
            moveDir = transform.forward;
        }
        //在这边先进行速度移动
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            {
                body.velocity = Input.GetAxis("Mouse ScrollWheel") * moveDir * Time.fixedDeltaTime * wheelSpeed * limitInside;
            }

            if(Input.GetAxis("Mouse ScrollWheel") < 0) // back
            {
                body.velocity = Input.GetAxis("Mouse ScrollWheel") * moveDir * Time.fixedDeltaTime * wheelSpeed *limitOutside;
            }
        }

    }
    private Vector3 moveDir = Vector3.zero;

    private void Update()
    {
        if(IsAutoMove)
        {
            
            if(Vector3.Distance(transform.position, AllPointOnTrack[(int)CurType].PointPos) < 0.1f)
            {
                body.velocity = transform.forward * Time.deltaTime * ChangeSpeed;
                if(DistanceOfPoint(CurCameraTrackPoint.LookAtPos) < CurCameraTrackPoint.Distance())
                {
                    IsAutoMove = false;
                    justEnterCirle = true;
                    justExitCirle = false;
                    judgeDir();
                }
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, AllPointOnTrack[(int)CurType].PointPos, 0.01f * ChangeSpeed);
            }
            curLookPoint = Vector3.Lerp(lateLookAtPoint, CurCameraTrackPoint.LookAtPos, (1 - GetChangeCurPointRatio()));
        }
        ChangeLookAtPoint();
        transform.LookAt(curLookPoint);
        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
        if(Input.GetKey(KeyCode.Mouse0) && !IsAutoMove)
        {
            transform.RotateAround(curLookPoint, Vector3.up, mouse_x * 5);
            transform.RotateAround(curLookPoint, transform.right, mouse_y * 5);

            float rotateTemp = transform.eulerAngles.x + mouse_y * 5;
            if(rotateTemp > 60 && rotateTemp < 300)
            {
                transform.RotateAround(curLookPoint, transform.right, -mouse_y * 5);
            }
            moveDir = GetAnyDirWhenRotate();
        }


        int index = 0;
        if( !IsAutoMove && CheckOutPointRange(out index))
        {
            ChangeCurTrackPoint(index);
            body.velocity = Vector3.zero;
            judgeDir();
        }
    }

    private void judgeDir()
    {
        if(CurType == CameraPointType.Feature)
        {
            return;
        }
        if(justExitCirle)
        {
            moveDir = GetAnyDirWhenExitCircle();
        }
        if(justEnterCirle)
        {
            moveDir = GetAnyDirWhenEnterCircle();
        }
    }
    //获取此时看向点的坐标
    private void ChangeLookAtPoint()
    {
        if(CurType == CameraPointType.Feature || IsAutoMove)
        {
            return;
        }

        if(body.velocity.magnitude > 0)
        {
            if(justEnterCirle)
            {
                float ratio = GetPointRatio();
                curLookPoint = Vector3.Lerp(CurCameraTrackPoint.LookAtPos, AllPointOnTrack[(int)CurType + 1].LookAtPos,  1-ratio);
            }

            if(justExitCirle)
            {
                float ratio = GetPointRatio();
                curLookPoint = Vector3.Lerp(CurCameraTrackPoint.LookAtPos, AllPointOnTrack[(int)CurType + 1].LookAtPos,1 - ratio);
            }
        }
    }

    //改变当前轨道点
    private void ChangeCurTrackPoint(int index)
    {
        CurType = CurType + index;
        CurCameraTrackPoint = AllPointOnTrack[(int)CurType];
    }

    //检查一下是否超过当前范围并给出要转换的index
    private bool CheckOutPointRange(out int index)
    {
        index = 0;
        float nowDis = DistanceOfPoint(CurCameraTrackPoint.LookAtPos);
        float nowPointDis = CurCameraTrackPoint.Distance();
        if(CurType == CameraPointType.Base)
        {
            if(nowDis >= nowPointDis)
            {
                return false;
            }
            float baseforwardDis = AllPointOnTrack[(int)CurType + 1].Distance();
            if(DistanceOfPoint(AllPointOnTrack[(int)CurType + 1].LookAtPos) < baseforwardDis)
            {
                justEnterCirle = true;
                justExitCirle = false;
                index = 1;
                return true;
            }
            return false;
        }

        if(CurType == CameraPointType.Feature)
        {
            if(nowDis <= nowPointDis)
            {
                return false;
            }
            if(DistanceOfPoint(CurCameraTrackPoint.LookAtPos) > nowPointDis)
            {
                justEnterCirle = false;
                justExitCirle = true;
                index = -1;
                return true;
            }
            return false;
        }

        float forwardDis = AllPointOnTrack[(int)CurType + 1].Distance();
        //进入到小圆范围
        if(DistanceOfPoint(AllPointOnTrack[(int)CurType + 1].LookAtPos) < forwardDis)
        {
            index = 1;
            justEnterCirle = true;
            justExitCirle = false;
            return true;
        }
        //进入到大圆范围
        if(DistanceOfPoint(CurCameraTrackPoint.LookAtPos) > nowPointDis)
        {
            index = -1;
            justEnterCirle = false;
            justExitCirle = true;
            return true;
        }
        return false;
    }

    //检查一下是否超过最大边界并以此决定是否限制滚轮移动 dir 为 1 意味着向内到达限制，dir 为-1 意味着向外到达限制，为零意味无限制
    private bool CheckOutBoundary(out LimitBounary limit)
    {
        limit = LimitBounary.NoneLimit;
        float nowDis = DistanceOfPoint(CurCameraTrackPoint.LookAtPos);
        float nowPointDis = CurCameraTrackPoint.Distance();

        if(CurType == CameraPointType.Base)
        {
            if(nowDis > nowPointDis)
            {
                limit = LimitBounary.OutsideLimit;
                return true;
            }
        }

        if(CurType == CameraPointType.Feature)
        {
            if(nowDis < nowPointDis)
            {
                limit = LimitBounary.InsideLimit;
                return true;
            }
        }
        return false;
    }

    public float DistanceOfPoint(Vector3 point)
    {
        return (transform.position - point).magnitude;
    }

    private void OnDrawGizmos()
    {
        foreach(var item in AllPointOnTrack)
        {
            Gizmos.color = PointColor;
            Gizmos.DrawSphere(item.PointPos, PointRadius);
            Gizmos.DrawLine(item.PointPos, item.LookAtPos);
            Gizmos.color = PointCheckColor;
            Gizmos.DrawSphere(item.LookAtPos, PointRadius);
            Gizmos.DrawWireSphere(item.LookAtPos, (item.Distance()));
        }
        Gizmos.color = CameraLookColor;
        Gizmos.DrawLine(transform.position, curLookPoint);
        Gizmos.color = DisColor;

        if(CurType == CameraPointType.Feature)
        {
            return;
        }

        Vector3 endShadow = new Vector3(transform.position.x, AllPointOnTrack[(int)CurType + 1].PointPos.y, transform.position.z);
        Vector3 endDir = (endShadow - AllPointOnTrack[(int)CurType + 1].LookAtPos).normalized;
        Vector3 end = endDir * AllPointOnTrack[(int)CurType + 1].Distance() + AllPointOnTrack[(int)CurType + 1].LookAtPos;



        Vector3 vecS3 = Vector3.Project((end - CurCameraTrackPoint.LookAtPos),transform.position - end);
        float disS1 = (end - CurCameraTrackPoint.LookAtPos).sqrMagnitude;
        float disS2 = disS1 - vecS3.sqrMagnitude;
        float disX = Mathf.Sqrt(Mathf.Pow(CurCameraTrackPoint.Distance(), 2) - disS2) - vecS3.magnitude;
        Vector3 start = (transform.position - end).normalized * disX + end;


        Vector3 shadowCenter = -vecS3 + end;
        Gizmos.DrawLine(shadowCenter, end);

        Gizmos.DrawSphere(endShadow, PointRadius);

        Gizmos.DrawLine(endShadow, AllPointOnTrack[(int)CurType + 1].LookAtPos);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(start, PointRadius);
        Gizmos.DrawSphere(end, PointRadius);
    }
}
