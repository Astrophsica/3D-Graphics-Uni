using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    class FrameBuffer
    {
        public int mFrameBuffer_ID;
        public int mColorBuffer_ID;
        public int mRenderBuffer_ID;
        private TextureUnit mTextureUnit;


        public FrameBuffer(TextureUnit pTextureUnit, int pWidth, int pHeight)
        {
            // References: https://www.youtube.com/watch?v=atp3bzebWJE , https://learnopengl.com/Advanced-OpenGL/Framebuffers
            mTextureUnit = pTextureUnit;
            
            // Frame buffer
            mFrameBuffer_ID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFrameBuffer_ID);

            // Color Buffer
            GL.ActiveTexture(mTextureUnit);
            mColorBuffer_ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, mColorBuffer_ID);

            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, pWidth, pHeight,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, mColorBuffer_ID, 0);

            // Render buffer
            mRenderBuffer_ID = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mRenderBuffer_ID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, pWidth, pHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, mRenderBuffer_ID);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new ApplicationException("Framebuffer is not complete");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public void RebindColorBuffer(int pWidth, int pHeight)
        {
            // Color Buffer
            GL.ActiveTexture(mTextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, mColorBuffer_ID);

            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, pWidth, pHeight,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void RebindRenderBuffer(int pWidth, int pHeight)
        {
            // Render Buffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, mRenderBuffer_ID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, pWidth, pHeight);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void Unload()
        {
            GL.DeleteTexture(mColorBuffer_ID);
            GL.DeleteRenderbuffer(mRenderBuffer_ID);
            GL.DeleteFramebuffer(mFrameBuffer_ID);
        }
    }
}
