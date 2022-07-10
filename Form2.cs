using Opc.UaFx;
using Opc.UaFx.Client;
using System;
using System.Windows.Forms;

namespace WF_OpcUa
{
    public partial class Form2 : Form
    {
        public static Form2 instance;
        public Form2()
        {
            InitializeComponent();
            instance = this;
            //opcClient.BrowseNode();
        }

        private void bFind_Click(object sender, EventArgs e)
        {
            OpcNodeInfo node = MES.client.BrowseNode(tbFindNode.Text);
            OpcValue opcValue = MES.client.ReadNode(node.NodeId);
            if (node != null)
            {
                MES.dt.Rows.Add(
                    node.DisplayName,
                    node.NodeId,
                    opcValue?.ToString()
                    );
            }


            //MES.lvInstance.Items.Add(new ListViewItem(new String[]
            //{
            //    node.DisplayName,
            //    node.NodeId.ToString(),
            //    opcValue?.ToString()
            //}));

            this.Close();
        }
    }
}
