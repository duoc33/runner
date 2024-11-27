using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Boids
{
    /// <summary>
    /// Boids 算法
    /// F = m * a
    /// V = V0 + at
    /// X = X0 + Vt
    /// </summary>
    public class Agent : MonoBehaviour
    {
        public Transform Target;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public Vector3 Position;
        public Bounds GroupBounds;

        public AgentConfig AgentConfig;

        private AgentGroupManager _agentGroupManager;
        private Transform _transform;
        private float _deltaTime;
        private float _mass;
        void Start()
        {
            _deltaTime = Time.deltaTime;
            _mass = AgentConfig.Mass;
            GroupBounds = _agentGroupManager.AgentAllOfGroupConfig.GroupBounds;
            Position = _transform.position;
        }
        void Update()
        {
            Acceleration = Combine();
            Acceleration = Vector3.ClampMagnitude(Acceleration, AgentConfig.MaxAcceleration);

            Velocity = Velocity + Acceleration * _deltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, AgentConfig.MaxSpeed);

            Position = Position + Velocity * _deltaTime;
            //这里也可以当位置到达边界时，改变位置到反方向位置，物体会顺着原来的力和速度又回到边界内。
            Position.x = Mathf.Clamp(Position.x, GroupBounds.min.x, GroupBounds.max.x);
            Position.y = Mathf.Clamp(Position.y, GroupBounds.min.y, GroupBounds.max.y);
            Position.z = Mathf.Clamp(Position.z, GroupBounds.min.z, GroupBounds.max.z);

            _transform.position = Position;
            if (Velocity.magnitude > 0)
            {
                _transform.LookAt(Position + Velocity); // Position + Velocity，想象为当前方向上物体"想要去的目标位置"。
                // RealRotation(Velocity, _transform.rotation, AgentConfig.RotationSpeed, _deltaTime); // 这里的RealRotation()函数，模拟物体的过程旋转。
            }
        }

        Vector3 Combine()
        {
            Vector3 acc = Cohesion(_mass) * AgentConfig.CohesionWeight +
                Separation(_mass) * AgentConfig.SeparationWeight +
                Alignment(_mass) * AgentConfig.AlignmentWeight +
                TargetForce(Target, _mass) * AgentConfig.TargetWeight;
            return acc;
        }
        /// <summary>
        /// 吸引力 , 模拟群体中心假设有一个吸引力，距离中心越近力越小，距离中心越远力越大
        /// </summary>
        /// <returns></returns>
        Vector3 Cohesion(float mass)
        {
            Vector3 force = Vector3.zero;

            List<Agent> neighbours = _agentGroupManager.GetNeighbours(this, AgentConfig.CohesionRadius);

            int count = neighbours.Count;

            if (count == 0) return force;

            int inViewFieldCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (InViewField(Velocity, neighbours[i].Position, Position, AgentConfig.MaxViewField))
                {
                    force += neighbours[i].Position;
                    inViewFieldCount++;
                }
            }
            if (inViewFieldCount == 0) return force;
            count = inViewFieldCount;

            force /= count;

            force -= Position;

            force.Normalize();

            Vector3 acc = force / mass;

            return acc;
        }

        /// <summary>
        /// 分散力 , 模拟群体分散，越近的邻居对分散力的影响越大。
        /// </summary>
        /// <returns></returns>
        Vector3 Separation(float mass)
        {
            Vector3 force = Vector3.zero;

            List<Agent> neighbours = _agentGroupManager.GetNeighbours(this, AgentConfig.SeparationRadius);

            int count = neighbours.Count;

            if (count == 0) return force;

            int inViewFieldCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (InViewField(Velocity, neighbours[i].Position, Position, AgentConfig.MaxViewField))
                {
                    Vector3 inverseDir = Position - neighbours[i].Position;
                    if (inverseDir.magnitude > 0)
                    {
                        force += inverseDir.normalized / inverseDir.magnitude; //  物理意义：越近的邻居分母越小，对分散力的影响越大。
                        inViewFieldCount++;
                    }
                }
            }
            if (inViewFieldCount == 0) return force;

            Vector3 acc = force.normalized / mass;

            return acc;
        }

        /// <summary>
        /// 队列力，反映一群物体的总趋势。。。将当前个体的速度方向调整为邻居速度的平均方向。
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        Vector3 Alignment(float mass, bool ignroeMagnitude = false)
        {
            Vector3 force = Vector3.zero;
            List<Agent> neighbours = _agentGroupManager.GetNeighbours(this, AgentConfig.AlignmentRadius);

            int count = neighbours.Count;
            if (count == 0) return force;

            int inViewFieldCount = 0;


            //通过累加，force 会变成一个反映邻居速度"总趋势"的向量（包含方向和大小）。这个过程的物理意义是将当前个体的速度方向调整为邻居速度的平均方向。
            for (int i = 0; i < count; i++)
            {
                if (InViewField(Velocity, neighbours[i].Position, Position, AgentConfig.MaxViewField))
                {
                    force += neighbours[i].Velocity;
                    inViewFieldCount++;
                }

            }
            if (inViewFieldCount == 0) return force;
            count = inViewFieldCount;

            // 保留幅值时，Alignment 力不仅会调整个体的方向，还会让个体的速度大小向邻居的平均速度靠拢。
            // force = force / count ; //此时还需要除以 count 才是真正的平均力。
            // Vector3 acc = force / mass; 
            //鸟群飞行：不同鸟的速度逐渐协调，形成统一的飞行速度。鱼群游动：鱼不仅游向相同的方向，而且游动速度也趋于一致。
            Vector3 acc = Vector3.zero;
            if (ignroeMagnitude)
            {
                acc = force.normalized / mass;
            }
            else
            {
                force /= count;
                acc = force / mass;
            }

            return acc;
        }

        Vector3 TargetForce(Transform target, float mass)
        {
            Vector3 force = Vector3.zero;

            if (target == null) return force;

            force = target.position - Position;

            Vector3 acc = force.normalized / mass;

            return acc;
        }




        /// <summary>
        /// 根据速度的比例调整质量，速度越快质量越大。maxSpeed 显然不超过光速....
        /// </summary>
        /// <param name="minMass"></param>
        /// <param name="maxMass"></param>
        /// <param name="maxSpeed"></param>
        /// <returns></returns>
        float RealSimMass(float minMass, float maxMass, float maxSpeed)
        {
            return Mathf.Lerp(minMass, maxMass, Velocity.magnitude / maxSpeed); ;
        }

        Quaternion RealRotation(Vector3 velocity, Quaternion currentRotation, float rotationSpeed, float deltaTime)
        {
            if (velocity.magnitude > 0)
            {
                // 计算目标方向
                Vector3 targetDirection = velocity.normalized;

                // 计算目标旋转（基于目标方向）
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                // 平滑旋转
                return Quaternion.RotateTowards(
                    currentRotation,           // 当前旋转
                    targetRotation,                // 目标旋转
                    rotationSpeed * deltaTime     // 旋转速度
                );
            }
            return Quaternion.identity;
        }

        bool InViewField(Vector3 myVelocity, Vector3 otherPos, Vector3 myPos, float viewField)
        {
            //Angle是度数
            return Vector3.Angle(myVelocity, otherPos - myPos) <= viewField;
        }
    }
    public class AgentGroupManager : MonoBehaviour
    {
        public AgentAllOfGroupConfig AgentAllOfGroupConfig;
        public List<Agent> AllOfAgents = new List<Agent>();
        public List<Agent> GetNeighbours(Agent agent, float radius)
        {
            List<Agent> neighbours = new List<Agent>();

            int count = AllOfAgents.Count;
            for (int i = 0; i < count; i++)
            {
                Agent other = AllOfAgents[i];
                if (other != null && other != agent && Vector3.Distance(agent.Position, other.Position) <= radius)
                {
                    neighbours.Add(other);
                }
            }
            return neighbours;
        }
    }
    public class AgentConfig : ScriptableObject
    {
        public float Mass = 1;

        public float CohesionRadius;
        public float SeparationRadius;
        public float AlignmentRadius;

        public float CohesionWeight;
        public float SeparationWeight;
        public float AlignmentWeight;

        public float MaxAcceleration;
        public float MaxSpeed;
        public float MaxViewField; // 视野范围

        public float TargetWeight;

        public float RotationSpeed;
    }

    public class AgentAllOfGroupConfig : ScriptableObject
    {
        public Bounds GroupBounds;
    }

}
