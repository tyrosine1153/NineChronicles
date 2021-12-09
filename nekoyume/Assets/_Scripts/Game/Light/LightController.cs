using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nekoyume.Game.Light
{
    using UniRx;

    [ExecuteInEditMode]
    public class LightController : MonoBehaviour
    {
        [SerializeField] [Range(0, 6000)]
        private int block;
        private IColorSetter[] _setters;
        private int _preBlock;
        private const int BlocksPerDay = 100;
        private readonly List<IDisposable> _disposablesFromOnEnable = new List<IDisposable>();

        public void Initialize()
        {
            Game.instance.Agent?.BlockIndexSubject.Subscribe(SetBlockIndex)
                .AddTo(_disposablesFromOnEnable);
        }

        public void GetSetters()
        {
            _setters = GetComponentsInChildren<IColorSetter>();
            if (_setters is null)
            {
                return;
            }

            foreach (var setter in _setters)
                setter.Refresh();
        }

        private void OnEnable()
        {
            block = 0;
            GetSetters();
            UpdateSetters();
        }

        private void SetBlockIndex(long blockIndex)
        {
            block = (int)(blockIndex % (BlocksPerDay + 1));
        }

        private void OnDisable()
        {
            block = 0;
            UpdateSetters();
            _disposablesFromOnEnable.DisposeAllAndClear();
        }

        private void Update()
        {
            if (_preBlock != block)
                UpdateSetters();
        }

        private void UpdateSetters()
        {
            _preBlock = block;
            var time = GetTime(block);
            if (_setters is null)
            {
                return;
            }
            foreach (var setter in _setters)
                setter.SetColor(time);
        }

        private float GetTime(int currentBlock)
        {
            var time = (float)currentBlock / BlocksPerDay;
            return time;
        }
    }
}
