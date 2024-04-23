using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("IExplore", "https://github.com/Oliver-Lief/RF_TOOL");
        }
    }
}
