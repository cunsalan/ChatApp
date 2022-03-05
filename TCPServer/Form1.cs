using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Dictionary<int, string> connectedPairs;
        SimpleTcpServer server;

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            server = new SimpleTcpServer(txtIP.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            connectedPairs = new Dictionary<int, string>();

        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtInfo.Text += $"Starting...{Environment.NewLine}";
            btnStart.Enabled = false;
            btnSend.Enabled = true;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)} {Environment.NewLine} ";
            });
            if (server.IsListening)
            {
                var myKey = connectedPairs.FirstOrDefault(x => x.Value == e.IpPort).Key;
                if(myKey!= 0)
                {
                    if(myKey%2 == 1)
                    {
                        var connectedIp = connectedPairs.FirstOrDefault(x => x.Key == myKey + 1).Value;

                        if(connectedIp != null)
                        {
                            server.Send(connectedIp, Encoding.UTF8.GetString(e.Data));
                        }
                        else
                        {
                            server.Send(e.IpPort, "There Is No one For You Loser!");
                        }

                    }
                    else
                    {
                        var connectedIp = connectedPairs.FirstOrDefault(x => x.Key == myKey - 1).Value;

                        if (connectedIp != null)
                        {
                            server.Send(connectedIp, Encoding.UTF8.GetString(e.Data));
                        }
                        else
                        {
                            server.Send(e.IpPort, "There Is No one For You Loser!");
                        }
                    }
                }
                
            }
            
            
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} disconnected {Environment.NewLine}";
                lstClientIP.Items.Remove(e.IpPort);
                connectedPairs.Remove(connectedPairs.FirstOrDefault(x => x.Value == e.IpPort).Key);

            });
            
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {

            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} connected {Environment.NewLine}";
                lstClientIP.Items.Add(e.IpPort);
                connectedPairs.Add(connectedPairs.Count + 1, e.IpPort);
            });
           
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (server.IsListening)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text) && lstClientIP.SelectedItem != null)
                {
                    server.Send(lstClientIP.SelectedItem.ToString(), txtMessage.Text.Trim());
                    txtInfo.Text += $"Server: {txtMessage.Text} {Environment.NewLine}";
                    txtMessage.Text = string.Empty;                   
                }
            }
        }
    }
}
