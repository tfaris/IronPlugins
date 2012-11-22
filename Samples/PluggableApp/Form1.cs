using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IronPlugins;
using IronPlugins.Plugin;

namespace PluggableApp
{
    public partial class Form1 : Form
    {
        PluginManager _pluginManager;
        TextBoxStreamWriter _writer;

        public Form1()
        {
            InitializeComponent();
            _pluginManager = new PluginManager(System.Threading.SynchronizationContext.Current);
            _pluginManager.PluginReloaded += new PluginManager.PluginReloadedEventHandler(_pluginManager_PluginReloaded);
            _pluginManager.PluginReloadedError += new PluginManager.PluginReloadErrorEventHandler(_pluginManager_PluginReloadedError);
        }

        /// <summary>
        /// A plugin was changed and automatically reloaded by the manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void _pluginManager_PluginReloaded(object sender, PluginReloadEventArgs args)
        {
            textBox1.AppendText(string.Format("{0} was reloaded.\r\n", args.Plugin));
        }

        /// <summary>
        /// A plugin was changed and while trying to reload it, the manager encountered an error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void _pluginManager_PluginReloadedError(object sender, PluginReloadErrorEventArgs args)
        {
            string message = string.Format("Error while reloading {0}:\r\n{1}", args.Plugin.ToString(), args.Error.Message);
            MessageBox.Show(message);
        }

        private void buttonAddPlug_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    FilePlugin fPlug = new FilePlugin(ofd.FileName);
                    AddPlugin(fPlug);
                }
                catch (PythonPluginException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddPlugin(IIronPlugin plugin)
        {
            IronPlugins.Plugin.PluginCollection.AddResult result = _pluginManager.AddPlugin(plugin);
            if (result == PluginCollection.AddResult.Added)
            {
                dataGridView1.Rows[
                    dataGridView1.Rows.Add(
                    plugin.Guid,
                    plugin.Source.Path)].Tag = plugin;
            }
            else if (result == PluginCollection.AddResult.OverWrote)
            {
                // Replace the existing row
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Tag.Equals(plugin))
                    {
                        row.Cells[0].Value = plugin.Guid;
                        row.Tag = plugin;
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == colRun.Index)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    IIronPlugin plugin = (IIronPlugin)row.Tag;
                    plugin.RunPlugin();
                    // Display a list of variables now in the plugin's context
                    IronPlugins.Context.VariableList variables = plugin.GetContextVariables();
                    string variableList = "Variables on context:\r\n**************************************************\r\n";
                    foreach (IronPlugins.Context.Variable var in variables)
                    {
                        string varString = string.Format("{0} : {1}\r\n", var.Name, var.Value);
                        variableList += varString;
                    }
                    MessageBox.Show(variableList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(new IronPlugins.Plugin.PythonPluginException(ex).Message);
                }
            }
            else if (e.ColumnIndex == colRemPlugin.Index)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                IIronPlugin plugin = (IIronPlugin)row.Tag;
                plugin.Dispose();
                _pluginManager.RemovePlugin(plugin);
                dataGridView1.Rows.Remove(row);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _writer = new TextBoxStreamWriter(textBox1);
            Console.SetOut(_writer);
        }
    }
}
