using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class FlexLayout : Layoutable {

            private readonly float Gap;
            private readonly bool Vertical;
            private readonly List<Layoutable> components = new List<Layoutable>();
            private readonly Dictionary<int, float> fixedSizes = new Dictionary<int, float>();

            public FlexLayout(float gap = 0.0f, bool vertical = false) : base(Vector2.Zero, Vector2.Zero) {
                Gap = gap;
                Vertical = vertical;
            }

            public FlexLayout(Vector2 position, Vector2 size, float gap = 0.0f, bool vertical = false) : base(position, size) {
                Gap = gap;
                Vertical = vertical;
            }

            public FlexLayout(Vector4 sizeAndPosition, float gap = 0.0f, bool vertical = false) : base(new Vector2(sizeAndPosition.Z, sizeAndPosition.W), new Vector2(sizeAndPosition.X, sizeAndPosition.Y)) {
                Gap = gap;
                Vertical = vertical;
            }

            public void SetFixedSize(int index, float size) {
                if (fixedSizes.ContainsKey(index)) {
                    fixedSizes[index] = size;
                } else {
                    fixedSizes.Add(index, size);
                }
            }

            public void AddComponent(Layoutable component) {
                components.Add(component);
                RecalculateComponents();
            }

            private void RecalculateComponents() {
                if (components.Count == 0) {
                    return;
                }

                float fixedSize = 0.0f;
                int componentsToAutoSize = components.Count;
                for (int i = 0; i < components.Count; i++) {
                    if (fixedSizes.ContainsKey(i)) {
                        fixedSize += fixedSizes[i];
                        componentsToAutoSize--;
                    }
                }

                if (Vertical) {
                    float autoItemSize = ((Size.Y - fixedSize) - (Gap * (components.Count - 1))) / componentsToAutoSize;
                    Vector2 newSize = new Vector2(Size.X, autoItemSize);
                    Vector2 newPos = Position;
                    for (int i = 0; i < components.Count; i++) {
                        newSize.Y = fixedSizes.ContainsKey(i) ? fixedSizes[i] : autoItemSize;
                        components[i].SetPositionAndSize(newPos, newSize);
                        newPos += new Vector2(0.0f, newSize.Y + Gap);
                    }
                } else {
                    float autoItemSize = ((Size.X - fixedSize) - Gap * (components.Count - 1)) / componentsToAutoSize;
                    Vector2 newSize = new Vector2(autoItemSize, Size.Y);
                    Vector2 newPos = Position;
                    for (int i = 0; i < components.Count; i++) {
                        newSize.X = fixedSizes.ContainsKey(i) ? fixedSizes[i] : autoItemSize;
                        components[i].SetPositionAndSize(newPos, newSize);
                        newPos += new Vector2(newSize.X + Gap, 0.0f);
                    }
                }

            }

        }
    }
}
