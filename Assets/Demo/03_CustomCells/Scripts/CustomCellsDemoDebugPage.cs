#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Demo._03_CustomCells.Scripts
{
    public sealed class CustomCellsDemoDebugPage : DebugPageBase
    {
        private const string CustomTextCellKey = "CustomTextCell";
        private const string SourceCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int MinLength = 30;
        private const int MaxLength = 300;
        private readonly List<CustomTextCellModel> _models = new List<CustomTextCellModel>();

        private int _cellCount;
        protected override string Title => "Custom Cell Demo Page";

        public int AddCustomText(CustomTextCellModel model)
        {
            return AddItem(CustomTextCellKey, model);
        }

        public void Setup(int cellCount)
        {
            _cellCount = cellCount;
        }

        public override IEnumerator Initialize()
        {
            for (var i = 0; i < _cellCount; i++)
            {
                var model = new CustomTextCellModel();
                model.Text = GetRandomString();
                model.Color = GetColor(i);
                AddCustomText(model);
                _models.Add(model);
            }

            Reload();
            
            yield break;
        }

        public void ChangeData()
        {
            for (var i = 0; i < _models.Count; i++)
            {
                var model = _models[i];
                model.Text = GetRandomString();
            }

            // Height of each cell will be changed, so call Reload() instead of RefreshData().
            Reload();
        }

        private static string GetRandomString()
        {
            var length = Random.Range(MinLength, MaxLength + 1);
            return GetRandomString(length);
        }

        private static string GetRandomString(int length)
        {
            var chars = new char[length];
            var random = new System.Random();

            for (var i = 0; i < chars.Length; i++)
                chars[i] = SourceCharacters[random.Next(SourceCharacters.Length)];

            return new string(chars);
        }

        private static Color GetColor(int index)
        {
            var colorIndex = index % 3;
            switch (colorIndex)
            {
                case 0:
                    return Color.Lerp(Color.red, Color.black, 0.25f);
                case 1:
                    return Color.Lerp(Color.green, Color.black, 0.25f);
                case 2:
                    return Color.Lerp(Color.blue, Color.black, 0.25f);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
#endif
