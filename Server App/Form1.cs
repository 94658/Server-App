using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
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
        public string clientSelected;
        public string groupSelected;
        public string memberSelected;
        public int membersPresent;

        public string pingreturn;
        public DirectorySearcher directorySearcher = null;
        public DirectorySearcher groupSearcher = new DirectorySearcher();
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

        //Runs when form loads
        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxUserName.Focus();

            label9.Visible = false;
            textBoxDomain.Text = GetSystemDomain();
        }


        /// <summary>
        /// get the current domain where the pc executing this exe runs
        /// </summary>
        /// <returns>Domain Name</returns>
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

        /// <summary>
        /// Check that Active Directory IP has been entered in TextboxAD
        /// Calls GetComputers() method to get list of client hostnames in the domain and displays them in a combobox
        /// </summary>
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

        /// <summary>
        /// Query the Active Durectory to get list of client hostnames in the domain and displays them in the combobox
        /// </summary>
        /// <returns>Combobox with client hostnames</returns>
        private object GetComputers()
        {

            List<string> computerNames = new List<string>();
            System.Net.NetworkInformation.Ping pingcheck = new System.Net.NetworkInformation.Ping();
            DirectoryEntry entry = new DirectoryEntry();

            if (checkBox1.Checked)
            {

                entry = new DirectoryEntry("LDAP:" + "//" + textBoxAD.Text, textBoxUserName.Text, textBoxPassword.Text);

            }
            else
            {
                entry = new DirectoryEntry("LDAP:" + "//" + textBoxAD.Text);
            }

            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            groupSearcher = mySearcher;
            mySearcher.Filter = ("(objectClass=computer)");
            mySearcher.SizeLimit = int.MaxValue;
            mySearcher.ServerTimeLimit = new TimeSpan(0, 0, 30);
            mySearcher.ClientTimeout = new TimeSpan(0, 10, 0);
            mySearcher.PageSize = 5;

            try
            {
                foreach (SearchResult result in mySearcher.FindAll())
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            computerNames.Sort();
            comboBoxServers.Items.AddRange(computerNames.ToArray());

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


        /// <summary>
        /// Gets local groups for the client host selected in the Server Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            memberGrid.DataSource = null;
            memberGrid.Refresh();
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
                string selected = this.comboBoxServers.GetItemText(this.comboBoxServers.SelectedItem);
                if (selected == "")
                {
                    label9.Text = "Please select Server";
                    label9.ForeColor = System.Drawing.Color.Red;
                    label9.Visible = true;
                }
                else
                {
                    clientSelected = selected;
                    DirectoryEntry machine = new DirectoryEntry("WinNT://" + selected + ",Computer");
                    try
                    {
                        label9.ForeColor = System.Drawing.Color.Black;
                        label9.Text = "Fetching Groups...";
                        label9.Visible = true;
                        foreach (DirectoryEntry child in machine.Children)
                        {
                            if (child.SchemaClassName == "Group")
                            {

                                groupList.Add(new StringValue(child.Name.ToString()));

                            }
                        }

                        label9.ForeColor = System.Drawing.Color.Green;
                        label9.Text = " Fetched all local groups for " + selected;

                    }
                    catch (Exception)
                    {
                        label9.Text = "Machine: " + selected + " is off";
                        label9.ForeColor = System.Drawing.Color.Red;
                        label9.Visible = true;

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

        /// <summary>
        /// Set label to visible when text is been entered in the AD Server Textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
        }

        /// <summary>
        /// Load ADCLients button that calls returnServers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadClients_Click(object sender, EventArgs e)
        {
            returnServers();
        }



          /// <summary>
          /// ComboxBox IndexChanged Handler that sets value of the label to false when index changes
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
        private void comboBoxServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            label9.Visible = false;
        }


        /// <summary>
        /// GroupGrid cellcontentclick handler that gets the value of the cell clicked
        /// Fetches the members of that group value
        /// Renders the members list to the Members Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //label9.Visible = false;
            string accountName;
            memberGrid.DataSource = null;
            button1.Enabled = true;
            var memberList = new List<StringValue>();
            if (groupGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                groupSelected = groupGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                // MessageBox.Show(groupGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());

                using (DirectoryEntry machine = new DirectoryEntry("WinNT://" + clientSelected))
                {
                    using (DirectoryEntry group = machine.Children.Find(groupSelected, "Group"))
                    {
                        object members = group.Invoke("Members", null);

                        foreach (object member in (IEnumerable)members)
                        {
                            //get account name

                            accountName = new DirectoryEntry(member).Name;
                            // MessageBox.Show(accountName);
                            if (accountName == "")
                            {

                                memberGrid.DataSource = null;
                                label9.Text = "No members in this group";
                                membersPresent = 0;
                                label9.Visible = true;
                                button1.Enabled = false;

                            }
                            else
                            {
                                membersPresent = 1;
                                memberList.Add(new StringValue(new DirectoryEntry(member).Name.ToString()));
                                var bindingList = new BindingList<StringValue>(memberList);
                                var source = new BindingSource(bindingList, null);
                                memberGrid.DataSource = source;
                            }

                            // label9.Visible = false;

                        }
                    }
                }
            }
        }


        /// <summary>
        /// Member Grid cellcontentclick handler that gets the value of the cell clicked  and assigns it to a global variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memberGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (memberGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                memberSelected = memberGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
        }


        /// <summary>
        /// Takes in clientSelected,groupSelected and memeberSelected and memberPresented variables to remove user from the selected group
        /// </summary>
        /// <param name="computerName"></param>
        /// <param name="groupSelect"></param>
        /// <param name="memberSelect"></param>
        /// <param name="memberPresence"></param>
        /// <returns>bool variable</returns>
        private bool RemoveUserFromGroup(string computerName, string groupSelect, string memberSelect, int memberPresence)
        {
            string messageBoxText = "Do you want to delete " + memberSelect + " from " + groupSelect + " ?";
            string caption = "Delete Member";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            MessageBoxIcon icon = MessageBoxIcon.Warning;
            if (memberPresence == 1) {
                try
                {
                    //var de = new DirectoryEntry("WinNT://" + computerName);
                    using (DirectoryEntry clientMachine = new DirectoryEntry("WinNT://" + computerName))
                    {
                        using (DirectoryEntry group = clientMachine.Children.Find(groupSelect, "Group"))
                        {
                            // var objGroup = de.Children.Find(Settings.AdministratorsGroup, "group");
                            object members = group.Invoke("Members", null);
                            foreach (object member in (IEnumerable)members)
                            {
                                using (var memberEntry = new DirectoryEntry(member))
                                {

                                    if (memberEntry.Name == memberSelect)
                                    {
                                        MessageBox.Show(memberEntry.Name.ToString());
                                        var result = MessageBox.Show(messageBoxText, caption, buttons, icon);
                                        switch (result)
                                        {
                                            case DialogResult.Yes:   // Yes button pressed
                                                group.Invoke("Remove", new[] { memberEntry.Path });
                                                MessageBox.Show("Deleting " + memberEntry.Name);
                                                break;
                                            case DialogResult.No:    // No button pressed
                                                                     // MessageBox.Show("You pressed No!");
                                                break;
                                            default:
                                                MessageBox.Show("Kindly select either yes or no");
                                                break;
                                        }

                                        // group.Invoke("Remove", new[]{memberEntry.Path });
                                    }
                                }

                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
            }
            else if (textBoxAD.Text == "" || this.comboBoxServers.SelectedItem == null || groupSelect == "" || memberSelect == "" || computerName == "")
            {
                label9.Text = "Missing AD Server IP or AD Clients, Group , Member not Selected";
                label9.ForeColor = System.Drawing.Color.Red;
                label9.Visible = true;
                return false;
            }

            else if (memberGrid.DataSource == null || groupGrid.DataSource == null || memberGrid.Rows.Count == 0)
            {
                label9.Text = "No values in GroupGridView or MemberGridView.Try get groups for Client Selected";
                label9.ForeColor = System.Drawing.Color.Red;
                label9.Visible = true;
                return false;
            }
            else
            {
                MessageBox.Show("No members in " + groupSelect);
                return false;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Refreshes the GroupGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            this.groupGrid.Refresh();
        }


        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void textBoxDomain_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This Delete User button removes the selected user from the group selected permanently
        /// it calls the RemoveUserFromGroup() method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            RemoveUserFromGroup(clientSelected, groupSelected, memberSelected, membersPresent);
        }



        /// <summary>
        /// Ping button to test if clients can ping host machine
        /// Message is either: Machine or Machine on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            label9.Visible = false;
            System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
            foreach (SearchResult result in groupSearcher.FindAll())
            {
                try
                {


                    pingreturn = pinger.Send(result.GetDirectoryEntry().Name.ToString().Remove(0, 3)).Status.ToString();
                    if (pingreturn == "Success")
                    {
                        label9.Text = "Machine: " + clientSelected + " is on";
                        label9.ForeColor = System.Drawing.Color.Green;
                        label9.Visible = true;
                    }
                    else
                    {
                        if (clientSelected != "")
                        {
                            label9.Text = "Machine: " + clientSelected + " is off";
                            label9.ForeColor = System.Drawing.Color.Red;
                            label9.Visible = true;
                        }

                        else
                        {
                            MessageBox.Show("Select Client PC");
                        }

                    }
                }

                catch(Exception ex)
                {
                    label9.Text = ex.Message;
                    label9.ForeColor = System.Drawing.Color.Red;
                    label9.Visible = true;
                }
            }

        }

    }

}

