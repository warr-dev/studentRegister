using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StudentsInformationSystem
{
    public partial class Form2 : Form
    {
        private readonly ErrorProvider errorProvider = new ErrorProvider();
        private StudentProfile _currentProfile;

        public Form2(StudentProfile profile) : this()
        {
            _currentProfile = profile;
        }

        public Form2()
        {
            InitializeComponent();
            txt_fathers_name.KeyPress += NameTextBox_KeyPress;
            txt_mothers_maiden_name.KeyPress += NameTextBox_KeyPress;
            txt_complete_name.KeyPress += NameTextBox_KeyPress;
            txt_complete_name2.KeyPress += NameTextBox_KeyPress;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            SetupComboBoxes();
            SetupAgeRanges();
            if (!LoadCurrentProfile())
            {
                MessageBox.Show("No student record found. Please complete Form 1 first.", "Missing Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_save.Enabled = false;
            }
        }

        private void SetupComboBoxes()
        {
            ComboBox[] statusBoxes = { cb_status, cb_status2, cb_status3, cb_status4 };
            ComboBox[] employmentBoxes = { cb_employment_status, cb_employment_status2, cb_employment_status3, cb_employment_status4 };

            string[] statusItems = { "Single", "Married", "Widowed", "Separated" };
            foreach (ComboBox cb in statusBoxes)
            {
                cb.Items.Clear();
                cb.Items.AddRange(statusItems);
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
            }

            string[] employmentItems = { "Student", "Employed (Full-time)", "Employed (Part-time)", "Self-Employed", "Unemployed" };
            foreach (ComboBox cb in employmentBoxes)
            {
                cb.Items.Clear();

                if (cb == cb_employment_status || cb == cb_employment_status2)
                {
                    for (int i = 1; i < employmentItems.Length; i++)
                    {
                        cb.Items.Add(employmentItems[i]);
                    }
                }
                else
                {
                    cb.Items.AddRange(employmentItems);
                }

                cb.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        private void SetupAgeRanges()
        {
            NumericUpDown[] ageBoxes = { nud_age, nud_age2, nud_age3, nud_age4 };
            foreach (NumericUpDown nud in ageBoxes)
            {
                nud.Minimum = 0;
                nud.Maximum = 120;
            }
        }

        private bool LoadCurrentProfile()
        {
            if (_currentProfile == null)
            {
                _currentProfile = DataStorage.StudentList.LastOrDefault();
            }

            if (_currentProfile == null)
            {
                return false;
            }

            if ((_currentProfile.FamilyMembers == null || _currentProfile.FamilyMembers.Count == 0) &&
                !string.IsNullOrWhiteSpace(_currentProfile.StudentNo))
            {
                _currentProfile.FamilyMembers = DataStorage.GetFamilyMembers(_currentProfile.StudentNo);
            }

            FamilyMember father = _currentProfile.FamilyMembers.FirstOrDefault(m => m.Role == "Father");
            FamilyMember mother = _currentProfile.FamilyMembers.FirstOrDefault(m => m.Role == "Mother");
            FamilyMember sibling1 = _currentProfile.FamilyMembers.FirstOrDefault(m => m.Role == "Sibling1");
            FamilyMember sibling2 = _currentProfile.FamilyMembers.FirstOrDefault(m => m.Role == "Sibling2");

            txt_fathers_name.Text = father != null ? father.Name : string.Empty;
            txt_mothers_maiden_name.Text = mother != null ? mother.Name : string.Empty;
            txt_complete_name.Text = sibling1 != null ? sibling1.Name : string.Empty;
            txt_complete_name2.Text = sibling2 != null ? sibling2.Name : string.Empty;

            if (father != null)
            {
                nud_age.Value = father.Age;
                cb_status.Text = father.Status;
                cb_employment_status.Text = father.EmploymentStatus;
            }

            if (mother != null)
            {
                nud_age2.Value = mother.Age;
                cb_status2.Text = mother.Status;
                cb_employment_status2.Text = mother.EmploymentStatus;
            }

            if (sibling1 != null)
            {
                nud_age3.Value = sibling1.Age;
                cb_status3.Text = sibling1.Status;
                cb_employment_status3.Text = sibling1.EmploymentStatus;

                if (sibling1.Relationship == "Brother") rb_brother.Checked = true;
                if (sibling1.Relationship == "Sister") rb_sister.Checked = true;
            }

            if (sibling2 != null)
            {
                nud_age4.Value = sibling2.Age;
                cb_status4.Text = sibling2.Status;
                cb_employment_status4.Text = sibling2.EmploymentStatus;

                if (sibling2.Relationship == "Brother") rb_brother2.Checked = true;
                if (sibling2.Relationship == "Sister") rb_sister2.Checked = true;
            }

            return true;
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            SaveForm2ToStore(performValidation: false);

            Form1 f1 = (Form1)Application.OpenForms["Form1"];
            if (f1 != null)
            {
                f1.Show();
                Hide();
            }
            else
            {
                Form1 newF1 = new Form1();
                newF1.Show();
                Hide();
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (!ValidateForm2())
            {
                MessageBox.Show("Please complete required family details.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SaveForm2ToStore(performValidation: false))
            {
                return;
            }

            Form3 f3 = new Form3();
            f3.Show();
            Close();
        }

        private bool ValidateForm2()
        {
            ClearErrors();
            bool isValid = true;

            isValid &= ValidateRequiredText(txt_fathers_name, "Father's name is required.");
            isValid &= ValidateProperName(txt_fathers_name, "Father's name");
            isValid &= ValidateRequiredAge(nud_age, "Father's age is required.");
            isValid &= ValidateRequiredCombo(cb_status, "Father's status is required.");
            isValid &= ValidateRequiredCombo(cb_employment_status, "Father's employment status is required.");

            isValid &= ValidateRequiredText(txt_mothers_maiden_name, "Mother's maiden name is required.");
            isValid &= ValidateProperName(txt_mothers_maiden_name, "Mother's maiden name");
            isValid &= ValidateRequiredAge(nud_age2, "Mother's age is required.");
            isValid &= ValidateRequiredCombo(cb_status2, "Mother's status is required.");
            isValid &= ValidateRequiredCombo(cb_employment_status2, "Mother's employment status is required.");

            isValid &= ValidateSiblingSection(
                txt_complete_name,
                nud_age3,
                cb_status3,
                cb_employment_status3,
                rb_brother.Checked || rb_sister.Checked,
                groupBox1,
                "Sibling #1");

            isValid &= ValidateSiblingSection(
                txt_complete_name2,
                nud_age4,
                cb_status4,
                cb_employment_status4,
                rb_brother2.Checked || rb_sister2.Checked,
                rb_brother2,
                "Sibling #2");

            return isValid;
        }

        private bool ValidateRequiredText(TextBox textBox, string message)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                return true;
            }

            errorProvider.SetError(textBox, message);
            return false;
        }

        private bool ValidateRequiredAge(NumericUpDown ageBox, string message)
        {
            if (ageBox.Value > 0)
            {
                return true;
            }

            errorProvider.SetError(ageBox, message);
            return false;
        }

        private bool ValidateRequiredCombo(ComboBox comboBox, string message)
        {
            if (comboBox.SelectedIndex >= 0)
            {
                return true;
            }

            errorProvider.SetError(comboBox, message);
            return false;
        }

        private bool ValidateProperName(TextBox textBox, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                return true;
            }

            if (IsProperName(textBox.Text))
            {
                return true;
            }

            errorProvider.SetError(textBox, fieldName + " is invalid.");
            return false;
        }

        private bool ValidateSiblingSection(
            TextBox nameBox,
            NumericUpDown ageBox,
            ComboBox statusBox,
            ComboBox employmentBox,
            bool relationshipSelected,
            Control relationshipControl,
            string siblingLabel)
        {
            bool hasAnyValue =
                !string.IsNullOrWhiteSpace(nameBox.Text) ||
                ageBox.Value > 0 ||
                statusBox.SelectedIndex >= 0 ||
                employmentBox.SelectedIndex >= 0 ||
                relationshipSelected;

            if (!hasAnyValue)
            {
                return true;
            }

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(nameBox.Text))
            {
                errorProvider.SetError(nameBox, siblingLabel + " name is required.");
                isValid = false;
            }
            else if (!IsProperName(nameBox.Text))
            {
                errorProvider.SetError(nameBox, siblingLabel + " name is invalid.");
                isValid = false;
            }

            if (ageBox.Value <= 0)
            {
                errorProvider.SetError(ageBox, siblingLabel + " age is required.");
                isValid = false;
            }

            if (statusBox.SelectedIndex < 0)
            {
                errorProvider.SetError(statusBox, siblingLabel + " status is required.");
                isValid = false;
            }

            if (employmentBox.SelectedIndex < 0)
            {
                errorProvider.SetError(employmentBox, siblingLabel + " employment status is required.");
                isValid = false;
            }

            if (!relationshipSelected)
            {
                errorProvider.SetError(relationshipControl, siblingLabel + " relationship is required.");
                isValid = false;
            }

            return isValid;
        }

        private bool SaveForm2ToStore(bool performValidation)
        {
            if (performValidation && !ValidateForm2())
            {
                return false;
            }

            if (_currentProfile == null)
            {
                _currentProfile = DataStorage.StudentList.LastOrDefault();
            }

            if (_currentProfile == null)
            {
                MessageBox.Show("Student profile not found. Please save Form 1 first.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_currentProfile.StudentNo))
            {
                MessageBox.Show("Student number is missing. Please complete Form 1 first.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _currentProfile.FamilyMembers = BuildFamilyMembers();
            DataStorage.ReplaceFamilyMembers(_currentProfile.StudentNo, _currentProfile.FamilyMembers);
            return true;
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear the form?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            txt_fathers_name.Clear();
            txt_mothers_maiden_name.Clear();
            txt_complete_name.Clear();
            txt_complete_name2.Clear();

            nud_age.Value = 0;
            nud_age2.Value = 0;
            nud_age3.Value = 0;
            nud_age4.Value = 0;

            cb_status.SelectedIndex = -1;
            cb_status2.SelectedIndex = -1;
            cb_status3.SelectedIndex = -1;
            cb_status4.SelectedIndex = -1;

            cb_employment_status.SelectedIndex = -1;
            cb_employment_status2.SelectedIndex = -1;
            cb_employment_status3.SelectedIndex = -1;
            cb_employment_status4.SelectedIndex = -1;

            rb_brother.Checked = false;
            rb_sister.Checked = false;
            rb_brother2.Checked = false;
            rb_sister2.Checked = false;

            ClearErrors();
        }

        private void ClearErrors()
        {
            foreach (Control control in panel1.Controls)
            {
                errorProvider.SetError(control, string.Empty);
            }

            errorProvider.SetError(groupBox1, string.Empty);
            errorProvider.SetError(rb_brother2, string.Empty);
        }

        private bool IsProperName(string value)
        {
            string trimmed = value.Trim();
            if (trimmed.Length < 2)
            {
                return false;
            }

            return Regex.IsMatch(trimmed, @"^[A-Za-z][A-Za-z\s'\.-]*$");
        }

        private void NameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool isAllowed = char.IsControl(e.KeyChar) ||
                             char.IsLetter(e.KeyChar) ||
                             char.IsWhiteSpace(e.KeyChar) ||
                             e.KeyChar == '\'' ||
                             e.KeyChar == '-' ||
                             e.KeyChar == '.';

            TextBox current = sender as TextBox;
            if (!isAllowed)
            {
                if (current != null)
                {
                    errorProvider.SetError(current, "Use letters/spaces only. Apostrophe, hyphen, period are allowed.");
                }

                e.Handled = true;
                return;
            }

            if (current != null)
            {
                errorProvider.SetError(current, string.Empty);
            }
        }

        private System.Collections.Generic.List<FamilyMember> BuildFamilyMembers()
        {
            System.Collections.Generic.List<FamilyMember> members = new System.Collections.Generic.List<FamilyMember>
            {
                new FamilyMember
                {
                    Role = "Father",
                    Name = txt_fathers_name.Text.Trim(),
                    Age = (int)nud_age.Value,
                    Status = cb_status.Text,
                    EmploymentStatus = cb_employment_status.Text,
                    Relationship = "Father"
                },
                new FamilyMember
                {
                    Role = "Mother",
                    Name = txt_mothers_maiden_name.Text.Trim(),
                    Age = (int)nud_age2.Value,
                    Status = cb_status2.Text,
                    EmploymentStatus = cb_employment_status2.Text,
                    Relationship = "Mother"
                }
            };

            if (!string.IsNullOrWhiteSpace(txt_complete_name.Text) ||
                nud_age3.Value > 0 ||
                cb_status3.SelectedIndex >= 0 ||
                cb_employment_status3.SelectedIndex >= 0 ||
                rb_brother.Checked ||
                rb_sister.Checked)
            {
                members.Add(new FamilyMember
                {
                    Role = "Sibling1",
                    Name = txt_complete_name.Text.Trim(),
                    Age = (int)nud_age3.Value,
                    Status = cb_status3.Text,
                    EmploymentStatus = cb_employment_status3.Text,
                    Relationship = rb_brother.Checked ? "Brother" : rb_sister.Checked ? "Sister" : string.Empty
                });
            }

            if (!string.IsNullOrWhiteSpace(txt_complete_name2.Text) ||
                nud_age4.Value > 0 ||
                cb_status4.SelectedIndex >= 0 ||
                cb_employment_status4.SelectedIndex >= 0 ||
                rb_brother2.Checked ||
                rb_sister2.Checked)
            {
                members.Add(new FamilyMember
                {
                    Role = "Sibling2",
                    Name = txt_complete_name2.Text.Trim(),
                    Age = (int)nud_age4.Value,
                    Status = cb_status4.Text,
                    EmploymentStatus = cb_employment_status4.Text,
                    Relationship = rb_brother2.Checked ? "Brother" : rb_sister2.Checked ? "Sister" : string.Empty
                });
            }

            return members;
        }

        public void FocusField(string role, string columnName)
        {
            string normalizedRole = (role ?? string.Empty).Trim();
            string normalizedColumn = (columnName ?? string.Empty).Trim();
            Control target = txt_fathers_name;

            if (normalizedRole == "Father")
            {
                target = ResolveFamilyControl(normalizedColumn, txt_fathers_name, nud_age, cb_status, cb_employment_status, rb_brother);
            }
            else if (normalizedRole == "Mother")
            {
                target = ResolveFamilyControl(normalizedColumn, txt_mothers_maiden_name, nud_age2, cb_status2, cb_employment_status2, rb_brother2);
            }
            else if (normalizedRole == "Sibling1")
            {
                target = ResolveFamilyControl(normalizedColumn, txt_complete_name, nud_age3, cb_status3, cb_employment_status3, rb_brother);
            }
            else if (normalizedRole == "Sibling2")
            {
                target = ResolveFamilyControl(normalizedColumn, txt_complete_name2, nud_age4, cb_status4, cb_employment_status4, rb_brother2);
            }
            else
            {
                target = ResolveFamilyControl(normalizedColumn, txt_fathers_name, nud_age, cb_status, cb_employment_status, rb_brother);
            }

            BeginInvoke((Action)(() => target.Focus()));
        }

        private Control ResolveFamilyControl(
            string columnName,
            Control nameControl,
            Control ageControl,
            Control statusControl,
            Control employmentControl,
            Control relationshipControl)
        {
            switch (columnName)
            {
                case "Name": return nameControl;
                case "Age": return ageControl;
                case "Status": return statusControl;
                case "EmploymentStatus": return employmentControl;
                case "Relationship": return relationshipControl;
                default: return nameControl;
            }
        }
    }
}
