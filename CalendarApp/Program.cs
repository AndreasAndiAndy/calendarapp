using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Globalization;
using System.Resources;
using System.Collections.Generic;
using System.Linq;

namespace CalendarApp
{
    public class CalendarForm : Form
    {

        private DateTime now = DateTime.Now;

        public CalendarForm()
        {

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);

            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            string currentMonthNumber = now.ToString("MM");
            string currentYearhNumber = now.ToString("yyyy");

            displayDaysAndMonth(daysInMonth, currentMonthNumber, currentYearhNumber);

            Size = new System.Drawing.Size(900, 500);

   

        }



        private void displayDates(string monthNumber, string currentYearhNumber)
        {

            int month = Int32.Parse(monthNumber);

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
            Console.WriteLine(connectionString); //Console.WriteLine("----------------------------------------------");


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                string monthString = $"{month:D2}";
                string selectQuery = $"SELECT * FROM calendardates WHERE month = '{monthString}' AND year = '{currentYearhNumber}' ORDER BY start;";

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

                                for (DateTime date = DateTime.Parse(start); date <= DateTime.Parse(end); date = date.AddDays(1)) { 

                                Label label = new Label();
                                label.BackColor = Color.Blue;

                                label.Size = new Size(50, 10);

                                int xPos = FindXFor(date);
                                int yPos = FindYFor(date);

                                label.Location = new Point(xPos, yPos);
                                Controls.Add(label);


                                }

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

        private static int FindYFor(DateTime date)
        {
            string[] arrayRowOne = { "1", "2", "3", "4", "5", "6", "7" };
            string[] arrayRowTwo = { "8", "9", "10", "11", "12", "13", "14" };
            string[] arrayRowThree = { "15", "16", "17", "18", "19", "20", "21" };
            string[] arrayRowFour = { "22", "23", "24", "25", "26", "27", "28" };
            string[] arrayRowFive = { "29", "30", "31" };

            int pos = 90;

                string day = date.Day.ToString();

                // Überprüfe für jeden Tag, zu welcher Reihe er angehört
                if (arrayRowOne.Contains(day)) { pos = 90; }
                if (arrayRowTwo.Contains(day)) { pos = 165; }
                if (arrayRowThree.Contains(day)) { pos = 240; }
                if (arrayRowFour.Contains(day)) { pos = 315; }
                if (arrayRowFive.Contains(day)) { pos = 390; }
            
            return pos;
        }


        private static int FindXFor(DateTime date)
        {
            string[] arrayRowOne = { "1", "8", "15", "22", "29" };
            string[] arrayRowTwo = { "2", "9", "16", "23", "30" };
            string[] arrayRowThree = { "3", "10", "17", "24", "31" };
            string[] arrayRowFour = { "4", "11", "18", "25" };
            string[] arrayRowFive = { "5", "12", "19", "26" };
            string[] arrayRowSix = { "6", "13", "20", "27" };
            string[] arrayRowSeven = { "7", "14", "21", "28" };

            int pos = 10;

            string day = date.Day.ToString();

                // Überprüfe für jeden Tag, zu welcher Reihe er angehört
                if (arrayRowOne.Contains(day)) { pos = 10; }
                if (arrayRowTwo.Contains(day)) { pos = 70; }
                if (arrayRowThree.Contains(day)) { pos = 130; }
                if (arrayRowFour.Contains(day)) { pos = 190; }
                if (arrayRowFive.Contains(day)) { pos = 250; }
                if (arrayRowSix.Contains(day)) { pos = 310; }
                if (arrayRowSeven.Contains(day)) { pos = 370; }

            return pos;


        }


        private void displayDays(int daysInMonth, string currentMonth, string currentYearhNumber)
        {

            // Zuerst alle vorherigen Labels entfernen
            List<Label> labelsToRemove = new List<Label>();

            foreach (Control control in Controls)
            {
                if (control is Label)
                {
                    labelsToRemove.Add((Label)control);
                }
            }

            foreach (var label in labelsToRemove)
            {
                Controls.Remove(label);
                label.Dispose(); // Ressourcen freigeben
            }


            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

            Text = rm.GetString(numberToMonth(currentMonth)) + "  " + currentYearhNumber;

            int Day = 1; int maxNumberOfDays = 0;

            Label plus = new Label();
            plus.Size = new Size(50, 50);
            plus.Location = new Point(10, 20);
            plus.Text = "+";
            plus.Click += UpdateCalendar;

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
                        //label.Click += UpdateCalendar; 
                         
                        Day++;
                        Controls.Add(label);

                        maxNumberOfDays++;

                    }
                }
                 
            }
            Controls.Add(plus);
        }

   
        private void UpdateCalendar(object sender, EventArgs e)
        {
            string senderText = "1";

            if (sender is Label label) {
                senderText = label.Text;
            }

            string[] monthYear = Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string year = "2000";

            if (monthYear.Length > 1) {
                year = monthYear[0];
            }
        
            AppointmentForm appointmentForm = new AppointmentForm(this, senderText, year);
            appointmentForm.ShowDialog();
             
        }

        private void displayDaysAndMonth(int daysInMonth, string currentMonthNumber, string currentYearhNumber)
        {
            displayDays(daysInMonth, currentMonthNumber, currentYearhNumber);
            displayDates(currentMonthNumber, currentYearhNumber);
        }

        private string numberToMonth(string currentMonth)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            return months[int.Parse(currentMonth) - 1];
        }

        public class AppointmentForm : Form
        {
            private CalendarForm calendarFormInstance;
            private TextBox editableTextBox;
            private DateTimePicker datePicker;
            private ComboBox comboBoxStartHours; ComboBox comboBoxStartMinutes;
            private ComboBox comboBoxEndHours; ComboBox comboBoxEndMinutes;

            private ResourceManager rm;

            public AppointmentForm(CalendarForm calendarForm, string dateClickedText, string currentMonth)
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

                this.calendarFormInstance = calendarForm;
                int currentYear = DateTime.Now.Year;

                string[] yearMonthDay = { currentYear.ToString(), currentMonth, dateClickedText };

                Text = rm.GetString("Year") + ": " + yearMonthDay[0] + " " + rm.GetString("Month") + ": " + yearMonthDay[1] + " " + rm.GetString("Day") + ": " + yearMonthDay[2];

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

                var date = DateTime.Now;
                int startHourIndex = date.Hour + 1;
                int endHourIndex = date.Hour + 2;

                // Startzeit auswählen
                comboBoxStartHours = new ComboBox();
                comboBoxEndHours = new ComboBox();
                setHours();
                comboBoxStartHours.SelectedIndex = startHourIndex;
                comboBoxStartHours.Location = new Point(10, 200);

                comboBoxStartMinutes = new ComboBox();
                comboBoxEndMinutes = new ComboBox();
                setMinutes();
                comboBoxStartMinutes.SelectedIndex = 0;
                comboBoxStartMinutes.Location = new Point(150, 200);


                // Endzeit auswählen
                setHours();
                comboBoxEndHours.SelectedIndex = endHourIndex;
                comboBoxEndHours.Location = new Point(10, 300);

                setMinutes();
                comboBoxEndMinutes.SelectedIndex = 0;
                comboBoxEndMinutes.Location = new Point(150, 300);

                Controls.Add(datePicker);
                Controls.Add(comboBoxStartHours);
                Controls.Add(comboBoxStartMinutes);
                Controls.Add(comboBoxEndHours);
                Controls.Add(comboBoxEndMinutes);


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

                DateTime pickedDate = datePicker.Value;
                string completeDate = pickedDate.ToString("yyyy-MM-dd");

                string pickedYear = completeDate.Split('-')[0];
                string pickedMonth = completeDate.Split('-')[1];
                string pickedDay = completeDate.Split('-')[2];

                string startHH = comboBoxStartHours.Text;
                string startMM = comboBoxStartMinutes.Text;
                string endHH = comboBoxEndHours.Text;
                string endMM = comboBoxEndMinutes.Text;

                //TODO: Hier in die DB kloppen, erst dann weiter.

                calendarFormInstance.displayDays(numberOfDaysInFollowingMonth(pickedMonth, pickedYear), pickedMonth, pickedYear);
                calendarFormInstance.displayDates(pickedMonth, pickedYear);
                
            }

            private void setHours()
            {
                for (int i = 0; i < 24; i++)
                {
                    string hour = i.ToString("00"); // Fügt eine führende Null hinzu, falls nötig
                    comboBoxStartHours.Items.Add(hour);
                    comboBoxEndHours.Items.Add(hour);
                }
            }

                private void setMinutes()
            {
                for (int i = -0; i < 61; i+=5)
                {
                    string hour = i.ToString("00"); // Fügt eine führende Null hinzu, falls nötig
                    comboBoxStartMinutes.Items.Add(hour);
                    comboBoxEndMinutes.Items.Add(hour);
                }
            }

            int numberOfDaysInFollowingMonth(string monthNumber, string year) {

                bool isLeapYear = DateTime.IsLeapYear(Int32.Parse(year));

                int feb = isLeapYear ? 29 : 28;

                IDictionary<int, int> monthsAntTheNumberOfTheirDays = new Dictionary<int, int>();
                monthsAntTheNumberOfTheirDays.Add(1, 31);
                monthsAntTheNumberOfTheirDays.Add(2, feb);
                monthsAntTheNumberOfTheirDays.Add(3, 31);
                monthsAntTheNumberOfTheirDays.Add(4, 30);
                monthsAntTheNumberOfTheirDays.Add(5, 31);
                monthsAntTheNumberOfTheirDays.Add(6, 30);
                monthsAntTheNumberOfTheirDays.Add(7, 31);
                monthsAntTheNumberOfTheirDays.Add(8, 31);
                monthsAntTheNumberOfTheirDays.Add(9, 30);
                monthsAntTheNumberOfTheirDays.Add(10, 31);
                monthsAntTheNumberOfTheirDays.Add(11, 30);
                monthsAntTheNumberOfTheirDays.Add(12, 31);

                return monthsAntTheNumberOfTheirDays[Int32.Parse(monthNumber)];
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
