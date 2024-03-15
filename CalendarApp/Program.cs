using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SQLite;


namespace CalendarApp
{
    public class CalendarForm : Form
    {
        private MonthCalendar calendar;
        private Button updateButton;

        public CalendarForm()
        {


            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
   
            Text = "Kalender App";
            Size = new System.Drawing.Size(500, 500);

            int Day = 1; int maxNumberOfDays = 0;  

            for (int j = 0; j < 5; j++) { 

              // Tageslabels erstellen und hinzufügen
              for (int i = 0; i < 7; i++)
              {

                if (maxNumberOfDays < daysInMonth) { 
                 Label label = new Label();
                 label.BackColor = Color.Blue;
                 label.Size = new Size(50, 20);
                 label.Location = new Point(10 + i * 60, 70 + j * 75);
                 label.Text = Day.ToString();
                 Day++;
                 Controls.Add(label);

                 maxNumberOfDays++;

                    }
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
            updateButton.Location = new System.Drawing.Point(10, 420);
            updateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            updateButton.Click += UpdateCalendar;
            Controls.Add(updateButton);
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {
            // Neue Instanz der AppointmentForm erstellen
            AppointmentForm appointmentForm = new AppointmentForm();

            // Wenn ShowDialog() aufgerufen wird, wird die Hauptform blockiert, bis die AppointmentForm geschlossen wird.
            // Dadurch wird sie als Popup geöffnet.
            appointmentForm.ShowDialog();
             
        }

        public class AppointmentForm : Form
        {

            public AppointmentForm()
            {
                DateTime now = DateTime.Now;
                int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

                Size = new System.Drawing.Size(450, 550);
                Button dateSaveButton = new Button();
                dateSaveButton.Text = "speichern";
                dateSaveButton.Location = new System.Drawing.Point(10, 420);
                dateSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                Controls.Add(dateSaveButton);

                Dictionary<int, int> beams = new Dictionary<int, int>();

                
                string connectionString = "Data Source=meinedatenbank.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Hier können Sie SQL-Abfragen ausführen

                    connection.Close();
                }
                
            }
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
