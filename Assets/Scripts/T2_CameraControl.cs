using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一凡哥给的
//摄像机操作   
//删减版   在实际的使用中可能会有限制的需求  比如最大远离多少  最近距离多少   不能旋转到地面以下等
public class T2_CameraControl : MonoBehaviour
{
    public Transform CenObj;//围绕的物体
    private Vector3 CenObj_Pos;
    private new Camera camera;
    public float damping = 3.0f;
    public float moveSpeed = 10f;

    public float mmSpeed;
    public float distance = 5;
    public float minDistance = 2;
    public float maxDistance = 30;

    void Start()
    {
        camera = GetComponent<Camera>();
        CenObj_Pos = CenObj.position;
    }
    void Update()
    {
        Ctrl_Cam_Move();
        Cam_Ctrl_Rotation();
    }
    //镜头的远离和接近
    public void Ctrl_Cam_Move()
    {
        //distance -= Input.GetAxis("Mouse ScrollWheel") * mmSpeed;
        //distance = Mathf.Clamp(distance, minDistance, maxDistance);
        ////Quaternion rotation = Quaternion.Euler(x_OriginAngle, y_OriginAngle, 0.0f);
        //Vector3 disVector = new Vector3(0.0f, 0.0f, -distance);
        //Vector3 position = rotation * disVector + target.position;
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(Vector3.forward * 0.5f);//速度可调  自行调整
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(Vector3.forward * -0.5f);//速度可调  自行调整
        }
    }
    //摄像机的旋转
    public void Cam_Ctrl_Rotation()
    {
        var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
        if(Input.GetKey(KeyCode.Mouse0))
        {
            transform.RotateAround(CenObj_Pos, Vector3.up, mouse_x * moveSpeed * Time.deltaTime * damping);
            transform.RotateAround(CenObj_Pos, transform.right, mouse_y * moveSpeed * Time.deltaTime * damping);
        }
    }
}