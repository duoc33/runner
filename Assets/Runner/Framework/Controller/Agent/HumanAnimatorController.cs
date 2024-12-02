using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Runner
{
    public class HumanAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private int speed_hash = Animator.StringToHash("t_speed");
        private int attack_hash = Animator.StringToHash("t_attack");
        private int death_hash = Animator.StringToHash("t_death");
        private void Start() => animator = GetComponentInChildren<Animator>();
        public void PlayDeath() => animator.SetTrigger(death_hash);
        public void PlayAttack() => animator.SetTrigger(attack_hash);
        public void SetSpeed(float speed) => animator.SetFloat(speed_hash, Mathf.Clamp01(speed));
        public bool IsAttackState() => animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        public void ResetAnim()=>animator.ResetTrigger(death_hash);
    }
}