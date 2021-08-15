using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

using TheLeftExit.Growtopia.Decoding;

namespace TheLeftExit.Itemsplorer
{
    public partial class Form1 : Form
    {
        private readonly String pathToItems = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "cache", "items.dat");
        private readonly String pathToTextures = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Growtopia", "game");
        private readonly Int32 maxres = 100;

        private ItemDefinition[] items;
        private Dictionary<String, Bitmap> textureFileCache = new();
        private Bitmap[] finalTextures;

        private PropertyDescriptor selectedProperty;

        public Form1()
        {
            items = ItemsDAT.Decode(pathToItems);
            finalTextures = new Bitmap[items.Length];

            InitializeComponent();
            this.MinimumSize = new Size(propertyGrid1.Width + 40, 200);

            propertyGrid1.SelectedObject = items.First();
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            selectedProperty = e.NewSelection.PropertyDescriptor;
            button1.Text = $"Search by {selectedProperty.Name}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Getting list of results.
            IEnumerable<ItemDefinition> sres;
            Type t = selectedProperty.PropertyType;
            try
            {
                // C# does not allow casting to type definitions. Thankfully, we're only working with four.
                if (t == typeof(Byte))
                    sres = items.Where(x => (Byte)selectedProperty.GetValue(x) == Byte.Parse(textBox1.Text));
                else if (t == typeof(Int16))
                    sres = items.Where(x => (Int16)selectedProperty.GetValue(x) == Int16.Parse(textBox1.Text));
                else if (t == typeof(Int32))
                    sres = items.Where(x => (Int32)selectedProperty.GetValue(x) == Int32.Parse(textBox1.Text));
                else if (t == typeof(String))
                    sres = items.Where(x => ((String)selectedProperty.GetValue(x)).ToLower().Contains(textBox1.Text.ToLower()));
                else
                    throw new NotImplementedException("Custom types not supported!");
            }
            catch(FormatException)
            {
                toolStripStatusLabel1.Text = $"Could not parse \"{textBox1.Text}\" to {t.Name}.";
                return;
            }

            // 2. Reporting count and trimming if necesary.
            bool includeSeeds = toolStripMenuItem1.Checked;
            ItemDefinition[] res = sres.Where(x => includeSeeds || (x.ItemID & 1) == 0).ToArray();
            if (res.Length <= maxres)
                toolStripStatusLabel1.Text = $"{res.Length} items found.";
            else
            {
                toolStripStatusLabel1.Text = $"{res.Length} items found (showing first {maxres}).";
                res = res.Take(maxres).ToArray();
            }

            // 3. Populating ListView.
            //listView1.LargeImageList?.Images.Clear();
            listView1.Clear();

            ImageList ilist = new();
            ilist.ImageSize = new(32, 32);

            foreach (ItemDefinition item in res)
            {
                ilist.Images.Add(item.Name, GetTexture(item));
            }
            listView1.LargeImageList = ilist;
            foreach (ItemDefinition item in res)
            {
                listView1.Items.Add(item.Name, item.Name);
            }
        }

        private Bitmap GetTextureFile(String name)
        {
            if (!textureFileCache.ContainsKey(name))
            {
                textureFileCache.Add(name, RTPACK.Decode(Path.Combine(pathToTextures, name)));
            }
            return textureFileCache[name];
        }
        private Bitmap GetTexture(ItemDefinition item)
        {
            if ((item.ItemID & 1) == 1)
                return new Bitmap(32, 32);
            if (finalTextures[item.ItemID] == null)
            {

                Int32 x, y;
                switch (item.SpreadType)
                {
                    case 3:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        x = item.TextureX + 3;
                        break;
                    case 2:
                    case 4:
                    case 5:
                        x = item.TextureX + 4;
                        break;
                    default:
                        x = item.TextureX;
                        break;
                }
                switch (item.SpreadType)
                {
                    case 2:
                    case 5:
                        y = item.TextureY + 1;
                        break;
                    default:
                        y = item.TextureY;
                        break;
                }
                finalTextures[item.ItemID] = GetTextureFile(item.Texture).Clone(new Rectangle(x * 32, y * 32, 32, 32), PixelFormat.Format64bppArgb);
            }
            return finalTextures[item.ItemID];
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            propertyGrid1.SelectedObject = items.Single(x => x.Name == e.Item.Text);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.PerformClick();
                e.Handled = true;
            }
        }
    }
}
