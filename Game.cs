using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame
{
    public class Game
    {
        public int Level;
        bool hasClicked; bool isWin;
        string data = "";
        int ms = 0;

        public long ID;
        public string GameName;
        public string Description;
        public long GameTypeID;

        public Game(long mode, Panel p, Form1 form, Label ll)
        {
            Mode = mode;
            form1 = form;
            GamePanel = p;
            labelLevel = ll;
        }
        public Game() { }
        public long Mode { get; set; }
        public Form1 form1 { get; set; }
        private Panel GamePanel { get; set; }
        private bool CanChoose { get; set; }
        private Label labelLevel { get; set; }
        private bool isActive { get; set; }

        public async void Play()
        {
            isActive = true;
            ms = 0;
            GamePanel.MouseDown += CheckCollisions;
            Level = 1;
            form1.circles = GenerateCircles(Level);
            while (Level <= 1000 && isActive)
            {
                hasClicked = false; isWin = false;
                CanChoose = false;
                labelLevel.Text = String.Format("Level: {0}", Level);

                // generate field
                form1.circles = GenerateCircles(Level, form1.circles);
                GamePanel.Invalidate();
                foreach (Circle c in form1.circles)
                {
                    form1.buffer.Add(c);
                }
                await Task.Delay(1000);

                // clear field, wait
                form1.circles.Clear();
                GamePanel.Invalidate();
                await Task.Delay(500);

                // show new field, wait for answer
                form1.circles = GenerateCircles(Level + 1, form1.buffer);
                GamePanel.Invalidate();
                CanChoose = true;
                await Task.Delay(2000); 
                

                // if user clicked and winned - continue, else lose
                if (hasClicked && isWin)
                {
                    Level++;
                    form1.buffer.Clear();
                }
                else
                {
                    isActive = false;
                    form1.endGame(Level, Mode, data);
                }
            }
        }

        // add circles to list
        private List<Circle> GenerateCircles(int n, List<Circle> circles)
        {
            Random random = new Random(Environment.TickCount);
            List<Circle> temp = new List<Circle>();

            // create new list to avoid change old instance
            foreach (Circle c in circles)
            {
                temp.Add(c);
            }

            while (temp.Count < n)
            {
                Circle circle = randomCircle();

                bool collision = false;

                // if new circle collides old, continue
                foreach (Circle c in temp)
                {
                    if (circle.Collides(c)) { collision = true; break; };
                }

                if (collision) { continue; }
                else
                {
                    temp.Add(circle);
                }
            }
            return temp;
        }

        // generate new list from empty
        private List<Circle> GenerateCircles(int n)
        {
            Random random = new Random(Environment.TickCount);
            List <Circle> circles = new List<Circle>();

            while (circles.Count < n)
            {
                Circle circle = randomCircle();

                bool collision = false;

                // if new circle collides old, continue
                foreach (Circle c in circles)
                {
                    if (circle.Collides(c)) { collision = true; break; };
                }

                if (collision) { continue; }
                else
                {
                    circles.Add(circle);
                }
            }
            return circles;
        }

        private Circle randomCircle()
        {
            Random random = new Random(Environment.TickCount);
            Circle circle;

            if (Mode == 3)
            {
                int R = random.Next(20, 60);
                circle = new Circle(random.Next(0 + R, GamePanel.Width - R), random.Next(0 + R, GamePanel.Height - R), R, Color.White);
            }
            else if (Mode == 2)
            {
                int R = 40;
                circle = new Circle(random.Next(0 + R, GamePanel.Width - R), random.Next(0 + R, GamePanel.Height - R), R, form1.randomColor());
            }
            else if (Mode == 4)
            {
                int R = random.Next(20, 60);
                circle = new Circle(random.Next(0 + R, GamePanel.Width - R), random.Next(0 + R, GamePanel.Height - R), R, form1.randomColor());
            }
            else
            {
                int R = 40;
                circle = new Circle(random.Next(0 + R, GamePanel.Width - R), random.Next(0 + R, GamePanel.Height - R), R, Color.White);
            }
            return circle;
        }

        private void CheckCollisions(object sender, MouseEventArgs e)
        {
            data = data + "[click: {x: " + e.X.ToString() + ", y: " + e.Y.ToString() + ", time: " + ms.ToString() + "}] ";
            Point mouse = new Point(e.X, e.Y);
            hasClicked = true;

            // check if player can choose circle
            if (CanChoose)
            {
                bool CollidesOld = false;
                bool CollidesNew = false;

                // check collision with new circles
                foreach (Circle c in form1.circles.ToList())
                {
                    if (c.Collides(mouse))
                    {
                        CollidesNew = true;
                    }
                }

                // check collision with old circles
                foreach (Circle c in form1.buffer.ToList())
                {

                    if (c.Collides(mouse))
                    {
                        CollidesOld = true;
                    }
                }

                if(CollidesOld)
                {
                    isWin = false;
                }
                else if(!CollidesOld && CollidesNew)
                {
                    isWin = true;
                }
                else if (!CollidesOld && !CollidesNew)
                {
                    isWin = false;
                }
            }
        }
    }
}
