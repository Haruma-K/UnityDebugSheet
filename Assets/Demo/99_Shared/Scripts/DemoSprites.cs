#if !EXCLUDE_UNITY_DEBUG_SHEET
using UnityEngine;

namespace Demo._99_Shared.Scripts
{
    public static class DemoSprites
    {
        public static class Icon
        {
            public static Sprite Settings => Load(AssetKeys.Resources.Icon.Settings);
            public static Sprite Tools => Load(AssetKeys.Resources.Icon.Tools);
            public static Sprite CharacterViewer => Load(AssetKeys.Resources.Icon.CharacterViewer);
            public static Sprite Model => Load(AssetKeys.Resources.Icon.Model);
            public static Sprite Motion => Load(AssetKeys.Resources.Icon.Motion);
            public static Sprite Position => Load(AssetKeys.Resources.Icon.Position);
            public static Sprite Rotation => Load(AssetKeys.Resources.Icon.Rotation);
            public static Sprite AutoRotation => Load(AssetKeys.Resources.Icon.AutoRotation);
            public static Sprite FPS => Load(AssetKeys.Resources.Icon.FPS);
            public static Sprite RAM => Load(AssetKeys.Resources.Icon.RAM);
            public static Sprite Console => Load(AssetKeys.Resources.Icon.Console);

            private static Sprite Load(string key)
            {
                return Resources.Load<Sprite>(key);
            }
        }
    }
}
#endif
