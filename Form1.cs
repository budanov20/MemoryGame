using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MemoryGame
{
    public partial class Form1 : Form
    {
        public List<Circle> circles;
        public List<Circle> buffer;

        private User user;
        private Game game;

        public UserDatabase database;

        public Form1()
        {
            InitializeComponent();
            circles = new List<Circle>();
            buffer = new List<Circle>();
            database = new UserDatabase("database.sqlite");
            FillRegisterCombos();
        }

        private void UpdateScores()
        {
            labelColorHighScore.Text = "High Score: " + user.ColorScore;
            labelColorSizeHighScore.Text = "High Score: " + user.ColorSizeScore;
            labelSizeHighScore.Text = "High Score: " + user.SizeScore;
            labelGameHighScore.Text = "High Score: " + user.OneColorScore;
        }

        private void UpdateAdmin()
        {
            DisplayAverageScores();
            DisplayMaximumScores();
            DisplayUsers();
            PlotScoreByAge();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text != "" && textBoxEmail.Text != "" && (int)numericUpDownAge.Value > 0 && 
                (int)numericUpDownAge.Value < 120 && comboBoxGender.Text != "")
            {
                user = new User()
                {
                    Name = textBoxName.Text,
                    Email = textBoxEmail.Text,
                    Password = HashPassword(textBoxPassword.Text),
                    Age = (int)numericUpDownAge.Value,
                    GenderID = (long)comboBoxGender.SelectedValue,
                    RoleID = (long)1,
                    EducationLevelID = (long)comboBoxEducationLevel.SelectedValue,
                    EducationTypeID = (long)comboBoxEducationType.SelectedValue
                };
                database.AddUser(user);
                user.Id = database.FindUserByEmail(user.Email).Id;
                MessageBox.Show("You are registered successfully!");
                tabControlMain.SelectedTab = tabPageMenu;

                UpdateScores();
            }
            else
            {
                MessageBox.Show("Please enter valid data!", "Wrong values!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void buttonSizeGame_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGame;
            panelGame.Paint += new PaintEventHandler(panel1_draw);
            game = new Game(3, panelGame, this, labelLevel);
            game.Play();
        }

        private void buttonColorGame_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGame;
            panelGame.Paint += new PaintEventHandler(panel1_draw);
            game = new Game(2, panelGame, this, labelLevel);
            game.Play();
        }

        private void buttonGame_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGame;
            panelGame.Paint += new PaintEventHandler(panel1_draw);
            game = new Game(1, panelGame, this, labelLevel);
            game.Play();
        }

        private void buttonColorSize_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageGame;
            panelGame.Paint += new PaintEventHandler(panel1_draw);
            game = new Game(4, panelGame, this, labelLevel);
            game.Play();
        }

        public void panel1_draw(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            foreach (Circle circle in circles)
            {
                circle.Draw(g);
            }
        }

        private void buttonToLogin_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageLogin;
        }

        private void buttonlogin_Click(object sender, EventArgs e)
        {
            if (database.GoodLogin(textBoxLoginEmail.Text, HashPassword(textBoxLoginPassword.Text)))
            {
                user = database.Login(textBoxLoginEmail.Text, HashPassword(textBoxLoginPassword.Text));
                MessageBox.Show("Logged in successfully!");
                if (user.RoleID == 2)
                {
                    buttonToAdminPanel.Visible = true;
                    UpdateAdmin();
                    tabControlMain.SelectedTab = tabPageAdmin;
                }
                else
                {
                    tabControlMain.SelectedTab = tabPageMenu;
                }
                UpdateScores();
            }
            else
            {
                MessageBox.Show("Wrong credentials!");
            }
        }


        public void endGame(int score, long mode, string data)
        {
            this.circles.Clear();
            this.buffer.Clear();

            MessageBox.Show("You ended the game! \nYour score is " + score);

            tabControlMain.SelectedTab = tabPageMenu;
            if (mode == 2)
            {
                if (user.ColorScore < score)
                {
                    user.ColorScore = score;
                }
                labelColorHighScore.Text = "High score: " + user.ColorScore.ToString();
                database.UpdateUser(user);
            }
            else if(mode == 3)
            {
                if (user.SizeScore < score)
                {
                    user.SizeScore = score;
                }
                labelSizeHighScore.Text = "High score: " + user.SizeScore.ToString();
                database.UpdateUser(user);
            }
            else if (mode == 1)
            {
                if (user.OneColorScore < score)
                {
                    user.OneColorScore = score;
                }
                labelGameHighScore.Text = "High score: " + user.OneColorScore.ToString();
                database.UpdateUser(user);
            }
            else if (mode == 4)
            {
                if (user.ColorSizeScore < score)
                {
                    user.ColorSizeScore = score;
                }
                labelColorSizeHighScore.Text = "High score: " + user.ColorSizeScore.ToString();
                database.UpdateUser(user);
            }
            Result result = new Result
            {
                UserID = user.Id,
                GameID = mode,
                ResultScore = score,
                Data = data,
                ResultDateTime = DateTime.UtcNow
            };
            database.AddResult(result);
            this.game = new Game();
        }

        public Color randomColor()
        {
            List<Color> colors = new List<Color> { Color.White, Color.Blue, Color.Red, Color.Yellow, Color.Green, Color.Orange, Color.Violet };
            Random random = new Random(Environment.TickCount);
            int R = random.Next(0, 6);
            return colors[R];
        }

        private void buttonGoHomeFromTop_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageMenu;
        }

        private void buttonTop_Click(object sender, EventArgs e)
        {
            List<Result> results = database.GetResultsByUserId(user.Id);
            labelOverAllScore.Text = "Your overall score: " + user.TotalScore.ToString();
            labelGamesPlayed.Text = "Your place among all players: " + database.GetUserRank(user.Id).ToString() + " / " + database.GetUsers().Count();
            labelLeaderPlace.Text = "Total games played: " + results.Count.ToString();
            var displayResults = results.Select(r => new 
            { 
                Game = database.GetGameNameById(r.GameID),
                Score = r.ResultScore, 
                Date = r.ResultDateTime.ToString(),
                Data = r.Data,
            }).ToList();
            dataGridViewHistory.DataSource = displayResults;
            tabControlMain.SelectedTab = tabPageHistory;
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            user = null;
            tabControlMain.SelectedTab = tabPageLogin;
        }

        private void buttonToRegister_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectedTab = tabPageRegister;
        }
        private void FillRegisterCombos()
        {
            string sql = "SELECT * FROM Gender";
            var dt = new DataTable();
            using (var adapter = new SQLiteDataAdapter(sql, database._connection))
            {
                adapter.Fill(dt);
            }
            comboBoxGender.DataSource = dt;
            comboBoxGender.DisplayMember = "Name";
            comboBoxGender.ValueMember = "ID";

            sql = "SELECT * FROM EducationLevel";
            dt = new DataTable();
            using (var adapter = new SQLiteDataAdapter(sql, database._connection))
            {
                adapter.Fill(dt);
            }
            comboBoxEducationLevel.DataSource = dt;
            comboBoxEducationLevel.DisplayMember = "Name";
            comboBoxEducationLevel.ValueMember = "ID";

            sql = "SELECT * FROM EducationType";
            dt = new DataTable();
            using (var adapter = new SQLiteDataAdapter(sql, database._connection))
            {
                adapter.Fill(dt);
            }
            comboBoxEducationType.DataSource = dt;
            comboBoxEducationType.DisplayMember = "Name";
            comboBoxEducationType.ValueMember = "ID";
        }
        public void DisplayAverageScores()
        {
            var games = database.GetGames();
            chartAverageScoreByGame.Series.Clear();
            var series = chartAverageScoreByGame.Series.Add("Average Scores");
            series.ChartType = SeriesChartType.Column;
            foreach (var game in games) 
            { 
                var results = database.GetResultsByGameId(game.ID);
                double averageScore = 0;
                if (results.Count > 0) 
                {
                    averageScore = results.Average(result => result.ResultScore); 
                }
                series.Points.AddXY(game.GameName, averageScore);
            }
        }
        public void DisplayMaximumScores()
        {
            var games = database.GetGames();
            chartMaximumScoreByGame.Series.Clear();
            var series = chartMaximumScoreByGame.Series.Add("Maximum Scores");
            series.ChartType = SeriesChartType.Column;
            foreach (var game in games)
            {
                var results = database.GetResultsByGameId(game.ID);
                int maximumScore = 0;
                if (results.Count > 0)
                {
                    maximumScore = results.Max(result => result.ResultScore);
                }
                series.Points.AddXY(game.GameName, maximumScore);
            }
        }
        public void DisplayUsers()
        {
            var users = database.GetUsers();
            var displayUsers = users.Select(u => new
            {
                Name = u.Name,
                Email = u.Email,
                Gender = database.GetGenderById(u.GenderID).Name,
                EducationType = database.GetEducationTypeById(u.EducationTypeID).Name,
                EducationLevel = database.GetEducationLevelById(u.EducationLevelID).Name,
                TotalScore = u.TotalScore
            }).ToList();
            dataGridViewUsers.DataSource = displayUsers;
        }
        public void PlotScoreByAge()
        {
            chartSimpleGameAge.Series.Clear();
            chartColorGameAge.Series.Clear();
            chartSizeGameAge.Series.Clear();
            chartColorSizeGameAge.Series.Clear();

            var users = database.GetUsers();

            var simpleSeries = chartSimpleGameAge.Series.Add("Simple Game");
            simpleSeries.ChartType = SeriesChartType.Line;

            var colorSeries = chartColorGameAge.Series.Add("Color Game");
            colorSeries.ChartType = SeriesChartType.Line;

            var sizeSeries = chartSizeGameAge.Series.Add("Size Game");
            sizeSeries.ChartType = SeriesChartType.Line;

            var colorSizeSeries = chartColorSizeGameAge.Series.Add("ColorSize Game");
            colorSizeSeries.ChartType = SeriesChartType.Line;

            users = users.OrderBy(u => u.Age).ToList();
            foreach (var user in users)
            {
                chartSimpleGameAge.Series[0].Points.AddXY(user.Age, user.OneColorScore);
                chartColorGameAge.Series[0].Points.AddXY(user.Age, user.ColorScore);
                chartSizeGameAge.Series[0].Points.AddXY(user.Age, user.SizeScore);
                chartColorSizeGameAge.Series[0].Points.AddXY(user.Age, user.ColorSizeScore);
            }
        }
        private void CustomiseChart(Chart chart, List<User> users)
        {
            chart.ChartAreas[0].AxisX.Title = "Age";
            chart.ChartAreas[0].AxisX.Minimum = users.Min(u => u.Age);
            chart.ChartAreas[0].AxisX.Maximum = users.Max(u => u.Age);
            chart.ChartAreas[0].AxisY.Title = "Score";
            chart.ChartAreas[0].AxisY.Minimum = 0;
        }


        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
