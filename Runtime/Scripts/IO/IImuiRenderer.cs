using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Imui.IO
{
    public delegate void ImuiRenderDelegate(IImuiRenderingContext context);
    
    public interface IImuiRenderer
    {
        Vector2 GetScreenSize();
        float GetScale();
        Vector2Int SetupRenderTarget(CommandBuffer cmd, bool needsDepth);
        void Schedule(ImuiRenderDelegate renderDelegate);
    }

    public interface IImuiRenderingScheduler: IDisposable
    {
        void Schedule(ImuiRenderDelegate renderDelegate);
    }

    public interface IImuiRenderingContext : IDisposable
    {
        CommandBuffer CreateCommandBuffer();
        void ReleaseCommandBuffer(CommandBuffer cmd);
        void ExecuteCommandBuffer(CommandBuffer cmd);
    }
}