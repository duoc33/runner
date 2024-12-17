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
        float smoothSpeed = 10f; // 控制Camera平滑速度
        Vector3 CameratargetPosition;

        private const string CinemachineCameraPath = "Runner/Camera/Cinemachine";
        private CinemachineVirtualCamera virtualCamera;
        private Transform cinemachineTarget;

        Bounds mapBounds;
        Vector2 PosXRange;
        Vector2 PosZRange;
        bool IsStart = false;


        

        UserInput userInput;
        void Start()
        {
            IsStart = false;
            stop = true;
            userInput = gameObject.AddComponent<UserInput>();

            this.RegisterEvent<OnMapGenerated>(StartMove).UnRegisterWhenGameObjectDestroyed(this);
        }
        private void InitCamera()
        {
            virtualCamera ??= Instantiate(Resources.Load<CinemachineVirtualCamera>(CinemachineCameraPath));
            cinemachineTarget = new GameObject("CinemachineTarget").transform;
            cinemachineTarget.position = Data.PlayerStartPos;
            virtualCamera.Follow = cinemachineTarget;
            virtualCamera.LookAt = cinemachineTarget;
            
            Cinemachine3rdPersonFollow virtualCameraFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>(); 
            virtualCameraFollow.VerticalArmLength = CameraVerticalLength;
            virtualCameraFollow.CameraDistance = CameraDistance;
            
            // 计算摄像机与目标的距离
            float distance = CameraDistance;
            // 计算新的FOV
            float fov = 2f * Mathf.Atan(Data.levelSO.Rule.GetMapRealWidth() / (2f * distance)) * Mathf.Rad2Deg;

            // 更新摄像机的FOV
            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(fov, 1f, 179f);
        }
        public void AdjustCamera(Bounds PlayerGroupRange)
        {
            // return;
            // 使用群体的尺寸动态计算摄像机参数
            float groupWidth = PlayerGroupRange.size.x;
            float groupHeight = PlayerGroupRange.size.y;
            float groupDepth = PlayerGroupRange.size.z;

            Vector3 groupCenter = PlayerGroupRange.center;

            // Debug.Log(PlayerGroupRange.size);
            // float x = groupDepth + 1;

            // 获取摄像机的跟随组件
            // Cinemachine3rdPersonFollow virtualCameraFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            // 确保整个群体都能在视野内
            // 计算摄像机到目标的距离
            // float distance = Mathf.Max(groupWidth, CameraDistance) / (2f * Mathf.Tan(virtualCamera.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad));
            // distance = Mathf.Max(distance, groupDepth); // 保证深度方向不超出视野
            // virtualCameraFollow.CameraDistance = distance;

            // // 设置摄像机高度（根据群体的中心和高度动态调整）
            // virtualCameraFollow.VerticalArmLength = groupHeight / 2f ; // CameraVerticalLength 是一个偏移值

            // 计算动态的 FOV（水平视角）
            // float horizontalFov = 2f * Mathf.Atan(groupDepth / (2f * CameraDistance)) * Mathf.Rad2Deg;
            // virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(horizontalFov, 1, 179f); // 避免 FOV 太小或太大
        }

        private void StartMove(OnMapGenerated e)
        {
            mapBounds = Data.levelSO.GetMapBounds();
            PosXRange = new Vector2(mapBounds.min.x, mapBounds.max.x);
            PosZRange = new Vector2(mapBounds.min.z, mapBounds.max.z);
            targetPosition = transform.position;
            transform.position = Data.PlayerStartPos;

            IsEnd = false;
            stop = false;

            InitCamera();

            endingZ = mapBounds.max.z - (Data.levelSO.Rule.EndingRemainLength / 2.0f);

            IsStart = true;
        }
        bool stop = false;
        Vector3 targetPosition; // 目标位置
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

            if(group != null && group.Count != 0)
            {
                Vector3 center = default;
                foreach (BehavioursController controller in group)
                {
                    center += controller.transform.position;
                }
                center /= group.Count;
                CameratargetPosition = new Vector3(cinemachineTarget.position.x, cinemachineTarget.position.y, center.z);
                // 平滑过渡目标位置
                cinemachineTarget.position = Vector3.Lerp(cinemachineTarget.position, CameratargetPosition, Time.deltaTime * smoothSpeed);
            }

            if (stop) return;


            UpdatePosX();

            userInput.isInput = false;

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
        public void StopMove() => this.stop = true;
        void UpdatePosX()
        {
            if(userInput.isInput)
            {
                float deltaX = targetPosition.x + (userInput.isRight ? horizontalMoveSpeed * Time.deltaTime : -horizontalMoveSpeed * Time.deltaTime);
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
            if(cinemachineTarget!=null)
            {
                Destroy(cinemachineTarget.gameObject);
            }
        }

    }


    public class UserInput : MonoBehaviour 
    {
        Vector2 screenSize;
        public bool isInput;
        public bool isRight;
        private void Awake() {
            screenSize = new Vector2(Screen.width, Screen.height);
        }
        private void Update() {
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
        }
    }

}
