﻿using System.Collections;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.Animation
{
    internal static class AnimationExtensions
    {
        public static IEnumerator CreatePlayRoutine(this IAnimation self)
        {
            var player = new AnimationPlayer(self);
            UpdateDispatcher.Instance.Register(player);
            player.Play();
            yield return new WaitUntil(() => player.IsFinished);
            UpdateDispatcher.Instance.Unregister(player);
        }
    }
}
