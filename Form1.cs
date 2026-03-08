using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentsInformationSystem
{
    public partial class Form1 : Form
    {
        ErrorProvider errorProvider = new ErrorProvider();
        private StudentProfile _currentProfile;
        private bool _isEditMode;
        private string _originalStudentNo = string.Empty;
        public Form1()
        {
            InitializeComponent();
            dtp_birthday.Format = DateTimePickerFormat.Short;
            txt_name_of_student.KeyPress += txt_name_of_student_KeyPress;
            txt_student_number.KeyPress += txt_student_number_KeyPress;
            txt_email.KeyPress += txt_email_KeyPress;
            txt_email.TextChanged += txt_email_TextChanged;
            txt_phone_number.KeyPress += txt_phone_number_KeyPress;
            btn_next.Click += btn_next_Click;
            btn_Clear.Click += btn_Clear_Click;
            btn_upload_photo.Click += btn_upload_photo_Click;
            p_b_student.SizeMode = PictureBoxSizeMode.Zoom;
            setInitialData();
        }

        public Form1(StudentProfile profile) : this()
        {
            _currentProfile = profile;
            _isEditMode = profile != null;
            _originalStudentNo = profile != null ? profile.StudentNo : string.Empty;
            LoadProfile(profile);
        }

        private void processAgeDate()
        {
            DateTime today = DateTime.Today;

            int computedAge = today.Year - dtp_birthday.Value.Year;
            if (dtp_birthday.Value.Date > today.AddYears(-computedAge))
            {
                computedAge--;
            }
            nud_age.Value = computedAge;
        }

        private void setInitialData()
        {
            dtp_birthday.Value = DateTime.Now.AddYears(-15);
            processAgeDate();

            string[] statusItems = { "Single", "Married", "Widowed", "Separated" };
            cb_status.Items.Clear();
            cb_status.Items.AddRange(statusItems);

            string[] yearItems = { "1st Year", "2nd Year", "3rd Year", "4th Year" };
            cb_year.Items.Clear();
            cb_year.Items.AddRange(yearItems);
        }

        private void nud_age_ValueChanged(object sender, EventArgs e)
        {
            int age = (int)nud_age.Value;
            dtp_birthday.Value = DateTime.Today.AddYears(-age);
        }

        private void dtp_birthday_ValueChanged(object sender, EventArgs e)
        {
            processAgeDate();
        }

        private void txt_name_of_student_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isAllowed = char.IsControl(e.KeyChar) || char.IsLetter(e.KeyChar) || char.IsWhiteSpace(e.KeyChar);
            if (!isAllowed)
            {
                errorProvider.SetError(txt_name_of_student, "Name allows letters and spaces only.");
                return;
            }

            errorProvider.SetError(txt_name_of_student, string.Empty);
        }

        private void txt_student_number_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isAllowed = char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar);
            if (!isAllowed)
            {
                errorProvider.SetError(txt_student_number, "Student number allows digits only.");
                return;
            }

            errorProvider.SetError(txt_student_number, string.Empty);
        }

        private void txt_email_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            bool isAllowedChar = char.IsLetterOrDigit(e.KeyChar) ||
                                 e.KeyChar == '@' ||
                                 e.KeyChar == '.' ||
                                 e.KeyChar == '_' ||
                                 e.KeyChar == '-' ||
                                 e.KeyChar == '+';

            if (!isAllowedChar)
            {
                errorProvider.SetError(txt_email, "Invalid character for email.");
                return;
            }

            if (e.KeyChar == '@' && txt_email.Text.Contains("@"))
            {
                errorProvider.SetError(txt_email, "Email can contain only one @ symbol.");
                return;
            }

            errorProvider.SetError(txt_email, string.Empty);
        }

        private void txt_email_TextChanged(object sender, EventArgs e)
        {
            string value = txt_email.Text.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                errorProvider.SetError(txt_email, "Email is required.");
                return;
            }

            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorProvider.SetError(txt_email, "Current email value is invalid.");
                return;
            }

            errorProvider.SetError(txt_email, string.Empty);
        }

        private void txt_phone_number_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                errorProvider.SetError(txt_phone_number, string.Empty);
                return;
            }

            bool isDigit = char.IsDigit(e.KeyChar);
            bool isPlus = e.KeyChar == '+';

            if (!isDigit && !isPlus)
            {
                errorProvider.SetError(txt_phone_number, "Phone allows digits and optional leading + only.");
                return;
            }

            if (isPlus)
            {
                bool isAtStart = txt_phone_number.SelectionStart == 0;
                bool alreadyHasPlus = txt_phone_number.Text.Contains("+");
                if (!isAtStart || alreadyHasPlus)
                {
                    errorProvider.SetError(txt_phone_number, "Plus sign is allowed only at the start.");
                    return;
                }
            }

            errorProvider.SetError(txt_phone_number, string.Empty);
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
             if (!ValidateForm1())
            {
                MessageBox.Show("Please complete all required fields with valid values.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveCurrentStudentToTable();
            Form2 form2 = new Form2(_currentProfile ?? DataStorage.StudentList.LastOrDefault());
            form2.Show();
            this.Hide();
        }

        private bool ValidateForm1()
        {
            ClearErrors();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txt_name_of_student.Text))
            {
                errorProvider.SetError(txt_name_of_student, "Name is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txt_student_number.Text))
            {
                errorProvider.SetError(txt_student_number, "Student number is required.");
                isValid = false;
            }

            if (!rbtn_female.Checked && !rbtn_male.Checked)
            {
                errorProvider.SetError(rbtn_male, "Sex is required.");
                isValid = false;
            }

            if (nud_age.Value <= 0)
            {
                errorProvider.SetError(nud_age, "Age must be greater than 0.");
                isValid = false;
            }

            if (nud_age.Value < 15 || nud_age.Value > 50)
            {
                errorProvider.SetError(nud_age, "Age must beetween 15 and 50.");
                isValid = false;
            }

            if (dtp_birthday.Value.Date > DateTime.Today)
            {
                errorProvider.SetError(dtp_birthday, "Birthday cannot be in the future.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(cb_status.Text))
            {
                errorProvider.SetError(cb_status, "Status is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(cb_year.Text))
            {
                errorProvider.SetError(cb_year, "Year is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txt_address_city.Text))
            {
                errorProvider.SetError(txt_address_city, "Address/City is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txt_email.Text))
            {
                errorProvider.SetError(txt_email, "Email is required.");
                isValid = false;
            }
            else if (!Regex.IsMatch(txt_email.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorProvider.SetError(txt_email, "Invalid email format.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txt_phone_number.Text))
            {
                errorProvider.SetError(txt_phone_number, "Phone number is required.");
                isValid = false;
            }
            else if (!Regex.IsMatch(txt_phone_number.Text.Trim(), @"^\+?\d{7,15}$"))
            {
                errorProvider.SetError(txt_phone_number, "Invalid phone format (digits only, optional +, 7-15 digits).");
                isValid = false;
            }

            if (!IsAnySkillSelected())
            {
                errorProvider.SetError(c_b_phyton, "Select at least one skill.");
                isValid = false;
            }

            return isValid;
        }

        private bool IsAnySkillSelected()
        {
            return c_b_phyton.Checked ||
                   c_b_java.Checked ||
                   c_b_cplus.Checked ||
                   c_b_kotlin.Checked ||
                   c_b_csharp.Checked ||
                   c_b_javascript.Checked ||
                   c_b_sql.Checked ||
                   c_b_rust.Checked ||
                   c_b_swift.Checked ||
                   c_b_go.Checked;
        }

        private void ClearErrors()
        {
            foreach (Control control in panel1.Controls)
            {
                errorProvider.SetError(control, string.Empty);
            }

            errorProvider.SetError(rbtn_male, string.Empty);
            errorProvider.SetError(rbtn_female, string.Empty);
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            txt_name_of_student.Clear();
            txt_student_number.Clear();
            rbtn_female.Checked = false;
            rbtn_male.Checked = false;

            dtp_birthday.Value = DateTime.Today.AddYears(-15);
            processAgeDate();

            cb_status.SelectedIndex = -1;
            cb_status.Text = string.Empty;
            cb_year.SelectedIndex = -1;
            cb_year.Text = string.Empty;

            txt_address_city.Clear();
            txt_email.Clear();
            txt_phone_number.Clear();

            c_b_phyton.Checked = false;
            c_b_java.Checked = false;
            c_b_cplus.Checked = false;
            c_b_kotlin.Checked = false;
            c_b_csharp.Checked = false;
            c_b_javascript.Checked = false;
            c_b_sql.Checked = false;
            c_b_rust.Checked = false;
            c_b_swift.Checked = false;
            c_b_go.Checked = false;

            p_b_student.Image = null;
            ClearErrors();
        }

        private void btn_upload_photo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select Student Photo";
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                dialog.Multiselect = false;

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    Image selectedImage = Image.FromFile(dialog.FileName);
                    if (p_b_student.Image != null)
                    {
                        p_b_student.Image.Dispose();
                    }
                    p_b_student.Image = selectedImage;
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to load selected image.", "Upload Photo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveCurrentStudentToTable()
        {
            string studentNo = txt_student_number.Text.Trim();
            string studentName = txt_name_of_student.Text.Trim();
            string selectedSex = rbtn_female.Checked ? "Female" : "Male";
            int age = (int)nud_age.Value;
            DateTime birthday = dtp_birthday.Value.Date;
            string status = cb_status.Text.Trim();
            string year = cb_year.Text.Trim();
            string address = txt_address_city.Text.Trim();
            string email = txt_email.Text.Trim();
            string phone = txt_phone_number.Text.Trim();
            string selectedSkills = string.Join(", ", GetSelectedSkills());

            if (_isEditMode && !string.IsNullOrWhiteSpace(_originalStudentNo))
            {
                _currentProfile = DataStorage.UpdateStudentForm1(
                    _originalStudentNo,
                    studentNo,
                    studentName,
                    selectedSex,
                    age,
                    birthday,
                    status,
                    year,
                    address,
                    email,
                    phone,
                    selectedSkills,
                    p_b_student.Image
                );
                _originalStudentNo = _currentProfile != null ? _currentProfile.StudentNo : _originalStudentNo;
            }
            else
            {
                _currentProfile = DataStorage.AddStudentForm1(
                    studentNo,
                    studentName,
                    selectedSex,
                    age,
                    birthday,
                    status,
                    year,
                    address,
                    email,
                    phone,
                    selectedSkills,
                    p_b_student.Image
                );
                _isEditMode = true;
                _originalStudentNo = _currentProfile != null ? _currentProfile.StudentNo : studentNo;
            }
        }

        private List<string> GetSelectedSkills()
        {
            List<string> skills = new List<string>();

            if (c_b_phyton.Checked) skills.Add(c_b_phyton.Text);
            if (c_b_java.Checked) skills.Add(c_b_java.Text);
            if (c_b_cplus.Checked) skills.Add(c_b_cplus.Text);
            if (c_b_kotlin.Checked) skills.Add(c_b_kotlin.Text);
            if (c_b_csharp.Checked) skills.Add(c_b_csharp.Text);
            if (c_b_javascript.Checked) skills.Add(c_b_javascript.Text);
            if (c_b_sql.Checked) skills.Add(c_b_sql.Text);
            if (c_b_rust.Checked) skills.Add(c_b_rust.Text);
            if (c_b_swift.Checked) skills.Add(c_b_swift.Text);
            if (c_b_go.Checked) skills.Add(c_b_go.Text);

            return skills;
        }

        private void LoadProfile(StudentProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            txt_name_of_student.Text = profile.Name ?? string.Empty;
            txt_student_number.Text = profile.StudentNo ?? string.Empty;
            rbtn_female.Checked = string.Equals(profile.Sex, "Female", StringComparison.OrdinalIgnoreCase);
            rbtn_male.Checked = string.Equals(profile.Sex, "Male", StringComparison.OrdinalIgnoreCase);
            nud_age.Value = Math.Max(nud_age.Minimum, Math.Min(nud_age.Maximum, profile.Age));

            if (DateTime.TryParse(profile.Birthday, out DateTime parsedBirthday))
            {
                dtp_birthday.Value = parsedBirthday;
            }

            cb_status.Text = profile.Status ?? string.Empty;
            cb_year.Text = profile.Year ?? string.Empty;
            txt_address_city.Text = profile.Address ?? string.Empty;
            txt_email.Text = profile.Email ?? string.Empty;
            txt_phone_number.Text = profile.Phone ?? string.Empty;
            p_b_student.Image = profile.Photo;

            c_b_phyton.Checked = false;
            c_b_java.Checked = false;
            c_b_cplus.Checked = false;
            c_b_kotlin.Checked = false;
            c_b_csharp.Checked = false;
            c_b_javascript.Checked = false;
            c_b_sql.Checked = false;
            c_b_rust.Checked = false;
            c_b_swift.Checked = false;
            c_b_go.Checked = false;

            string[] skills = (profile.Skills ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            foreach (string skill in skills)
            {
                if (string.Equals(skill, c_b_phyton.Text, StringComparison.OrdinalIgnoreCase)) c_b_phyton.Checked = true;
                if (string.Equals(skill, c_b_java.Text, StringComparison.OrdinalIgnoreCase)) c_b_java.Checked = true;
                if (string.Equals(skill, c_b_cplus.Text, StringComparison.OrdinalIgnoreCase)) c_b_cplus.Checked = true;
                if (string.Equals(skill, c_b_kotlin.Text, StringComparison.OrdinalIgnoreCase)) c_b_kotlin.Checked = true;
                if (string.Equals(skill, c_b_csharp.Text, StringComparison.OrdinalIgnoreCase)) c_b_csharp.Checked = true;
                if (string.Equals(skill, c_b_javascript.Text, StringComparison.OrdinalIgnoreCase)) c_b_javascript.Checked = true;
                if (string.Equals(skill, c_b_sql.Text, StringComparison.OrdinalIgnoreCase)) c_b_sql.Checked = true;
                if (string.Equals(skill, c_b_rust.Text, StringComparison.OrdinalIgnoreCase)) c_b_rust.Checked = true;
                if (string.Equals(skill, c_b_swift.Text, StringComparison.OrdinalIgnoreCase)) c_b_swift.Checked = true;
                if (string.Equals(skill, c_b_go.Text, StringComparison.OrdinalIgnoreCase)) c_b_go.Checked = true;
            }
        }

        public void FocusField(string columnName)
        {
            Control target = txt_name_of_student;

            switch ((columnName ?? string.Empty).Trim())
            {
                case "StudentNumber": target = txt_student_number; break;
                case "StudentName": target = txt_name_of_student; break;
                case "Sex": target = rbtn_female; break;
                case "Age": target = nud_age; break;
                case "Birthday": target = dtp_birthday; break;
                case "Status": target = cb_status; break;
                case "Year": target = cb_year; break;
                case "AddressCity": target = txt_address_city; break;
                case "Email": target = txt_email; break;
                case "PhoneNumber": target = txt_phone_number; break;
                case "Skills": target = c_b_phyton; break;
            }

            BeginInvoke((Action)(() => target.Focus()));
        }

    }
}
