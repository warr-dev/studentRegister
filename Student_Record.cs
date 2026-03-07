using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace StudentsInformationSystem
{
    public class FamilyMember
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Status { get; set; }
        public string EmploymentStatus { get; set; }
        public string Relationship { get; set; }
    }

    public static class DataStorage
    {
        public static readonly List<StudentProfile> StudentList = new List<StudentProfile>();
        public static readonly DataTable StudentsTable = CreateStudentsTable();
        public static readonly DataTable FamilyMembersTable = CreateFamilyMembersTable();

        private static DataTable CreateStudentsTable()
        {
            DataTable table = new DataTable("Students");
            table.Columns.Add("StudentNumber", typeof(string));
            table.Columns.Add("StudentName", typeof(string));
            table.Columns.Add("Sex", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Birthday", typeof(System.DateTime));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Year", typeof(string));
            table.Columns.Add("AddressCity", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("PhoneNumber", typeof(string));
            table.Columns.Add("Skills", typeof(string));
            return table;
        }

        private static DataTable CreateFamilyMembersTable()
        {
            DataTable table = new DataTable("FamilyMembers");
            table.Columns.Add("StudentNo", typeof(string));
            table.Columns.Add("Role", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("EmploymentStatus", typeof(string));
            table.Columns.Add("Relationship", typeof(string));
            return table;
        }

        public static void AddStudentForm1(
            string studentNumber,
            string studentName,
            string sex,
            int age,
            System.DateTime birthday,
            string status,
            string year,
            string addressCity,
            string email,
            string phoneNumber,
            string skills,
            Image photo)
        {
            StudentsTable.Rows.Add(
                studentNumber,
                studentName,
                sex,
                age,
                birthday,
                status,
                year,
                addressCity,
                email,
                phoneNumber,
                skills
            );

            StudentList.Add(new StudentProfile
            {
                StudentNo = studentNumber,
                Name = studentName,
                Sex = sex,
                Age = age,
                Birthday = birthday.ToString("yyyy-MM-dd"),
                Status = status,
                Year = year,
                Address = addressCity,
                Email = email,
                Phone = phoneNumber,
                Skills = skills,
                Photo = photo
            });
        }

        public static void ReplaceFamilyMembers(string studentNo, List<FamilyMember> members)
        {
            if (string.IsNullOrWhiteSpace(studentNo))
            {
                return;
            }

            DataRow[] existingRows = FamilyMembersTable.Select("StudentNo = '" + studentNo.Replace("'", "''") + "'");
            foreach (DataRow row in existingRows)
            {
                FamilyMembersTable.Rows.Remove(row);
            }

            foreach (FamilyMember member in members)
            {
                FamilyMembersTable.Rows.Add(
                    studentNo,
                    member.Role ?? string.Empty,
                    member.Name ?? string.Empty,
                    member.Age,
                    member.Status ?? string.Empty,
                    member.EmploymentStatus ?? string.Empty,
                    member.Relationship ?? string.Empty
                );
            }
        }

        public static List<FamilyMember> GetFamilyMembers(string studentNo)
        {
            if (string.IsNullOrWhiteSpace(studentNo))
            {
                return new List<FamilyMember>();
            }

            return FamilyMembersTable
                .AsEnumerable()
                .Where(r => string.Equals(r.Field<string>("StudentNo"), studentNo))
                .Select(r => new FamilyMember
                {
                    Role = r.Field<string>("Role"),
                    Name = r.Field<string>("Name"),
                    Age = r.Field<int>("Age"),
                    Status = r.Field<string>("Status"),
                    EmploymentStatus = r.Field<string>("EmploymentStatus"),
                    Relationship = r.Field<string>("Relationship")
                })
                .ToList();
        }

        public static void RemoveStudent(string studentNo)
        {
            if (string.IsNullOrWhiteSpace(studentNo))
            {
                return;
            }

            DataRow[] studentRows = StudentsTable.Select("StudentNumber = '" + studentNo.Replace("'", "''") + "'");
            foreach (DataRow row in studentRows)
            {
                StudentsTable.Rows.Remove(row);
            }

            DataRow[] familyRows = FamilyMembersTable.Select("StudentNo = '" + studentNo.Replace("'", "''") + "'");
            foreach (DataRow row in familyRows)
            {
                FamilyMembersTable.Rows.Remove(row);
            }

            StudentProfile profile = StudentList.FirstOrDefault(s => s.StudentNo == studentNo);
            if (profile != null)
            {
                StudentList.Remove(profile);
            }
        }
    }

    public class StudentProfile
    {
        // Form 1 Data
        public string Name { get; set; }
        public string StudentNo { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Birthday { get; set; }
        public string Status { get; set; }
        public string Year { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Skills { get; set; }
        public Image Photo { get; set; }

        public List<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
    }
}
