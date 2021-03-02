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
        private string dragTarget;


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
            bool exists = false;
            for(int i=0; i< projectList.Count; i++)
            {
                if(projectList[i].id == p.id)
                {
                    projectList[i] = p;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                projectList.Add(p);
            }
            

            string str_json = JsonConvert.SerializeObject(projectList, Formatting.Indented);
            File.WriteAllText(json_path, str_json);
        }

        private void DeleteProject(string id)
        {
            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i].id == id)
                {
                    projectList.RemoveAt(i);
                    break;
                }
            }

            string str_json = JsonConvert.SerializeObject(projectList, Formatting.Indented);
            File.WriteAllText(json_path, str_json);
        }


        private void ListProjects()
        {
            container.Controls.Clear();
            foreach(ProjectInfo i in projectList)
            {
                DeadlineWatcherListItem.ProjectListItem item = new DeadlineWatcherListItem.ProjectListItem();
                item.Dock = DockStyle.Top;

                item.project_name = i.name;
                item.id = i.id;
                item.project_desc = i.desc;
                item.project_start = i.start;
                item.project_end = i.end;

                item.AllowDrop = true;

                item.DeleteClicked += İtem_DeleteClicked;
                item.UpdateClicked += İtem_UpdateClicked;

                item.DragEnter += İtem_DragEnter;
                item.DragLeave += İtem_DragLeave;
                item.DragDrop += İtem_DragDrop;
                


                container.Controls.Add(item);
            }
        }

        private void İtem_DragLeave(object sender, EventArgs e)
        {
            DeadlineWatcherListItem.ProjectListItem itm = (DeadlineWatcherListItem.ProjectListItem)sender;
            itm.showDragArrow(false);
        }

        private void İtem_DragDrop(object sender, DragEventArgs e)
        {
            DeadlineWatcherListItem.ProjectListItem itm = (DeadlineWatcherListItem.ProjectListItem)sender;
            itm.showDragArrow(false);

            string source = e.Data.GetData("DragSourceID").ToString();
            //Console.WriteLine(source + " -> " + dragTarget);

            int index = 0;
            int source_index = -1;
            int target_index = -1;
            foreach(ProjectInfo i in projectList)
            {
                if(i.id == source)
                {
                    source_index = index;
                }
                if(i.id == dragTarget)
                {
                    target_index = index;
                }
                index++;
            }

            if(source_index != -1 && target_index != -1 && source_index != target_index)
            {
                ProjectInfo temp = projectList[source_index];
                projectList[source_index] = projectList[target_index];
                projectList[target_index] = temp;
                string str_json = JsonConvert.SerializeObject(projectList, Formatting.Indented);
                File.WriteAllText(json_path, str_json);
                ListProjects();

            }

            dragTarget = "";
            
            //Console.WriteLine(e.Data.ToString());
        }



        private void İtem_DragEnter(object sender, DragEventArgs e)
        {
            DeadlineWatcherListItem.ProjectListItem itm = (DeadlineWatcherListItem.ProjectListItem)sender;
            e.Effect = DragDropEffects.Move;
            itm.showDragArrow(true);
            dragTarget = itm.id;
        }

        private void BtnAddProject_Click(object sender, EventArgs e)
        {

            ProjectDetails pd = new ProjectDetails();
            if(pd.ShowDialog() == DialogResult.OK)
            {
                ProjectInfo i = pd.getInfo();
                projectList.Add(i);

                SaveProject(i);
                ListProjects();
            }
        }

        private void İtem_UpdateClicked(object sender, string id)
        {
            DeadlineWatcherListItem.ProjectListItem itm = (DeadlineWatcherListItem.ProjectListItem)sender;
            ProjectDetails pd = new ProjectDetails();
            foreach(ProjectInfo i in projectList)
            {
                if(i.id == itm.id)
                {
                    pd.setInfo(i);
                    break;
                }
            }
            if(pd.ShowDialog() == DialogResult.OK)
            {
                for(int i=0; i<projectList.Count; i++)
                {
                    if(projectList[i].id == pd.getInfo().id)
                    {
                        projectList[i] = pd.getInfo();
                        SaveProject(pd.getInfo());
                        break;
                    }
                }
                ListProjects();
            }


        }

        private void İtem_DeleteClicked(object sender, string id)
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
                        ListProjects();
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
            this.ActiveControl = chkStartup;
        }




        private void ChkStartup_CheckedChanged(object sender, EventArgs e)
        {
            Startup.setState(chkStartup.Checked);
        }
    }


    
}
