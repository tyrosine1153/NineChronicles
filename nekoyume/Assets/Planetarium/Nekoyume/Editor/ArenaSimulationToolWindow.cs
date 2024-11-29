using System.Collections.Generic;
using System.Text;
using BalanceTool.Util;
using Libplanet.Action;
using Nekoyume.Arena;
using Nekoyume.Game;
using Nekoyume.Helper;
using Nekoyume.Model;
using Nekoyume.Model.BattleStatus.Arena;
using Nekoyume.Model.Buff;
using Nekoyume.Model.Stat;
using Nekoyume.Model.State;
using UnityEditor;
using UnityEngine;

namespace Planetarium.Nekoyume.Editor
{
    public class ArenaSimulationToolWindow : EditorWindow
    {
        [MenuItem("Tools/Arena Simulation Tool")]
        private static void ShowWindow()
        {
            GetWindow<ArenaSimulationToolWindow>("Arena Simulation Tool", true).Show();
        }

        private Vector2 _playDataCsvScrollPos;
        private Vector2 _outputScrollPos;
        private bool _workingInCalculate;

        private void OnGUI()
        {
            GUILayout.Label("Inputs", EditorStyles.boldLabel);
            GUILayout.Label("Play Data Csv");
            _playDataCsvScrollPos = EditorGUILayout.BeginScrollView(_playDataCsvScrollPos);
            // playDataCsv = EditorGUI.TextArea(
            //     GetRect(minLineCount: 3),
            //     playDataCsv);
            EditorGUILayout.EndScrollView();

            EditorGUI.BeginDisabledGroup(_workingInCalculate);
            if (GUILayout.Button("Calculate"))
            {
                var log = Calculate();

                GUILayout.Label("Outputs", EditorStyles.boldLabel);
                _outputScrollPos = EditorGUILayout.BeginScrollView(_outputScrollPos);
                EditorGUI.TextArea(
                    GetRect(minLineCount: 3),
                    GenerateLog(log));
                EditorGUILayout.EndScrollView();
            }

            EditorGUI.EndDisabledGroup();
        }

        private TableSheets _tableSheets;
        private IRandom _random;
        private AvatarState _avatarState1;
        private AvatarState _avatarState2;

        private ArenaAvatarState _arenaAvatarState1;
        private ArenaAvatarState _arenaAvatarState2;

        private void OnEnable()
        {
            minSize = new Vector2(300f, 300f);

            _playDataCsvScrollPos = Vector2.zero;
            _outputScrollPos = Vector2.zero;
            _workingInCalculate = false;

            _tableSheets = TableSheets.MakeTableSheets(TableSheetsHelper.ImportSheets());
            _random = new TestRandom();

            _avatarState1 = AvatarState.Create(
                default,
                default,
                0,
                _tableSheets.GetAvatarSheets(),
                default
            );
            _avatarState2 = AvatarState.Create(
                default,
                default,
                0,
                _tableSheets.GetAvatarSheets(),
                default
            );

            _arenaAvatarState1 = new ArenaAvatarState(_avatarState1);
            _arenaAvatarState2 = new ArenaAvatarState(_avatarState2);
        }

        private ArenaLog Calculate()
        {
            _workingInCalculate = true;

            var simulator = new ArenaSimulator(_random, 10);
            var myDigest = new ArenaPlayerDigest(_avatarState1, _arenaAvatarState1);
            var enemyDigest = new ArenaPlayerDigest(_avatarState2, _arenaAvatarState2);
            var arenaSheets = _tableSheets.GetArenaSimulatorSheets();
            var log = simulator.Simulate(
                myDigest,
                enemyDigest,
                arenaSheets,
                new List<StatModifier>
                {
                    new (StatType.ATK, StatModifier.OperationType.Add, 1),
                    new (StatType.HP, StatModifier.OperationType.Add, 100),
                },
                new List<StatModifier>
                {
                    new (StatType.DEF, StatModifier.OperationType.Add, 1),
                    new (StatType.HP, StatModifier.OperationType.Add, 100),
                },
                _tableSheets.BuffLimitSheet,
                _tableSheets.BuffLinkSheet
            );

            _workingInCalculate = false;

            return log;
        }

