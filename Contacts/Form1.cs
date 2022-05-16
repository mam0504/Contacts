using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contacts
{
    public partial class ContactForm : Form
    {
        public static string ConStr = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                        AttachDbFilename=C:\Users\amezh\source\repos\Contacts\Contacts\Persons.mdf;
                                        Integrated Security=True";
        string imgPath = "";
        SqlConnection con = new SqlConnection(ConStr);
        SqlCommand cmd;
        public ContactForm()
        {
            InitializeComponent();
            Setup();
        }

        public void Setup()
        {
            this.groupBox1.Text = "Новый контакт";
            this.label1.Text = "Имя";
            this.label2.Text = "Фамилия";
            this.label3.Text = "Номер телефона";
            this.label4.Text = "E-Mail";
            this.label5.Text = "Адрес";
        }

        public void Read_Data()
        {
            byte[] img = null;
            string sql = "select FirstName, LastName, Phone, Email, Address, Picture from Person where id = '"+txtFirstName.Text+"'";
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            SqlDataReader dr = cmd.ExecuteReader();
            if(dr.HasRows)
            {
                while(dr.Read())
                {
                    txtFirstName.Text = dr[0].ToString();
                    txtLastName.Text = dr[1].ToString();
                    txtPhone.Text = dr[2].ToString();
                    txtEmail.Text = dr[3].ToString();
                    txtAddress.Text = dr[4].ToString();
                    img = (byte[])(dr[5]);
                    if (img == null)
                        picPhoto.Image = null;
                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        picPhoto.Image = Image.FromStream(ms);
                    }
                }
                con.Close();
            } 
            else
            {
                con.Close();
                MessageBox.Show("There is not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        public void Insert()
        {
            byte[] img = null;
            FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            img = br.ReadBytes((int)fs.Length);
            string sql = "insert into Person(FirstName, LastName, Phone, Email, Address, Picture) values('" + txtFirstName.Text + "', '" + txtLastName.Text + "', '" + txtPhone.Text + "', '"+txtEmail.Text+"', '"+txtAddress.Text+"', @img)";
            if(con.State != ConnectionState.Open)
               con.Open();
            cmd = new SqlCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@img", img));
            int x = cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show(x.ToString()+ " " + "Data is inserted successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            picPhoto.Image = null;

            this.Validate();
            this.personTableAdapter.Update(personsDataSet);
            dataGridView1.Update();
            dataGridView1.Refresh();
        }

        public void Delete_Data()
        {
            string sql = "delete from Person where id = '"+txtFirstName.Text+"'";
            if(con.State != ConnectionState.Open)
            {
                con.Open();
                cmd = new SqlCommand(sql, con);
                int x = cmd.ExecuteNonQuery();
                con.Close();
                personBindingSource.RemoveAt(x);
                MessageBox.Show("Data is " + x.ToString() + " removed successfuly !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            Insert();
        }

        private void btnPicture_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG Files (*.JPG)|*.JPG|All Files (*.*)|*.*";
            openFileDialog.Title = "Select Image";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imgPath = openFileDialog.FileName.ToString();
                picPhoto.ImageLocation = openFileDialog.FileName;
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            Read_Data();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            picPhoto.Image = null;
        }

        private void ContactForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'personsDataSet.Person' table. You can move, or remove it, as needed.
            this.personTableAdapter.Fill(this.personsDataSet.Person);

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete_Data();
        }
    }
}
