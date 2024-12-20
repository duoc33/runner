using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using QFramework;
using RuntimeComponentAdjustTools;
using UnityEngine;
namespace Runner
{
    /// <summary>
    /// 玩家代理组，
    /// </summary>
    public class PlayerAgentGroup : AbstractAgentGroupController
    {
        public int MaxVisablePlayer = 15;
        public bool IsFreeMove = false; // 是否是自由移动移动
        public int SplitMoveCount = 2; // 分割水平移动次数

        private int ScorePerPlayer = 3;
        public PlayerSO playerSO;
        private List<BehavioursController> selfGroup;

        public List<BehavioursController> SelfGroup => selfGroup;

        private PlayerGroupTargetMotion motion;

        private List<Transform> locomotions;
        private int locomotionsCurrentIndex = 0;
        private List<BehavioursController> currentEnemies;
        private Transform currentLocomotion;

        // private Grid<BehavioursController> agents;
        // private int GridWidth = 0;
        // private int MaxGridY = 10;
        // private int currentGridX = 0;
        // private int currentGridY = 0;
        // private Vector3 playerSize;
        // private float RowSpacing = 0.2f;
        // private float ColSpacing = 0.2f;
        private void InitGrid()
        {
            // Vector2 rangeX = Data.MapXRange;
            // float totalWidth = (rangeX.y - rangeX.x ) / SplitMoveCount;
            // playerSize = playerSO.modelSO.GetModelSize();
            // GridWidth = Mathf.FloorToInt(totalWidth / (playerSize.x + RowSpacing));
            // agents = new Grid<BehavioursController>(GridWidth, MaxGridY);
        }

        private int GetCurrentVisiableCount() => selfGroup.Select(x => x.gameObject.activeSelf).Count();

        private void SetCurrentFullVisiable()
        {
            int index = 0;
            int currentVisiableCount = GetCurrentVisiableCount();
            int count = MaxVisablePlayer - currentVisiableCount;
            if(count>0)
            {
                foreach (var item in selfGroup)
                {
                    if (!item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(true);
                        item.transform.position = transform.position;
                        index++;
                        if (index == count)return;
                    }
                }
            }
        }

        private void SpawnPlayer()
        {
            BehavioursController player = playerSO.Spawn(transform).GetComponent<BehavioursController>();

            int visiableCount = GetCurrentVisiableCount();

            if( visiableCount > MaxVisablePlayer)
            {
                player.gameObject.SetActive(false);
            }
            else
            {
                Data.VFXSO?.PlaySpawnVFX(player.transform, default);
            }

            selfGroup.Add(player);
        }
        
        private void DestroyPlayer()
        {
            if (selfGroup.Count > 0)
            {
                BehavioursController player = selfGroup.First(g=>g.gameObject.activeSelf);
                Data.VFXSO?.PlayDeathVFX(player.transform.position);
                player.Death();
                selfGroup.Remove(player);
            }
        }
        
        void Start()
        {
            EndOnce = false;
            onGameOverCmd = new OnGameOverCmd();
            selfGroup = new List<BehavioursController>();
            IsStart = false;

            motion = GetComponent<PlayerGroupTargetMotion>();
            motion.IsFreeMove = IsFreeMove;
            SplitMoveCount = SplitMoveCount <= 1 ? 2 : SplitMoveCount;
            motion.SplitMoveCount = SplitMoveCount;

            this.RegisterEvent<OnMapGenerated>(InitPlayerAgent).UnRegisterWhenGameObjectDestroyed(this);
            this.RegisterEvent<OnTriggerLevelItemEvent>(PlayerCountAdd).UnRegisterWhenGameObjectDestroyed(this);
        }

        private bool IsStart = false;

        private void InitPlayerAgent(OnMapGenerated e)
        {
            #region  Init Grid
            
            InitGrid();
            
            #endregion
            
            
            int count = playerSO.InitCount;
            count = Mathf.Max(count, 1);
            for (int i = 0; i < count; i++)
            {
                SpawnPlayer();
            }

            locomotions = Data.LocomotionGroup;
            locomotionsCurrentIndex = 0;
            Data.PlayerCount.Value = selfGroup.Count;
            IsStart = true;
            IsLocomotionEnd = false;

            motion.StartMove();
        }

        private void PlayerCountAdd(OnTriggerLevelItemEvent e)
        {
            selfGroup.RemoveAll(p => p == null);
            if (selfGroup.Count <= 0)
            {
                this.SendCommand<OnGameOverCmd>();
                return;
            }
            if (e.AddCount == 0) return;
            if (e.AddCount > 0)
            {
                for (int i = 0; i < e.AddCount; i++)
                {
                    SpawnPlayer();
                }
            }
            else if (e.AddCount < 0)
            {
                for (int i = 0; i < -e.AddCount; i++)
                {
                    DestroyPlayer();
                }
            }
            // motion.AdjustCamera(GetBounds()) ;
        }

