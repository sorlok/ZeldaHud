using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Management;
//using usb2snes.utils;
using System.Web.Script.Serialization;
using System.Threading;
using WebSocketSharp;
using EvalCSCode;

namespace zeldaGui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        string currentIconset = @"IconsSets\Defaults";
        string currentBgr = @"None";
        Bitmap bgr;
        List<string> item_found = new List<string>();
        private void Form1_Load(object sender, EventArgs e)
        {
            globalTimer.MakeTransparent(Color.Fuchsia);
            globalCount.MakeTransparent(Color.Fuchsia);
            this.Text = "ALTTP Rando HUD";
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);

            addItems();
            setDefaultItems();

            loadLayout();
            loadIconsSet(currentIconset);
            if (currentBgr != "None")
            {
                bgr = new Bitmap(currentBgr);
                bgr.MakeTransparent(Color.Fuchsia);
            }
            drawIcons();
            if (checkUpdate)
            {
                if (false && Version.CheckUpdate() == true)
                {
                    var window = MessageBox.Show("There is a new version avaiable do you want to download the update?", "Update Avaible", MessageBoxButtons.YesNo);
                    if (window == DialogResult.Yes)
                    {
                        Help.ShowHelp(null, @"https://zarby89.github.io/ZeldaHud/");
                    }
                }
            }
            ws.Log.Output = (_, __) => { };
        }
        int nbrIcons = 74;
        public static Bitmap[] iconSet;
        public Bitmap globalTimer = new Bitmap("IconsSets\\Global\\timer.png");
        public Bitmap globalCount = new Bitmap("IconsSets\\Global\\count.png");

        public void loadIconsSet(string data)
        {
            nbrIcons = Directory.GetFiles(data).Length;

            iconSet = new Bitmap[nbrIcons];
            for (int i = 0; i < nbrIcons; i++)
            {
                if (File.Exists(data + "\\" + i.ToString("D4") + ".png"))
                {
                    iconSet[i] = new Bitmap(data + "\\" + i.ToString("D4") + ".png");
                    iconSet[i].MakeTransparent(Color.Fuchsia);
                }
            }
            if (currentBgr != "None")
            {
                bgr = new Bitmap(currentBgr);
                bgr.MakeTransparent(Color.Fuchsia);
            }
            currentIconset = data;
        }

        public void drawIcons()
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.Clear(clearColor);

            ColorMatrix colorMatrix = new ColorMatrix(
            new float[][]
            {
                new float[] {.15f, .15f, .15f, 0, 0},
                new float[] {.16f, .16f, .16f, 0, 0},
                new float[] {.06f, .06f, .06f, 0, 0},
                new float[] {0, 0, 0,1f, 0},
                new float[] {0, 0, 0, 0f, 0f}
            });

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(colorMatrix);

            for (int x = 0; x < widthIcons; x++)
            {
                for (int y = 0; y < heightIcons; y++)
                {
                    if (itemsArray[x, y] != null)
                    {
                        try
                        {
                            if (currentBgr != "None")
                            {
                                if (bgr != null)
                                {
                                    if (itemsArray[x, y].name != "Timer")
                                    {
                                        g.DrawImage(bgr, new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel);
                                    }
                                }
                            }
                            if (itemsArray[x, y].name != "Timer")
                            {
                                if (itemsArray[x, y].on == true)
                                {

                                    g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel);
                                    if (itemsArray[x, y].count == true)
                                    {
                                        if (itemsArray[x, y].counter >= 1)
                                        {
                                            drawCounter(g, x * 32, y * 32, itemsArray[x, y].counter);
                                        }
                                    }
                                }
                                else
                                {
                                    g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, ia);
                                }
                            }
                            else
                            {
                                if (timer)
                                {

                                }
                                else
                                {
                                    g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, ia);
                                }
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
            if (timer)
            {
                TimeSpan objt = DateTime.Now.Subtract(timestarted);
                if (timerend == false)
                {
                    objt = DateTime.Now.Subtract(timestarted);
                }
                else
                {

                    objt = timeended.Subtract(timestarted);
                }

                //g.DrawString(objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"), label1.Font, Brushes.White, new Point(timerpospixel.X - 2, timerpospixel.Y + 4));
                drawTime(g, objt);
            }

            pictureBox1.Refresh();
        }

        public void drawCounter(Graphics g, int x, int y, byte count)
        {
            string s = count.ToString("D2");
            if (count <= 9)
            {
                g.DrawImage(globalCount, new Rectangle(x + 22, y + 18, 10, 14), count * 10, 0, 10, 14, GraphicsUnit.Pixel);
            }
            else
            {
                int b = ((int)s[0] - 48);
                g.DrawImage(globalCount, new Rectangle(x + 12, y + 18, 10, 14), b * 10, 0, 10, 14, GraphicsUnit.Pixel);
                b = ((int)s[1] - 48);
                g.DrawImage(globalCount, new Rectangle(x + 22, y + 18, 10, 14), b * 10, 0, 10, 14, GraphicsUnit.Pixel);
            }


            g.DrawImage(globalCount, new Rectangle((x * 32) + 22, (y * 32) + 18, 10, 14), 0, 0, 32, 32, GraphicsUnit.Pixel);
        }

        public void drawTime(Graphics g, TimeSpan time)
        {

            string s = time.Hours.ToString("D2");
            int b = ((int)s[0] - 48);
            g.DrawImage(globalTimer, timerpospixel.X + 1, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//hour1
            b = ((int)s[1] - 48);
            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 13, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//hour2

            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 26, timerpospixel.Y + 4, new Rectangle(120, 0, 8, 26), GraphicsUnit.Pixel);//:
            s = time.Minutes.ToString("D2");
            b = ((int)s[0] - 48);
            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 35, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute1
            b = ((int)s[1] - 48);
            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 48, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute2

            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 61, timerpospixel.Y + 4, new Rectangle(120, 0, 8, 26), GraphicsUnit.Pixel);//:
            s = time.Seconds.ToString("D2");
            b = ((int)s[0] - 48);
            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 69, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute1
            b = ((int)s[1] - 48);
            g.DrawImage(globalTimer, (timerpospixel.X + 1) + 82, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute2

        }


        protected void OnTitlebarClick(Point pos)
        {
            contextMenuStrip1.Show(pos);
        }


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0xa4)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                OnTitlebarClick(pos);
                return;
            }
            base.WndProc(ref m);
        }

        Graphics g;
        public static List<CustomItem> itemsList = new List<CustomItem>();
        public static CustomItem[,] itemsArray = new CustomItem[24, 24];
        public static Color clearColor = Color.FromArgb(10, 10, 15);

        public void addItems()
        {
            /* itemsList.Add(new CustomItem(new byte[] { 0, 48, 49, 49 }, "Bow"));//Bow 0
             itemsList.Add(new CustomItem(new byte[] { 1 }, "Blue Boomerang"));//Blue Boomerang 1
             itemsList.Add(new CustomItem(new byte[] { 2 }, "HookShot"));//Hookshot 2
             itemsList.Add(new CustomItem(new byte[] { 3 }, "Bombs"));//Bombs 3
             itemsList.Add(new CustomItem(new byte[] { 4, 56 }, "Mushroom"));//Mushroom 4
             itemsList.Add(new CustomItem(new byte[] { 5 }, "Fire Rod"));//Fire Rod 5
             itemsList.Add(new CustomItem(new byte[] { 6 }, "Ice Rod"));//Ice Rod 6
             itemsList.Add(new CustomItem(new byte[] { 7 }, "Bombos"));//Bombos 7
             itemsList.Add(new CustomItem(new byte[] { 8 }, "Ether"));//Ether 8 
             itemsList.Add(new CustomItem(new byte[] { 9 }, "Quake"));//Quake 9
             itemsList.Add(new CustomItem(new byte[] { 10 }, "Lamp"));//Lamp 10
             itemsList.Add(new CustomItem(new byte[] { 11 }, "Hammer"));//Hammer  11
             itemsList.Add(new CustomItem(new byte[] { 12, 57 }, "Shovel"));//Shovel 12
             itemsList.Add(new CustomItem(new byte[] { 13 }, "Net"));//Net 13 
             itemsList.Add(new CustomItem(new byte[] { 14 }, "Book"));//Book 14
             itemsList.Add(new CustomItem(new byte[] { 15 }, "Cane Somaria"));//Cane Somaria 15
             itemsList.Add(new CustomItem(new byte[] { 16 }, "Cane of Byrna"));//Cane Byrna 16
             itemsList.Add(new CustomItem(new byte[] { 17 }, "Cape"));//Cape 17
             itemsList.Add(new CustomItem(new byte[] { 18, 18 }, "Mirror"));//Mirror 18
             itemsList.Add(new CustomItem(new byte[] { 19, 43 }, "Power Glove"));//Power Glove 19
             itemsList.Add(new CustomItem(new byte[] { 20 }, "Boots"));//Boots 20
             itemsList.Add(new CustomItem(new byte[] { 21 }, "Flippers"));//Flippers 21
             itemsList.Add(new CustomItem(new byte[] { 22 }, "Moon Pearl"));//MoonPearl 22
             itemsList.Add(new CustomItem(new byte[] { 23, 38, 39, 40 }, "Sword"));//Sword 23
             itemsList.Add(new CustomItem(new byte[] { 24, 44, 45 }, "Shield"));//Shield 24
             itemsList.Add(new CustomItem(new byte[] { 25, 41, 42 }, "Tunic"));//Tunic //25
             itemsList.Add(new CustomItem(new byte[] { 26, 26, 50, 51, 52, 53, 54, 54 }, "Bottle 1",false,true));//Bottle1 26
             itemsList.Add(new CustomItem(new byte[] { 26, 26, 50, 51, 52, 53, 54, 54 }, "Bottle 2",false,true));//Bottle2 27
             itemsList.Add(new CustomItem(new byte[] { 26, 26, 50, 51, 52, 53, 54, 54 }, "Bottle 3",false,true));//Bottle3 28
             itemsList.Add(new CustomItem(new byte[] { 26, 26, 50, 51, 52, 53, 54, 54 }, "Bottle 4",false,true));//Bottle4 29
             itemsList.Add(new CustomItem(new byte[] { 27 }, "Eastern Pendant"));//Eastern (Green) //30
             itemsList.Add(new CustomItem(new byte[] { 28 }, "Desert Pendant"));//Desert (Blue) 31
             itemsList.Add(new CustomItem(new byte[] { 29 }, "Hera Pendant"));//Hera (Red) 32
             itemsList.Add(new CustomItem(new byte[] { 30 }, "Crystal 1"));//Crystal 1  33 pod
             itemsList.Add(new CustomItem(new byte[] { 31 }, "Crystal 2"));//Crystal 2  34 swamp
             itemsList.Add(new CustomItem(new byte[] { 32 }, "Crystal 3"));//Crystal 3 35 sw
             itemsList.Add(new CustomItem(new byte[] { 33 }, "Crystal 4"));//Crystal 4 36 tt
             itemsList.Add(new CustomItem(new byte[] { 34 }, "Crystal 5"));//Crystal 5 37 ice
             itemsList.Add(new CustomItem(new byte[] { 35 }, "Crystal 6"));//Crystal 6 38 mm
             itemsList.Add(new CustomItem(new byte[] { 36 }, "Crystal 7"));//Crystal 7 //39 trock
             itemsList.Add(new CustomItem(new byte[] { 37 }, "Red Boomerang"));//Red Boomerang 40
             itemsList.Add(new CustomItem(new byte[] { 46 }, "Powder"));//Powder 41
             itemsList.Add(new CustomItem(new byte[] { 47 }, "Flute"));//Flute 42
             itemsList.Add(new CustomItem(new byte[] { 55 }, "Agahnim"));//Agahnim //43
             itemsList.Add(new CustomItem(new byte[] { 58, 59 }, "Chest"));//Agahnim //44
             itemsList.Add(new CustomItem(new byte[] { 60 }, "Sanc Heart"));//Agahnim //45
             itemsList.Add(new CustomItem(new byte[] { 61 }, "Item Id 61"));//Agahnim //46
             itemsList.Add(new CustomItem(new byte[] { 62 }, "Item Id 62"));//Agahnim //47
             itemsList.Add(new CustomItem(new byte[] { 63 }, "Item Id 63"));//Agahnim //48
             itemsList.Add(new CustomItem(new byte[] { 64 }, "Item Id 64"));//Agahnim //49
             itemsList.Add(new CustomItem(new byte[] { 65 }, "Item Id 65"));//Agahnim //50
             itemsList.Add(new CustomItem(new byte[] { 66,67,68,69 }, "Heart Pieces",true));//Agahnim //51
             itemsList.Add(new CustomItem(new byte[] { 70, 71, 72, 73 }, "Bottle Counter"));//Agahnim //52
             itemsList.Add(new CustomItem(new byte[] { 26, 71, 72, 73 }, "Bottle Counter2"));//Agahnim //53*/

            //itemsList[51].on = true;

            string[] s = File.ReadAllLines("itemlist.txt");

            foreach (string l in s)
            {
                string line = l;
                string name = "";
                byte[] order = new byte[0];
                bool loop = false;
                bool bottle = false;
                bool count = false;
                bool dungeon = false;
                string eval = "";

                line = line.TrimEnd(new Char[] { ';' });

                //Read character
                if (line.Length > 1)
                {
                    if (line[0] == '/' && line[1] == '/')//skip line
                    {

                    }
                    else
                    {
                        if (line[0] == ':')
                        {
                            for (int i = 1; i < line.Length; i++)//name
                            {
                                if (line[i] == ',')
                                {
                                    break;
                                }
                                name += line[i];
                            }
                            int sta = line.IndexOf('{') + 1;
                            int end = line.IndexOf('}');
                            string ord = line.Substring(sta, end - sta);
                            string[] ords = ord.Split(',');
                            order = new byte[ords.Length];
                            for (int i = 0; i < ords.Length; i++)
                            {
                                order[i] = Convert.ToByte(ords[i]);
                            }
                            string last = line.Substring(end + 2);

                            // don't split the eval block
                            string[] loopbottle = last.Split(new char[] { ',' }, 5);
                            if (loopbottle[0] == "false")
                            {
                                loop = false;
                            }
                            if (loopbottle[0] == "true")
                            {
                                loop = true;
                            }

                            if (loopbottle[1] == "false")
                            {
                                bottle = false;
                            }
                            if (loopbottle[1] == "true")
                            {
                                bottle = true;
                            }

                            if (loopbottle[2][0] == 'f')
                            {
                                count = false;
                            }
                            if (loopbottle[2][0] == 't')
                            {
                                count = true;
                            }

                            if (loopbottle[3][0] == 'f')
                            {
                                dungeon = false;
                            }
                            if (loopbottle[3][0] == 't')
                            {
                                dungeon = true;
                            }

                            // eval function is at the end
                            if (loopbottle.Length > 4 && !loopbottle[4].IsNullOrEmpty())
                            {
                                eval = loopbottle[4];
                            }

                            CustomItem ci = new CustomItem(order, name, loop, bottle, count, dungeon, eval, null, null);
                            if (count == true)
                            {
                                ci.on = true;
                            }
                            itemsList.Add(ci);

                        }
                    }
                }
            }

            // create evals
            Parallel.ForEach(itemsList, (item) =>
            //foreach (var item in itemsList)
            {
                var r = Tuple.Create<object, System.Reflection.MethodInfo>(null, null);
                if (!item.eval.IsNullOrEmpty() && item.eval != "default")
                {
                    r = EvalCSCode.EvalCSCode.GetWithParamType<byte[], int, int>(item.eval, "d", "i", "c");
                    item.co = r.Item1;
                    item.mi = r.Item2;
                }
            }
            );

        }


        public void setDefaultItems()
        {
            //30,31,32,17,25,60,36,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
            byte[] ditems = new byte[] {15 ,0 ,1 ,2 ,3 ,41,30,
                                        16,5 ,6 ,7 ,8 ,9 ,31,
                                        18,10,11,42,13, 14,32,
                                        27,46,44,255,255,45,17,
                                        19,22,61,62,51,24,25,
                                        20,21,40,12,4,23,60,
                                        34,37,39,38,35,33,36};
            int i = 0;
            for (int y = 0; y < 7; y++)
            {
                for (int x = 0; x < 7; x++)
                {

                    if (itemsList.Count >= ditems[i])
                    {
                        itemsArray[x, y] = itemsList[ditems[i]];

                    }
                    i++;
                }
            }
        }


        byte widthIcons = 7;
        byte heightIcons = 6;
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

            //Resize the form to always be a multiple of 32
            int w = (this.Size.Width / 32) * 32;
            int h = (this.Size.Height / 32) * 32;
            if (w >= 24 * 32)
            {
                w = (24 * 32);
            }
            if (h >= 24 * 32)
            {
                h = (24 * 32);
            }
            if (h <= 64)
            {
                h = 64;
            }
            if (w <= 48)
            {
                w = 32;
            }
            this.Size = new Size((w) + 16, (h) + 7);
            widthIcons = (byte)(w / 32);
            heightIcons = (byte)((h / 32) - 1);
            drawIcons();

        }
        bool editMode = false;
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (toolStripMenuItem1.Checked)
            {
                editMode = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                editMode = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
            }
        }
        bool mDown = false;
        bool startDrag = true;
        CustomItem dragged = null;
        CustomItem swapped = null;
        int xdpos = -1;
        int ydpos = -1;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (editMode == true)
            {
                if (mDown == false)
                {
                    uiChanged = true;
                    dragged = itemsArray[(e.X / 32), (e.Y / 32)];
                    startDrag = true;
                    xdpos = (e.X / 32);
                    ydpos = (e.Y / 32);
                    Cursor = Cursors.Hand;
                    mDown = true;
                }
            }
        }

        //private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (editMode == true)
            {
                if (mDown == true)
                {
                    if (startDrag == true)
                    {
                        if ((e.X / 32) < 24 && (e.X / 32) >= 0)
                        {
                            if ((e.Y / 32) < 24 && (e.Y / 32) >= 0)
                            {
                                uiChanged = true;
                                swapped = itemsArray[(e.X / 32), (e.Y / 32)];
                                itemsArray[(e.X / 32), (e.Y / 32)] = dragged;
                                if (dragged != null)
                                {
                                    if (dragged.name == "Timer")
                                    {
                                        timerpospixel = new Point((e.X / 32) * 32, (e.Y / 32) * 32);
                                    }
                                }
                                itemsArray[xdpos, ydpos] = swapped;
                                swapped = null;
                                dragged = null;
                                startDrag = false;
                                xdpos = -1;
                                ydpos = -1;
                                Cursor = Cursors.Default;
                                mDown = false;
                            }
                        }
                    }
                }
            }
            drawIcons();
        }


        DateTime timestarted;
        DateTime timeended;
        bool timerend = false;
        bool uiChanged = false;
        //private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int mX = (e.X / 32);
            int mY = (e.Y / 32);
            if (editMode == false)
            {
                if (itemsArray[mX, mY] != null)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        /* behavior for dungeon type icons are different */
                        if (itemsArray[mX, mY].dungeon)
                        {
                            if (itemsArray[mX, mY].on)
                            {
                                itemsArray[mX, mY].on = false;
                            }
                            else
                            {
                                itemsArray[mX, mY].on = true;
                            }
                        }
                        else if (itemsArray[mX, mY].on)
                        {
                            if (itemsArray[mX, mY].count)
                            {
                                itemsArray[mX, mY].counter++;
                            }
                            if (itemsArray[mX, mY].level < itemsArray[mX, mY].iconsId.Length - 1)
                            {
                                if (itemsArray[mX, mY].name == "Timer_StartDone")
                                {
                                    timeended = DateTime.Now;
                                    timerend = true;
                                }
                                //if (itemsArray)
                                if (itemsArray[mX, mY].bottle)
                                {
                                    if (itemsArray[mX, mY].level == 0)
                                    {
                                        itemsArray[mX, mY].level += 2;
                                    }
                                    else
                                    {
                                        if (itemsArray[mX, mY].level < 6)
                                        {
                                            itemsArray[mX, mY].level++;
                                        }
                                    }
                                }
                                else
                                {
                                    itemsArray[mX, mY].level++;
                                }
                                //drawIcons();
                            }
                            else
                            {
                                if (itemsArray[mX, mY].level == itemsArray[mX, mY].iconsId.Length - 1)
                                {
                                    if (itemsArray[mX, mY].loop == true)
                                    {
                                        itemsArray[mX, mY].level = 0;
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (itemsArray[mX, mY].name == "Timer_Start" || itemsArray[mX, mY].name == "Timer_StartDone")
                            {

                                for (int x = 0; x < widthIcons; x++)
                                {
                                    for (int y = 0; y < heightIcons; y++)
                                    {
                                        if (itemsArray[x, y] != null)
                                        {
                                            if (itemsArray[x, y].name == "Timer")
                                            {
                                                timerpospixel = new Point(x * 32, y * 32);
                                                timer2.Enabled = true;
                                                timer = true;
                                                timestarted = DateTime.Now;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (itemsArray[mX, mY].name == "Timer_Done")
                            {
                                timeended = DateTime.Now;
                                timerend = true;
                            }
                            itemsArray[mX, mY].on = true;
                            //drawIcons();
                        }
                        TimeSpan objt = DateTime.Now.Subtract(timestarted);
                        if (itemsArray[mX, mY].loop == false)
                        {
                            item_found.Add(itemsArray[mX, mY].name + " Added at " + objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"));
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        /* behavior for dungeon type icons are different */
                        if (itemsArray[mX, mY].dungeon)
                        {
                            /* change the levels for dungeons */
                            if (itemsArray[mX, mY].level < itemsArray[mX, mY].iconsId.Length - 1)
                            {
                                itemsArray[mX, mY].level++;
                            }
                            else
                            {
                                itemsArray[mX, mY].level = 0;
                            }
                        }
                        else if (itemsArray[mX, mY].on)
                        {
                            if (itemsArray[mX, mY].name == "Timer_Start" || itemsArray[mX, mY].name == "Timer_StartDone")
                            {
                                if (itemsArray[mX, mY].level == 0)
                                {
                                    timer = false;
                                }
                            }


                            if (itemsArray[mX, mY].count == false)
                            {
                                if (itemsArray[mX, mY].name == "Timer_Done" || itemsArray[mX, mY].name == "Timer_StartDone")
                                {

                                    timerend = false;
                                }
                                TimeSpan objt = DateTime.Now.Subtract(timestarted);
                                if (itemsArray[mX, mY].loop == false)
                                {
                                    item_found.Add(itemsArray[mX, mY].name + " Removed at " + objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"));
                                }
                                if (itemsArray[mX, mY].level > 0)
                                {
                                    itemsArray[mX, mY].level--;
                                    if (itemsArray[mX, mY].bottle)
                                    {
                                        if (itemsArray[mX, mY].level == 1)
                                        {
                                            itemsArray[mX, mY].level--;
                                        }
                                    }
                                }
                                else if (itemsArray[mX, mY].level == 0)
                                {
                                    if (itemsArray[mX, mY].loop == false)
                                    {
                                        itemsArray[mX, mY].on = false;
                                    }
                                }
                            }
                            else
                            {
                                itemsArray[mX, mY].counter--;
                            }
                        }
                    }
                }
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    ItemSelectorForm itemForm = new ItemSelectorForm();
                    if (itemForm.ShowDialog() == DialogResult.OK)
                    {
                        if (itemForm.selectedItem == 255)
                        {
                            itemsArray[mX, mY] = null;
                        }
                        else
                        {
                            itemsArray[mX, mY] = itemsList[itemForm.selectedItem];
                        }
                    }
                }
            }
            drawIcons();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm();
            of.checkBox1.Checked = checkUpdate;
            of.iconset = currentIconset;
            of.bgr = currentBgr;
            of.ShowDialog();
            checkUpdate = of.checkBox1.Checked;
            currentBgr = of.bgr;
            loadIconsSet(of.iconset);
            drawIcons();
        }

        private void topMostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TopMost == false)
            {
                TopMost = true;
                topMostToolStripMenuItem.Checked = true;
            }
            else
            {
                TopMost = false;
                topMostToolStripMenuItem.Checked = false;
            }
        }
        bool autoUpdate = false;
        bool autoUpdateFile = false;
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                toolStripMenuItem3.Checked = false;
                autoUpdateUSBToolStripMenuItem.Checked = false;
                autoUpdate = false;
                timer1.Enabled = false;
            }
            else
            {
                autoUpdateUSBToolStripMenuItem.Checked = false;
                toolStripMenuItem3.Checked = true;
                autoUpdate = true;
                autoUpdateFile = true;
                timer1.Interval = 5000;
                timer1.Enabled = true;
            }

        }

        private void Timeout()
        {
            toolStripMenuItem3.Checked = false;
            autoUpdateUSBToolStripMenuItem.Checked = false;
            autoUpdate = false;
            timer1.Enabled = false;
            this.Text = "Connection Timeout";
        }

        public class ResponseType
        {
            public List<string> Results { get; set; }
        }
        ResponseType _rsp = new ResponseType();
        AutoResetEvent _ev = new AutoResetEvent(false);
        int _bufferIndex = 0;
        int _fileSize = 0;
        Byte[] _buffer = new Byte[0x500];

        private void ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Type == Opcode.Text)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                _rsp = serializer.Deserialize<ResponseType>(e.Data);
                _ev.Set();
            }
            else if (e.Type == Opcode.Binary)
            {
                Array.Copy(e.RawData, 0, _buffer, _bufferIndex, e.RawData.Length);
                _bufferIndex += e.RawData.Length;
                if (_bufferIndex >= _fileSize)
                {
                    _bufferIndex = 0;
                    _ev.Set();
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            fSerialPorts.portListView.Clear();
            fSerialPorts.portListView.Columns.Add("Num");
            fSerialPorts.portListView.Columns.Add("Name");
            fSerialPorts.portListView.Columns.Add("Description");

            if (ws.ReadyState != WebSocketState.Closed)
            {
                ws.Close();

                var c = autoUpdateUSBToolStripMenuItem.Checked;

                toolStripMenuItem3.Checked = false;
                //autoUpdateUSBToolStripMenuItem.Checked = false;
                autoUpdate = false;
                timer1.Enabled = false;

                if (!c)
                {
                    this.Text = "ALTTP Rando HUD";
                    //clearItemsToolStripMenuItem.PerformClick();
                    return;
                }
            }

            // reopen websocket
            ws = new WebSocket("ws://localhost:8080/");
            ws.Log.Output = (_, __) => { };

            ws.OnMessage += ws_OnMessage;

            ws.WaitTime = TimeSpan.FromSeconds(3);
            ws.Connect();
            if (ws.ReadyState != WebSocketState.Open && ws.ReadyState != WebSocketState.Connecting)
            {
                Timeout();
                return;
            }

            string msg = "{\"Opcode\":\"DeviceList\", \"Space\":\"SNES\" }";
            ws.Send(msg);
            _ev.WaitOne();
            //_ev.Reset();

            int index = 0;
            foreach (var device in _rsp.Results)
            {
                ListViewItem item = new ListViewItem(index++.ToString());
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device));
                //item.SubItems.Add(new ListViewItem.ListViewSubItem(item, device.bus_description.Trim() + " / " + device.description));
                fSerialPorts.portListView.Items.Add(item);
            }

            fSerialPorts.ShowDialog();

            if (fSerialPorts.portListView.SelectedIndices.Count != 1)
            {
                toolStripMenuItem3.Checked = false;
                autoUpdateUSBToolStripMenuItem.Checked = false;
                autoUpdate = false;
                timer1.Enabled = false;
                ws.Close();
            }
            else
            {
                msg = "{\"Opcode\":\"Attach\", \"Space\":\"SNES\", \"Operands\":[\"" + fSerialPorts.portListView.SelectedItems[0].SubItems[1].Text + "\"]}";
                ws.Send(msg);
                //_ev.WaitOne();

                toolStripMenuItem3.Checked = false;
                //autoUpdateUSBToolStripMenuItem.Checked = true;
                autoUpdate = true;
                autoUpdateFile = false;
                timer1.Interval = 200;
                timer1.Enabled = true;
            }
        }

        private void Form1_FormClosing(object sender, CancelEventArgs e)
        {
            if (uiChanged == true)
            {
                if (MessageBox.Show("Your Layout has changed do you want to save?", "Warning",
                   MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //e.Cancel = true;
                    save();
                }
            }
        }

        public void autoUpdateHud()
        {
            if (autoUpdate == true && (!autoUpdateFile || File.Exists(openFileDialog1.FileName)))
            {
                byte[] buffer = new byte[0x500];

                try
                {
                    if (autoUpdateFile)
                    {
                        FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                        fs.Position = 0x1E00;
                        fs.Read(buffer, 0x300, 255);
                        fs.Close();
                    }
                    else
                    {

                        Text = "Connected: " + fSerialPorts.portListView.SelectedItems[0].SubItems[1].Text;

                        string msg = "{\"Opcode\":\"GetAddress\", \"Space\":\"SNES\", \"Operands\":[\"007FD7\", \"1\", \"180213\", \"2\"]}";
                        _fileSize = 3;
                        ws.Send(msg);
                        if (!_ev.WaitOne(10000)) { Timeout(); return; }

                        if (_buffer[0] < 0xB || (_buffer[1] == 0 && _buffer[2] == 1))
                        {
                            //string msg = "{\"Opcode\":\"GetAddress\", \"Space\":\"SNES\", \"Operands\":[\"F5F340\", \"FF\"]}";
                            msg = "{\"Opcode\":\"GetAddress\", \"Space\":\"SNES\", \"Operands\":[\"F5F000\", \"500\"]}";
                            _fileSize = 0x500;
                            ws.Send(msg);
                            if (!_ev.WaitOne(10000)) { Timeout(); return; }
                            //_ev.Reset();

                            Buffer.BlockCopy(_buffer, 0, buffer, 0, buffer.Length);
                        }
                        else
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.Text = e.Message.ToString();
                }

                try
                {
                    // perform generalized eval for all supported types
                    foreach (var item in itemsList)
                    {
                        if (!item.eval.IsNullOrEmpty())
                        {
                            if (item.eval == "default")
                            {
                                var v = buffer[0x340 + item.iconsId[0]];
                                item.on = v != 0;
                                if (item.iconsId.Length > 1) item.level = Convert.ToByte(v > 0 ? (v - 1) & 0xFF : v);
                            }
                            else
                            {
                                try
                                {
                                    var r = item.mi.Invoke(item.co, new object[] { buffer, item.iconsId[0] + 0x340, item.iconsId.Length });

                                    //var r = EvalCSCode.EvalCSCode.EvalWithParamType(item.eval, "d", buffer, "i", item.iconsId[0]);
                                    item.on = (bool)r.GetType().GetProperty("on").GetValue(r, null);
                                    var level = Convert.ToByte((int)r.GetType().GetProperty("level").GetValue(r, null));
                                    if (!item.dungeon && level != 0xFF) item.level = level;
                                }
                                catch (Exception x)
                                {
                                    MessageBox.Show("Error in item: " + item.name + "\neval: '" + item.eval + "'\nmessage: " + x.Message);
                                    // only show the error once
                                    item.eval = "";
                                }
                            }
                        }
                    }

                    drawIcons();
                }
                catch (Exception e)
                {
                    this.Text = e.Message.ToString();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Autoupdate
            timer1.Enabled = false;
            autoUpdateHud();
            timer1.Enabled = true;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://zarby89.github.io/ZeldaHud/");
        }
        public bool checkUpdate = true;
        private void save()
        {
            string[] config = new string[10];
            config[0] = "width=" + widthIcons;
            config[1] = "height=" + heightIcons;
            config[2] = "items=" + stringitems();
            byte b = 0;
            if (TopMost)
            { b = 1; }
            else
            { b = 0; }
            config[3] = "topmost=" + b;
            config[4] = "winposx=" + this.Location.X;
            config[5] = "winposy=" + this.Location.Y;
            config[6] = "color=" + clearColor.ToArgb();
            config[7] = "iconset=" + currentIconset;
            if (checkUpdate)
            { b = 1; }
            else
            { b = 0; }
            config[8] = "checkupdate=" + b;
            config[9] = "background=" + currentBgr;

            File.WriteAllLines("layout.config", config);
        }
        private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
            uiChanged = false;
        }

        public void loadLayout(string sx = "Layout.config")
        {
            if (File.Exists(sx))
            {
                string[] s = File.ReadAllLines(sx);
                string[] itl = s[2].Split('=')[1].Split(',');
                int w = Convert.ToInt32(s[0].Split('=')[1]);
                int h = Convert.ToInt32(s[1].Split('=')[1]);
                this.Location = new Point(Convert.ToInt32(s[4].Split('=')[1]), Convert.ToInt32(s[5].Split('=')[1]));
                int b = Convert.ToInt32(s[3].Split('=')[1]);
                if (b == 1)
                { TopMost = true; topMostToolStripMenuItem.Checked = true; }
                else
                { TopMost = false; topMostToolStripMenuItem.Checked = false; }
                b = Convert.ToInt32(s[8].Split('=')[1]);
                if (b == 1)
                { checkUpdate = true; }
                else
                { checkUpdate = false; }
                clearColor = Color.FromArgb(Convert.ToInt32(s[6].Split('=')[1]));
                currentIconset = s[7].Split('=')[1];
                if (s.Length >= 10)
                {
                    currentBgr = s[9].Split('=')[1];
                }



                int p = 0;
                for (int x = 0; x < 24; x++)
                {
                    for (int y = 0; y < 24; y++)
                    {
                        if (Convert.ToInt32(itl[p]) != -1)
                        {
                            itemsArray[x, y] = itemsList[Convert.ToInt32(itl[p])];
                        }
                        else
                        {
                            itemsArray[x, y] = null;
                        }
                        p++;
                    }
                }
                this.Size = new Size((w * 32) + 16, ((h + 1) * 32) + 7);
                widthIcons = (byte)(w);
                heightIcons = (byte)((h));
                drawIcons();
            }
        }


        private string stringitems()
        {
            string s = "";
            int p = 0;
            for (int x = 0; x < 24; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    if (itemsArray[x, y] != null)
                    {
                        s += itemsList.FindIndex(i => i.Equals(itemsArray[x, y])).ToString();
                        s += ",";
                    }
                    else
                    {
                        s += "-1,";
                    }
                    p++;
                }
            }
            return s;
        }

        private void importOldLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
            {

            }
            else
            {
                /*string[] s = File.ReadAllLines(openFileDialog2.FileName);

                int w = Convert.ToInt32(s[0].Split('=')[1]);
                int h = Convert.ToInt32(s[1].Split('=')[1]);
                string[] itl = s[3].Split('=')[1].Split(',');
                int p = 0;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (Convert.ToInt32(itl[p]) != 254)
                        {
                            itemsArray[x, y] = itemsList[Convert.ToInt32(itl[p])];
                        }
                        else
                        {
                            itemsArray[x, y] = null;
                        }
                        p++;
                    }
                }
                        this.Size = new Size((w*32) + 16, ((h+1)*32) + 7);
                widthIcons = (byte)(w);
                heightIcons = (byte)((h) - 1);*/

                loadLayout(openFileDialog2.FileName);
                drawIcons();
            }
        }

        private void clearItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 24; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    if (itemsArray[x, y] != null)
                    {
                        itemsArray[x, y].on = false;
                        itemsArray[x, y].level = 0;
                    }
                }
            }
            drawIcons();
        }

        private void showStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timestarted = DateTime.Now;
        }

        private void showStatsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Stats f = new Stats();
            for (int i = 0; i < item_found.Count; i++)
            {
                f.richTextBox1.AppendText(item_found[i] + "\n");
            }
            f.ShowDialog();
        }

        private void imageStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap b = new Bitmap(288, 384);
            Bitmap bgr = new Bitmap("bgrtimeico.png");
            Graphics g = Graphics.FromImage(b);
            int y = 0;
            int x = 0;
            for (int i = 0; i < item_found.Count; i++)
            {
                string s = item_found[i].Substring(item_found[i].Length - 8, 8);
                string[] ss = item_found[i].Split(' ');
                g.DrawImage(bgr, new Point(x * 72, y * 24));
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(iconSet[Convert.ToInt32(ss[0])], new Rectangle((x * 72) + 2, (y * 24) + 4, 16, 16), 0, 0, 32, 32, GraphicsUnit.Pixel);
                g.DrawString(s, new Font(label1.Font, FontStyle.Regular), Brushes.White, new PointF((x * 72) + 20, (y * 24) + 05));
                x++;
                if (x >= 4)
                {
                    y++;
                    x = 0;
                }
            }
            b.Save("Test.png");
        }

        bool timer = false;
        Point timerpospixel;
        private void timer2_Tick(object sender, EventArgs e)
        {
            drawIcons();
        }

    }
}
