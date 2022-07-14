using System;
using System.Collections;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.Animation;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public interface ITransitionAnimation : IAnimation
    {
        void SetPartner(RectTransform partnerRectTransform);

        void Setup(RectTransform rectTransform);
    }

    internal static class TransitionAnimationExtensions
    {
        public static IEnumerator CreatePlayRoutine(this ITransitionAnimation self, IProgress<float> progress = null)
        {
            var player = new AnimationPlayer(self);
            UpdateDispatcher.Instance.Register(player);
            progress?.Report(0.0f);
            player.Play();
            while (!player.IsFinished)
            {
                yield return null;
                progress?.Report(player.Time / self.Duration);
            }

            UpdateDispatcher.Instance.Unregister(player);
        }
    }
}
