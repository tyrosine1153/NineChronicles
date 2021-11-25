using System;
using System.Collections;
using System.Linq;
using Nekoyume.Game.Controller;
using Nekoyume.Game.VFX;
using Nekoyume.Game.VFX.Skill;
using Nekoyume.Model.Buff;
using Nekoyume.Model.Elemental;
using Nekoyume.Model.Skill;
using UniRx;
using UnityEngine;

namespace Nekoyume.Game.Character
{
    using UniRx;

    public class PrologueCharacter : MonoBehaviour
    {
        public bool AttackEndCalled { get; set; }
        public CharacterAnimator Animator { get; protected set; }
        public string TargetTag { get; protected set; }
        private CharacterSpineController SpineController { get; set; }
        private bool _forceQuit;
        private Player _target;

        private void Awake()
        {
            Animator = new EnemyAnimator(this);

            TargetTag = Tag.Player;
        }

        public void Set(int characterId, Player target)
        {
            var spineResourcePath = $"Character/Monster/{characterId}";


            _target = target;
        }

        private void OnAnimatorEvent(string eventName)
        {
            switch (eventName)
            {
                case "attackStart":
                    AudioController.PlaySwing();
                    break;
                case "attackPoint":
                    AttackEndCalled = true;
                    break;
                case "footstep":
                    AudioController.PlayFootStep();
                    break;
            }
        }

        public IEnumerator CoNormalAttack(int dmg, bool critical)
        {
            yield return StartCoroutine(CoAnimationAttack(critical));
            Prologue.PopupDmg(dmg, _target.gameObject, false, critical, ElementalType.Normal, false);
        }

        private IEnumerator CoAnimationAttack(bool isCritical)
        {
            while (true)
            {
                AttackEndCalled = false;
                if (isCritical)
                {
                }
                else
                {
                }

                _forceQuit = false;
                var coroutine = StartCoroutine(CoTimeOut());
                yield return new WaitUntil(() => AttackEndCalled || _forceQuit);
                StopCoroutine(coroutine);
                if (_forceQuit)
                {
                    continue;
                }

                break;
            }
        }

        private IEnumerator CoTimeOut()
        {
            yield return new WaitForSeconds(1f);
            _forceQuit = true;
        }

        public IEnumerator CoBlowAttack(ElementalType elementalType)
        {
            AttackEndCalled = false;
            yield return StartCoroutine(CoAnimationCast(elementalType));

            yield return StartCoroutine(CoAnimationCastBlow(elementalType));

            var dmgMap = new[] {1374, 2748, 4122, 8244, 16488};
            var effect = Game.instance.Stage.SkillController.Get<SkillBlowVFX>(_target.gameObject, elementalType, SkillCategory.BlowAttack, SkillTargetType.Enemies);
            effect.Play();
            for (var i = 0; i < 5; i++)
            {
                var sec = i == 0 ? 0 : i / 10f;
                Prologue.PopupDmg(dmgMap[i], _target.gameObject, false, i == 4, elementalType, false);
                yield return new WaitForSeconds(sec);
            }
        }

        protected virtual IEnumerator CoAnimationCast(ElementalType elementalType)
        {
            var sfxCode = AudioController.GetElementalCastingSFX(elementalType);
            AudioController.instance.PlaySfx(sfxCode);
            var pos = transform.position;
            var effect = Game.instance.Stage.SkillController.Get(pos, elementalType);
            effect.Play();
            yield return new WaitForSeconds(0.6f);
        }

        private IEnumerator CoAnimationCastBlow(ElementalType elementalType)
        {
            yield return StartCoroutine(CoAnimationCast(elementalType));

            var pos = transform.position;
            yield return CoAnimationCastAttack(false);
            var effect = Game.instance.Stage.SkillController.GetBlowCasting(
                pos,
                SkillCategory.BlowAttack,
                elementalType);
            effect.Play();
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator CoAnimationCastAttack(bool isCritical)
        {
            while (true)
            {
                AttackEndCalled = false;
                if (isCritical)
                {
                }
                else
                {
                }

                _forceQuit = false;
                var coroutine = StartCoroutine(CoTimeOut());
                yield return new WaitUntil(() => AttackEndCalled || _forceQuit);
                StopCoroutine(coroutine);
                if (_forceQuit)
                {
                    continue;
                    ;
                }

                break;
            }
        }

        public IEnumerator CoDoubleAttack(int[] damageMap, bool[] criticalMap)
        {
            var go = _target.gameObject;
            var effect = Game.instance.Stage.SkillController.Get<SkillDoubleVFX>(go, ElementalType.Fire, SkillCategory.DoubleAttack, SkillTargetType.Enemy);
            for (var i = 0; i < 2; i++)
            {
                var first = i == 0;

                yield return StartCoroutine(CoAnimationAttack(!first));
                if (first)
                {
                    effect.FirstStrike();
                }
                else
                {
                    effect.SecondStrike();
                }
                Prologue.PopupDmg(damageMap[i], go, false, criticalMap[i], ElementalType.Fire, false);

            }
        }

        public IEnumerator CoBuff(Buff buff)
        {
            yield return StartCoroutine(CoAnimationBuffCast(buff));
            AudioController.instance.PlaySfx(AudioController.SfxCode.FenrirGrowlCastingAttack);
            var effect = Game.instance.Stage.BuffController.Get<BuffVFX>(_target, buff);
            effect.Play();
            yield return new WaitForSeconds(0.6f);
        }

        private IEnumerator CoAnimationBuffCast(Buff buff)
        {
            AttackEndCalled = false;
            var sfxCode = AudioController.GetElementalCastingSFX(ElementalType.Normal);
            AudioController.instance.PlaySfx(sfxCode);
            var pos = transform.position;
            var effect = Game.instance.Stage.BuffController.Get(pos, buff);
            effect.Play();
            yield return new WaitForSeconds(0.6f);
        }

        public IEnumerator CoFinisher(int[] damageMap, bool[] criticalMap)
        {
            AttackEndCalled = false;
            var position = ActionCamera.instance.Cam.ScreenToWorldPoint(
                new Vector2((float) Screen.width / 2, (float) Screen.height / 2));
            position.z = 0f;
            var effect = Game.instance.Stage.objectPool.Get<FenrirSkillVFX>(position);
            effect.Stop();
            AudioController.instance.PlaySfx(AudioController.SfxCode.FenrirGrowlSkill);
            yield return new WaitForSeconds(1f);
        }

        public IEnumerator CoHit()
        {
            yield return null;
        }
    }

    internal class CharacterSpineController
    {
    }
}
