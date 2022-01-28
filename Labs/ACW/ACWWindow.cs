using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        public ACWWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }



        private int[] mTexture_ID = new int[2];
        private DrawableObject[] mDrawableObjects = new DrawableObject[12];
        private FrameBuffer mFrameBuffer;

        private ShaderUtility mLightShader, mTextureShader, mScreenShader;

        private FlatPlane mGround, mRightWall, mLeftWall, mFrontWall, mBackWall;
        private Cone mCone;
        private Cube mCube;
        private Model mArmadillo;
        private Model mCylinder1, mCylinder2, mCylinder3;
        private Screen mScreen;

        private Material mWhiteRubber, mEmerald, mChrome, mJade;
        private Material mFloorTextureMaterial, mWallTextureMaterial;

        private Camera mFreeRoamCamera, mFixedCamera1, mFixedCamera2;
        private Camera mCurrentCamera;

        private List<Light> mAllLights = new List<Light>();
        private PointLight mRedLight, mGreenLight, mBlueLight;
        private SpotLight mSpotLight1;
        private DirectionalLight mDirectionalLight;
        
        private float mModelRotationRate;
        private float mConeXTranslationRate;
        private float mCubeYTranslationRate;
        private DateTime mPrevTime;

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            // Load shaders
            mLightShader = new ShaderUtility(@"ACW/Shaders/Material/vPassThrough.vert", @"ACW/Shaders/Material/fLighting.frag");
            mTextureShader = new ShaderUtility(@"ACW/Shaders/Texture/vTexture.vert", @"ACW/Shaders/Texture/fTexture.frag");
            mScreenShader = new ShaderUtility(@"ACW/Shaders/Screen/vScreen.vert", @"ACW/Shaders/Screen/fScreen.frag");


            // Gets location of vNormal and vPosition
            GL.UseProgram(mLightShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mLightShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mLightShader.ShaderProgramID, "vNormal");

            GL.UseProgram(mTextureShader.ShaderProgramID);
            int vTexturePositionLocation = GL.GetAttribLocation(mTextureShader.ShaderProgramID, "vPosition");
            int vTextureNormalLocation = GL.GetAttribLocation(mTextureShader.ShaderProgramID, "vNormal");
            int vTexCoordsLocation = GL.GetAttribLocation(mTextureShader.ShaderProgramID, "vTexCoords");

            GL.UseProgram(mScreenShader.ShaderProgramID);
            int vScreenPositionLocation = GL.GetAttribLocation(mScreenShader.ShaderProgramID, "vPosition");
            int vScreenTexCoordsLocation = GL.GetAttribLocation(mScreenShader.ShaderProgramID, "vTexCoords");

            // Disable inverse mode in ScreenShader frag
            int uEffecteModeLocation = GL.GetUniformLocation(mScreenShader.ShaderProgramID, "uEffectMode");
            GL.Uniform1(uEffecteModeLocation, 0);

            // Load textures
            GL.GenTextures(2, mTexture_ID);

            string filePath = @"ACW/Textures/Texture1.png";
            LoadImageTexture(filePath, TextureUnit.Texture0, 0);

            filePath = @"ACW/Textures/Texture2.png";
            LoadImageTexture(filePath, TextureUnit.Texture1, 1);


            // --------- Frame Buffer --------- \\
            mFrameBuffer = new FrameBuffer(TextureUnit.Texture2, ClientRectangle.Width, ClientRectangle.Height);

            int uTextureSamplerLocation = GL.GetUniformLocation(mScreenShader.ShaderProgramID, "uTextureSampler");
            GL.Uniform1(uTextureSamplerLocation, 2);


            // --------- Camera --------- \\
            // Fixed camera 1
            Matrix4 cameraMatrix = Matrix4.CreateTranslation(7.5f, -8f, 0) * Matrix4.CreateRotationY(0.7f) * Matrix4.CreateRotationX(0.7f); ;
            mFixedCamera1 = new Camera(1, cameraMatrix, true, mLightShader, mTextureShader);

            // Fixed camera 2
            cameraMatrix = Matrix4.CreateTranslation(-7.5f, -8f, 0) * Matrix4.CreateRotationY(-0.7f) * Matrix4.CreateRotationX(0.7f);
            mFixedCamera2 = new Camera(2, cameraMatrix, true, mLightShader, mTextureShader);

            // FreeRoam camera
            cameraMatrix = Matrix4.CreateTranslation(0, -2.75f, 0);
            mFreeRoamCamera = new Camera(0, cameraMatrix, false, mLightShader, mTextureShader);
            mFreeRoamCamera.UseCamera();
            mCurrentCamera = mFreeRoamCamera;


            // --------- Lights --------- \\
            // Light 0
            Vector4 lightPosition = new Vector4(5.0f, 5f, -5.0f, 1);
            Vector3 ambientLight = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 diffuseLight = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 specularLight = new Vector3(0.0f, 0.0f, 1.0f);

            mBlueLight = new PointLight(0, true, mLightShader, mTextureShader, lightPosition, cameraMatrix, ambientLight, diffuseLight, specularLight);
            mAllLights.Add(mBlueLight);


            // Light 1
            lightPosition = new Vector4(0.0f, 5.0f, -5.0f, 1);
            ambientLight = new Vector3(0.1f, 0.1f, 0.1f);
            diffuseLight = new Vector3(0.0f, 1.0f, 0.0f);
            specularLight = new Vector3(0.0f, 1.0f, 0.0f);

            mGreenLight = new PointLight(1, true, mLightShader, mTextureShader, lightPosition, cameraMatrix, ambientLight, diffuseLight, specularLight);
            mAllLights.Add(mGreenLight);


            // Light 2
            lightPosition = new Vector4(-5.0f, 5.0f, -5.0f, 1);
            ambientLight = new Vector3(0.1f, 0.1f, 0.1f);
            diffuseLight = new Vector3(1.0f, 0.0f, 0.0f);
            specularLight = new Vector3(1.0f, 0.0f, 0.0f);

            mRedLight = new PointLight(2, true, mLightShader, mTextureShader, lightPosition, cameraMatrix, ambientLight, diffuseLight, specularLight);
            mAllLights.Add(mRedLight);


            // SpotLight 1
            lightPosition = new Vector4(0.0f, 10.0f, -6.0f, 1);
            ambientLight = new Vector3(0.1f, 0.1f, 0.1f);
            diffuseLight = new Vector3(0.0f, 1.0f, 1.0f);
            specularLight = new Vector3(0.0f, 1.0f, 1.0f);
            Vector4 direction = new Vector4(0.0f, -1.0f, -0.5f, 0.0f);
            float angle = MathHelper.DegreesToRadians(15);
            float cutOff = (float)Math.Cos(angle);

            mSpotLight1 = new SpotLight(0, true, mLightShader, mTextureShader, lightPosition, cameraMatrix, ambientLight, diffuseLight, specularLight, cutOff, direction);
            mAllLights.Add(mSpotLight1);


            // DirectionalLight 1
            ambientLight = new Vector3(0.1f, 0.1f, 0.1f);
            diffuseLight = new Vector3(0.5f, 0.5f, 0.5f);
            specularLight = new Vector3(0.5f, 0.5f, 0.5f);
            direction = new Vector4(-1.0f, -1.0f, -1.0f, 0.0f);

            mDirectionalLight = new DirectionalLight(0, true, mLightShader, mTextureShader, cameraMatrix, ambientLight, diffuseLight, specularLight, direction);
            mAllLights.Add(mDirectionalLight);

            // --------- Materials --------- \\
            // White rubber: http://devernay.free.fr/cours/opengl/materials.html
            Vector3 mAmbientReflectivity = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 mDiffuseReflectivity = new Vector3(0.55f, 0.55f, 0.55f);
            Vector3 mSpecularReflectivity = new Vector3(0.7f, 0.7f, 0.7f);
            float mShininess = 32;
            mWhiteRubber = new Material(mLightShader, mAmbientReflectivity, mDiffuseReflectivity, mSpecularReflectivity, mShininess);

            // Chrome: http://devernay.free.fr/cours/opengl/materials.html
            mAmbientReflectivity = new Vector3(0.25f, 0.25f, 0.25f);
            mDiffuseReflectivity = new Vector3(0.4f, 0.4f, 0.4f);
            mSpecularReflectivity = new Vector3(0.774597f, 0.774597f, 0.774597f);
            mShininess = 25;
            mChrome = new Material(mLightShader, mAmbientReflectivity, mDiffuseReflectivity, mSpecularReflectivity, mShininess);

            // Jade: http://devernay.free.fr/cours/opengl/materials.html
            mAmbientReflectivity = new Vector3(0.135f, 0.2225f, 0.1575f);
            mDiffuseReflectivity = new Vector3(0.54f, 0.89f, 0.63f);
            mSpecularReflectivity = new Vector3(0.5f, 0.5f, 0.5f);
            mShininess = 3f;
            mJade = new Material(mLightShader, mAmbientReflectivity, mDiffuseReflectivity, mSpecularReflectivity, mShininess);

            // Emerald: http://devernay.free.fr/cours/opengl/materials.html
            mAmbientReflectivity = new Vector3(0.0215f, 0.01745f, 0.0215f);
            mDiffuseReflectivity = new Vector3(0.07568f, 0.61424f, 0.07568f);
            mSpecularReflectivity = new Vector3(0.633f, 0.727811f, 0.633f);
            mShininess = 1.5f;
            mEmerald = new Material(mLightShader, mAmbientReflectivity, mDiffuseReflectivity, mSpecularReflectivity, mShininess);

            // Textured Materials
            mFloorTextureMaterial = new Material(mTextureShader, 0, 32);
            mWallTextureMaterial = new Material(mTextureShader, 1, 32);

            // --------- Drawable Objects --------- \\
            // Cube
            Matrix4 cubeMatrix = Matrix4.CreateTranslation(0, 2f, 0);
            mCube = new Cube(2, cubeMatrix, mWhiteRubber, mLightShader, vPositionLocation, vNormalLocation);
            mDrawableObjects[0] = mCube;

            // Cone
            Matrix4 coneMatrix = Matrix4.CreateTranslation(0, 1f, 0);
            mCone = new Cone(1.5f, 100, 2, coneMatrix, mJade, mLightShader, vPositionLocation, vNormalLocation);
            mDrawableObjects[1] = mCube;

            // Cylinder 1
            Matrix4 cylinderMatrix1 = Matrix4.CreateTranslation(-5f, 1, -5f);
            ModelUtility cylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");
            mCylinder1 = new Model(cylinderModelUtility, cylinderMatrix1, mChrome, mLightShader, vPositionLocation, vNormalLocation);
            mDrawableObjects[2] = mCube;

             // Cylinder 2
            Matrix4 cylinderMatrix2 = Matrix4.CreateTranslation(0, 1, -5f);
            mCylinder2 = new Model(mCylinder1);
            mCylinder2.mMatrix = cylinderMatrix2;
            mDrawableObjects[3] = mCube;

            // Cylinder 3
            Matrix4 cylinderMatrix3 = Matrix4.CreateTranslation(5f, 1, -5f);
            mCylinder3 = new Model(mCylinder1);
            mCylinder3.mMatrix = cylinderMatrix3;
            mDrawableObjects[4] = mCube;

            // Armadillo
            Matrix4 armadilloMatrix = Matrix4.CreateTranslation(0, 2f, 0);
            ModelUtility armadilloModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");
            mArmadillo = new Model(armadilloModelUtility, armadilloMatrix, mEmerald, mLightShader, vPositionLocation, vNormalLocation);
            mDrawableObjects[5] = mCube;

            // Ground
            Matrix4 groundMatrix = Matrix4.CreateTranslation(0, 0, -5f);
            mGround = new FlatPlane(groundMatrix, mFloorTextureMaterial, mTextureShader, vTexturePositionLocation, vTextureNormalLocation, vTexCoordsLocation);
            mDrawableObjects[6] = mCube;

            // Right Wall
            Matrix4 rightWallMatrix = Matrix4.CreateTranslation(10f, 10f, 0f);
            rightWallMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90f)) * rightWallMatrix;
            rightWallMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90f)) * rightWallMatrix;
            mRightWall = new FlatPlane(mGround);
            mRightWall.mMatrix = rightWallMatrix;
            mRightWall.mMaterial = mWallTextureMaterial;
            mDrawableObjects[7] = mCube;

            // Front Wall
            Matrix4 frontWallMatrix = Matrix4.CreateTranslation(0f, 10f, -10f);
            frontWallMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90f)) * frontWallMatrix;
            mFrontWall = new FlatPlane(mGround);
            mFrontWall.mMatrix = frontWallMatrix;
            mFrontWall.mMaterial = mWallTextureMaterial;
            mDrawableObjects[8] = mCube;

            // Left Wall
            Matrix4 leftWallMatrix = Matrix4.CreateTranslation(-10f, 10f, 0f);
            leftWallMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(-90f)) * leftWallMatrix;
            leftWallMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90f)) * leftWallMatrix;
            mLeftWall = new FlatPlane(mGround);
            mLeftWall.mMatrix = leftWallMatrix;
            mLeftWall.mMaterial = mWallTextureMaterial;
            mDrawableObjects[9] = mCube;

            // Back Wall
            Matrix4 backWallMatrix = Matrix4.CreateTranslation(0f, 10f, 10f);
            backWallMatrix = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90f)) * backWallMatrix;
            mBackWall = new FlatPlane(mGround);
            mBackWall.mMatrix = backWallMatrix;
            mBackWall.mMaterial = mWallTextureMaterial;
            mDrawableObjects[10] = mCube;

            // Screen
            mScreen = new Screen(mScreenShader, mFrameBuffer.mColorBuffer_ID, vScreenPositionLocation, vScreenTexCoordsLocation);
            mDrawableObjects[11] = mScreen;

            // --------- Misc --------- \\
            GL.BindVertexArray(0);

            mModelRotationRate = 0.5f;
            mConeXTranslationRate = 0.025f;
            mCubeYTranslationRate = 0.025f;
            mPrevTime = DateTime.Now;


            base.OnLoad(e);
        }

        private void LoadImageTexture(string pFilePath, TextureUnit pTextureUnit, int pTextureID)
        {
            Bitmap TextureBitmap;
            BitmapData TextureData;

            // File Check and Load
            if (System.IO.File.Exists(pFilePath))
            {
                TextureBitmap = new Bitmap(pFilePath);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                TextureData = TextureBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                    TextureBitmap.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            }
            else
            {
                throw new Exception("Could not find file " + pFilePath);
            }

            //Bind Texture
            GL.ActiveTexture(pTextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, mTexture_ID[pTextureID]);

            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            TextureBitmap.UnlockBits(TextureData);
        }

        private Matrix4 GetRotatedYModelMatrix(Matrix4 pModel, float pRotation)
        {
            Vector3 t = pModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            // Moved ground model back to centre, rotates and then place back in its original positon
            return pModel * inverseTranslation * Matrix4.CreateRotationY(pRotation) * translation;
        }


        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            // Switch to camera 1
            if (e.Key == Key.Left)
            {
                mCurrentCamera = mFixedCamera1;
                mCurrentCamera.UseCamera();
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Switch to camera 2
            else if (e.Key == Key.Right)
            {
                mCurrentCamera = mFixedCamera2;
                mCurrentCamera.UseCamera();
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Switch to camera 0 (Freeroam)
            else if (e.Key == Key.Down)
            {
                mCurrentCamera = mFreeRoamCamera;
                mCurrentCamera.UseCamera();
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Switch to and reset camera 0 (Freeroam)
            else if (e.Key == Key.Up)
            {

                Matrix4 cameraMatrix = Matrix4.CreateTranslation(0, -2.75f, 0);
                mFreeRoamCamera.UpdateCameraMatrix(cameraMatrix);
                mFreeRoamCamera.UseCamera();
                mCurrentCamera = mFreeRoamCamera;
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }

            base.OnKeyDown(e);
        }

        private void UpdateAllLightPositions(Matrix4 cameraMatrix)
        { 
            foreach (Light light in mAllLights)
            {
                light.UpdateLightOnCameraChange(cameraMatrix);
            }
        }

        private void DisableAllLights()
        {
            foreach (Light light in mAllLights)
            {
                light.SetEnabled(false);
            }
        }

        private void EnableAllLights()
        {
            foreach (Light light in mAllLights)
            {
                light.SetEnabled(true);
            }
        }

        private void UpdateScreenShaderEffect(int pEffect_ID)
        {
            GL.UseProgram(mScreenShader.ShaderProgramID);
            int uEffectModeLocation = GL.GetUniformLocation(mScreenShader.ShaderProgramID, "uEffectMode");
            GL.Uniform1(uEffectModeLocation, pEffect_ID);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // Move camera forwards
            if (e.KeyChar == 'w')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera left
            if (e.KeyChar == 'a')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationY(-0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera right
            if (e.KeyChar == 'd')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationY(0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Move camera backwards
            if (e.KeyChar == 's')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Move camera up
            if (e.KeyChar == 'r')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateTranslation(0.0f, -0.05f, 0.0f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Move camera down
            if (e.KeyChar == 'f')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateTranslation(0.0f, 0.05f, 0.0f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera left
            if (e.KeyChar == 'q')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationZ(-0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera right
            if (e.KeyChar == 'e')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationZ(0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera up
            if (e.KeyChar == 't')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationX(-0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }
            // Rotate camera down
            if (e.KeyChar == 'g')
            {
                Matrix4 cameraMatrix = mCurrentCamera.GetCameraMatrix();
                cameraMatrix = cameraMatrix * Matrix4.CreateRotationX(0.025f);
                mCurrentCamera.UpdateCameraMatrix(cameraMatrix);
                UpdateAllLightPositions(mCurrentCamera.GetCameraMatrix());
            }


            // Rotate floor left
            if (e.KeyChar == 'z')
            {
                mGround.mMatrix = GetRotatedYModelMatrix(mGround.mMatrix, 0.025f);
            }
            // Rotate floor right
            if (e.KeyChar == 'x')
            {
                mGround.mMatrix = GetRotatedYModelMatrix(mGround.mMatrix, -0.025f);
            }

            // Enables all lights
            if (e.KeyChar == '1')
            {
                EnableAllLights();
            }
            // Enables only point lights
            if (e.KeyChar == '2')
            {
                DisableAllLights();
                mRedLight.SetEnabled(true);
                mGreenLight.SetEnabled(true);
                mBlueLight.SetEnabled(true);
            }
            // Enables only spotlight
            if (e.KeyChar == '3')
            {
                DisableAllLights();
                mSpotLight1.SetEnabled(true);
            }
            // Enables only directional light
            if (e.KeyChar == '4')
            {
                DisableAllLights();
                mDirectionalLight.SetEnabled(true);
            }

            // Enable inverse screen effect
            if (e.KeyChar == '5')
            {
                UpdateScreenShaderEffect(1);
            }
            // Enable grayscale screen effect
            if (e.KeyChar == '6')
            {
                UpdateScreenShaderEffect(2);
            }
            // Enable sharpen screen effect
            if (e.KeyChar == '7')
            {
                UpdateScreenShaderEffect(3);
            }
            // Enable blur screen effect
            if (e.KeyChar == '8')
            {
                UpdateScreenShaderEffect(4);
            }
            // Enable edge detection screen effect
            if (e.KeyChar == '9')
            {
                UpdateScreenShaderEffect(5);
            }
            // Disable screen effect
            if (e.KeyChar == '0')
            {
                UpdateScreenShaderEffect(0);
            }
            

            base.OnKeyPress(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mLightShader != null)
            {
                GL.UseProgram(mLightShader.ShaderProgramID);
                int uProjectionLocation = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 30);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
            if (mTextureShader != null)
            {
                GL.UseProgram(mTextureShader.ShaderProgramID);
                int uTextureProjectionLocation = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 30);
                GL.UniformMatrix4(uTextureProjectionLocation, true, ref projection);
            }
            mFrameBuffer.RebindColorBuffer(ClientRectangle.Width, ClientRectangle.Height);
            mFrameBuffer.RebindRenderBuffer(ClientRectangle.Width, ClientRectangle.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Deltatime/Timestep from: https://stackoverflow.com/questions/26110228/c-sharp-delta-time-implementation
            DateTime currentTime = DateTime.Now;
            float timestep = (currentTime.Ticks - mPrevTime.Ticks) / 10000000f;
            mPrevTime = currentTime;

            // Rotates armadillo
            float rotationChange = mModelRotationRate * timestep;
            mArmadillo.mMatrix = GetRotatedYModelMatrix(mArmadillo.mMatrix, rotationChange);

            // Translate X Axis of cone
            float mXTranslationChange = mConeXTranslationRate * timestep;
            Vector3 coneTranslation = mCone.mMatrix.ExtractTranslation();

            if (coneTranslation.X + mXTranslationChange < -1 || coneTranslation.X + mXTranslationChange > 1)
            {
                mConeXTranslationRate = -mConeXTranslationRate;
                mXTranslationChange = mConeXTranslationRate * timestep;
            }

            mCone.mMatrix = mCone.mMatrix * Matrix4.CreateTranslation(mConeXTranslationRate, 0, 0);


            // Translate Y Axis of cube
            float mYTranslationChange = mCubeYTranslationRate * timestep;
            Vector3 cubeTranslation = mCube.mMatrix.ExtractTranslation();

            if (cubeTranslation.Y + mYTranslationChange < 2 || cubeTranslation.Y + mYTranslationChange > 3)
            {
                mCubeYTranslationRate = -mCubeYTranslationRate;
                mYTranslationChange = mCubeYTranslationRate * timestep;
            }

            mCube.mMatrix = mCube.mMatrix * Matrix4.CreateTranslation(0, mCubeYTranslationRate, 0);

            base.OnUpdateFrame(e);
        }


        void UpdateUModel(ref Matrix4 newModel)
        {
            // Light Shader
            GL.UseProgram(mLightShader.ShaderProgramID);
            int uModel = GL.GetUniformLocation(mLightShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref newModel);

            // Texture Shader
            GL.UseProgram(mTextureShader.ShaderProgramID);
            int uTextureModel = GL.GetUniformLocation(mTextureShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uTextureModel, true, ref newModel);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

           
            Matrix4 m;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, mFrameBuffer.mFrameBuffer_ID);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);


            // --------- Ground --------- \\
            m = mGround.mMatrix;
            UpdateUModel(ref m);

            mGround.Draw();

            // --------- Right Wall --------- \\
            m = mRightWall.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mRightWall.Draw();

            // --------- Front Wall --------- \\
            m = mFrontWall.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mFrontWall.Draw();

            // --------- Left Wall --------- \\
            m = mLeftWall.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mLeftWall.Draw();

            // --------- Back Wall --------- \\
            m = mBackWall.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mBackWall.Draw();

            // --------- Cylinder 1 --------- \\
            m = mCylinder1.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mCylinder1.Draw();

            // --------- Cone --------- \\
            m = mCone.mMatrix * m;
            UpdateUModel(ref m);

            mCone.Draw();

            // --------- Cylinder 2 --------- \\
            m = mCylinder2.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mCylinder2.Draw();

            // --------- Model --------- \\
            m = mArmadillo.mMatrix * m;
            UpdateUModel(ref m);

            mArmadillo.Draw();

            // --------- Cylinder 3 --------- \\
            m = mCylinder3.mMatrix * mGround.mMatrix;
            UpdateUModel(ref m);

            mCylinder3.Draw();

            // --------- Cube --------- \\
            m = mCube.mMatrix * m;
            UpdateUModel(ref m);

            mCube.Draw();

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);


            mScreen.Draw();

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindVertexArray(0);

            for(int i = 0; i < mDrawableObjects.Length; i++)
            {
                mDrawableObjects[i].Unload();
            }

            GL.DeleteTextures(mTexture_ID.Length, mTexture_ID);

            mFrameBuffer.Unload();

            mLightShader.Delete();
            mTextureShader.Delete();
            mScreenShader.Delete();

            base.OnUnload(e);
        }
    }
}
