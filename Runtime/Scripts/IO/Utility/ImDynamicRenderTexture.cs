using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Imui.IO.Utility
{
    public class ImDynamicRenderTexture: IDisposable
    {
        private const int RES_MIN = 32;
        private const int RES_MAX = 4096;

        public RenderTexture Texture { get; private set; }

        private bool disposed;

        public Vector2Int SetupRenderTarget(CommandBuffer cmd, Vector2Int requestedSize, bool needsDepth, out bool textureChanged)
        {
            AssertDisposed();

            textureChanged = SetupTexture(requestedSize, 1.0f, needsDepth, out var targetSize);

            cmd.Clear();
            cmd.SetRenderTarget(Texture);
            cmd.ClearRenderTarget(true, true, Color.clear);

            return targetSize;
        }

        private bool SetupTexture(Vector2Int size, float scale, bool needsDepth, out Vector2Int targetSize)
        {
            AssertDisposed();

            var w = Mathf.Clamp((int)(size.x * scale), RES_MIN, RES_MAX);
            var h = Mathf.Clamp((int)(size.y * scale), RES_MIN, RES_MAX);

            targetSize = new Vector2Int(w, h);

            if (w == 0 || h == 0)
            {
                return false;
            }

            if (Texture && Texture.IsCreated() && Texture.width == w && Texture.height == h && (!needsDepth || Texture.depth > 0))
            {
                return false;
            }

            ReleaseTexture();

            var depth = needsDepth ? 16 : 0;
            var desc = new RenderTextureDescriptor(w, h, RenderTextureFormat.ARGB32, depth, 0, RenderTextureReadWrite.Linear);
            
            Texture = new RenderTexture(desc)
            {
                name = "ImuiRenderBuffer"
            };

            return Texture.Create();
        }

        private void ReleaseTexture()
        {
            if (Texture != null)
            {
                Texture.Release();
                Texture = null;
            }
        }
        
#if UNITY_2022_2_OR_NEWER
        [HideInCallstack]
#endif
        private void AssertDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(ImDynamicRenderTexture));
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            ReleaseTexture();
            disposed = true;
        }
    }
}