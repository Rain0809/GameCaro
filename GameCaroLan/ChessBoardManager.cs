using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCarobyhuy
{
    public class ChessBoardManager
    {

        #region Properties
        private Panel chessBroad;
        public Panel ChessBoard
        {
            get { return chessBroad; }
            set { chessBroad = value; } 

        }
        private List<Player> player;
        public List<Player> Player { get => player; set => player = value; }
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
        public TextBox PlayerrName { get => playerrName; set => playerrName = value; }
        public PictureBox Mark { get => playerMark; set => playerMark = value; }
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public Stack<PlayInfo> PlayTimeLine { get => playTimeLine; set => playTimeLine = value; }

        private int currentPlayer;
        private PictureBox playerMark;
        private TextBox playerrName;
        private List<List<Button>> matrix;
        private event EventHandler <ButtonClickEvent> playerMarked;
        public event EventHandler <ButtonClickEvent> PlayerMarked
        {

            add
            {
                playerMarked += value;  
            }
            remove
            {
                playerMarked -= value;
            }

        }
        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {

            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }

        }
        private Stack<PlayInfo> playTimeLine;
        #endregion

        #region Initialize
        public ChessBoardManager (Panel chessBoard ,TextBox playerName,PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.playerrName = playerName;
            this.playerMark = mark;
            this.player = new List<Player>() 
            { 
                new Player("Player 1", Image.FromFile(Application.StartupPath + "\\img\\iconXiao.jpg")),
                new Player("Player 2", Image.FromFile(Application.StartupPath + "\\img\\iconVenti.jpg"))

            };
            PlayTimeLine = new Stack<PlayInfo>();
           
        }
        #endregion

        #region Methods

        #endregion
        public void DrawChessBoard() // khởi tạo nút X O
        {
            chessBroad.Enabled = true;
            chessBroad.Controls.Clear();
            playTimeLine = new Stack<PlayInfo>();
            currentPlayer = 0;
            matrix = new List<List<Button>>();  
            Button oldbutton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldbutton.Location.X + oldbutton.Width, oldbutton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += btn_Click;

                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldbutton = btn;
                }

                oldbutton.Location = new Point(0, oldbutton.Location.Y + Cons.CHESS_HEIGHT);
                oldbutton.Width = 0;
                oldbutton.Height = 0;

            }

        }
        private bool isEndGame(Button btn)
        {
            
           return isEndHorizontal(btn) || isEndVertical(btn) || isEndSub(btn) || isEndPrimary(btn);
           
        }
         

        public bool Undo()
        {
            if (PlayTimeLine.Count <= 0)

            {
                return false;
            }
            PlayInfo oldPoint = PlayTimeLine.Pop();

            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];
            btn.BackgroundImage = null;
            
            if (PlayTimeLine.Count <= 0)             
            {
                currentPlayer = 0 ;
            }
            else
            {
                oldPoint = playTimeLine.Peek();

                currentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;
            }


            ChangePlayer();
            return true;
        }


        private Point GetChessPoint(Button btn)
        {
            

            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);
            Point point = new Point(horizontal, vertical);
            return point;

        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countleft = 0;
           
            for (int i = point.X; i >=0 ; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countleft++;
                }
                else
                {
                    break;
                }
            }
            int countright = 0;
            for (int i = point.X + 1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countright++;
                }
                else
                {
                    break;
                }
            }

            return countleft + countright == 5 ;

        }
        private bool isEndVertical(Button btn)
        {

            Point point = GetChessPoint(btn);
            int countTop = 0;

            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }
            }
            int countBottom = 0;
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }
            }

            return countTop + countBottom == 5;

        }
        private bool isEndPrimary(Button btn)
        {

            Point point = GetChessPoint(btn);
            int countTop = 0;

            for (int i = 0; i <= point.X ; i++)
            {
                if (point.X - i <0 || point.Y - i <0)
                    break;
                if (Matrix[point.Y - i][point.X - i ].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X ; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X + i >= Cons.CHESS_BOARD_WIDTH)
                    break;
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }
            }

            return countTop + countBottom == 5;

        }
        private bool isEndSub(Button btn)
        {

            Point point = GetChessPoint(btn);
            int countTop = 0;

            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0)
                    break;
                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0)
                    break;
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }
            }

            return countTop + countBottom == 5;


        }
        void btn_Click(object sender, EventArgs e)

        {

            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
           
            btn.BackgroundImage = player[currentPlayer].Mark;
           

            playTimeLine.Push(new PlayInfo( GetChessPoint(btn),currentPlayer));
            currentPlayer = currentPlayer == 1 ? 0 : 1;

            ChangePlayer();


            if (playerMarked != null)
                playerMarked(this, new ButtonClickEvent(GetChessPoint(btn)));

            if (isEndGame(btn))
            {
                EndGame();
            }
           
            
         }
        public void OtherPlayerMark (Point point )
        {
            
            Button btn = Matrix[point.Y][point.X];
            if (btn.BackgroundImage != null)
                return;



            ChessBoard.Enabled = true;
            btn.BackgroundImage = player[currentPlayer].Mark;


            playTimeLine.Push(new PlayInfo(GetChessPoint(btn), currentPlayer));
            currentPlayer = currentPlayer == 1 ? 0 : 1;

            ChangePlayer();


            if (isEndGame(btn))
            {
                EndGame();
            }
        }

        public void EndGame()
        {
            if(endedGame != null)
               endedGame(this,new EventArgs());    
        }
        private void ChangePlayer()
        {

            playerrName.Text = player[currentPlayer].Name;
            playerMark.Image = player[currentPlayer].Mark;

        }
        
    }
    public class ButtonClickEvent : EventArgs
    {
        private Point ClickedPoint;

        public Point ClickedPoint1 { get => ClickedPoint; set => ClickedPoint = value; }
        public ButtonClickEvent(Point point)
        {
            this.ClickedPoint = point;  
        }
    }
}

