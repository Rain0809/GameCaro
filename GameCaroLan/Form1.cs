using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCarobyhuy
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager socket;
        #endregion

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            ChessBoard = new ChessBoardManager(pnlChessboard,txbPlayerName,pictureBox1);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            prcbCoolDown.Step = Cons.COOL_DOWN_STEP;
            prcbCoolDown.Maximum = Cons.COOL_DOWN_TIME;
            prcbCoolDown.Value = 0;
            tmCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;

            socket = new SocketManager();   

            ChessBoard.DrawChessBoard();
            NewGame();

        }
        void EndGame()
        {
            tmCoolDown.Stop();
            pnlChessboard.Enabled = false;
            thoátToolStripMenuItem.Enabled = false; 
            MessageBox.Show("kết thúc");
        }
        void NewGame()
        {
           
            prcbCoolDown.Value = 0;
            tmCoolDown.Stop();
            thoátToolStripMenuItem.Enabled = true;
            ChessBoard.DrawChessBoard();
        }
        void Undo()
        {
            ChessBoard.Undo();  
        }
        void Quit()
        {
            if (MessageBox.Show("bạn có muốn thoát không ?", "thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)

                Application.Exit();

        }
        void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmCoolDown.Start();
            pnlChessboard.Enabled = false;
            
            prcbCoolDown.Value = 0;

            socket.Send(new SocketData((int)SocketCommad.SEND_POINT,"",e.ClickedPoint1));

            Listen();
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
            socket.Send(new SocketData((int)SocketCommad.END_GAME, "", new Point()));
        }



        #region from

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        #endregion

        private void tmCoolDown_Tick(object sender, EventArgs e)
        {
            prcbCoolDown.PerformStep();
            if (prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {
               
                EndGame();
                socket.Send(new SocketData((int)SocketCommad.TIME_OUT, "", new Point()));

            }
        }

        private void gameMớiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (MessageBox.Show("bạn có muốn thoát không ?", "thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)

            {
                Application.Exit();
               
            }
            else
            {
                    socket.Send(new SocketData((int)SocketCommad.QUIT,"",new Point()));
            }
        }

        private void btnLAN_Click(object sender, EventArgs e)
        {
            socket.IP = txbIp.Text;
            if (!socket.ConnectServer ())
            {

                socket.isServer = true;
                pnlChessboard.Enabled = true;
                socket.CreateServer();
                
               

            }
            else
            {
                socket.isServer = false;
                pnlChessboard.Enabled = false;
                Listen();
               


            }
            
        }
        private void Form1_Shown(object sender,EventArgs e)
        {
            txbIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (string.IsNullOrEmpty(txbIp.Text))
            {
                txbIp.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }
        void Listen()
        {
          
                Thread listenthread = new Thread(() =>
                {
                try
                {
                    SocketData data = (SocketData)socket.Receive();
                    ProcessData(data);
                 }
                 catch
                  {
                       
                  }
                });
                listenthread.IsBackground = true;
                listenthread.Start();
           
          
        }
        private void ProcessData(SocketData data)

        {
            switch (data.Commad)
            {
                case (int)SocketCommad.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommad.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        ChessBoard.OtherPlayerMark(data.Point);
                        prcbCoolDown.Value = 0;
                        pnlChessboard.Enabled = true;
                        tmCoolDown.Start();
                        ChessBoard.OtherPlayerMark(data.Point);

                    }));
                    
                    break;
                case (int)SocketCommad.NEW_GAME:

                    break;
                case (int)SocketCommad.UNDO:

                    break;
                case (int)SocketCommad.QUIT:
                    tmCoolDown.Stop();
                    MessageBox.Show("người chơi đã thoát");
                    break;
                case (int)SocketCommad.TIME_OUT:
                    MessageBox.Show("hết giờ , bạn đã thua");
                    break;
                default:
                    break;
            }
            Listen(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();    
        }
    }
}