        private bool IsLocomotionEnd = false;

        protected override List<BehavioursController> GetAnotherGroup()
        {
            if (motion.IsEnd) return null;
            if (IsLocomotionEnd) return null;

            if (locomotions == null || locomotions.Count == 0) return null;
            locomotionsCurrentIndex = Mathf.Clamp(locomotionsCurrentIndex, 0, locomotions.Count);
            currentLocomotion = locomotions[locomotionsCurrentIndex];

            // CombatMusic

            if (CheckDistanceTo(currentLocomotion))
            {
                currentEnemies ??= currentLocomotion.GetComponentsInChildren<BehavioursController>()?.ToList();
                if (currentEnemies != null && currentEnemies.Count > 0)
                {
                    Data.MusicSO?.PlayCombatMusic();
                    motion.StopMove();
                    return currentEnemies;
                }
                else
                { // 证明已经消灭完了
                    currentEnemies = null;
                    Data.MusicSO?.PlayCombatMusic(false);
                    locomotionsCurrentIndex++;
                    if (locomotionsCurrentIndex >= locomotions.Count)
                    {
                        IsLocomotionEnd = true;
                    }
                    motion.StartMove();
                }
            }
            return null;
        }

        protected override List<BehavioursController> GetSelfGroup()
        {
            SetCurrentFullVisiable();
            return selfGroup;
        }

        protected override bool RunningCondition()
        {
            return IsStart;
        }

        protected override void WhenSelfCountEmpty()
        {
            if (!EndOnce)
            {
                motion.StopMove();
                onGameOverCmd.isWin = false;
                this.SendCommand(onGameOverCmd);
                EndOnce = true;
            }
        }

        private OnGameOverCmd onGameOverCmd;

        public BehavioursController[,] PlayersRect;





        protected override void OnAttackProcess(BehavioursController self)
        {
            Data.VFXSO?.PlayPlayerHitVFX(self.transform.position + (self.transform.up * self.ModelSize.y) + (self.transform.forward * self.ModelSize.z));
        }
        private bool EndOnce = false;
        protected override void OnUpdate()
        {
            Data.PlayerCount.Value = selfGroup.Count;
            Data.Score.Value = selfGroup.Count * ScorePerPlayer;
            if ((!EndOnce) && motion.IsEnd && Data.LastLevelItem == null)
            {
                EndOnce = true;
                onGameOverCmd.isWin = true;
                this.SendCommand(onGameOverCmd);
            }
        }

        void OnDestroy()
        {
            if (selfGroup == null || selfGroup.Count == 0) return;
            foreach (var player in selfGroup)
            {
                if (player == null) continue;
                Destroy(player.gameObject);
            }
        }
    }


    /// <summary>
    /// Grid 网格 ， 
    /// Grid[x,y] = T ,  T = Grid[x,y];
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T>
    {
        private T[,] mGrid;
        private int mWidth;
        private int mHeight;

        public int Width => mWidth;
        public int Height => mHeight;

        public Grid(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mGrid = new T[width, height];
        }


        public void Fill(T t)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = t;
                }
            }
        }

        public void Fill(Func<int, int, T> onFill)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    mGrid[x, y] = onFill(x, y);
                }
            }
        }

        public void Resize(int width, int height, Func<int, int, T> onAdd)
        {
            var newGrid = new T[width, height];
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    newGrid[x, y] = mGrid[x, y];
                }

                // x addition
                for (var y = mHeight; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            for (var x = mWidth; x < width; x++)
            {
                // y addition
                for (var y = 0; y < height; y++)
                {
                    newGrid[x, y] = onAdd(x, y);
                }
            }

            // 清空之前的
            Fill(default(T));

            mWidth = width;
            mHeight = height;
            mGrid = newGrid;
        }

        public void ForEach(Action<int, int, T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(x, y, mGrid[x, y]);
                }
            }
        }

        public void ForEach(Action<T> each)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    each(mGrid[x, y]);
                }
            }
        }

        public T this[int xIndex, int yIndex]
        {
            get
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    return mGrid[xIndex, yIndex];
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                    return default;
                }
            }
            set
            {
                if (xIndex >= 0 && xIndex < mWidth && yIndex >= 0 && yIndex < mHeight)
                {
                    mGrid[xIndex, yIndex] = value;
                }
                else
                {
                    Debug.LogWarning($"out of bounds [{xIndex}:{yIndex}] in grid[{mWidth}:{mHeight}]");
                }
            }
        }

        public void Clear(Action<T> cleanupItem = null)
        {
            for (var x = 0; x < mWidth; x++)
            {
                for (var y = 0; y < mHeight; y++)
                {
                    cleanupItem?.Invoke(mGrid[x, y]);
                    mGrid[x, y] = default;
                }
            }

            mGrid = null;
        }
    }

}

