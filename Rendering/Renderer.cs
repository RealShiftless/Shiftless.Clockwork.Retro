using OpenTK.Graphics.OpenGL4;
using Shiftless.Clockwork.Retro.Mathematics;
using System.Drawing;
using System.Reflection;

#nullable disable
namespace Shiftless.Clockwork.Retro.Rendering
{
    public sealed class Renderer
    {
        // Constants
        public const int NATIVE_WIDTH = 240;
        public const int NATIVE_HEIGHT = 135;

        public const float NATIVE_ASPECT_RATIO = (float)NATIVE_WIDTH / NATIVE_HEIGHT;


        //  Texture Units
        public const TextureUnit TILESET_UNIT = TextureUnit.Texture0;

        public const TextureUnit TILEMAP_UNIT = TextureUnit.Texture1;
        public const TextureUnit TILEMAP_INFO_UNIT = TextureUnit.Texture2;

        public const TextureUnit PALETTE_UNIT = TextureUnit.Texture3;

        public const TextureUnit OBJECT_BUFFER_UNIT = TextureUnit.Texture4;
        public const TextureUnit OBJECT_BUCKET_UNIT = TextureUnit.Texture5;

        public const TextureUnit FBO_UNIT = TextureUnit.Texture15;


        // Values
        private int _paletteHandle;
        private Palette[] _palettes = new Palette[Palette.MAX];

        private int _frameBufferHandle;
        private int _frameTextureHandle;

        private int _vpQuadVAO;
        private int _vpQuadVBO;

        private Rectangle _screenViewport;

        private Shader _mainShader;
        private Shader _frameBufferShader;


        // Properties
        public Rectangle Viewport => _screenViewport;


        // Constructor
        internal Renderer(Window window)
        {
            // Bind to window resized to change the viewport
            window.OnResized += WindowResized;

            // Call window resized once to make sure the viewport is always valid
            _screenViewport = new(0, 0, window.ClientSize.X, window.ClientSize.Y);
        }


        // Func
        internal void Initialize()
        {
            // Set the pixel store stuff
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackRowLength, 0);       // critical for 2-byte rows
            GL.PixelStore(PixelStoreParameter.UnpackImageHeight, 0);

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.PackRowLength, 0);       // critical for 2-byte rows
            GL.PixelStore(PixelStoreParameter.PackImageHeight, 0);

            // Initialize palettes
            InitializeFramebuffer();
            InitializePalettes();
            InitializeShaders();

            // And create the viewport quad to render to
            (_vpQuadVAO, _vpQuadVBO) = CreateViewportQuad();
        }

        internal void Render()
        {
            // First render the game to the native resolution frame buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferHandle);
            GL.Viewport(0, 0, NATIVE_WIDTH, NATIVE_HEIGHT);

            _mainShader.Use();
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            // Now render said framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(_screenViewport);

            _frameBufferShader.Use();
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        private void WindowResized(Window window)
        {
            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Reset the viewport
            int width = window.ClientSize.X;
            int height = window.ClientSize.Y;

            if (width / (float)height > NATIVE_ASPECT_RATIO)
                width = (int)(height * NATIVE_ASPECT_RATIO);
            else
                height = (int)(width / NATIVE_ASPECT_RATIO);

            int offsetX = (window.ClientSize.X - width) / 2;
            int offsetY = (window.ClientSize.Y - height) / 2;

            _screenViewport = new(offsetX, offsetY, width, height);
        }


        public void SetPalette(PaletteIndex index, Palette palette)
        {
            if (_palettes[(int)index] != null)
                _palettes[(int)index].PaletteUpdated -= PaletteUpdated;

            _palettes[(int)index] = palette;
            palette.PaletteUpdated += PaletteUpdated;

            RefreshPalettes();
        }
        public Palette GetPalette(PaletteIndex index) => _palettes[(int)index];


        private void InitializeFramebuffer()
        {
            // First create the stuff
            _frameBufferHandle = GL.GenFramebuffer();
            _frameTextureHandle = GL.GenTexture();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferHandle);

            GL.ActiveTexture(FBO_UNIT);
            GL.BindTexture(TextureTarget.Texture2D, _frameTextureHandle);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, NATIVE_WIDTH, NATIVE_HEIGHT, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _frameTextureHandle, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        private void InitializePalettes()
        {
            // First we actually create all the palettes n shi
            for (int i = 0; i < Palette.MAX; i++)
                _palettes[i] = new(new(), new(), new(), new());

            // Now we create the handle for said palette
            _paletteHandle = GL.GenTexture();

            // Bind it, we bind is as a texture1D array as its a 1d array or colors
            GL.ActiveTexture(PALETTE_UNIT);
            GL.BindTexture(TextureTarget.Texture1DArray, _paletteHandle);

            // We setup some parameters for the texture
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture1DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // And we refresh the palette to fill it before we reach any rendering, it will be filled with #FF00FF, which is the transparancy color.
            RefreshPalettes();
        }
        private void InitializeShaders()
        {
            // Setup the main shader
            _mainShader = Shader.LoadEmbeddedShader(Assembly.GetExecutingAssembly(), "shaders.main.vert", "shaders.main.frag");

            _mainShader.Set("tileset", TILESET_UNIT - TextureUnit.Texture0);
            _mainShader.Set("tilemap", TILEMAP_UNIT - TextureUnit.Texture0);
            _mainShader.Set("info", TILEMAP_INFO_UNIT - TextureUnit.Texture0);
            _mainShader.Set("palette", PALETTE_UNIT - TextureUnit.Texture0);

            // Setup the framebuffer shader
            _frameBufferShader = Shader.LoadEmbeddedShader(Assembly.GetExecutingAssembly(), "shaders.main.vert", "shaders.frame_buffer.frag");

            _frameBufferShader.Set("frameBuffer", FBO_UNIT - TextureUnit.Texture0);
        }




        private void RefreshPalettes()
        {
            // Because we always keep our palette bound to PALETTE_UNIT we dont have to bind the texture, only set it as the active one
            GL.ActiveTexture(PALETTE_UNIT);

            // We create a buffer for the color data
            ushort[] paletteData = new ushort[Palette.MAX * Palette.COLORS];

            // We loop thru all the color data
            for (int i = 0; i < Palette.MAX; i++)
            {
                for (int j = 0; j < Palette.COLORS; j++)
                    paletteData[i * Palette.COLORS + j] = _palettes[i][j];
            }

            // And we set the data to the palette
            GL.TexImage2D(TextureTarget.Texture1DArray, 0, PixelInternalFormat.R16ui, 4, Palette.MAX, 0, PixelFormat.RedInteger, PixelType.UnsignedShort, paletteData);
        }

        private static (int vao, int vbo) CreateViewportQuad()
        {
            // Firt create the important stuff
            int vbo = GL.GenBuffer();
            int vao = GL.GenVertexArray();

            float[] vertices = new float[] {
                    // Position
                    -1f,  1f,
                     1f,  1f,
                    -1f, -1f,
                     1f, -1f
                };

            // Then bind the VBO and set its data
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            // Now enable the tings :)
            GL.BindVertexArray(vao);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            return (vao, vbo);
        }


        // Callbacks
        private void PaletteUpdated(Palette palette) => RefreshPalettes();
    }
}
#nullable enable
