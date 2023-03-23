using System;
using System.Collections.Generic;
using System.Linq;
using UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    public class StatefulDrawer : Drawer
    {
        [SerializeField] [Range(0.0f, 1.0f)] private float _minProgress = 0.1f;
        [SerializeField] private bool _useMiddleState;
        [SerializeField] [Range(0.0f, 1.0f)] private float _middleProgress = 0.5f;
        [SerializeField] [Range(0.0f, 1.0f)] private float _maxProgress = 1.0f;

        public float MinProgress
        {
            get => _minProgress;
            set => _minProgress = value;
        }

        public bool UseMiddleState
        {
            get => _useMiddleState;
            set => _useMiddleState = value;
        }

        public float MiddleProgress
        {
            get => _middleProgress;
            set => _middleProgress = value;
        }

        public float MaxProgress
        {
            get => _maxProgress;
            set => _maxProgress = value;
        }

        protected override void Start()
        {
            if (Application.isPlaying)
                SetState(OpenOnStart ? DrawerState.Max : DrawerState.Min);
        }

        public void SetState(DrawerState state)
        {
            Progress = GetStateProgress(state);
        }

        public YieldInstruction SetStateWithAnimation(DrawerState to, float durationSec, EaseType easeType,
            Action completed = null)
        {
            return PlayProgressAnimation(GetStateProgress(to), durationSec, easeType, completed: completed);
        }

        public YieldInstruction SetStateWithAnimation(DrawerState from, DrawerState to, float durationSec,
            EaseType easeType, Action completed = null)
        {
            return PlayProgressAnimation(GetStateProgress(from), GetStateProgress(to), durationSec, easeType,
                completed: completed);
        }

        /// <summary>
        ///     Returns the state with a nearest progress with the current one.
        /// </summary>
        /// <returns></returns>
        public DrawerState GetNearestState()
        {
            var nearestState = DrawerState.Min;
            float nearestDistance = 1;
            foreach (var state in GetValidStates())
            {
                var distance = Mathf.Abs(GetStateProgress(state) - Progress);
                if (distance <= nearestDistance)
                {
                    nearestState = state;
                    nearestDistance = distance;
                }
            }

            return nearestState;
        }

        /// <summary>
        ///     Returns the state with a greater progress than the current one.
        ///     If current progress is 1.0, returns DrawerState.Max.
        /// </summary>
        /// <returns></returns>
        public DrawerState GetUpperState()
        {
            var upperState = DrawerState.Max;
            foreach (var state in GetValidStates().OrderByDescending(GetStateProgress))
            {
                if (GetStateProgress(state) <= Progress)
                    break;

                upperState = state;
            }

            return upperState;
        }

        /// <summary>
        ///     Returns the state with a lower progress than the current one.
        ///     If current progress is 0.0, returns DrawerState.Min.
        /// </summary>
        /// <returns></returns>
        public DrawerState GetLowerState()
        {
            var lowerState = DrawerState.Min;
            foreach (var state in GetValidStates().OrderBy(GetStateProgress))
            {
                if (GetStateProgress(state) >= Progress)
                    break;

                lowerState = state;
            }

            return lowerState;
        }

        public float GetStateProgress(DrawerState state)
        {
            switch (state)
            {
                case DrawerState.Min:
                    var min = Mathf.Min(_minProgress, _maxProgress);
                    if (_useMiddleState)
                        min = Mathf.Min(min, _middleProgress);
                    return min;
                case DrawerState.Middle:
                    if (_useMiddleState)
                    {
                        var middle = Mathf.Max(_minProgress, _middleProgress);
                        middle = Mathf.Min(middle, _maxProgress);
                        return middle;
                    }
                    else
                    {
                        throw new Exception(
                            "The middle state progress is requested, but the Middle state is not enabled.");
                    }
                case DrawerState.Max:
                    var max = Mathf.Max(_minProgress, _maxProgress);
                    if (_useMiddleState)
                        max = Mathf.Max(max, _middleProgress);
                    return max;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private IEnumerable<DrawerState> GetValidStates()
        {
            return Enum.GetValues(typeof(DrawerState)).Cast<DrawerState>()
                .Where(x => _useMiddleState || x != DrawerState.Middle);
        }
    }
}
