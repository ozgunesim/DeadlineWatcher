using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DeadlineWatcher
{

    public enum ProjectEditMode
    {
        CREATE,
        UPDATE
    }
    public partial class ProjectDetails : Form
    {
        public ProjectEditMode mode = ProjectEditMode.CREATE;
        private ProjectInfo info;
        private Random rnd = new Random();

        public ProjectDetails()
        {
            InitializeComponent();
        }

        private void ProjectDetails_Load(object sender, EventArgs e)
        {
            dateStart.Format = DateTimePickerFormat.Custom;
            dateStart.CustomFormat = "dd/MM/yyyy hh:mm:ss";

            dateEnd.Format = DateTimePickerFormat.Custom;
            dateEnd.CustomFormat = "dd/MM/yyyy hh:mm:ss";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ProjectInfo i = new ProjectInfo();

            i.id = MD5.encode(DateTime.Now.ToFileTime().ToString() + rnd.Next(0, 10000).ToString());
            i.name = txtName.Text;
            i.desc = txtDetails.Text;
            i.start = dateStart.Value;
            i.end = dateEnd.Value;
            this.info = i;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public ProjectInfo getInfo()
        {
            return this.info;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}


public class ProjectInfo
{
    public string id;
    public string name;
    public string desc;
    public DateTime start;
    public DateTime end;
}




