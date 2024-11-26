using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runner
{
    public class PlayerGroupTargetMotion : RunnerController
    {
        public float moveSpeed = 5f; // Z轴移动速度
        public float horizontalMoveSpeed = 5f; // X轴移动速度
        public float CameraVerticalLength = 5f; // 相机垂直距离
        public float CameraDistance = 13f; // 相机距离距离跟谁物体绝对距离

        private const string CinemachineCameraPath = "Runner/Camera/Cinemachine";
        private CinemachineVirtualCamera virtualCamera;
        private Transform cinemachineTarget;

        Bounds mapBounds;
        Vector2 PosXRange;
        Vector2 PosZRange;
        bool IsStart = false;
        Vector2 screenSize;
        void Start()
        {
            virtualCamera ??= Instantiate(Resources.Load<CinemachineVirtualCamera>(CinemachineCameraPath));
            IsStart = false;
            // 获取屏幕尺寸，以便后续判断
            screenSize = new Vector2(Screen.width, Screen.height);
            isInput = false;
            isRight = false;
            stop = true;
            
            cinemachineTarget = new GameObject("CinemachineTarget").transform;
            virtualCamera.Follow = cinemachineTarget;
            virtualCamera.LookAt = cinemachineTarget;
            
            Cinemachine3rdPersonFollow virtualCameraFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>(); 
            virtualCameraFollow.VerticalArmLength = CameraVerticalLength;
            virtualCameraFollow.CameraDistance = CameraDistance;
            

            this.RegisterEvent<OnMapGenerated>(StartMove).UnRegisterWhenGameObjectDestroyed(this);
        }
        private void StartMove(OnMapGenerated e)
        {
            mapBounds = Data.levelSO.GetMapBounds();
            PosXRange = new Vector2(mapBounds.min.x, mapBounds.max.x);
            PosZRange = new Vector2(mapBounds.min.z, mapBounds.max.z);
            targetPosition = transform.position;
            transform.position = Data.PlayerStartPos;
            cinemachineTarget.position = Data.PlayerStartPos;
            
            IsEnd = false;
            stop = false;
            
            // 计算摄像机与目标的距离
            float distance = CameraDistance;
            // 计算新的FOV
            float fov = 2f * Mathf.Atan(Data.levelSO.Rule.GetMapRealWidth() / (2f * distance)) * Mathf.Rad2Deg;

            endingZ = mapBounds.max.z - (Data.levelSO.Rule.EndingRemainLength / 2.0f);

            // 更新摄像机的FOV
            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(fov, 1f, 179f);


            IsStart = true;
        }
        bool stop = false;
        Vector3 targetPosition; // 目标位置
        bool isInput;
        bool isRight;
        float endingZ;
        private List<BehavioursController> group;

        public bool IsEnd = false;
        void Update()
        {
            if(!IsStart) return;
            if(targetPosition.z >= endingZ)
            {
                IsEnd = true;
                stop = true;
            }
            if (stop) {
                return;
            }
            // 检查是否按下鼠标左键
            if (Mouse.current.leftButton.isPressed)
            {
                isInput = true;
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                isRight = mousePosition.x > screenSize.x / 2;
            }
            // AD left Arrow right Arrow
            if (Keyboard.current.aKey.isPressed)
            {
                isInput = true;
                isRight = false;
            }
            if (Keyboard.current.dKey.isPressed)
            {
                isInput = true;
                isRight = true;
            }

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                isInput = true;
                isRight = false;
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                isInput = true;
                isRight = true;
            }

            UpdatePosX(isInput,isRight);
            
            cinemachineTarget.position = new Vector3(cinemachineTarget.position.x , cinemachineTarget.position.y ,targetPosition.z);

            isInput = false;

            if(group == null || group.Count == 0) return;
            foreach(BehavioursController controller in group)
            {
                controller.Move(gameObject);
            }
            
        }
        public void StartMove()
        {
            if(group==null || group.Count == 0) 
            {
                group = GetComponent<PlayerAgentGroup>().SelfGroup;
                if(group==null || group.Count==0) 
                {
                    this.stop = true;
                    return;
                }
            }
            this.stop = false;
        }
        public void StopMove()
        {
            this.stop = true;
        }
        Vector3 GetCenterPos(List<Vector3> pos)
        {
            if (pos == null || pos.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 center = Vector3.zero;

            foreach (var point in pos)
            {
                center += point;
            }

            // 计算中心坐标
            center /= pos.Count;

            return center;
        }
        void UpdatePosX(bool hasInput , bool isRight)
        {
            if(hasInput)
            {
                float deltaX = targetPosition.x + (isRight ? horizontalMoveSpeed * Time.deltaTime : -horizontalMoveSpeed * Time.deltaTime);
                float clampedX = Mathf.Clamp(deltaX, PosXRange.x, PosXRange.y);
                targetPosition = new Vector3(clampedX, transform.position.y , targetPosition.z);
            }
            
            // 在Z轴上按照固定速度移动
            targetPosition.z += moveSpeed * Time.deltaTime;
            // 移动物体到目标位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        }

        void OnDestroy()
        {
            if(virtualCamera!=null)
            {
                Destroy(virtualCamera.gameObject);
            }
            Destroy(cinemachineTarget);
        }

    }
}
