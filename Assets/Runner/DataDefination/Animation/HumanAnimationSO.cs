using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
namespace Runner
{
    public class HumanAnimationSO : SOBase
    {
        public string IdleAnimUrl = "";
        public string WalkAnimUrl = "";
        public string RunAnimUrl = "";
        public string AttackAnimUrl = "";
        public string DeathAnimUrl = "";
        

        private static string templatePath = "Runner/AnimatorController/InnerAnimatorState"; 
        private static RuntimeAnimatorController runtimeAnimatorController;
        private static string IdleClipInnerName = "Idle";
        private static string WalkClipInnerName = "Walk";
        private static string RunClipInnerName = "Run";
        private static string AttackClipInnerName = "Attack";
        private static string DeathClipInnerName = "Death";


        private AnimatorOverrideController animatorOverrideController;
        /// <summary>
        /// 显然需要 提前Avatar确定制作好
        /// </summary>
        /// <returns></returns>
        public override async UniTask Download()
        {
            runtimeAnimatorController ??= Resources.Load<RuntimeAnimatorController>(templatePath);
            AnimationClip IdleClip = await DownloadAnimationClip(IdleAnimUrl);
            AnimationClip WalkClip = await DownloadAnimationClip(WalkAnimUrl);
            AnimationClip RunClip = await DownloadAnimationClip(RunAnimUrl);
            AnimationClip AttackClip = await DownloadAnimationClip(AttackAnimUrl);
            AnimationClip DeathClip = await DownloadAnimationClip(DeathAnimUrl);

            animatorOverrideController = new AnimatorOverrideController(runtimeAnimatorController);
            animatorOverrideController.name = "Animator Override";
            animatorOverrideController[IdleClipInnerName] = IdleClip;
            animatorOverrideController[WalkClipInnerName] = WalkClip;
            animatorOverrideController[RunClipInnerName] = RunClip;
            animatorOverrideController[AttackClipInnerName] = AttackClip;
            animatorOverrideController[DeathClipInnerName] = DeathClip;
        }

        public void Apply(GameObject target)
        {
            Animator animator = target.GetComponentInChildren<Animator>();
            if(animator == null || animator.avatar == null) return;
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = animatorOverrideController;
            target.AddComponent<HumanAnimatorController>();
        }

        public override void OnDestroy()
        {
            Destroy(animatorOverrideController);
        }
    }
}