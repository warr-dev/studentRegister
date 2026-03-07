using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentsInformationSystem
{
    public partial class Form3 : Form
    {
        private string _activeGrid = "Students";
        private string _activeColumn = "StudentName";
        private string _activeFamilyRole = string.Empty;

        public Form3()
        {
            InitializeComponent();
            Students_Information.SelectionChanged += Students_Information_SelectionChanged;
            Students_Information.CellClick += Students_Information_CellContentClick;
            Family_Background.CellClick += Family_Background_CellContentClick;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            RefreshSummaryView();
        }

        private void RefreshSummaryView()
        {
            Students_Information.AutoGenerateColumns = true;
            Family_Background.AutoGenerateColumns = true;

            Students_Information.DataSource = null;
            Students_Information.DataSource = DataStorage.StudentsTable;

            Family_Background.DataSource = null;
            Family_Background.DataSource = DataStorage.FamilyMembersTable;

            Students_Information.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            Family_Background.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            if (Students_Information.Columns.Count > 0)
            {
                if (Students_Information.Columns.Contains("StudentNumber")) Students_Information.Columns["StudentNumber"].HeaderText = "Student No";
                if (Students_Information.Columns.Contains("StudentName")) Students_Information.Columns["StudentName"].HeaderText = "Name";
            }

            if (Family_Background.Columns.Count > 0)
            {
                if (Family_Background.Columns.Contains("StudentNo")) Family_Background.Columns["StudentNo"].Visible = false;
                if (Family_Background.Columns.Contains("Role")) Family_Background.Columns["Role"].Visible = false;
                if (Family_Background.Columns.Contains("Relationship")) Family_Background.Columns["Relationship"].HeaderText = "Relationship";
            }

            if (Students_Information.Rows.Count > 0)
            {
                Students_Information.ClearSelection();
                Students_Information.Rows[0].Selected = true;
                LoadPhotoForSelectedStudent();
            }
            else
            {
                pb_savephoto.Image = null;
            }
        }

        private void Students_Information_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            _activeGrid = "Students";
            _activeColumn = Students_Information.Columns[e.ColumnIndex].Name;
            _activeFamilyRole = string.Empty;
        }

        private void Family_Background_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            _activeGrid = "Family";
            _activeColumn = Family_Background.Columns[e.ColumnIndex].Name;

            DataGridViewRow row = Family_Background.Rows[e.RowIndex];
            if (row.Cells["Role"] != null)
            {
                _activeFamilyRole = Convert.ToString(row.Cells["Role"].Value);
            }
            else if (row.Cells["Relationship"] != null)
            {
                _activeFamilyRole = NormalizeRoleFromRelationship(Convert.ToString(row.Cells["Relationship"].Value));
            }
        }

        private void pb_savephoto_Click(object sender, EventArgs e)
        {
        }

        private void btn_Delete(object sender, EventArgs e)
        {
            string studentNo = GetSelectedStudentNo();
            if (!string.IsNullOrWhiteSpace(studentNo))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this record?",
                                                      "Confirm Delete",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DataStorage.RemoveStudent(studentNo);

                    pb_savephoto.Image = null;
                    RefreshSummaryView();

                    MessageBox.Show("Record successfully deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void btn_back_Click(object sender, EventArgs e)
        {
            Form1 newF1 = new Form1();
            newF1.Show();
            Hide();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            string studentNo = GetSelectedStudentNo();
            if (string.IsNullOrWhiteSpace(studentNo))
            {
                MessageBox.Show("Please select a student row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StudentProfile profile = DataStorage.StudentList.FirstOrDefault(s => s.StudentNo == studentNo);
            if (profile == null)
            {
                MessageBox.Show("Student profile not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_activeGrid == "Family")
            {
                Form2 form2 = new Form2(profile);
                form2.Show();
                form2.FocusField(_activeFamilyRole, _activeColumn);
                Hide();
                return;
            }

            Form1 form1 = new Form1(profile);
            form1.Show();
            form1.FocusField(_activeColumn);
            Hide();
        }

        private void Students_Information_SelectionChanged(object sender, EventArgs e)
        {
            LoadPhotoForSelectedStudent();
        }

        private void LoadPhotoForSelectedStudent()
        {
            if (Students_Information.CurrentRow == null || Students_Information.CurrentRow.Cells["StudentNumber"] == null)
            {
                pb_savephoto.Image = null;
                ApplyFamilyFilter(string.Empty);
                return;
            }

            string studentNo = Convert.ToString(Students_Information.CurrentRow.Cells["StudentNumber"].Value);
            ApplyFamilyFilter(studentNo);
            StudentProfile selectedProfile = DataStorage.StudentList.FirstOrDefault(s => s.StudentNo == studentNo);

            if (selectedProfile != null && selectedProfile.Photo != null)
            {
                pb_savephoto.Image = selectedProfile.Photo;
                pb_savephoto.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pb_savephoto.Image = null;
            }
        }

        private void ApplyFamilyFilter(string studentNo)
        {
            DataView view = DataStorage.FamilyMembersTable.DefaultView;
            if (string.IsNullOrWhiteSpace(studentNo))
            {
                view.RowFilter = string.Empty;
            }
            else
            {
                view.RowFilter = "StudentNo = '" + studentNo.Replace("'", "''") + "'";
            }
        }

        private string GetSelectedStudentNo()
        {
            if (Students_Information.CurrentRow != null && Students_Information.CurrentRow.Cells["StudentNumber"] != null)
            {
                return Convert.ToString(Students_Information.CurrentRow.Cells["StudentNumber"].Value);
            }

            if (Family_Background.CurrentRow != null && Family_Background.CurrentRow.Cells["StudentNo"] != null)
            {
                return Convert.ToString(Family_Background.CurrentRow.Cells["StudentNo"].Value);
            }

            return string.Empty;
        }

        private string NormalizeRoleFromRelationship(string relationship)
        {
            switch ((relationship ?? string.Empty).Trim())
            {
                case "Father": return "Father";
                case "Mother": return "Mother";
                case "Brother":
                case "Sister":
                    return "Sibling1";
                default:
                    return string.Empty;
            }
        }
    }
}
