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
    public partial class InsertContactForm : Form
    {
        const string baseAddress = "https://localhost:44358/";
        public List<ContactType> TypesIDs;
        
        public InsertContactForm()
        {
            InitializeComponent();
            TypesIDs = new List<ContactType>();
            GetTypes();
        }

        public InsertContactForm( Contact contact)
        {
            InitializeComponent();
            TypesIDs = new List<ContactType>();
            GetTypes();
            textBox1.Text = contact.Value;
            comboBox1.SelectedItem = comboBox1.Items[TypesIDs.Select(x => x.Id).ToList().BinarySearch((int) contact.ContactTypeId)];
        }

        private void GetTypes()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                HttpResponseMessage response = client.GetAsync("api/contactTypes").Result;
                if (response.IsSuccessStatusCode)
                {
                    foreach (ContactType t in response.Content.ReadAsAsync<ContactType[]>().Result)
                    {
                        TypesIDs.Add(t);
                        comboBox1.Items.Add(t.Title);
                    }
                }

            }
        }

    }
}
