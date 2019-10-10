// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 10.0f; //#bilo je 7k

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        /// Iscrtavanje pogloge povrsine preko quads primitive
        /// </summary>
        /// <param name="gl"></param>
        private void drawBase(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Begin(OpenGL.GL_QUADS);
            gl.Rotate(270, 0f, 1f, 0f);
            gl.Translate(0f, -1.3f, 0f);
            gl.Scale(50f, 50f, 50f);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(-1f, -1f, 1f);
            gl.Vertex(1f, -1f, 1f);
            gl.Vertex(1f, -1f, -1f);
            gl.Vertex(-1f, -1f, -1f);
            gl.End();
            gl.PopMatrix();
        }

        /// <summary>
        /// Iscrtavanje drzaca bureta u vidu cilindra (treba da bude poluotvoren i da se u njemu nalazi bure
        /// </summary>
        /// <param name="gl"></param>
        private void drawCylinder(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(1f, 0f, 1f);
            gl.Scale(0.7f, 0.7f, 0.7f);
            gl.Translate(0f, 2f, 0f);
            gl.Rotate(90, 1f, 0f, 0f);
            Cylinder cilindar = new Cylinder();
            cilindar.CreateInContext(gl);
            cilindar.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        /// <summary>
        /// Iscrtava rupu kroz koje bure propada
        /// </summary>
        /// <param name="gl"></param>
        private void drawDisk(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(0f, 1f, 0f);
            gl.Translate(0f, 0.1f, 0f);
            gl.Rotate(270, 1f, 0f, 0f);
            gl.Scale(0.4f, 0.4f, 0.4f);
            Disk disk = new Disk();
            disk.InnerRadius = 1.5f;
            disk.OuterRadius = 2f;
            disk.Slices = 100;
            disk.Loops = 120;
            disk.CreateInContext(gl);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        /// <summary>
        /// Ispis 2d Teksta dole desno
        /// </summary>
        /// <param name="gl"></param>
        private void drawText(OpenGL gl)
        {
            //#ovde realno ne treba push i pop matrix, nema sta da radi sa matricom
            //#redefinisanje viewPort (ne reaguje uopste, pa sam rucno pomerio)
            gl.PushMatrix();
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            gl.DrawText(370, 90, 1f, 1f, 0f, "", 12, "");
            gl.DrawText(370, 70, 1f, 1f, 0f, "Helvetica", 12, "Predmet: Racunarska grafika");
            gl.DrawText(370, 58, 1f, 1f, 0f, "Helvetica", 12, "Sk.god: 2018/19");
            gl.DrawText(370, 46,  1f, 1f, 0f, "Helvetica", 12, "Ime: Nikola");
            gl.DrawText(370, 34, 1f, 1f, 0f, "Helvetica", 12, "Prezme: Maric");
            gl.DrawText(370, 22, 1f, 1f, 0f, "Helvetica", 12, "Sifra zad: 13.1");
            gl.PopMatrix();
        }

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH);
            m_scene.LoadScene();
            m_scene.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //#grid samo za laksu orijentaciju (obrisi)
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(15f, 0f, 1f, 1f);
            Grid grid = new Grid();
            grid.Render(gl, RenderMode.Design);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation+10, 1.0f, 0.0f, 0.0f); 
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Scale(2f, 2f, 2f);

            //#iscrtavanje baze
            drawBase(gl);

            //#iscrtavanje modela bureta
            gl.PushMatrix();
            gl.Translate(-0.1f, 1.3f, -0.153f);
            
            m_scene.Draw();
            gl.PopMatrix();

            //#iscrtavanje cilindra
            drawCylinder(gl);

            //#iscrtavanje diska
            drawDisk(gl);

            gl.PopMatrix();

            //#ispis 2d teksta
            drawText(gl);

            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;


            gl.Viewport(0, 0, m_width, m_height);

            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1f, 20000f); //#far je koliko moze u dubinu ici dok ne nestane, near vrv isto samo prema nama
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
