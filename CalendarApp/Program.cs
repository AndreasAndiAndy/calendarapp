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

        public CalendarForm()
        {

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);
             
            DateTime now = DateTime.Now;
            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

            string currentMonth = now.ToString("MM"); 

            displayDays(daysInMonth, currentMonth); 

            displayDates(DateTime.Now.Month);
            
            Size = new System.Drawing.Size(900, 500);
            
            calendar = new MonthCalendar();
            calendar.Visible = false;
            calendar.Location = new System.Drawing.Point(10, 180);
            calendar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            calendar.ForeColor = System.Drawing.Color.Purple;
            Controls.Add(calendar);

        }

        private void displayDates(int month)
        {
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
                    Console.WriteLine($"Fehler beim Verbinden zur Datenbank: {ex.Message}");
                }

                connection.Close();
            }

         
        }
        

        private void displayDays(int daysInMonth, string currentMonth)
        {
            

            Text = currentMonthIntToI18N(currentMonth);

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
            string senderText = "1";

            if (sender is Label label) {
                senderText = label.Text;
                //Console.WriteLine("senderText is " + senderText);
            }
            
            AppointmentForm appointmentForm = new AppointmentForm(this, senderText, Text);
            appointmentForm.ShowDialog();
             
        }

        private string currentMonthIntToI18N(string currentMonth)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);
            return rm.GetString(months[int.Parse(currentMonth) - 1]);
        }

        public class AppointmentForm : Form
        {
            private CalendarForm calendarFormInstance;
            private TextBox editableTextBox;
            private DateTimePicker datePicker;
            private DateTimePicker startTimePicker;
            private DateTimePicker endTimePicker;


            //2 - März
            public AppointmentForm(CalendarForm calendarForm, string dateClickedText, string currentMonth)
            {
                this.calendarFormInstance = calendarForm;

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

                int currentYear = DateTime.Now.Year;

                string[] yearMonthDay = { currentYear.ToString(), currentMonth, dateClickedText };
                

                Text = yearMonthDay[0] + " " + yearMonthDay[1] + " " + yearMonthDay[2];

                //Text festlegen.
                editableTextBox = new TextBox
                {
                    AcceptsReturn = true,
                    AcceptsTab = true,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = System.Windows.Forms.ScrollBars.Vertical
                };

                // Hier die Höhe anpassen, z.B. dreimal so hoch wie zuvor
                editableTextBox.Height = 3 * editableTextBox.Font.Height;


                // Datum auswählen
                datePicker = new DateTimePicker();
                datePicker.Format = DateTimePickerFormat.Short;
                datePicker.Location = new Point(10, 150);
               

                // Startzeit auswählen TODO: Stattdessen dropdown.
                startTimePicker = new DateTimePicker();
                startTimePicker.Format = DateTimePickerFormat.Time;
                startTimePicker.Location = new Point(10, 180);

                // Endzeit auswählen TODO: Stattdessen dropdown.
                endTimePicker = new DateTimePicker();
                endTimePicker.Format = DateTimePickerFormat.Time;
                endTimePicker.Location = new Point(10, 210);
                
                Controls.Add(datePicker);
                Controls.Add(startTimePicker);
                Controls.Add(endTimePicker);
                

                Panel textboxBoxPanel = new Panel
                {
                    Location = new System.Drawing.Point(10, 10),
                    Size = new System.Drawing.Size(400, 70), 
                };
                textboxBoxPanel.Controls.Add(editableTextBox);
        
                Controls.Add(textboxBoxPanel);
                

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

                // Lese den eingegebenen Text aus dem Textfeld
                string enteredText = editableTextBox.Text;

                calendarFormInstance.displayDays(31, "03");
                //calendarFormInstance.displayDates(DateTime.Now.Month);
                
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
