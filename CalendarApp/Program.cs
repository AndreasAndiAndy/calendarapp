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
            Size = new System.Drawing.Size(900, 500);

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
            

            string connectionString = "Data Source=C:\\Users\\user\\source\\repos\\calendarapp\\CalendarApp\\calendar.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int month = DateTime.Now.Month;
                string monthString = $"{month:D2}";
                string selectQuery = $"SELECT * FROM calendardates WHERE month = '{monthString}' ORDER BY start;";


                try
                {

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string text = reader.GetString(1);
                                string start = reader.GetString(2);
                                string end = reader.GetString(3);

                                Console.WriteLine(start);
                                //TODO: Hier werden dann die Balken entstehen.
                                //...und hier werde ich das ganze Zeug auch nach rechts schreiben - aber wie?


                                //Console.WriteLine(value);   
                                //Label label = new Label();
                                //label.BackColor = Color.Blue;
                                //label.Size = new Size(50, 10);
                                //label.Location = new Point(70, 170);
                                //Controls.Add(label);


                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Bei einem Fehler wird dieser Block ausgeführt
                    Console.WriteLine($"Fehler beim Verbinden zur Datenbank: {ex.Message}");
                }

                connection.Close();
            }

         
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
