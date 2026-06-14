using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        private Button[,] buttons = new Button[3, 3];
        private bool isPlayerXTurn = true;
        private int movesCount = 0;
        private Label statusLabel;
        private Button resetButton;
        private Label scoreLabelX;
        private Label scoreLabelO;
        private int scoreX = 0;
        private int scoreO = 0;
        private Panel gamePanel;
        private Timer animationTimer;
        private float animationProgress = 0;
        private Button lastClickedButton;
        private Label titleLabel;
        private Button resetScoresButton;

        public Form1()
        {
            SetupForm();
            InitializeGame();
            SetupGradientBackground();
        }

        private void SetupForm()
        {
            this.Text = "Ultimate Tic Tac Toe";
            this.Size = new Size(600, 700);  // Fixed size that fits perfectly
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;  // Fixed size for perfect display
            this.MaximizeBox = false;  // Disable maximize to maintain perfect layout
            this.BackColor = Color.FromArgb(25, 25, 35);
        }

        private void SetupGradientBackground()
        {
            this.Paint += (sender, e) =>
            {
                using (LinearGradientBrush gradient = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(25, 25, 35),
                    Color.FromArgb(45, 35, 55),
                    90f))
                {
                    e.Graphics.FillRectangle(gradient, this.ClientRectangle);
                }
            };
        }

        private void InitializeGame()
        {
            // Title Label
            titleLabel = new Label
            {
                Text = "TIC TAC TOE",
                Location = new Point(0, 15),
                Size = new Size(this.Width, 40),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Score Panel
            Panel scorePanel = new Panel
            {
                Location = new Point(40, 65),
                Size = new Size(520, 55),
                BackColor = Color.FromArgb(35, 35, 45)
            };
            MakeRoundedControl(scorePanel, 12);
            this.Controls.Add(scorePanel);

            scoreLabelX = new Label
            {
                Text = "✖ PLAYER X: 0",
                Location = new Point(40, 12),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 200, 255),
                BackColor = Color.Transparent
            };

            scoreLabelO = new Label
            {
                Text = "◯ PLAYER O: 0",
                Location = new Point(280, 12),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 140),
                BackColor = Color.Transparent
            };

            scorePanel.Controls.Add(scoreLabelX);
            scorePanel.Controls.Add(scoreLabelO);

            // Game Panel (Smaller to fit perfectly)
            gamePanel = new Panel
            {
                Location = new Point(40, 135),
                Size = new Size(520, 420),
                BackColor = Color.FromArgb(30, 30, 40)
            };
            MakeRoundedControl(gamePanel, 15);
            this.Controls.Add(gamePanel);

            // Create smaller buttons that fit perfectly
            int buttonSize = 150;  // Smaller button size
            int margin = 10;       // Space between buttons
            int startX = 15;       // Starting X
            int startY = 15;       // Starting Y

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(buttonSize, buttonSize),
                        Location = new Point(startX + (j * (buttonSize + margin)),
                                           startY + (i * (buttonSize + margin))),
                        Font = new Font("Segoe UI", 44, FontStyle.Bold),
                        BackColor = Color.FromArgb(45, 45, 55),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Tag = new Tuple<int, int>(i, j)
                    };

                    btn.FlatAppearance.BorderSize = 0;

                    btn.MouseEnter += (sender, e) =>
                    {
                        if (btn.Text == "")
                            btn.BackColor = Color.FromArgb(65, 65, 75);
                    };
                    btn.MouseLeave += (sender, e) =>
                    {
                        if (btn.Text == "")
                            btn.BackColor = Color.FromArgb(45, 45, 55);
                    };

                    btn.Click += Button_Click;
                    MakeRoundedControl(btn, 10);
                    buttons[i, j] = btn;
                    gamePanel.Controls.Add(btn);
                }
            }

            // Draw grid lines on game panel
            gamePanel.Paint += DrawGridLines;

            // Status Panel
            Panel statusPanel = new Panel
            {
                Location = new Point(40, 575),
                Size = new Size(380, 45),
                BackColor = Color.FromArgb(40, 40, 50)
            };
            MakeRoundedControl(statusPanel, 10);
            this.Controls.Add(statusPanel);

            statusLabel = new Label
            {
                Text = "✖ PLAYER X'S TURN",
                Location = new Point(0, 0),
                Size = new Size(380, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 200, 255),
                BackColor = Color.Transparent
            };
            statusPanel.Controls.Add(statusLabel);

            // Reset Button
            resetButton = new Button
            {
                Text = "⟳ NEW GAME",
                Location = new Point(435, 575),
                Size = new Size(125, 45),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(80, 70, 120),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resetButton.FlatAppearance.BorderSize = 0;
            resetButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 90, 140);
            resetButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 50, 100);
            resetButton.Click += ResetButton_Click;
            MakeRoundedControl(resetButton, 10);
            this.Controls.Add(resetButton);

            // Reset Scores Button
            resetScoresButton = new Button
            {
                Text = "↺ RESET SCORES",
                Location = new Point(435, 635),
                Size = new Size(125, 30),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            resetScoresButton.FlatAppearance.BorderSize = 0;
            resetScoresButton.Click += ResetScoresButton_Click;
            MakeRoundedControl(resetScoresButton, 8);
            this.Controls.Add(resetScoresButton);

            animationTimer = new Timer();
            animationTimer.Interval = 10;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        private void DrawGridLines(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int buttonSize = 150;
            int margin = 10;
            int startX = 15;
            int startY = 15;

            using (Pen gridPen = new Pen(Color.FromArgb(100, 100, 130), 3))
            {
                // Draw vertical lines
                for (int i = 1; i < 3; i++)
                {
                    int x = startX + (i * (buttonSize + margin)) - margin / 2;
                    g.DrawLine(gridPen, x, startY - 2, x, startY + 3 * buttonSize + 2 * margin + 2);
                }

                // Draw horizontal lines
                for (int i = 1; i < 3; i++)
                {
                    int y = startY + (i * (buttonSize + margin)) - margin / 2;
                    g.DrawLine(gridPen, startX - 2, y, startX + 3 * buttonSize + 2 * margin + 2, y);
                }
            }

            // Draw outer border
            using (Pen borderPen = new Pen(Color.FromArgb(120, 120, 150), 2))
            {
                Rectangle borderRect = new Rectangle(startX - 3, startY - 3,
                    3 * buttonSize + 2 * margin + 6,
                    3 * buttonSize + 2 * margin + 6);
                g.DrawRectangle(borderPen, borderRect);
            }
        }

        private void MakeRoundedControl(Control control, int radius)
        {
            control.Paint += (sender, e) =>
            {
                GraphicsPath path = new GraphicsPath();
                Rectangle rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.X + rect.Width - radius, rect.Y + rect.Height - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Y + rect.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            };
        }

        private async void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton.Text != "" || CheckWinner() != null)
                return;

            lastClickedButton = clickedButton;

            clickedButton.BackColor = Color.FromArgb(70, 70, 85);
            await System.Threading.Tasks.Task.Delay(50);
            clickedButton.BackColor = Color.FromArgb(45, 45, 55);

            clickedButton.Text = isPlayerXTurn ? "✖" : "◯";
            clickedButton.ForeColor = isPlayerXTurn ? Color.FromArgb(100, 200, 255) : Color.FromArgb(255, 140, 140);
            clickedButton.Font = new Font("Segoe UI", 44, FontStyle.Bold);
            movesCount++;

            animationProgress = 0;
            animationTimer.Start();

            string winner = CheckWinner();
            if (winner != null)
            {
                statusLabel.Text = winner == "✖" ? "✨ PLAYER X WINS! ✨" : "✨ PLAYER O WINS! ✨";
                statusLabel.ForeColor = Color.Gold;
                HighlightWinningCombo();
                UpdateScore(winner);
                DisableButtons();
                await CelebrateWin();
            }
            else if (movesCount == 9)
            {
                statusLabel.Text = "🤝 IT'S A DRAW! 🤝";
                statusLabel.ForeColor = Color.Orange;
                DisableButtons();
            }
            else
            {
                isPlayerXTurn = !isPlayerXTurn;
                UpdateStatusWithAnimation();
            }
        }

        private async System.Threading.Tasks.Task CelebrateWin()
        {
            for (int i = 0; i < 3; i++)
            {
                statusLabel.BackColor = Color.FromArgb(80, 70, 90);
                await System.Threading.Tasks.Task.Delay(100);
                statusLabel.BackColor = Color.Transparent;
                await System.Threading.Tasks.Task.Delay(100);
            }
        }

        private void UpdateStatusWithAnimation()
        {
            statusLabel.Text = isPlayerXTurn ? "✖ PLAYER X'S TURN" : "◯ PLAYER O'S TURN";
            statusLabel.ForeColor = isPlayerXTurn ? Color.FromArgb(100, 200, 255) : Color.FromArgb(255, 140, 140);

            statusLabel.BackColor = Color.FromArgb(80, 70, 90);
            Timer flashTimer = new Timer();
            flashTimer.Interval = 200;
            flashTimer.Tick += (s, e) =>
            {
                statusLabel.BackColor = Color.Transparent;
                flashTimer.Stop();
            };
            flashTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationProgress += 0.1f;
            if (animationProgress >= 1 && lastClickedButton != null)
            {
                animationTimer.Stop();
                lastClickedButton.Font = new Font("Segoe UI", 44, FontStyle.Bold);
            }
            else if (lastClickedButton != null)
            {
                float scale = 1 + (animationProgress * 0.1f);
                lastClickedButton.Font = new Font("Segoe UI", 44 * scale, FontStyle.Bold);
            }
        }

        private string CheckWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (buttons[i, 0].Text != "" &&
                    buttons[i, 0].Text == buttons[i, 1].Text &&
                    buttons[i, 1].Text == buttons[i, 2].Text)
                {
                    return buttons[i, 0].Text;
                }
            }

            for (int j = 0; j < 3; j++)
            {
                if (buttons[0, j].Text != "" &&
                    buttons[0, j].Text == buttons[1, j].Text &&
                    buttons[1, j].Text == buttons[2, j].Text)
                {
                    return buttons[0, j].Text;
                }
            }

            if (buttons[0, 0].Text != "" &&
                buttons[0, 0].Text == buttons[1, 1].Text &&
                buttons[1, 1].Text == buttons[2, 2].Text)
            {
                return buttons[0, 0].Text;
            }

            if (buttons[0, 2].Text != "" &&
                buttons[0, 2].Text == buttons[1, 1].Text &&
                buttons[1, 1].Text == buttons[2, 0].Text)
            {
                return buttons[0, 2].Text;
            }

            return null;
        }

        private void HighlightWinningCombo()
        {
            for (int i = 0; i < 3; i++)
            {
                if (buttons[i, 0].Text != "" &&
                    buttons[i, 0].Text == buttons[i, 1].Text &&
                    buttons[i, 1].Text == buttons[i, 2].Text)
                {
                    HighlightButton(buttons[i, 0]);
                    HighlightButton(buttons[i, 1]);
                    HighlightButton(buttons[i, 2]);
                    return;
                }
            }

            for (int j = 0; j < 3; j++)
            {
                if (buttons[0, j].Text != "" &&
                    buttons[0, j].Text == buttons[1, j].Text &&
                    buttons[1, j].Text == buttons[2, j].Text)
                {
                    HighlightButton(buttons[0, j]);
                    HighlightButton(buttons[1, j]);
                    HighlightButton(buttons[2, j]);
                    return;
                }
            }

            if (buttons[0, 0].Text != "" &&
                buttons[0, 0].Text == buttons[1, 1].Text &&
                buttons[1, 1].Text == buttons[2, 2].Text)
            {
                HighlightButton(buttons[0, 0]);
                HighlightButton(buttons[1, 1]);
                HighlightButton(buttons[2, 2]);
                return;
            }

            if (buttons[0, 2].Text != "" &&
                buttons[0, 2].Text == buttons[1, 1].Text &&
                buttons[1, 1].Text == buttons[2, 0].Text)
            {
                HighlightButton(buttons[0, 2]);
                HighlightButton(buttons[1, 1]);
                HighlightButton(buttons[2, 0]);
            }
        }

        private async void HighlightButton(Button btn)
        {
            btn.BackColor = Color.Gold;
            btn.ForeColor = Color.Black;
            for (int i = 0; i < 3; i++)
            {
                btn.BackColor = Color.Gold;
                await System.Threading.Tasks.Task.Delay(150);
                btn.BackColor = Color.Orange;
                await System.Threading.Tasks.Task.Delay(150);
            }
            btn.BackColor = Color.Gold;
        }

        private void UpdateScore(string winner)
        {
            if (winner == "✖")
            {
                scoreX++;
                scoreLabelX.Text = $"✖ PLAYER X: {scoreX}";
                AnimateScore(scoreLabelX);
            }
            else if (winner == "◯")
            {
                scoreO++;
                scoreLabelO.Text = $"◯ PLAYER O: {scoreO}";
                AnimateScore(scoreLabelO);
            }
        }

        private async void AnimateScore(Label label)
        {
            Color originalColor = label.ForeColor;
            for (int i = 0; i < 3; i++)
            {
                label.ForeColor = Color.Gold;
                await System.Threading.Tasks.Task.Delay(100);
                label.ForeColor = originalColor;
                await System.Threading.Tasks.Task.Delay(100);
            }
        }

        private void DisableButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }

        private void EnableButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    buttons[i, j].Text = "";
                    buttons[i, j].BackColor = Color.FromArgb(45, 45, 55);
                    buttons[i, j].ForeColor = Color.White;
                    buttons[i, j].Enabled = true;
                }
            }

            movesCount = 0;
            isPlayerXTurn = true;
            statusLabel.Text = "✖ PLAYER X'S TURN";
            statusLabel.ForeColor = Color.FromArgb(100, 200, 255);
            statusLabel.BackColor = Color.Transparent;

            gamePanel.Invalidate();

            resetButton.BackColor = Color.FromArgb(100, 90, 140);
            Timer resetTimer = new Timer();
            resetTimer.Interval = 200;
            resetTimer.Tick += (s, ev) =>
            {
                resetButton.BackColor = Color.FromArgb(80, 70, 120);
                resetTimer.Stop();
            };
            resetTimer.Start();
        }

        private void ResetScoresButton_Click(object sender, EventArgs e)
        {
            scoreX = 0;
            scoreO = 0;
            scoreLabelX.Text = "✖ PLAYER X: 0";
            scoreLabelO.Text = "◯ PLAYER O: 0";
            ResetButton_Click(sender, e);

            resetScoresButton.BackColor = Color.FromArgb(80, 80, 90);
            Timer resetTimer = new Timer();
            resetTimer.Interval = 200;
            resetTimer.Tick += (s, ev) =>
            {
                resetScoresButton.BackColor = Color.FromArgb(60, 60, 70);
                resetTimer.Stop();
            };
            resetTimer.Start();
        }
    }
}