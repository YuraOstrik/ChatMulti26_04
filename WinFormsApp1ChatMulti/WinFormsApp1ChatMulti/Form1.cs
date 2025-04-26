namespace WinFormsApp1ChatMulti
{
    public partial class Form1 : Form
    {
        Client client;

        public Form1()
        {
            InitializeComponent();
            client = new Client();
            client.WriteMessage += Client_WriteMessage;
        }

        private void Client_WriteMessage(string obj)
        {
            listBox1.Items.Add(obj);

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            client.Connect(textBox1.Text);
            textBox1.ReadOnly = true;
            _ = Task.Run(() => client.ReceiveM());
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string message = textBox2.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                await client.SendM(message);
                textBox2.Clear();
            }
        }
    }
}
