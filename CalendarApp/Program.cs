using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Globalization;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalendarApp
{
    public class CalendarForm : Form
    {

        private DateTime now = DateTime.Now;

        private string fieldDayNumber; private string fieldMonthNumber; private string fieldYearNumber;
        private DataGridView dataGridView;

        public CalendarForm()
        { //TODO: Logik zum Löschen implementieren.
            

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);


            int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            string currentDayNumber = now.Day.ToString();
            string currentMonthNumber = now.ToString("MM");
            string currentYearhNumber = now.ToString("yyyy");

            Size = new System.Drawing.Size(900, 500);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Erstelle DataGridView
            dataGridView = new DataGridView();

            //dataGridView.Size = new Size(400, 400);
            dataGridView.Location = new Point(450, 20);

            dataGridView.AllowUserToAddRows = false; // Verhindert eine zusätzliche leere Zeile am Ende
            dataGridView.AllowUserToResizeRows = false; // Verhindert die Änderung der Zeilenhöhe durch den Benutzer
            dataGridView.ScrollBars = ScrollBars.Vertical; // Legt die Art der Scrollleiste fest.

            // Berechnung der Höhe basierend auf der Anzahl der Zeilen und der Höhe einer einzelnen Zeile
            int rowHeight = dataGridView.RowTemplate.Height;
            int numRowsToShow = 10; // Anzahl der Zeilen, die angezeigt werden sollen
            int totalRowHeight = numRowsToShow * rowHeight;
            dataGridView.Height = totalRowHeight + dataGridView.ColumnHeadersHeight;

            // Füge Spalten zur DataGridView hinzu
            dataGridView.Columns.Add("Date", rm.GetString("Date"));
            dataGridView.Columns.Add("Time", rm.GetString("Start") + "-" + rm.GetString("End"));

            dataGridView.Width = 425;
            dataGridView.Columns[0].Width = 180;
            dataGridView.Columns[1].Width = 220;

            displayDaysMonthAndAppointment(daysInMonth, currentDayNumber, currentMonthNumber, currentYearhNumber);


        }



        public void displayDates(string dayNumber, string monthNumber, string yearNumber)
        {
            
            
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
            //cd C:\Users\user\source\repos\calendarapp\CalendarApp\bin\Debug
            //sqlite3 calendar.db
            //.tables select *


            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int month = Int32.Parse(monthNumber);

                string monthString = $"{month:D2}";
                string selectQuery = $"SELECT * FROM calendardates WHERE month = '{monthString}' AND year = '{yearNumber}' ORDER BY start;";

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


        public void displayDays(int daysInMonth, string day, string month, string year)
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

            Text = year + " " + rm.GetString(numberToMonth(month)) + "  " + day;

            int Day = 1; int maxNumberOfDays = 0;

            Label plus = new Label();
            plus.Size = new Size(50, 50);
            plus.Location = new Point(10, 20);
            plus.Text = "+";
            plus.Click += UpdateCalendar;

            Label left = new Label();
            left.Size = new Size(20, 20);
            left.Location = new Point(80, 20);
            left.Text = "<";
            left.Click += (sender, e) =>
             {
                 Shift(year, month, "<");
             };

            Label right = new Label();
            right.Size = new Size(20, 20);
            right.Location = new Point(150, 20);
            right.Text = ">";
            right.Click += (sender, e) =>
            {
                Shift(year, month, ">");
            };
            

            Label leftBig = new Label();
            leftBig.Size = new Size(20, 20);
            leftBig.Location = new Point(57, 20);
            leftBig.Text = "<";
            leftBig.Click += (sender, e) =>
            {
                ShiftBig(year, month, "<");
            };

            Label rightBig = new Label();
            rightBig.Size = new Size(20, 20);
            rightBig.Location = new Point(173, 20);
            rightBig.Text = ">";
            rightBig.Click += (sender, e) =>
            {
                ShiftBig(year, month, ">");
            };

           

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
                        label.Text = Day.ToString() + " " + rm.GetString(dayNameForDayMonthAndYear(Day.ToString(), month, year));
                        label.Click += (sender, e) =>
                        {
                            ShowAppointments(label.Text, month, year);
                        };

                        Day++;
                        Controls.Add(label);

                        maxNumberOfDays++;

                    }
                }
                 
            }
            Controls.Add(plus);
            Controls.Add(left);
            Controls.Add(right);
            Controls.Add(leftBig);
            Controls.Add(rightBig);
        }

        private string dayNameForDayMonthAndYear(string dayString, string monthString, string yearString)
        {

                // Parse the month and year strings to integers
                int day = int.Parse(dayString);
                int month = int.Parse(monthString);
                int year = int.Parse(yearString);

                string dayName = new DateTime(year, month, day).DayOfWeek.ToString();

                string dayNameShort = dayName.Substring(0, 3);

            return dayNameShort;

            }

        //////////////////////////////////////////////

        private void ShiftBig(string year, string month, string direction)
        {


            if (direction.Equals(">"))
            {

                // Datum mit dem gegebenen Monat und dem ersten Tag des Monats erstellen
                DateTime dt = DateTime.ParseExact(year, "yyyy", CultureInfo.InvariantCulture);

                // Den Monat um eins erhöhen
                dt = dt.AddYears(1);

                // Den neuen Monat als String im Format "MM" erhalten
                string nextYear = dt.ToString("yyyy");

                year = nextYear;


            }
            else if (direction.Equals("<"))
            {

                // Datum mit dem gegebenen Monat und dem ersten Tag des Monats erstellen
                DateTime dt = DateTime.ParseExact(year, "yyyy", CultureInfo.InvariantCulture);

                // Den Monat um eins erhöhen
                dt = dt.AddYears(-1);

                // Den neuen Monat als String im Format "MM" erhalten
                string nextYear = dt.ToString("yyyy");

                year = nextYear;
                
                DateTime date = new DateTime(int.Parse(year), 1, 1);

                // Ein Jahr zum Datum hinzufügen
                date = date.AddYears(-1);

                // Datum in eine Zeichenfolge im gewünschten Format konvertieren
                string incrementedYear = date.ToString("yyyy");

                year = incrementedYear;
                 

            }

            int daysInMonth = numberOfDaysInThisMonthAndYear(month, year);

            DateTime now = DateTime.Now;
            string currentDay = now.ToString("dd");

            string d = isCurrentMonth(month) ? currentDay : "01";

            displayDays(daysInMonth, d, month, year);
            displayDates(d, month, year);

        }
        //////////////////////////////////////////////


        private void Shift(string year, string month, string direction)
        {
            
            
            if (direction.Equals(">")) {
                
                // Datum mit dem gegebenen Monat und dem ersten Tag des Monats erstellen
                DateTime dt = DateTime.ParseExact(month, "MM", CultureInfo.InvariantCulture);

                // Den Monat um eins erhöhen
                dt = dt.AddMonths(1);

                // Den neuen Monat als String im Format "MM" erhalten
                string nextMonth = dt.ToString("MM");

                month = nextMonth;

                if (month.Equals("01")) {

                    DateTime date = new DateTime(int.Parse(year), 1, 1);

                    // Ein Jahr zum Datum hinzufügen
                    date = date.AddYears(1);

                    // Datum in eine Zeichenfolge im gewünschten Format konvertieren
                    string incrementedYear = date.ToString("yyyy");

                    year = incrementedYear;
                }


            }
            else if (direction.Equals("<")) {

                // Datum mit dem gegebenen Monat und dem ersten Tag des Monats erstellen
                DateTime dt = DateTime.ParseExact(month, "MM", CultureInfo.InvariantCulture);

                // Den Monat um eins erhöhen
                dt = dt.AddMonths(-1);

                // Den neuen Monat als String im Format "MM" erhalten
                string nextMonth = dt.ToString("MM");

                month = nextMonth;

                if (month.Equals("12"))
                {

                    DateTime date = new DateTime(int.Parse(year), 1, 1);

                    // Ein Jahr zum Datum hinzufügen
                    date = date.AddYears(-1);

                    // Datum in eine Zeichenfolge im gewünschten Format konvertieren
                    string incrementedYear = date.ToString("yyyy");

                    year = incrementedYear;
                }

            }

            int daysInMonth = numberOfDaysInThisMonthAndYear(month, year);

            DateTime now = DateTime.Now;
            string currentDay = now.ToString("dd");

            string d = isCurrentMonth(month) ? currentDay : "01";

            displayDays(daysInMonth, d, month, year);
            displayDates(d, month, year);

        }

        private bool isCurrentMonth(string monthToCheck)
        {

            // Wandele den String in einen Integer um
            int gesuchterMonat = Convert.ToInt32(monthToCheck);

            // Erstelle ein DateTime-Objekt für den aktuellen Monat
            DateTime jetzt = DateTime.Now;

            // Überprüfe, ob die gegebene Zahl dem aktuellen Monat entspricht
            if (gesuchterMonat == jetzt.Month)
            {
                return true;
            }
            return false;
        }

        //Verstoß gegen das DRY-Prinzip ((
        int numberOfDaysInThisMonthAndYear(string monthNumber, string year)
        {

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




        //Es werden für den aktuell angezeigten Monat innerhalb des aktuell angezeigten Jahres die Termine an einem Tag angezeigt. 
        private void ShowAppointments(string day, string month, string year)
        {
            day = Regex.Split(day, @"\s+")[0]; //Mich interessiert nur die Zahl. Schade, dass alles so hart koiert ist :\
            
            dataGridView.Rows.Clear();

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);

            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int intMonth = Int32.Parse(month);

                string monthString = $"{intMonth:D2}";
                string selectQuery = $"SELECT * FROM calendardates WHERE month = '{monthString}' AND year = '{year}' ORDER BY start;";
                try
                {

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        { 
                            while (reader.Read())
                            {
                                
                                string text = reader.GetString(1);
                                string start = reader.GetString(2);
                                string end = reader.GetString(3);

                                bool isBetween = DatumPruefen(year + "-" + month + "-" + day, start.Split('T')[0], end.Split('T')[0]);
                                
                                if (isBetween) {
                                   dataGridView.Rows.Add(text, start.Split('T')[0] + ", " + start.Split('T')[1].Substring(0, 5) + " " + end.Split('T')[0] + ", " + end.Split('T')[1].Substring(0, 5));
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

         
            // Füge DataGridView zum Formular hinzu
            Controls.Add(dataGridView);

        }

        private bool DatumPruefen(string datumYMD, string start, string end)
        {
            DateTime datum = DateTime.ParseExact(datumYMD, "yyyy-MM-dd", null);
            DateTime startDatum = DateTime.ParseExact(start, "yyyy-MM-dd", null);
            DateTime endDatum = DateTime.ParseExact(end, "yyyy-MM-dd", null);

            return startDatum <= datum && datum <= endDatum;
        }

        private void UpdateCalendar(object sender, EventArgs e)
        {   
            AppointmentForm appointmentForm = new AppointmentForm(this, fieldDayNumber, fieldMonthNumber, fieldYearNumber);
        
            appointmentForm.ShowDialog();
             
        }

        private void displayDaysMonthAndAppointment(int daysInMonth, string currentDayNumber, string currentMonthNumber, string currentYearNumber)
        {

 

            dataGridView.Rows.Clear();

            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var rm = new ResourceManager("CalendarApp.i18n.resources", typeof(CalendarForm).Assembly);

            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                int intMonth = Int32.Parse(currentMonthNumber);

                string monthString = $"{intMonth:D2}";
                string selectQuery = $"SELECT * FROM calendardates WHERE month = '{monthString}' AND year = '{currentYearNumber}' ORDER BY start;";
                try
                {

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                string text = reader.GetString(1);
                                string start = reader.GetString(2);
                                string end = reader.GetString(3);

                                bool isBetween = DatumPruefen(currentYearNumber + "-" + currentMonthNumber + "-" + currentDayNumber, start.Split('T')[0], end.Split('T')[0]);

                                if (isBetween)
                                {
                                    dataGridView.Rows.Add(text, start.Split('T')[0] + ", " + start.Split('T')[1].Substring(0, 5) + " " + end.Split('T')[0] + ", " + end.Split('T')[1].Substring(0, 5));
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



            // Füge DataGridView zum Formular hinzu
            Controls.Add(dataGridView);
            displayDays(daysInMonth, currentDayNumber, currentMonthNumber, currentYearNumber);
            displayDates(currentDayNumber, currentMonthNumber, currentYearNumber);
        }



        private string numberToMonth(string currentMonth)
        {
            string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            return months[int.Parse(currentMonth) - 1];
        }


        /*
        public class AppointmentForm : Form
        {
            private CalendarForm calendarFormInstance;
            private TextBox editableTextBox;
            private DateTimePicker datePickerStrt; private DateTimePicker datePickerEnd;
            private ComboBox comboBoxStartHours; ComboBox comboBoxStartMinutes;
            private ComboBox comboBoxEndHours; ComboBox comboBoxEndMinutes;

            private ResourceManager rm;
            
            public AppointmentForm(CalendarForm calendarForm, string dayNumber, string monthNumber, string yearNumber)
            {
                this.FormBorderStyle = FormBorderStyle.FixedDialog;

                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                rm = new ResourceManager("CalendarApp.i18n.resources", typeof(AppointmentForm).Assembly);

                this.calendarFormInstance = calendarForm;
                int currentYear = DateTime.Now.Year;
               
                string[] yearMonthDay = { yearNumber, monthNumber, dayNumber };

                Text = rm.GetString("Year") + ": " + yearMonthDay[0] + " " + rm.GetString("Month") + ": " + yearMonthDay[1] + " " + rm.GetString("Day") + ": " + yearMonthDay[2];

                //Text festlegen.
                editableTextBox = new TextBox
                {
                    AcceptsReturn = false,
                    AcceptsTab = false,
                    //Dock = System.Windows.Forms.DockStyle.Fill,
                    Multiline = false,
                    //ScrollBars = System.Windows.Forms.ScrollBars.Vertical
                    Width = 300
                };

                // Hier die Höhe anpassen, z.B. dreimal so hoch wie zuvor
                editableTextBox.Height = 3 * editableTextBox.Font.Height;


                // Start Datum auswählen
                datePickerStrt = new DateTimePicker();
                datePickerStrt.Format = DateTimePickerFormat.Short;
                datePickerStrt.Location = new Point(10, 150);

                // End Datum auswählen
                datePickerEnd = new DateTimePicker();
                datePickerEnd.Format = DateTimePickerFormat.Short;
                datePickerEnd.Location = new Point(10, 250);

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

                Controls.Add(datePickerStrt);
                Controls.Add(datePickerEnd);
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
                DateTime pickedDateStrt = datePickerStrt.Value;
                string completeDateStrt = pickedDateStrt.ToString("yyyy-MM-dd");

                DateTime pickedDateEnd = datePickerEnd.Value;
                string completeDateEnd = pickedDateEnd.ToString("yyyy-MM-dd");

                string pickedYear = completeDateStrt.Split('-')[0];
                string pickedMonth = completeDateStrt.Split('-')[1];
                string pickedDay = completeDateStrt.Split('-')[2];

                string pickedYearEnd = completeDateEnd.Split('-')[0];
                string pickedMonthEnd = completeDateEnd.Split('-')[1];
                string pickedDayEnd = completeDateEnd.Split('-')[2];

                string startHH = comboBoxStartHours.Text;
                string startMM = comboBoxStartMinutes.Text;
                string endHH = comboBoxEndHours.Text;
                string endMM = comboBoxEndMinutes.Text;

                string text = editableTextBox.Text;

                if (string.IsNullOrEmpty(text)) {
                    text = rm.GetString("My Event");
                }

                string start = completeDateStrt + "T" + startHH + ":" + startMM + ":00";
                string end = pickedYearEnd + "-" + pickedMonthEnd + "-" + pickedDayEnd + "T" + endHH + ":" + endMM + ":00";
                string month = pickedMonth;
                string year = pickedYear;

                bool isStartBeforeEnd = startBeforeEnd(start, end);

                if (isStartBeforeEnd) { 

                string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + "\\calendar.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO calendardates (text, start, end, month, year) VALUES (@Text, @Start, @End, @Month, @Year)";

                    try
                    {
                         
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Text", text);
                            command.Parameters.AddWithValue("@Start", start);
                            command.Parameters.AddWithValue("@End", end);
                            command.Parameters.AddWithValue("@Month", month);
                            command.Parameters.AddWithValue("@Year", year);

                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine("Eingefügte Datensätze: " + rowsAffected);
                        }
                         

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Verbinden zur Datenbank: {ex.Message}");
                    }

                }

                }

               calendarFormInstance.displayDays(numberOfDaysInThisMonthAndYear(pickedMonth, pickedYear), pickedDay, pickedMonth, pickedYear);
               calendarFormInstance.displayDates(pickedDay, pickedMonth, pickedYear);

                this.Close();
            }

            private bool startBeforeEnd(string start, string end)
            {
                // DateTime-Objekte aus den Zeichenfolgen erstellen
                DateTime datumA = DateTime.Parse(start);
                DateTime datumB = DateTime.Parse(end);

                // Prüfen, ob Datum A vor Datum B liegt
                if (datumA < datumB)
                {
                    return true;
                }
                else if (datumA > datumB)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                
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

            int numberOfDaysInThisMonthAndYear(string monthNumber, string year) {

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
        */

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalendarForm());
        }
    }
}
