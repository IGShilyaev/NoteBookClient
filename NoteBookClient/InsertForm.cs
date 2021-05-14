using NotebookAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoteBookClient
{
    public partial class InsertForm : Form
    {
        public InsertForm()
        {
            InitializeComponent();
            dataGridView1.Rows.Add();

        }

        

        public InsertForm(Person p)
        {
            InitializeComponent();
            dataGridView1.Rows.Add();
            dataGridView1.Rows[0].Cells[0].Value = p.Firstname.ToString();
            dataGridView1.Rows[0].Cells[1].Value = p.Secondname.ToString();
            dataGridView1.Rows[0].Cells[2].Value = p.BirthDay.ToShortDateString();
            button2.Text = "Сохранить изменения";
        }



    }
}
