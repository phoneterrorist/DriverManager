using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TeachersAndSubjects
{
    public partial class JournalForm : Form
    {
        static string connectionSrting = @"Data Source = (LocalDB)\MSSQLLocalDB;
                                    AttachDbFilename=C:\Users\Dasha\source\repos\TeachersAndSubjects\TeachersAndSubjects\Journal.mdf;
                                    Integrated Security = True";
        SqlConnection connection;

        public JournalForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            disp_data();
        }

        public void disp_data()
        {
            using (connection = new SqlConnection(connectionSrting))
            using (SqlDataAdapter adapter = new SqlDataAdapter("Select* from [dbo].[Journal]", connection))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }       
        }
        
        private void AddButton_Click(object sender, EventArgs e) // кнопка добавения новой пары
        {
            string teacher = textTeacherName.Text;
            string subject = textSubject.Text;

            if (string.IsNullOrEmpty(teacher) || string.IsNullOrEmpty(subject)) //если пусто, ждем пока поле заполниться
            {
                return;
            }
            else
            {
                using (connection = new SqlConnection(connectionSrting))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM" +
                        "[dbo].[Journal] WHERE NOT EXISTS(SELECT * FROM [dbo].[Journal]" +
                        "WHERE Teacher = @Teacher and Subject = @Subject)", connection); // с помощью EXISTS проверяем наличие пары
                        cmd.Parameters.AddWithValue("Teacher", teacher);
                    cmd.Parameters.AddWithValue("Subject", subject);
                    if (cmd.ExecuteScalar().ToString() == "0") // проверяем, есть ли уже такая пара
                    {
                        MessageBox.Show("Такая пара значений уже есть");
                    }
                    else
                    {
                        cmd = new SqlCommand("insert into [dbo].[Journal] values(@Teacher, @Subject)", connection); // добавляем новую пару
                        cmd.Parameters.AddWithValue("Teacher", teacher);
                        cmd.Parameters.AddWithValue("Subject", subject);
                        cmd.ExecuteNonQuery();
                        disp_data();
                    }
                }
            }
        }
    }
}