        private static StringBuilder GenerateLog(ArenaLog log)
        {
            var sb = new StringBuilder();
            var eventCount = log.Events.Count;
            for (var i = 0; i < eventCount; i++)
            {
                LogEvent(log.Events[i], i + 1);
            }

            return sb;

            void LogEvent(ArenaEventBase e, int eventIndex)
            {
                switch (e)
                {
                    case ArenaAreaAttack areaAttack:
                        sb.AppendLine($"OnAreaAttack: {areaAttack.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {areaAttack.Character.Id}");
                        sb.AppendLine($"- SkillId: {areaAttack.SkillId}");
                        foreach (var skill in areaAttack.SkillInfos)
                        {
                            sb.AppendLine($"- has skill: {skill}");
                            sb.AppendLine($"  - skillCategory: {skill.SkillCategory}");
                            sb.AppendLine($"  - id: {skill.Target?.Id}");
                        }

                        break;
                    case ArenaBlowAttack blowAttack:
                        sb.AppendLine($"OnBlowAttack: {blowAttack.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {blowAttack.Character.Id}");
                        sb.AppendLine($"- SkillId: {blowAttack.SkillId}");
                        break;
                    case ArenaBuff buff:
                        sb.AppendLine($"OnBuff: {buff.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {buff.Character.Id}");
                        sb.AppendLine($"- SkillId: {buff.SkillId}");
                        sb.AppendLine("Character Stat: ");
                        sb.AppendLine($"- ATK: {buff.Character.ATK}");
                        sb.AppendLine($"- DEF: {buff.Character.DEF}");
                        sb.AppendLine($"- HIT: {buff.Character.HIT}");
                        sb.AppendLine($"- SPD: {buff.Character.SPD}");
                        sb.AppendLine($"- DRV: {buff.Character.DRV}");
                        sb.AppendLine($"- DRR: {buff.Character.DRR}");
                        sb.AppendLine($"- CDMG: {buff.Character.CDMG}");
                        if (buff.BuffInfos != null)
                        {
                            foreach (var buffInfo in buff.BuffInfos)
                            {
                                if (buffInfo.Buff == null)
                                {
                                    continue;
                                }

                                sb.AppendLine($"- has buff: {buffInfo.Buff.BuffInfo.Id}");
                                sb.AppendLine($"  - GroupId: {buffInfo.Buff.BuffInfo.GroupId}");
                                sb.AppendLine($"  - Chance: {buffInfo.Buff.BuffInfo.Chance}");
                                sb.AppendLine($"  - Duration: {buffInfo.Buff.BuffInfo.Duration}");
                            }
                        }

                        break;
                    case ArenaBuffRemovalAttack buffRemovalAttack:
                        sb.AppendLine($"OnBuffRemovalAttack: {buffRemovalAttack.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {buffRemovalAttack.Character.Id}");
                        sb.AppendLine($"- SkillId: {buffRemovalAttack.SkillId}");
                        break;
                    case ArenaDead dead:
                        sb.AppendLine($"OnDead: {dead.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {dead.Character.Id}");
                        break;
                    case ArenaDoubleAttack doubleAttack:
                        sb.AppendLine($"OnDoubleAttack: {doubleAttack.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {doubleAttack.Character.Id}");
                        sb.AppendLine($"- SkillId: {doubleAttack.SkillId}");
                        foreach (var skill in doubleAttack.SkillInfos)
                        {
                            sb.AppendLine($"- has skill: {skill}");
                            sb.AppendLine($"  - skillCategory: {skill.SkillCategory}");
                            sb.AppendLine($"  - id: {skill.Target?.Id}");
                        }

                        break;
                    case ArenaDoubleAttackWithCombo doubleAttackWithCombo:
                        sb.AppendLine($"OnDoubleAttackWithCombo: {doubleAttackWithCombo.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {doubleAttackWithCombo.Character.Id}");
                        sb.AppendLine($"- SkillId: {doubleAttackWithCombo.SkillId}");
                        foreach (var skill in doubleAttackWithCombo.SkillInfos)
                        {
                            sb.AppendLine($"- has skill: {skill}");
                            sb.AppendLine($"  - skillCategory: {skill.SkillCategory}");
                            sb.AppendLine($"  - id: {skill.Target?.Id}");
                        }

                        break;
                    case ArenaHeal heal:
                        sb.AppendLine($"OnHealSkill: {heal.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {heal.Character.Id}");
                        sb.AppendLine($"- SkillId: {heal.SkillId}");
                        break;
                    case ArenaNormalAttack normalAttack:
                        sb.AppendLine($"OnNormalAttack: {normalAttack.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {normalAttack.Character.Id}");
                        sb.AppendLine($"- SkillId: {normalAttack.SkillId}");
                        foreach (var skill in normalAttack.SkillInfos)
                        {
                            sb.AppendLine($"- has skill: {skill}");
                            sb.AppendLine($"  - skillCategory: {skill.SkillCategory}");
                            sb.AppendLine($"  - id: {skill.Target?.Id}");
                        }

                        break;
                    case ArenaRemoveBuffs removeBuffs:
                        sb.AppendLine($"OnRemoveBuffs: {removeBuffs.Character.Id} {GetProgressText()}");
                        break;
                    case ArenaShatterStrike shatterStrike:
                        sb.AppendLine($"OnShatterStrike: {shatterStrike.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {shatterStrike.Character.Id}");
                        sb.AppendLine($"- SkillId: {shatterStrike.SkillId}");
                        break;
                    case ArenaSpawnCharacter spawnCharacter:
                        sb.AppendLine($"OnSpawnCharacter: {spawnCharacter.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {spawnCharacter.Character.Id}");
                        break;
                    case ArenaTick tick:
                        sb.AppendLine(
                            $"OnTick: {tick.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {tick.Character.Id}");
                        sb.AppendLine($"- SkillId: {tick.SkillId}");
                        if (AuraIceShield.IsFrostBiteBuff(tick.SkillId))
                        {
                            foreach (var kvp in tick.Character.Buffs)
                            {
                                if (!AuraIceShield.IsFrostBiteBuff(kvp.Key))
                                {
                                    continue;
                                }

                                if (kvp.Value is not StatBuff frostBite)
                                {
                                    continue;
                                }

                                sb.AppendLine($"- has Frostbite: {frostBite}");
                                sb.AppendLine($"  - Id: {frostBite.RowData.Id}");
                                sb.AppendLine($"  - Stack: {frostBite.Stack}");
                                sb.AppendLine($"  - CustomField(Power): {frostBite.CustomField}");
                                sb.AppendLine($"  - GroupId: {frostBite.BuffInfo.GroupId}");
                                sb.AppendLine($"  - Duration: {frostBite.BuffInfo.Duration}");
                            }
                        }

                        break;
                    case ArenaTickDamage tickDamage:
                        sb.AppendLine(
                            $"OnTickDamage: {tickDamage.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {tickDamage.Character.Id}");
                        sb.AppendLine($"- SkillId: {tickDamage.SkillId}");
                        break;
                    case ArenaTurnEnd waveTurnEnd:
                        sb.AppendLine(
                            $"OnWaveTurnEnd: {waveTurnEnd.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {waveTurnEnd.Character.Id}");
                        break;
                    case ArenaSkill skill:
                        sb.AppendLine(
                            $"OnSkill: {skill.Character.Id} {GetProgressText()}");
                        sb.AppendLine($"- Id: {skill.Character.Id}");
                        sb.AppendLine($"- SkillId: {skill.SkillId}");
                        foreach (var skillInfo in skill.SkillInfos)
                        {
                            sb.AppendLine($"- has skill: {skillInfo}");
                            sb.AppendLine($"  - skillCategory: {skillInfo.SkillCategory}");
                            sb.AppendLine($"  - id: {skillInfo.Target?.Id}");
                        }

                        break;
                }

                return;

                string GetProgressText()
                {
                    return $"(Event Count: {eventIndex}/{eventCount})";
                }
            }
        }

        private static Rect GetRect(int? minLineCount = null)
        {
            var minHeight = minLineCount.HasValue
                ? EditorGUIUtility.singleLineHeight * minLineCount.Value +
                  EditorGUIUtility.standardVerticalSpacing * (minLineCount.Value - 1)
                : EditorGUIUtility.singleLineHeight;
            return GUILayoutUtility.GetRect(0, minHeight, GUILayout.ExpandWidth(true));
        }
    }
}
