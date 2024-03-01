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
            Size = new System.Drawing.Size(300, 300);

            calendar = new MonthCalendar();
            calendar.Location = new System.Drawing.Point(10, 10);
            calendar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            calendar.BackColor = System.Drawing.Color.Purple; //TODO: Wieso wird das Ding nicht lila??

            calendar.DateSelected += new System.Windows.Forms.DateRangeEventHandler(Calendar_DateSelected);

            Controls.Add(calendar);

            updateButton = new Button();
            updateButton.Text = "neu";
            updateButton.Location = new System.Drawing.Point(10, 220);
            updateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            updateButton.Click += UpdateCalendar;
            Controls.Add(updateButton);
        }

        private void Calendar_DateSelected(object sender, DateRangeEventArgs e) {
            // Show the start and end dates in the text box.
            Console.WriteLine("Date Selected: Start = " +
                e.Start.ToShortDateString() + " : End = " + e.End.ToShortDateString());
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {
            // Show the start and end dates in the text box.
            Console.WriteLine("What the heck am I supposed to do?");
            // Hier könntest du die Logik implementieren, um den Kalender zu aktualisieren.
            // In diesem Beispiel wird einfach der Kalender des aktuellen Monats erneut gesetzt.

            /*
            Controls.Remove(calendar);
            calendar.Dispose();

            calendar = new MonthCalendar();
            calendar.Location = new System.Drawing.Point(10, 10);
            calendar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(calendar);
            */
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
