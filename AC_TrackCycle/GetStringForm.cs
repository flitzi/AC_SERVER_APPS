using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC_TrackCycle
{
    public partial class GetStringForm : Form
    {
        public string String { get; private set; }
        public GetStringForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.String = textBox1.Text;
        }
    }
}
