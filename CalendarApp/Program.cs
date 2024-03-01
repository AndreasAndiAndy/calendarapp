using System;
using System.Drawing;
using System.Windows.Forms;

namespace CalendarApp
{
    public class CalendarForm : Form
    {
        private MonthCalendar calendar;
        private Button updateButton;

        public CalendarForm()
        {
            Text = "Kalender App";
            Size = new System.Drawing.Size(500, 500);

            int Day = 1;

            for (int j = 0; j < 4; j++) { 

              // Tageslabelss erstellen und hinzufügen
              for (int i = 0; i < 7; i++)
              {
                 Label label = new Label();
                 label.BackColor = Color.Blue;
                 label.Size = new Size(50, 20);
                 label.Location = new Point(10 + i * 60, 70 + j * 75);
                 label.Text = Day.ToString();
                 Day++;
                 Controls.Add(label);
              }
            
            }

            //Hier werde ich kucken, wie viele Tage der Monat hat.
            //Wenn ein Termin hinzugefügt wird, dann soll eine Logik wie bei Android erscheinen.
            //Eingepflegte Daten sollen (max. 5 untereinander) wie bei Android unter den Zahlen stehen.

            calendar = new MonthCalendar();
            calendar.Visible = false;
            calendar.Location = new System.Drawing.Point(10, 180);
            calendar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            calendar.ForeColor = System.Drawing.Color.Purple;

            Controls.Add(calendar);

            updateButton = new Button();
            updateButton.Text = "neu";
            updateButton.Location = new System.Drawing.Point(10, 400);
            updateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            updateButton.Click += UpdateCalendar;
            Controls.Add(updateButton);
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {
            Console.WriteLine("What the heck am I supposed to do?");
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalendarForm());
        }
    }
}
