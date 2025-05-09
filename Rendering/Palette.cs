using OpenTK.Graphics.OpenGL4;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Rendering
{
    public sealed class Palette
    {
        // Constants
        public const int COLORS = 4;

        public const int MAX = 16;
        public const TextureUnit TEXTURE_UNIT = TextureUnit.Texture2;


        // Values
        private Color565[] _colors = new Color565[COLORS];

        private PaletteIndex? _id;


        // Properties
        public PaletteIndex Id => _id ?? throw new InvalidOperationException("Palette was not bound!");


        // Indexer
        public Color565 this[int i]
        {
            get => _colors[i];
            set
            {
                if (_colors[i] == value) return;

                _colors[i] = value;

                PaletteUpdated?.Invoke(this);
            }
        }


        // Events
        internal event Action<Palette>? PaletteUpdated;


        // Constructor
        public Palette(Color565 color1, Color565 color2, Color565 color3, Color565 color4)
        {
            _colors[0] = color1;
            _colors[1] = color2;
            _colors[2] = color3;
            _colors[3] = color4;
        }


        // Func
        internal void Bind(PaletteIndex? id) => _id = id;

        public ushort[] GetData() => [
            _colors[0],
            _colors[1],
            _colors[2],
            _colors[3]
        ];
    }
}
