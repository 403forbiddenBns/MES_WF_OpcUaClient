using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WF_OpcUa
{
    public partial class MES : Form
    {
        public static OpcClient client;
        System.Windows.Forms.Timer updateCounter;
        public static DataTable dt;


        public MES()
        {
            InitializeComponent();

            updateCounter = new System.Windows.Forms.Timer();
            updateCounter.Interval = 2000;
            updateCounter.Tick += Timer_Tick;
            updateCounter.Start();

            dt = new DataTable();
            dataGridView1.DataSource = dt;
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dataGridView1.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        private void FillTreeNodes(string nodeId)
        {
            try
            {
                foreach (var node in client.BrowseNode(nodeId).Children()) //Get list of root elements;
                {
                    TreeNode treeNode = new TreeNode(nodeId);
                    treeNode.Text = node.DisplayName;
                    treeNode.Name = node.NodeId.ToString();
                    FillTreeNode(treeNode, node.NodeId.ToString());
                    treeView1.Nodes.Add(treeNode);
                }
            }
            catch (Exception) { }
        }

        private void FillTreeNode(TreeNode opcNode, string nodeId)
        {

            foreach (var item in client.BrowseNode(nodeId).Children())
            {
                string nodeKey = item.NodeId.ToString();
                opcNode.Nodes.Add(nodeKey, item.DisplayName.Value);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            client = new OpcClient(addressLine.Text.Trim(), new Opc.UaFx.OpcSecurityPolicy(Opc.UaFx.OpcSecurityMode.None));
            client.Connect();
            FillTreeNodes("i=84");
            statusColor.BackColor = Color.Green;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client?.Disconnect();
            Environment.Exit(0);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                FillTreeNode(e.Node, e.Node.Name);
                OpcNodeInfo selectedNode = client.BrowseNode(e.Node.Name);

                lNodeName.Text = selectedNode.DisplayName;
                lNodeId.Text = selectedNode.NodeId.ToString();
                lNodeClass.Text = selectedNode.Category.ToString();
            }
            catch (Exception) { }
        }

        private void buttonAdd_click(object sender, EventArgs e)
        {
            var node = treeView1.SelectedNode.Name;
            OpcNodeInfo selectedNode = client.BrowseNode(node);
            OpcValue opcValue = client.ReadNode(selectedNode.NodeId);

            dt.Rows.Add(
                    selectedNode.DisplayName,
                    selectedNode.NodeId,
                    opcValue?.ToString()
                    );
        }

        private void buttonDelete_click(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.SelectedRows.Count > 0)
                // && this.dataGridView1.SelectedRows[0].Index !=
                //this.dataGridView1.Rows.Count - 1)

                {
                    this.dataGridView1.Rows.RemoveAt(
                        this.dataGridView1.SelectedRows[0].Index);
                }

            }
            catch (Exception) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form findForm = new Form2();
            findForm.Show();
        }

        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            var subNodes = client.BrowseNode(e.Node.Name);
            try
            {
                foreach (var node in client.BrowseNode(e.Node.Name).Children()) //Get list of root elements;
                {
                    TreeNode treeNode = new TreeNode(e.Node.Name);
                    treeNode.Text = node.DisplayName;
                    treeNode.Name = node.NodeId.ToString();
                    FillTreeNode(treeNode, node.NodeId.ToString());
                    e.Node.Nodes.Add(treeNode);
                }
            }
            catch (Exception) { }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var selectedRows = dt.Rows;
            try
            {
                foreach (DataRow row in selectedRows)
                {
                    string nodeId = row[1].ToString(); //Get index
                    OpcNodeInfo currentNode = client.BrowseNode(nodeId);
                    OpcValue currentNodeValue = client.ReadNode(nodeId);
                    row[2] = currentNodeValue.ToString();
                }
            }
            catch (Exception){}
        }
    }
}
