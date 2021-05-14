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
using NotebookAPI.Models;



namespace NoteBookClient
{
    public partial class Form1 : Form
    {
        const string baseAddress = "https://localhost:44358/";

        public Form1()
        {
            
            InitializeComponent();
            UpdateTable();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddPerson();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                int id = (int)listView1.SelectedItems[0].Tag;

                UpdatePerson(id);

                UpdateTable();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                int id = (int)listView1.SelectedItems[0].Tag;

                DeletePerson(id);

                UpdateTable();
            }

        }

        private void UpdateTable()
        {
            listView1.Items.Clear();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));



                HttpResponseMessage response;

                response = client.GetAsync("api/People").Result;
                if (response.IsSuccessStatusCode)
                {
                    Person[] reports = response.Content.ReadAsAsync<Person[]>().Result;
                    foreach (var p in reports)
                    {
                        var item = new ListViewItem(new[] { p.Firstname, p.Secondname, p.BirthDay.ToShortDateString() });
                        item.Tag = p.Id;
                        listView1.Items.Add(item);
                    }
                }
            }
        }


        private void DeletePerson(int delete)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.DeleteAsync("api/People/" + delete).Result;

            }

        }

        private void AddPerson()
        {
            Person newReport = new Person();
            InsertForm insForm = new InsertForm();
            try
            {
                if (insForm.ShowDialog() == DialogResult.OK)
                {
                    newReport = new Person()
                    {
                        Firstname = insForm.dataGridView1.Rows[0].Cells[0].Value.ToString(),
                        Secondname = insForm.dataGridView1.Rows[0].Cells[1].Value.ToString(),
                        BirthDay = DateTime.Parse(insForm.dataGridView1.Rows[0].Cells[2].Value.ToString())
                    };
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

                HttpResponseMessage response = client.PostAsXmlAsync("api/People", newReport).Result;

            }

            UpdateTable();

        }


        private void UpdatePerson(int update)
        {
            Person newReport = new Person();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.GetAsync("api/People/" + update).Result;

                if (response.IsSuccessStatusCode)
                {
                    Person reports = response.Content.ReadAsAsync<Person>().Result;

                    InsertForm insForm = new InsertForm(reports);
                    try
                    {
                        if (insForm.ShowDialog() == DialogResult.OK)
                        {
                            newReport = new Person()
                            {
                                Firstname = insForm.dataGridView1.Rows[0].Cells[0].Value.ToString(),
                                Secondname = insForm.dataGridView1.Rows[0].Cells[1].Value.ToString(),
                                BirthDay = DateTime.Parse(insForm.dataGridView1.Rows[0].Cells[2].Value.ToString())
                            };

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
                    newReport.Contacts = reports.Contacts;
                    HttpResponseMessage response2 = client.PutAsXmlAsync("api/People/" + update, newReport).Result;


                }
            }

        }

        private void ToContactsButton_Click(object sender, EventArgs e)
        {
            Person selected = new Person();

            if (listView1.SelectedItems.Count != 0)
            {
                int id = (int)listView1.SelectedItems[0].Tag;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    HttpResponseMessage response = client.GetAsync("api/People/" + id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        selected = response.Content.ReadAsAsync<Person>().Result;
                        

                        ContactsForm contactsForm = new ContactsForm(selected);
                        contactsForm.ShowDialog();
                    }
                }
            }
        }
    }
}

