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

                if (string.IsNullOrEmpty(text))
                {
                    text = rm.GetString("My Event");
                }

                string start = completeDateStrt + "T" + startHH + ":" + startMM + ":00";
                string end = pickedYearEnd + "-" + pickedMonthEnd + "-" + pickedDayEnd + "T" + endHH + ":" + endMM + ":00";
                string month = pickedMonth;
                string year = pickedYear;

                bool isStartBeforeEnd = startBeforeEnd(start, end);

                if (isStartBeforeEnd)
                {

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
                for (int i = -0; i < 61; i += 5)
                {
                    string hour = i.ToString("00"); // Fügt eine führende Null hinzu, falls nötig
                    comboBoxStartMinutes.Items.Add(hour);
                    comboBoxEndMinutes.Items.Add(hour);
                }
            }

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


        }
    }