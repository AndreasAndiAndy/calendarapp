using System;
using System.Windows.Forms;

namespace CalendarApp
{
    public class Form1 : Form
    {
        private Button myButton;

        public Form1()
        {
            this.Text = "Meine Windows Forms App";

            myButton = new Button();
            myButton.Text = "Klick mich!";
            myButton.Click += MyButton_Click;

            this.Controls.Add(myButton);
        }

        private void MyButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button wurde geklickt!");
        }
    }
}
