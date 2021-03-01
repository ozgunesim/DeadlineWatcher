using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace DeadlineWatcher
{
    public partial class Form1 : Form
    {

        private List<ProjectInfo> projectList = new List<ProjectInfo>();
        private string json_path = null;


        public Form1()
        {
            InitializeComponent();
        }

        private List<ProjectInfo> LoadProjects()
        {
            string appdata = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string appdir = appdata + "/DeadlineWatcher";
            if (!Directory.Exists(appdir))
            {
                Directory.CreateDirectory(appdir);
            }

            string json_file = appdir + "/projects.json";
            if (!File.Exists(json_file))
            {
                File.WriteAllText(json_file, "[]");
            }

            this.json_path = json_file;

            string json_data = File.ReadAllText(json_file);

            return JsonConvert.DeserializeObject<List<ProjectInfo>>(json_data);
        }

        private void SaveProject(ProjectInfo p)
        {
            List<ProjectInfo> list = LoadProjects();
            bool exists = false;
            for(int i=0; i<list.Count; i++)
            {
                if(list[i].id == p.id)
                {
                    list[i] = p;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                list.Add(p);
            }
            

            string str_json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(json_path, str_json);
        }

        private void DeleteProject(string id)
        {
            List<ProjectInfo> list = LoadProjects();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].id == id)
                {
                    list.RemoveAt(i);
                    break;
                }
            }

            string str_json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(json_path, str_json);
        }


        private void ListProjects()
        {
            foreach(ProjectInfo i in projectList)
            {
                DeadlineWatcherListItem.ProjectListItem item = new DeadlineWatcherListItem.ProjectListItem();
                item.Dock = DockStyle.Top;
                container.Controls.Add(item);

                item.project_name = i.name;
                item.id = i.id;
                item.project_desc = i.desc;
                item.project_start = i.start;
                item.project_end = i.end;

                item.DeleteClicked += İtem_DeleteClicked;
            }
        }

        private void BtnAddProject_Click(object sender, EventArgs e)
        {


            ProjectDetails pd = new ProjectDetails();
            if(pd.ShowDialog() == DialogResult.OK)
            {
                ProjectInfo i = pd.getInfo();
                string json_val = JsonConvert.SerializeObject(i, Formatting.Indented);


                DeadlineWatcherListItem.ProjectListItem item = new DeadlineWatcherListItem.ProjectListItem();
                item.Dock = DockStyle.Top;
                container.Controls.Add(item);

                item.project_name = i.name;
                item.id = i.id;
                item.project_desc = i.desc;
                item.project_start = i.start;
                item.project_end = i.end;

                item.DeleteClicked += İtem_DeleteClicked;

                SaveProject(i);
            }
        }

        private void İtem_DeleteClicked(object sender)
        {
            if(MessageBox.Show("Emin Misiniz?", "Proje Sil", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DeadlineWatcherListItem.ProjectListItem itm = (DeadlineWatcherListItem.ProjectListItem)sender;
                for (int i = 0; i < container.Controls.Count; i++)
                {
                    Control c = container.Controls[i];
                    if (((DeadlineWatcherListItem.ProjectListItem)c).id == itm.id)
                    {
                        DeleteProject(itm.id);
                        container.Controls.RemoveAt(i);
                        break;
                    }
                }
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            projectList = LoadProjects();
            ListProjects();
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width;
            this.Top = 0;
            chkStartup.Checked = Startup.isRegistered();
        }




        private void ChkStartup_CheckedChanged(object sender, EventArgs e)
        {
            Startup.setState(chkStartup.Checked);
        }
    }


    
}
