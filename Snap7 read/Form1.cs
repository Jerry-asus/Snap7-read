using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Snap7;
using System.Threading;

namespace Snap7_read
{
    public partial class Form1 : Form
    {
        private S7Client client1=new S7Client();
        private S7Client client2 = new S7Client();

        private byte[] DB_A = new byte[256];
        private byte[] DB_B = new byte[256];
        

        delegate void setTextCallback(string text);


        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                while (!this.textBox1.IsHandleCreated)
                {
                    if (this.textBox1.Disposing || this.textBox1.IsDisposed)
                    {
                        return;
                    }
                }
                setTextCallback d = new setTextCallback(SetText);
                this.textBox1.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }  
        }

        private void SetText1(string text)
        {
            if (this.textBox2.InvokeRequired)
            {
                while (!this.textBox2.IsHandleCreated)
                {
                    if (this.textBox2.Disposing || this.textBox2.IsDisposed)
                    {
                        return;
                    }
                }
                setTextCallback d = new setTextCallback(SetText1);
                this.textBox2.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox2.Text = text;
            }
        }
        private void ShowResult(int Result)
        {
            // This function returns a textual explaination of the error code
           // TextError.Text = Client.ErrorText(Result);
        }

        private string Dump(TextBox Box, byte[] Buffer, int Size)
        {
            string str = "";
            // Declaration separated from the code for readability
            int y;
            //Box.Text = "";
            y = 0;
            for (int c = 0; c < Size; c++)
            {
                String S = Convert.ToString(Buffer[c], 16);
                if (S.Length == 1) S = "0" + S;
                str = str+ "0x" + S + " ";
                y++;
                if (y == 8)
                {
                    y = 0;
                   str = str + (char)13 + (char)10; 
                }
            }
            return str;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread1 = new Thread(() =>
            {

                int result1 = client1.ConnectTo("192.168.1.110", 0, 1);
                if (result1 == 0)
                {
                    MessageBox.Show("ConnectOk!");
                }
                try
                {
                    S7MultiVar Reader = new S7MultiVar(client1);
                    Reader.Add(S7Client.S7AreaDB, S7Client.S7WLByte, 4, 0, 16, ref DB_A);
                    int result = Reader.Read();
                    ShowResult(result);
                    if (Reader.Results[0] == 0)
                    {
                        this.SetText(Dump(textBox1, DB_A, 16));
                    }
                    MessageBox.Show("ReadOk!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.ToString());
                }
            });

            Thread thread2 = new Thread(() =>
            {

                int result2 = client2.ConnectTo("192.168.1.111", 0, 1);
                if (result2 == 0)
                {
                    MessageBox.Show("ConnectOk!");
                }
                try
                {
                    S7MultiVar Reader1 = new S7MultiVar(client2);
                    Reader1.Add(S7Client.S7AreaDB, S7Client.S7WLByte, 4, 0, 16, ref DB_B);
                    int result1 = Reader1.Read();
                    ShowResult(result1);
                    if (Reader1.Results[0] == 0)
                    {
                        this.SetText1(Dump(textBox2, DB_B, 16));
                    }
                    MessageBox.Show("ReadOk!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error", ex.ToString());
                }
            });


            thread1.IsBackground = true;
            thread1.Start();

            thread2.IsBackground = true;
            thread2.Start();
        }




        private void button1_Click(object sender, EventArgs e)
        {
           
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          
            
        }
    }
}
