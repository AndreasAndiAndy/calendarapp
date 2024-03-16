using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Globalization;
using System.Resources;



namespace CalendarApp
{
    public class CalendarForm : Form
    {
        private MonthCalendar calendar;
        private Button updateButton;

        public CalendarForm()
        {

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);
             
            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            string currentMonth = now.ToString("MMMM");

            displayDays(daysInMonth, currentMonth); 

            displayDates(DateTime.Now.Month);
            
            Size = new System.Drawing.Size(900, 500);
            
            calendar = new MonthCalendar();
            calendar.Visible = false;
            calendar.Location = new System.Drawing.Point(10, 180);
            calendar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            calendar.ForeColor = System.Drawing.Color.Purple;

            Controls.Add(calendar);

            //updateButton = new Button();
            //updateButton.Text = rm.GetString("new");
            //updateButton.Location = new System.Drawing.Point(10, 420);
            //updateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //updateButton.Click += UpdateCalendar;
            //Controls.Add(updateButton);
        }

        private void displayDates(int month)
        {
            //string connectionString = "Data Source=C:\\Users\\user\\source\\repos\\calendarapp\\CalendarApp\\calendar.db;Version=3;";
            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
            
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
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
        

        private void displayDays(int daysInMonth, string currentMonth)
        {

            Text = currentMonth;

            int Day = 1; int maxNumberOfDays = 0;

            for (int j = 0; j < 5; j++)
            {

                // Tageslabels erstellen und hinzufügen
                for (int i = 0; i < 7; i++)
                {

                    if (maxNumberOfDays < daysInMonth)
                    {
                        Label label = new Label();
                        label.BackColor = Color.LightGreen;
                        label.Size = new Size(50, 20);
                        label.Location = new Point(10 + i * 60, 70 + j * 75);
                        label.Text = Day.ToString();

                        label.Click += UpdateCalendar;

                        Day++;
                        Controls.Add(label);

                        maxNumberOfDays++;

                    }
                }

            }
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {

            Console.WriteLine(sender + "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

            AppointmentForm appointmentForm = new AppointmentForm(this);
            appointmentForm.ShowDialog();
             
        }

        public class AppointmentForm : Form
        {
            private CalendarForm calendarFormInstance;
            

            public AppointmentForm(CalendarForm calendarForm)
            {

                this.calendarFormInstance = calendarForm;

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

                Size = new System.Drawing.Size(450, 550);
                Button dateSaveButton = new Button();
                dateSaveButton.Text = rm.GetString("save");
                dateSaveButton.Location = new System.Drawing.Point(10, 420);
                dateSaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                dateSaveButton.Click += UpdateDate;
                Controls.Add(dateSaveButton);
                
            }

            private void UpdateDate(object sender, EventArgs e)
            {
                //TODO: Dieses Ding soll den ausgewählten Monat, dessen AZ der Tage und
                //und v. a. den ausgewählten Tag abgreifen können.
                //calendarFormInstance.displayDays(daysInMonth, currentMonth);

                calendarFormInstance.displayDates(DateTime.Now.Month);
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
