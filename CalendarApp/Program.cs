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

            displayDays(daysInMonth);

            displayDates();

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

        private void displayDates()
        {
            //Represent data as beams (stack max. 2).
            Dictionary<string, List<CalendarDate>> beams = new Dictionary<string, List<CalendarDate>>();
            AddValue(beams, "2024-03-15T20:55:59", new CalendarDate()); //Console.WriteLine(now.ToString("s")); 2024-03-15T20:55:59
            AddValue(beams, "2024-03-15T20:55:59", new CalendarDate()); //TODO: Read this from db with below code.
            AddValue(beams, "2024-03-16T20:55:59", new CalendarDate());

            HashSet<string> dateList = new HashSet<string>();
            dateList.Add("2024-03-15T20:55:59");
            dateList.Add("2024-03-16T20:55:59");

            string connectionString = "Data Source=calendar.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Below code is here.

                connection.Close();
            }

            foreach (string date in dateList)
            {
                // Check if the key exists
                if (beams.ContainsKey(date))
                {
                    // Print all values associated with the key
                    Console.WriteLine($"Values for Key {date}:");
                    foreach (var value in beams[date])
                    {
                        Console.WriteLine(value); //TODO: Hier werden die Balken entstehen.  
                        Label label = new Label();
                        label.BackColor = Color.Blue;
                        label.Size = new Size(50, 10);
                        label.Location = new Point(70, 170);
                        Controls.Add(label);
                    }
                }
            }
        }



        private void AddValue(Dictionary<string, List<CalendarDate>> dictionary, string key, CalendarDate value)
        {
            // Check if the key already exists
            if (!dictionary.ContainsKey(key))
            {
                // If the key doesn't exist, create a new list for values
                dictionary[key] = new List<CalendarDate>();
            }

            // Add the value to the list associated with the key
            dictionary[key].Add(value);
        }

        private void displayDays(int daysInMonth)
        {
            int Day = 1; int maxNumberOfDays = 0;

            for (int j = 0; j < 5; j++)
            {

                // Tageslabels erstellen und hinzufügen
                for (int i = 0; i < 7; i++)
                {

                    if (maxNumberOfDays < daysInMonth)
                    {
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
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {
            AppointmentForm appointmentForm = new AppointmentForm();

            appointmentForm.ShowDialog();
             
        }

        public class AppointmentForm : Form
        {

            

            public AppointmentForm()
            {
                Size = new System.Drawing.Size(450, 550);
                Button dateSaveButton = new Button();
                dateSaveButton.Text = "speichern";
                dateSaveButton.Location = new System.Drawing.Point(10, 420);
                dateSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                Controls.Add(dateSaveButton);
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
