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
        public int clientCount = 0;
        public Form1()
        {
            InitializeComponent();
        }
        public string receveMessege (object socket)
        {
            Socket handler = socket as Socket;
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] dataReceve = new byte[256]; // буфер для получаемых данных
            do
            {
                bytes = handler.Receive(dataReceve);
                builder.Append(Encoding.UTF8.GetString(dataReceve, 0, bytes));
            }
            while (handler.Available > 0);
            string messege = String.Format("Сообщение от {0} : {1} \n", handler.RemoteEndPoint, builder.ToString());
            return messege;
        }
        public void sendMessege(object socket, string message)
        {
            Socket handler = socket as Socket;
            byte[] dataSend = new byte[256]; // буфер для получаемых данных
            dataSend = Encoding.UTF8.GetBytes(message);
            handler.Send(dataSend);
        }
        public void clientHandler(object socket)
        {
            Socket handler = socket as Socket;
            label3.Invoke(new Action(() => label3.Text = "Количество подключенных устройств: " + clientCount)); 
            while (true)
            {
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байтов
                byte[] dataReceve = new byte[256]; // буфер для получаемых данных
                do
                {
                    bytes = handler.Receive(dataReceve);
                    builder.Append(Encoding.UTF8.GetString(dataReceve, 0, bytes));
                }
                while (handler.Available > 0);
                string messege = String.Format("Сообщение от {0} : {1} \n", handler.RemoteEndPoint, builder.ToString());
                richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(messege)));
            }
        }
        public void serverAccept(object socket)
        {
            try
            {
                Socket listenSocket = socket as Socket;
                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(String.Format("Подключился {0} \n", handler.RemoteEndPoint))));
                    clientCount++;
                    Thread workerThread = new Thread(new ParameterizedThreadStart(clientHandler));
                    workerThread.Start(handler);
                }
            }       
            catch (Exception e)
            {
                richTextBox1.Invoke(new Action(() => richTextBox1.AppendText(e.Message)));
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
                listenSocket.Listen(100);
                richTextBox1.AppendText("Сервер запущен: \n IP: " + textBox1.Text + " \n Port: " + textBox2.Text + " \n");
                Thread listenThread = new Thread(new ParameterizedThreadStart(serverAccept));
                listenThread.Start(listenSocket);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Ошибка: " + ex.Message + " \n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
