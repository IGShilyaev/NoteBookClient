using NotebookAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteBookClient
{
    public partial class ContactsForm : Form
    {
        const string baseAddress = "https://localhost:44358/";
        Person p = new Person();
        public ContactsForm()
        {
            InitializeComponent();
        }

        public ContactsForm(Person src)
        {
            InitializeComponent();
            p = src;
            UpdateTable();
        }


        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                int id = (int)listView1.SelectedItems[0].Tag;

                DeleteContact(id);

                UpdateTable();
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddContact();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                int id = (int)listView1.SelectedItems[0].Tag;

                UpdateContact(id);

                UpdateTable();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void UpdateTable()
        {
            listView1.Items.Clear();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.GetAsync("api/People/" + p.Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    p = response.Content.ReadAsAsync<Person>().Result;
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            foreach (Contact contact in p.Contacts)
            {
                var item = new ListViewItem(new[] { contact.ContactType.Title.ToString(), contact.Value.ToString() });
                item.Tag = contact.Id;
                listView1.Items.Add(item);
            }
        }

        public void DeleteContact(int delete)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.DeleteAsync("api/Contacts/" + delete).Result;

            }
        }


        private void AddContact()
        {
            Contact newReport = new Contact();
            InsertContactForm insForm = new InsertContactForm();
            try
            {
                if (insForm.ShowDialog() == DialogResult.OK)
                {
                    newReport.Value = insForm.textBox1.Text.ToString();
                    newReport.ContactTypeId = insForm.TypesIDs[insForm.comboBox1.SelectedIndex].Id;
                    newReport.PersonId = p.Id;
                }
                else
                {
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Неверный формат ввода!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.PostAsXmlAsync("api/Contacts", newReport).Result;

            }

            UpdateTable();

        }


        private void UpdateContact(int update)
        {
            Contact newReport = new Contact();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.GetAsync("api/Contacts/" + update).Result;

                if (response.IsSuccessStatusCode)
                {
                    Contact reports = response.Content.ReadAsAsync<Contact>().Result;

                    InsertContactForm insForm = new InsertContactForm(reports);
                    try
                    {
                        if (insForm.ShowDialog() == DialogResult.OK)
                        {
                            newReport.Value = insForm.textBox1.Text.ToString();
                            newReport.ContactTypeId = insForm.TypesIDs[insForm.comboBox1.SelectedIndex].Id;
                            newReport.PersonId = p.Id;
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Неверный формат ввода!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    newReport.Id = update;
                    HttpResponseMessage response2 = client.PutAsXmlAsync("api/Contacts/" + update, newReport).Result;


                }
            }

        }

    }
}
