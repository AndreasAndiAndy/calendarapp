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

            // Panels erstellen und hinzufügen
            for (int i = 0; i < 3; i++)
            {
                Panel panel = new Panel();
                panel.BackColor = Color.Blue;
                panel.Size = new Size(50, 50);
                panel.Location = new Point(10 + i * 160, 10);
                Controls.Add(panel);
            }

            calendar = new MonthCalendar();
            calendar.Location = new System.Drawing.Point(10, 180);
            calendar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            calendar.ForeColor = System.Drawing.Color.Purple;

            Controls.Add(calendar);

            updateButton = new Button();
            updateButton.Text = "neu";
            updateButton.Location = new System.Drawing.Point(10, 380);
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
