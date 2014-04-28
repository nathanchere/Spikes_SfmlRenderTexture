using System;
using SFML.Graphics;
using SFML.Window;

namespace Sfml.RenderTexture
{
    public class FboTest
    {
        Action Render;
        Text Text;
        RenderWindow window;
        bool DisplayText;
        
        // White square in corner
        SFML.Graphics.RenderTexture textureOne;

        // Red square following mouse
        SFML.Graphics.RenderTexture textureTwo;
        RectangleShape shapeTwo;

        // Highlighting background issue
        SFML.Graphics.RenderTexture textureThree;
        RectangleShape shapeThree;
        

        public FboTest()
        {            
            Text = new Text("", new Font("lekton.ttf"), 20)
            {
                Color=Color.White,
                Position = new Vector2f(10,10),
            };

            textureOne = new SFML.Graphics.RenderTexture(32,32);
            textureTwo = new SFML.Graphics.RenderTexture(640,480);
            textureThree = new SFML.Graphics.RenderTexture(640,480);

            shapeTwo = new RectangleShape(new Vector2f(20,20)){ FillColor = Color.Red, };
            shapeThree = new RectangleShape(new Vector2f(40,40)){ FillColor = new Color(0,0,0,30), Origin = new Vector2f(20,20) };
            
            window = new RenderWindow(new VideoMode(640, 480), "SFML.Net RenderTexture FBO bug spike", Styles.Default, new ContextSettings { DepthBits = 32 });
            window.SetActive();
            window.Closed += OnClosed;
            window.KeyPressed += OnKeyPressed;            
        }

        public void Run()
        {
            OnKeyPressed(null, new KeyEventArgs(new KeyEvent() {Code = Keyboard.Key.Num1}));

            while (window.IsOpen())
            {
                window.DispatchEvents();
                Render();
                
                if(DisplayText) window.Draw(Text); // Note #3

                window.Display();
            }
        }

        private void RenderWorking()
        {
            window.Clear(Color.Black);

            textureOne.Clear(Color.White);
            textureOne.Display();

            var renderSprite = new Sprite(textureOne.Texture);

            // Note #2
            // renderSprite.Position = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);

            renderSprite.Draw(window, RenderStates.Default);

            // Note #1
            // window.Display();
        }

        private void RenderBuggy()
        {
            shapeTwo.Position = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);

            textureTwo.Clear();
            textureTwo.Draw(shapeTwo);
            textureTwo.Display();

            window.Clear();
            var renderSprite = new Sprite(textureTwo.Texture);
            window.Draw(renderSprite);
        }

        private void RenderMoreBuggy()
        {
            textureThree.Clear(new Color(
                (byte) (DateTime.Now.Millisecond%128),
                (byte) (DateTime.Now.Millisecond%64),
                (byte) (DateTime.Now.Millisecond%255)
            ));
            textureThree.Display();

            shapeThree.Position = new Vector2f(Mouse.GetPosition(window).X, Mouse.GetPosition(window).Y);
            shapeThree.Rotation = 0.36f*DateTime.Now.Millisecond;
            textureThree.Draw(shapeThree);            

            window.Clear();
            var renderSprite = new Sprite(textureThree.Texture);
            window.Draw(renderSprite);
        }

        void OnClosed(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Close();
        }

        void OnKeyPressed(object sender, KeyEventArgs e)
        {
            var window = (Window)sender;
            if (e.Code == Keyboard.Key.Escape) window.Close();
            
            if (e.Code == Keyboard.Key.Num1)
            {
                Text.DisplayedString = "1: small white square";
                Render = RenderWorking;
            }
            
            if (e.Code == Keyboard.Key.Num2)
            {
                Text.DisplayedString = "2: RenderTexture used as background";
                Render = RenderBuggy;
            }
            
            if (e.Code == Keyboard.Key.Num3)
            {
                Text.DisplayedString = "3: More exaggerated example of #2";
                Render = RenderMoreBuggy;
            }

            if (e.Code == Keyboard.Key.Space)
            {
                DisplayText = !DisplayText;
            }
        }        
    }
}