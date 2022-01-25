using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace FlagsWarGame
{
    public partial class Form1 : Form
    {
        private static int pointdata;
        private static int receivedpointdata;
        private static byte[] data = new byte[1024];
        private static int turnCount;
        Gamelogic game;
        private static Button[] gridbuttons = new Button[37];
        private static Socket client;
        private Boolean gameReady;
        private static ListBox textLog;
        private Boolean canCapture;
        private static Boolean winCondition;
        private static Boolean flagcaptured;

        public Form1()
        {
            flagcaptured = false;
            canCapture = false;
            turnCount = 0;
            receivedpointdata = -1;
            pointdata = -1;
            gameReady = false;
            game = new Gamelogic();
            winCondition = false;
            InitializeComponent();
            InitializeGrids();
            InitButtonClicks();
            InitChatLog();
            MessageBox.Show("Welcome to the Flags War Game! ");
        }
        private void InitChatLog()
        {
            textLog = new ListBox();
            textLog.Parent = this;
            textLog.Location = new Point(500, 605);
            textLog.Size = new Size(500, 100);
        }

        private void InitializeGrids()
        {
            //for largest area grids:
            gridbuttons[0] = grid3_0;
            gridbuttons[1] = grid3_1;
            gridbuttons[2] = grid3_2;
            gridbuttons[3] = grid3_3;
            gridbuttons[4] = grid3_4;
            gridbuttons[5] = grid3_5;
            gridbuttons[6] = grid3_6;
            gridbuttons[7] = grid3_7;
            gridbuttons[8] = grid3_8;
            gridbuttons[9] = grid3_9;
            gridbuttons[10] = grid3_10;
            gridbuttons[11] = grid3_11;
            gridbuttons[12] = grid3_12;
            gridbuttons[13] = grid3_13;
            gridbuttons[14] = grid3_14;
            gridbuttons[15] = grid3_15;
            gridbuttons[16] = grid3_16;
            gridbuttons[17] = grid3_17;
            gridbuttons[18] = grid3_18;
            gridbuttons[19] = grid3_19;
            gridbuttons[20] = grid3_20;
            gridbuttons[21] = grid3_21;
            gridbuttons[22] = grid3_22;
            gridbuttons[23] = grid3_23;
            gridbuttons[24] = grid3_24;
            gridbuttons[25] = grid3_25;
            gridbuttons[26] = grid3_26;
            gridbuttons[27] = grid3_27;
            gridbuttons[28] = grid3_28;
            gridbuttons[29] = grid3_29;
            gridbuttons[30] = grid3_30;
            gridbuttons[31] = grid3_31;
            gridbuttons[32] = grid3_32;
            gridbuttons[33] = grid3_33;
            gridbuttons[34] = grid3_34;
            gridbuttons[35] = grid3_35;
            gridbuttons[36] = grid3_36;
            //for medium area grids:

            //for small area grids:

        }
        private void InitButtonClicks()
        {
            for (int i = 0; i < gridbuttons.Length; i++)
            {
                gridbuttons[i].Click += handlegridButtonclick;
                gridbuttons[i].Tag = i; //button id
                
            }
            hostButton.Click += new EventHandler(ButtonHostOnClick);
            hostButton.Tag = 0;
            connectButton.Click += new EventHandler(ButtonConnectOnClick);
            connectButton.Tag = 0;
            StartGameButton.Click += new EventHandler(ButtonStartGameOnClick);
            StartGameButton.Tag = 0;
            endTurn.Click += new EventHandler(ButtonEndTurnOnClick);
            buttonDisconnect.Click += new EventHandler(ButtonDisconnectOnClick);
        }



        private void handlegridButtonclick(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            if((int)hostButton.Tag==1 || (int)connectButton.Tag == 1) 
            {
                if (turnCount % 2 == 0)
                {
                    if (!gameReady)
                    {
                        if (game.Grid[(int)clickedButton.Tag] == 1)
                        {
                            clickedButton.Image = null;
                            game.Grid[(int)clickedButton.Tag] = 0;
                            game.increaseFlagsCount();
                        }
                        else if (game.Flags_count <= 0)
                        {
                            MessageBox.Show("You dont have enough flags!");
                        }
                        else if (game.Flags_count > 0)
                        {
                            Image img = Image.FromFile("redflag.png");
                            clickedButton.Image = img;
                            game.Grid[(int)clickedButton.Tag] = 1;
                            game.decreaseFlagsCount();
                        }
                        flagCount.Text = $"x{game.Flags_count}";
                    }
                    if (turnCount > 0)
                    {
                        if (gameReady)
                        {
                            // it should be only clicked once per turn.
                            if (canCapture)
                            {
                                if (game.GridPoint[(int)clickedButton.Tag] != 1 && !winCondition)
                                {
                                    clickedButton.Text = "X";
                                    pointdata = (int)clickedButton.Tag;
                                    game.GridPoint[(int)clickedButton.Tag] = 1;
                                    canCapture = false;
                                }
                                else
                                    MessageBox.Show("You already occupied this place!");
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("This is not your turn! Please wait for the other player finish its turn...");
            }
            else
                MessageBox.Show("Please Click on Host or Connect button to start the game!");


        }


        private void ButtonStartGameOnClick(object sender, EventArgs e)
        {

            if (gameReady)
                MessageBox.Show("You already placed all of your flags");
            else if (game.Flags_count == 0)
            {
                gameReady = true;
                Color slateBlue = Color.FromName("SlateBlue");
                StartGameButton.BackColor = slateBlue;
                StartGameButton.Tag = 1;
            }
            else if (game.Flags_count > 0)
                MessageBox.Show("Please place all of your flags");
        }

        private void ButtonHostOnClick(object sender, EventArgs e)
        {
            if ((int)hostButton.Tag == 0&&(int)connectButton.Tag==0)
            {
                try
                {
                    textLog.Items.Add("waiting for a client...");
                    Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
                    server.Bind(iep);
                    server.Listen(5);
                    server.BeginAccept(new AsyncCallback(AcceptConn), server);
                    MessageBox.Show("Game is hosted successfully... waiting for second player");
                    hostButton.Tag = 1;
                    Color hostColor = Color.FromArgb(0, 255, 0);
                    hostButton.BackColor = hostColor;
                    censorPicture.Visible = false;
                    StartGameButton.Enabled = false;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    MessageBox.Show(" ERROR ! THE SOCKET IS IN USE");
                }
            }
            else
                MessageBox.Show("Game is already hosted");
        }
        void ButtonConnectOnClick(object obj, EventArgs ea)
        {
            if ((int)hostButton.Tag == 0 && (int)connectButton.Tag == 0)
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(textBoxIP.Text), 9050);
                client.BeginConnect(iep, new AsyncCallback(Connected), client);
                connectButton.Tag = 1;
                turnCount = 1;
                censorPicture.Visible = false;
                connectButton.BackColor = Color.FromArgb(0, 255, 255);
            }
            else if ((int)connectButton.Tag == 0 && (int)hostButton.Tag == 1)
            {
                MessageBox.Show("Warning! You can't connect to the host because you are the host!");
            }
        }
        void Connected(IAsyncResult iar)
        {
            try
            {
                client.EndConnect(iar);
                Action<byte[]> add = str =>
                {
                    textLog.Items.Add("Connected to: " + client.RemoteEndPoint.ToString());
                };

                BeginInvoke(add, data);
                Thread receiver = new Thread(new ThreadStart(ReceiveData));
                MessageBox.Show("Successfully Connected to the host");
                receiver.Start();

                }
                catch (SocketException)
                {
                    MessageBox.Show("Error on connecting make sure there is a host to connect");
                }
        }
        void AcceptConn(IAsyncResult iar)
        {
            try
            {
                Socket oldserver = (Socket)iar.AsyncState;
                client = oldserver.EndAccept(iar);
                Action<byte[]> add = str =>
                {
                    textLog.Items.Add("Connected to: " + client.RemoteEndPoint.ToString());
                    textLog.Items.Add("Your turn!");
                    StartGameButton.Enabled = true;
                };
                BeginInvoke(add, data);
               
                Thread receiver = new Thread(new ThreadStart(ReceiveData));
                receiver.Start();
            }
            catch (Exception e){
                MessageBox.Show("Warning! Please try on running the app from FlagsWarGame>bin>Debug>FlagsWarGame.exe"+e);
            }
        }
        private void ButtonEndTurnOnClick(object sender, EventArgs e)
        {
            if (gameReady){
                if (!canCapture)
                {
                    if (turnCount % 2 == 0)
                    {
                        turnCount++;
                        byte[] integerData = BitConverter.GetBytes(pointdata);
                        try
                        {
                            client.BeginSend(integerData, 0, integerData.Length, 0, new AsyncCallback(SendData), client);
                            endTurn.BackColor = Color.FromArgb(255, 0, 0);
                        }
                        catch (System.NullReferenceException)
                        {
                            MessageBox.Show("Error there is noone to send data! Please wait for  client to connect before ending the turn. ");
                            client.Close();
                        }

                    }
                    else
                        MessageBox.Show("This is not your turn!");
                }
                else
                    MessageBox.Show("Please click on a grid to mark");
            }
            else
                MessageBox.Show("Please after placing all of your flags click on I am Ready button!");

        }
        private void SendData(IAsyncResult ar)
        {

            
            Socket remote = (Socket)ar.AsyncState;
            remote.EndSend(ar);
            canCapture = true;

        }
        private void ReceiveData()
        {

            int recv;
            int intData;
            while (true)
            {
                recv = client.Receive(data);
                //stringData = Encoding.ASCII.GetString(data, 0, recv);
                intData = BitConverter.ToInt32(data, 0);
                
                receivedpointdata = intData;
                flagcaptured = false;
                if (receivedpointdata <= gridbuttons.Length)
                {
                    turnCount++;
                    textLog.Invoke((MethodInvoker)delegate
                    {
                        textLog.Items.Add("Your turn!");
                        endTurn.BackColor = Color.FromArgb(255, 255, 255);
                    });
                }
               
                if (intData > -1&& receivedpointdata <= gridbuttons.Length) 
                {
                    gridbuttons[receivedpointdata].Invoke((MethodInvoker)delegate 
                    {
                        gridbuttons[receivedpointdata].BackColor = Color.FromArgb(127, 127, 127);
                        
                        if (game.Grid[receivedpointdata] == 1)
                        {
                            game.Playerhealthleft--;
                            textLog.Items.Add("Your oppenent captured your flag! Flags left: "+game.Playerhealthleft);
                            flagcaptured = true;
                            
                            if (game.Playerhealthleft <= 0)
                            {
                                winCondition = true;
                                textLog.Items.Add("You lost the game ! your opponent captured all of your flags!");
                                MessageBox.Show("You lost the game");
                            }
                        }
                    });
                }
               
                if (flagcaptured)
                {
                    byte[] flagcapturedcode = BitConverter.GetBytes(888);
                    client.BeginSend(flagcapturedcode, 0, flagcapturedcode.Length, 0, new AsyncCallback(SendData), client);

                }
                if (receivedpointdata == 888)
                {
                    Action<byte[]> add = str =>
                    {
                        textLog.Items.Add("You captured one of your opponents flag !");
                    };
                    BeginInvoke(add, data);
                }
                if (receivedpointdata == 999)
                {
                    Action<byte[]> add = str =>
                    {
                        textLog.Items.Add("You won the game!");
                    };
                    BeginInvoke(add, data);
                   
                }
                if (winCondition)
                    break;
            }

            byte[] integerData = BitConverter.GetBytes(999);
            client.BeginSend(integerData, 0, integerData.Length, 0, new AsyncCallback(SendData), client);
            return;
        }
       
        private void ButtonDisconnectOnClick(object sender, EventArgs e)
        {
            client.Close();
          
        }

       
   

        private void grid3_0_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender,EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }
    }
}
