using Newtonsoft.Json;
using SpotifyMatch.Data;
using SpotifyMatch.Logic;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SpotifyMatch.GUI.Forms
{
    public partial class MainForm : Form
    {
        private readonly SettingsFile settingsFile;
        private const string settingsPath = @"settings.json";

        public MainForm()
        {
            InitializeComponent();
            _ = new Library.Auth.SpotifyAPI(Properties.Settings.Default.SpotifyID, Properties.Settings.Default.SpotifySecret);

            if (File.Exists(settingsPath))
            {
                using StreamReader file = File.OpenText(settingsPath);
                {
                    settingsFile = (SettingsFile)new JsonSerializer().Deserialize(file, typeof(SettingsFile));
                    tbPath.Text = settingsFile.Path;
                }
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            listView1.Update();

            TrackFinder finder = new TrackFinder(tbPath.Text);

            pBar.Maximum = finder.FileCount();
            pBar.Minimum = 0;
            pBar.Value = 0;

            int ItemsNotFound = 0;
            int ItemsFound = 0;

            foreach (var itemResultSet in finder.GetResultFromDirectory())
            {
                if (itemResultSet.FoundTracks != null)
                {
                    ItemsFound++;

                    foreach (var found in itemResultSet.FoundTracks)
                    {
                        ListViewItem listViewItem = new ListViewItem();
                        listViewItem.Text = itemResultSet.Id.ToString();
                        listViewItem.SubItems.Add(itemResultSet.Filename);
                        listViewItem.SubItems.Add(found.Name);
                        listViewItem.Tag = found.Id;
                        listView1.Items.Add(listViewItem);
                    }
                }
                else
                {
                    ItemsNotFound++;

                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Text = itemResultSet.Id.ToString();
                    listViewItem.SubItems.Add(itemResultSet.Filename);
                    listViewItem.BackColor = Color.IndianRed;
                    listView1.Items.Add(listViewItem);
                }

                pBar.Value++;
                pBar.Update();


                lblFound.Text = "Found: " + ItemsFound;
                lblFound.Update();

                lblNotFound.Text = "Not Found: " + ItemsNotFound;
                lblNotFound.Update();

                lblAll.Text = "All: " + finder.FileCount();
                lblAll.Update();
            }

            pBar.Value = 0;
            pBar.Update();

            MessageBox.Show("Finished", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbPath.Text))
                folderBrowserDialog1.SelectedPath = tbPath.Text;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                tbPath.Text = folderBrowserDialog1.SelectedPath;

                var settings = new SettingsFile
                {
                    Path = tbPath.Text
                };

                using StreamWriter file = File.CreateText(settingsPath);
                new JsonSerializer().Serialize(file, settings);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var id = (string)listView1.SelectedItems[0].Tag;
                if (!string.IsNullOrEmpty(id))
                {
                    var track = SpotifyMatch.Library.Spotify.GetTrack(id);
                    System.Diagnostics.Process.Start(track.Uri);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var id = (string)listView1.SelectedItems[0].Tag;
                if (!string.IsNullOrEmpty(id))
                {
                    SpotifyMatch.Library.Spotify.LoveSong(id);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var settings = new SettingsFile
            {
                Path = tbPath.Text
            };

            using StreamWriter file = File.CreateText(settingsPath);
            new JsonSerializer().Serialize(file, settings);
        }
    }
}
