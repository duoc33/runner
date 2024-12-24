using System;
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
        public bool IsFreeMove = false; // 是否是自由移动移动
        public int SplitMoveCount = 2 ; // 分割水平移动次数
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

        void Update()
        {
            if(!IsStart) return;
            
            if(targetPosition.z >= endingZ)
            {
                IsEnd = true;
                stop = true;
            }

            UpdateCamera(group);

            if (stop) return;

            UpdatePosX();

            userInput.isInput = false;

            PlayerNormalMove(group);
        }


        public void PlayerMove(Grid<BehavioursController> players , Vector3 playerSize)
        {
            Vector3 EdgePos = new Vector3(targetPosition.x - deltaX/2, targetPosition.z, 0);
            
            for (int h = 0; h < players.Height; h++)
            {
                for (int w = 0; w < players.Width; w++)
                {
                    if(players[w,h]!=null)
                    {
                        Vector3 agentPosition = EdgePos + new Vector3(w * playerSize.x, 0, - h * playerSize.z);
                        players[w, h].Move(agentPosition);
                    }
                }
            }
        }

        public void PlayerNormalMove(List<BehavioursController> group)
        {
            if(group == null || group.Count == 0) return;
            foreach(BehavioursController controller in group)
            {
                if(controller.gameObject.activeSelf)
                {
                    controller.Move(gameObject);
                }
            }
        }

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
            GameObject mcamera = Camera.main.gameObject;
            mcamera.TryGetComponent(out Cinemachine.CinemachineBrain brain);
            if(brain==null)
            {
                mcamera.AddComponent<CinemachineBrain>();
            }

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

        private List<float> TargetXPos = new List<float>();
        private int TargetXIndex = 0;
        public float GetCurrentTargetXPos() => TargetXPos[TargetXIndex];

        private float deltaX;
        private void StartMove(OnMapGenerated e)
        {
            mapBounds = Data.levelSO.GetMapBounds();
            PosXRange = new Vector2 ( mapBounds.min.x, mapBounds.max.x ) ;
            PosZRange = new Vector2 ( mapBounds.min.z, mapBounds.max.z ) ;
            targetPosition = transform.position ;
            transform.position = Data.PlayerStartPos ;
            deltaX = (PosXRange.y - PosXRange.x) / SplitMoveCount;
            for (int i = 0; i < SplitMoveCount; i++)
            {
                float x = PosXRange.x + (i+1) * deltaX - (deltaX/2);
                TargetXPos.Add(x);
            }
            TargetXIndex = SplitMoveCount / 2;

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
        
        private void UpdateCamera(List<BehavioursController> group)
        {
            if(group != null && group.Count != 0)
            {
                int count = 0;
                Vector3 center = default;
                foreach (BehavioursController controller in group)
                {
                    if(controller.gameObject.activeSelf)
                    {
                        center += controller.transform.position;
                        count++;
                    }
                }
                if(count<=0) return; 
                center /= count;
                CameratargetPosition = new Vector3(cinemachineTarget.position.x, cinemachineTarget.position.y, center.z);
                // 平滑过渡目标位置
                cinemachineTarget.position = Vector3.Lerp(cinemachineTarget.position, CameratargetPosition, Time.deltaTime * smoothSpeed);
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
            if(IsFreeMove)
            {
                if (userInput.isInput)
                {
                    float deltaX = targetPosition.x + (userInput.isRight ? horizontalMoveSpeed * Time.deltaTime : -horizontalMoveSpeed * Time.deltaTime);
                    float clampedX = Mathf.Clamp(deltaX, PosXRange.x, PosXRange.y);
                    targetPosition = new Vector3(clampedX, transform.position.y, targetPosition.z);
                }
                // 在Z轴上按照固定速度移动
                targetPosition.z += moveSpeed * Time.deltaTime;
                // 移动物体到目标位置
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            }
            else{
                if (userInput.isInput)
                {
                    // float oldX =  targetPosition.x;
                    
                    // float deltaX = (PosXRange.y - PosXRange.x) / SplitMoveCount;

                    float currentX = 0;

                    if(userInput.isRight)
                    {
                        TargetXIndex ++;
                        if(TargetXIndex >= TargetXPos.Count)
                        {
                            TargetXIndex = TargetXPos.Count - 1;
                        }
                        currentX = TargetXPos[TargetXIndex];
                    }
                    else{

                        TargetXIndex --;
                        if(TargetXIndex < 0)
                        {
                            TargetXIndex = 0;
                        }
                        currentX = TargetXPos[TargetXIndex];
                    
                    }

                    targetPosition = new Vector3(currentX, transform.position.y, targetPosition.z);
                }
                // 在Z轴上按照固定速度移动
                targetPosition.z += moveSpeed * Time.deltaTime;
                // 移动物体到目标位置
                transform.position = targetPosition;
            }
            
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
