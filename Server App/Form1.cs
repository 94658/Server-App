using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly:SecurityPermission(action: SecurityAction.RequestMinimum,Execution =true)]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly:DirectoryServicesPermission(action: SecurityAction.RequestMinimum)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace Server_App
{
    public partial class Form1 : Form
    {
        public string pingreturn;
        public DirectorySearcher directorySearcher = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxUserName.Focus();

            label9.Visible = false;
            textBoxDomain.Text = GetSystemDomain();
        }

        private string GetSystemDomain()
        {
            try
            {
                return Domain.GetComputerDomain().ToString().ToLower();
            }
            catch (Exception e)
            {
                e.Message.ToString();
                return string.Empty;
            }     
        }
        private void returnServers()
        {
            label9.Visible = false;
            if (textBoxAD.Text == "")
            {
                MessageBox.Show("Invalid Server.Enter AD Server");
            }

            else
            {
                GetComputers();
            }
        }

        private object GetComputers()
        {

            List<string> computerNames = new List<string>();
            System.Net.NetworkInformation.Ping pingcheck = new System.Net.NetworkInformation.Ping();

            DirectoryEntry entry = new DirectoryEntry("LDAP:" + "//" + textBoxAD.Text);
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(objectClass=computer)");
            mySearcher.SizeLimit = int.MaxValue;
            mySearcher.ServerTimeLimit = new TimeSpan(0, 0, 30);
            mySearcher.ClientTimeout = new TimeSpan(0, 10, 0);
            mySearcher.PageSize = 5;

            foreach (SearchResult result in mySearcher.FindAll())
            {
                try
                {
                    //System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
                    //pingreturn = pinger.Send(result.GetDirectoryEntry().Name.ToString().Remove(0, 3)).Status.ToString();

                    //if(pingreturn == "Success")
                    //{
                    computerNames.Add(result.GetDirectoryEntry().Name.ToString().Remove(0, 3));
                    //}
                    //else
                    //{
                    //    computerNames.Add(result.GetDirectoryEntry().Name.ToString().Remove(0, 3) + " <---- OFF!!!");
                    //}

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                computerNames.Sort();
                comboBoxServers.Items.AddRange(computerNames.ToArray());

            }
            label9.Text = "Servers successfully loaded.You can now select server";
            label9.ForeColor = System.Drawing.Color.Green;
            label9.Visible = true;
            return null;
        }
        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

       

        private void btnConnect_Click(object sender, EventArgs e)
        {
            var groupList = new List<StringValue>();
            label9.Visible = false;
            if (textBoxAD.Text == "" && this.comboBoxServers.SelectedItem == null)
            {

                label9.Text = "Missing AD Server IP or AD Clients not loaded";
                label9.ForeColor = System.Drawing.Color.Red;
                label9.Visible = true;
                

            }
            else
            {
                int count;
                DataTable dt = new DataTable();
                dt.Columns.Add("Groups");
                
                
                string selected = this.comboBoxServers.GetItemText(this.comboBoxServers.SelectedItem);
                if(selected == "")
                {
                    label9.Text = "Please select Server";
                    label9.ForeColor= System.Drawing.Color.Red;
                    label9.Visible = true;
                }
                else
                {
                    DirectoryEntry machine = new DirectoryEntry("WinNT://" + selected + ",Computer");
                    foreach (DirectoryEntry child in machine.Children)
                    {
                        if (child.SchemaClassName == "Group")
                        {
                          
                            groupList.Add(new StringValue(child.Name.ToString()));
                           
                        }
                    }
                    //count = groupList.Count;
                    //for (int i = 0; i < count; i++)
                    //{

                    //}

                    var bindingList = new BindingList<StringValue>(groupList);
                    var source = new BindingSource(bindingList, null);
                    groupGrid.DataSource = source;


                }
                

            }


         
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
        }

        private void buttonLoadClients_Click(object sender, EventArgs e)
        {
            returnServers();
        }

        private void comboBoxServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
