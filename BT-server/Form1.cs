using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace BT_server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void serverHandler(object argument)
        {
            Socket listenSocket = argument as Socket;
            while (true)
            {
                Socket handler = listenSocket.Accept();
                //nen
                richTextBox1.AppendText(String.Format("Подключился {0}", handler.RemoteEndPoint));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText("Попытка запуска! \n");
            int port = Convert.ToInt32(textBox2.Text);
            IPAddress ipServer = IPAddress.Parse(textBox1.Text);
            IPEndPoint ipPoint = new IPEndPoint(ipServer, port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                richTextBox1.AppendText("Сервер запущен: \n IP: " + textBox1.Text + " \n Port: " + textBox2.Text + " \n");
                Thread listenThread = new Thread(new ParameterizedThreadStart(serverHandler));
                listenThread.Start(listenSocket);
                

            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Ошибка: " + ex.Message + " \n");
            }

        }
    }
}
