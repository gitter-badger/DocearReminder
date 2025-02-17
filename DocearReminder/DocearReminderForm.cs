﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using yixiaozi.Config;
using yixiaozi.Model.DocearReminder;
using yixiaozi.MyConvert;
using yixiaozi.Security;
using yixiaozi.Windows;
using yixiaozi.WinForm.Control;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;

namespace DocearReminder
{
    public partial class DocearReminderForm : Form
    {
        #region 全局变量
        public System.Windows.Forms.Timer hoverTimer = new System.Windows.Forms.Timer();
        public System.Windows.Forms.Timer addFanQieTimer = new System.Windows.Forms.Timer();
        public static string PassWord = "";
        public static int tomatoCount = 0;
        private bool InReminderBool = true;
        private bool IsSelectReminder = true;
        private int reminderSelectIndex = -1;
        private int mindmapSelectIndex = -1;
        private bool InMindMapBool = true;
        private bool isCodeFenlei = false;
        SoundPlayer simpleSound = new SoundPlayer();
        public static bool[] fanqiePosition = new bool[(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 30) / 120];
        private HotKeys hotKeys = new HotKeys();
        private IniFile ini = new IniFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        private string[] noFolder = new string[] { };
        private string[] noFiles = new string[] { };
        private CustomCheckedListBox mindmaplis1 = new CustomCheckedListBox();
        private CustomCheckedListBox.ObjectCollection mindmaplist_backup;
        public string CurrentLanguage = "";
        public string jsonHasMindmaps = "";
        public List<mindmapfile> mindmapfiles = new List<mindmapfile>();
        public List<String> remindmapsList = new List<String>();
        public List<mindmapfile> mindmapfilesAll = new List<mindmapfile>();
        public List<node> nodes = new List<node>();
        public List<node> nodesicon = new List<node>();
        public List<node> allfiles = new List<node>();
        public int mindmapnumdesc = 0;
        private DirectoryInfo rootrootpath = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory).Parent.Parent;
        private DirectoryInfo rootpath = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory).Parent.Parent;
        public bool isInReminderlistSelect = false;
        public string mindmapPath = "";
        string CalendarImagePath = "";
        public static bool isZhuangbi = false;
        AutoCompleteStringCollection acsc = new AutoCompleteStringCollection();
        public bool showfenge = false;
        public bool isHasNoFenleiModel = false;
        Reminder reminderObjectOut = new Reminder();
        string showMindmapName = "";//用于Tree中当前导图的名称
        string renameTaskName = "";
        string renameMindMapPath = "";
        string renameMindMapFileIDParent = "";
        string renameMindMapFileID = "";
        bool isRename = false;
        List<string> pathArr = new List<string>();
        public static List<string> ignoreSuggest = new List<string>();
        public static string command = "ga;gc;";
        public static List<string> usedSuggest = new List<string>();
        public static List<string> usedSuggest2 = new List<string>();
        public static List<string> usedSuggest3 = new List<string>();
        public static List<string> usedSuggest4 = new List<string>();
        public static List<string> unchkeckmindmap = new List<string>();
        public static Color BackGroundColor = Color.White;
        bool isRefreshMindmap = false;
        public List<MyListBoxItemRemind> reminderboxList = new List<MyListBoxItemRemind>();
        int ebconfig = 98765432;
        string ebdefault = "";
        List<MyListBoxItemRemind> RemindersOtherPath = new List<MyListBoxItemRemind>();
        string nodeIconString = "";
        bool lockForm = false;
        bool isSearchFileOrNode = false;
        DirectoryInfo fileTreePath;
        bool isPlaySound = false;
        bool playBackGround = false;
        static string logpass = "niqishihenhao";
        bool allFloder = false;
        bool IsEncryptBool = false;
        object reminderlistSelectedItem;
        List<StationInfo> suggestListData = new List<StationInfo>();
        Encrypt encrypt;
        Encrypt encryptlog;
        bool selectedpath = true;
        UsedTimer usedTimer = new UsedTimer();
        Guid currentUsedTimerId;

        #endregion
        public DocearReminderForm()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            try
            {
                SwitchToLanguageMode();
                GetIniFile();
                ReadBookmarks();
                if (RecentlyFileHelper.GetStartFiles() != null)
                {
                    suggestListData.AddRange(RecentlyFileHelper.GetStartFiles());
                }
                //频繁刷新导致界面闪烁解决方法我也不知道有没有用
                pathArr.Add(ini.ReadString("path", "rootpath", ""));
                mindmapPath = ini.ReadString("path", "rootpath", "");
                //HookManager.KeyDown += HookManager_KeyDown;
                //HookManager.KeyDown += HookManager_KeyDown_saveKeyBoard;
                this.DoubleBuffered = true;//设置本窗体
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
                SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
                SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                UpdateStyles();
                hoverTimer.Interval = 100;//定时周期3秒
                hoverTimer.Tick += new EventHandler(Hover);//到3秒了自动隐藏
                hoverTimer.Enabled = false; //是否不断重复定时器操作
                hoverTimer.Start();
                addFanQieTimer.Interval = 60000;
                addFanQieTimer.Tick += new EventHandler(AddFanQie);//到3秒了自动隐藏
                addFanQieTimer.Enabled = false; //是否不断重复定时器操作
                addFanQieTimer.Start();
                InitializeComponent();
                mindmaplist.Height = 424;
                Reminderlistboxchange();
                try
                {
                    this.Opacity = Convert.ToDouble(ini.ReadString("appearance", "Opacity", ""));
                }
                catch (Exception)
                {
                }
                ReadTagFile();

                SearchText_suggest.ScrollAlwaysVisible = false;
                SearchText_suggest.MaximumSize = new Size(600, 444);
                SearchText_suggest.Top = 31;
                SearchText_suggest.DrawItem += SuggestText_DrawItem;
                SearchText_suggest.DrawMode = DrawMode.OwnerDrawVariable;
                mindmaplist.DrawItem += Mindmaplist_DrawItem;
                nodetree.DrawMode = TreeViewDrawMode.OwnerDrawText;
                FileTreeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
                mindmaplist.DrawMode = DrawMode.OwnerDrawVariable;
                reminderList.Top = 51;
                reminderListBox.Top = 51;
                mindmaplist.DisplayMember = "Text";
                mindmaplist.ValueMember = "Value";
                reminderList.DisplayMember = "Text";
                reminderList.ValueMember = "Value";
                mindmaplist.Items.Clear();
                reminderList.Items.Clear();
                string no = ini.ReadString("path", "no", "");
                string noFilesString = ini.ReadString("path", "nofiles", "");
                CalendarImagePath = ini.ReadStringDefault("path", "CalendarImagePath", "");
                ebdefault = ini.ReadString("path", "ebdefault", "");
                ebconfig = Convert.ToInt32(ini.ReadString("config", "ebconfig", ""));
                isPlaySound = ini.ReadString("sound", "playsounddefault", "") == "true";
                playBackGround = ini.ReadString("sound", "playBackGround", "") == "true";

                string birthday = ini.ReadString("info", "birthday", "");
                string scorestr = ini.ReadString("info", "score", "");
                fenshu.Text = scorestr;
                command = ini.ReadString("config", "command", "");
                logpass = ini.ReadString("password", "abc", "");
                encryptlog = new Encrypt(logpass);
                if (!Directory.Exists(ini.ReadStringDefault("path", "rootpath", "")))
                {
                    Directory.CreateDirectory(ini.ReadStringDefault("path", "rootpath", ""));
                    File.Copy(System.AppDomain.CurrentDomain.BaseDirectory+ @"\Demo\calander.mm", ini.ReadStringDefault("path", "rootpath", "")+@"\calander.mm");
                    Process.Start(ini.ReadStringDefault("path", "rootpath", ""));
                }
                rootpath = new DirectoryInfo(ini.ReadStringDefault("path", "rootpath", ""));
                

                rootrootpath = new DirectoryInfo(ini.ReadStringDefault("path", "rootpath", ""));
                ignoreSuggest = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\ignoreSuggest.txt");
                usedSuggest = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest.txt");
                usedSuggest2 = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest2.txt");
                usedSuggest3 = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest3.txt");
                usedSuggest4 = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest4.txt");
                unchkeckmindmap = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\unchkeckmindmap.txt");
                remindmapsList = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\remindmaps.txt");
                #region UsedTimer
                UsedTimerOnLoad();
                #endregion
                FileTreeView.AfterSelect += FileTreeView_AfterSelect;
                string calanderpath = ini.ReadString("path", "calanderpath", "");
                IntPtr nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
                fileTreePath = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                this.Height = 540;
                noterichTextBox.LoadFile(ini.ReadString("path", "note", System.AppDomain.CurrentDomain.BaseDirectory + @"\note.txt"));
                richTextSubNode.Height = 0;
                fathernode.Text = "";
                try
                {

                    DateTime birthdayD = Convert.ToDateTime(birthday);
                    TimeSpan diff = DateTime.Today - birthdayD;
                    this.Text = ini.ReadString("info", "myword", ""); //"开心，高效，认真，专注";
                    this.Text += ("   " + diff.TotalDays);
                    int yeardiff = DateTime.Now.Year - birthdayD.Year;
                    int monthdiff = DateTime.Now.Month - birthdayD.Month;
                    if (monthdiff < 0)
                    {
                        yeardiff--;
                        monthdiff += 12;
                    }
                    int daydiff = DateTime.Now.Day - birthdayD.Day;
                    if (daydiff < 0)
                    {
                        monthdiff--;
                        if (monthdiff < 0)
                        {
                            yeardiff--;
                            monthdiff += 12;
                        }
                        daydiff += 30;
                    }
                    string birthString = (yeardiff.ToString() + "年" + (monthdiff != 0 ? monthdiff.ToString() + "月" : "") + (daydiff != 0 ? daydiff.ToString() + "天" : ""));
                    this.Text += ("   " + birthString);
                    foreach (string item in calanderpath.Split(';'))
                    {
                        if (item != "")
                        {
                            PathcomboBox.Items.Add(item);
                        }
                    }
                    PathcomboBox.Items.Add("all");
                }
                catch (Exception)
                {
                }
                noFolder = no.Split(';');
                noFiles = noFilesString.Split(';');
                nodes.Clear();
                JavaScriptSerializer js = new JavaScriptSerializer
                {
                    MaxJsonLength = Int32.MaxValue
                };
                FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"allnode.json");
                using (StreamReader sw = fi.OpenText())
                {
                    string s = sw.ReadToEnd();
                    nodes = js.Deserialize<List<node>>(s);
                }
                fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"allfiles.json");
                using (StreamReader sw = fi.OpenText())
                {
                    string s = sw.ReadToEnd();
                    allfiles = js.Deserialize<List<node>>(s);
                }
                taskcount.Text = "0";
                isRefreshMindmap = true;
                LoadFile(rootpath);
                for (int i = 0; i < mindmaplist.Items.Count; i++)
                {
                    mindmaplist.SetItemChecked(i, true);
                    string file = ((MyListBoxItem)mindmaplist.Items[i]).Value;
                    if (unchkeckmindmap.Contains(file))
                    {
                        mindmaplist.SetItemChecked(i, false);
                    }
                }
                isRefreshMindmap = false;
                mindmaplist_backup = mindmaplis1.Items;
                mindmaplist_backup.Clear();
                mindmaplist_backup.AddRange(mindmaplist.Items);
                mindmaplist_count.Text = mindmaplist.Items.Count.ToString();
                for (int i = 0; i < mindmaplist.Items.Count; i++)
                {
                    mindmaplist.SetItemChecked(i, true);
                }
                if (true)
                {
                    RRReminderlist();
                }
                hotKeys.Regist(this.Handle, (int)HotKeys.HotkeyModifiers.Shift, Keys.Space, CallBack);
                hotKeys.Regist(this.Handle, (int)HotKeys.HotkeyModifiers.Alt, Keys.Space, showcalander);

                foreach (var item in mindmapfiles)
                {
                    acsc.Add(item.name);
                }
                GetAllFilesJsonIconFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UsedTimerOnLoad()
        {
            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"UsedTimer.json"))
            {
                SaveUsedTimerFile(new UsedTimer());
            }
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"UsedTimer.json");
            using (StreamReader sw = fi.OpenText())
            {
                string s = sw.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                usedTimer = serializer.Deserialize<UsedTimer>(s);
            }
            currentUsedTimerId = Guid.NewGuid();
            usedCount.Text = usedTimer.Count.ToString();
            usedtimelabel.Text = usedTimer.AllTime.ToString(@"dd\.hh\:mm\:ss");
            todayusedtime.Text = usedTimer.todayTime.TotalMinutes.ToString("N0");
            usedTimer.NewOneTime(currentUsedTimerId);
        }
        private void SaveUsedTimerFile(UsedTimer data)
        {
            string json = new JavaScriptSerializer().Serialize(data);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"UsedTimer.json", "");
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"UsedTimer.json");
            using (StreamWriter sw = fi.AppendText())
            {
                sw.Write(json);
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            List<MyListBoxItemRemind> Reminders = reminderList.Items.Cast<MyListBoxItemRemind>().ToList();
            RemindersOtherPath.AddRange(Reminders);
            mindmaplist.Items.Clear();
            reminderList.Items.Clear();
            //nodes.Clear();
            //if (!pathArr.Contains(rootpath.FullName))
            //{
            //    Thread th = new Thread(() => GetAllNode(rootpath));
            //    th.Start();
            //}

            taskcount.Text = "0";
            isRefreshMindmap = true;
            LoadFile(rootpath);
            //for (int i = 0; i < mindmaplist.Items.Count; i++)
            //{
            //    string file = ((MyListBoxItem)mindmaplist.Items[i]).Value;
            //    if (unchkeckmindmap.Contains(file))
            //    {
            //        mindmaplist.SetSelected(i, false);
            //    }
            //}
            mindmaplist_backup.Clear();
            mindmaplist_backup.AddRange(mindmaplist.Items);
            mindmaplist_count.Text = mindmaplist.Items.Count.ToString();
            for (int i = 0; i < mindmaplist.Items.Count; i++)
            {
                mindmaplist.SetItemChecked(i, true);
                string file = ((MyListBoxItem)mindmaplist.Items[i]).Value;
                if (unchkeckmindmap.Contains(file))
                {
                    mindmaplist.SetItemChecked(i, false);
                }
            }
            isRefreshMindmap = false;
            shaixuanfuwei();
            RRReminderlist();

        }

        #region 窗体事件
        private void DocearReminderForm_Deactivate(object sender, EventArgs e)
        {
            if (lockForm)
            {

            }
            else
            {
                MyHide();
            }
        }
        private void Hover(object O, EventArgs ev)
        {
            //if (this.Location.X < 5 && ((Cursor.Position.X < this.Location.X || Cursor.Position.Y < this.Location.Y) || (Cursor.Position.X > this.Location.X + 836 || Cursor.Position.Y > this.Location.Y + 544)))
            //{
            //    if (this.Location.X < 5 && Cursor.Position.X > 5)
            //    {
            //        Center();//= new Point(-825, this.Location.Y);sdfadfsf
            //    }
            //}
        }
        private void DocearReminderForm_SizeChanged(object sender, EventArgs e)
        {
            //asc = new AutoSizeFormClass();
            //asc.controlAutoSize(this);
            Center();
        }

        private void DocearReminderForm_Load(object sender, EventArgs e)
        {
            Center();
        }
        //按下快捷键时被调用的方法
        public void CallBack()
        {
            if (this.Visible == true)
            {
                MyHide();

            }
            else
            {
                this.BackColor = BackGroundColor;
                PlaySimpleSound("show");
                isInReminderlistSelect = false;
                MyShow();
                usedCount.Text = usedTimer.Count.ToString();
                usedtimelabel.Text = usedTimer.AllTime.ToString(@"dd\.hh\:mm\:ss");
                todayusedtime.Text = usedTimer.todayTime.TotalMinutes.ToString("N0");
                usedTimer.NewOneTime(currentUsedTimerId);
            }
        }
        public void showcalander()
        {
            if (mindmaplist.Focused)
            {
                IsSelectReminder = false;
            }
            Thread thCalendarForm = new Thread(() => Application.Run(new Calendar.CalendarForm(mindmapPath)));
            thCalendarForm.Start();
            //Center();//= new Point(this.Location.X, -1569);
            MyHide();
        }
        #endregion

        #region 番茄钟
        private void AddFanQie(object O, EventArgs ev)
        {
            RemindersOtherPath.RemoveAll(m => m.rootPath == rootpath.FullName);
            List<MyListBoxItemRemind> Reminders = reminderList.Items.Cast<MyListBoxItemRemind>().ToList();
            RemindersOtherPath.AddRange(Reminders);
            List<string> name = new List<string>();
            foreach (MyListBoxItemRemind selectedReminder in RemindersOtherPath.Distinct().Where(m => m.Time.DayOfYear==DateTime.Now.DayOfYear&& m.Time.Year == DateTime.Now.Year&&m.Time.Hour==DateTime.Now.Hour&&m.Time.Minute==DateTime.Now.Minute))
            {
                if (name.Contains(selectedReminder.Name))
                {
                    continue;
                }
                else
                {
                    name.Add(selectedReminder.Name);
                }
                if (selectedReminder != null && selectedReminder.rtaskTime != 0)
                {
                    if (GetPosition() < 20)
                    {
                        Thread th = new Thread(() => OpenFanQie(selectedReminder.rtaskTime, selectedReminder.Name, selectedReminder.Value, GetPosition()));
                        th.Start();
                    }
                    if (IsURL(selectedReminder.Name.Trim()))
                    {
                        System.Diagnostics.Process.Start(GetUrl(selectedReminder.Name));
                        CompleteTask(selectedReminder);
                    }
                    else if (IsFileUrl(selectedReminder.Name.Trim()))
                    {
                        System.Diagnostics.Process.Start(getFileUrlPath(selectedReminder.Name));
                        CompleteTask(selectedReminder);
                    }
                    //如果是小时循环的，则自动完成
                    if (selectedReminder.remindertype == "hour")
                    {
                        try
                        {
                            CompleteTask(selectedReminder);
                            Thread th1 = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th1.Start();
                        }
                        catch (Exception)
                        {
                            if (reminderList.Items.Count > 0)
                            {
                                reminderList.SetSelected(0, true);
                            }

                        }
                    }
                }
            }
        }

        #endregion

        #region 窗体大小
        public void Center()
        {
            int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
            int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
            this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
            this.Location = (System.Drawing.Point)new Size(x, y);         //窗体的起始位置为(x,y)
        }
        public void MyHide()
        {
            PlaySimpleSound("hide");
            SearchText_suggest.Visible = false;
            this.Hide();
            usedTimer.SetEndDate(currentUsedTimerId);
            SaveUsedTimerFile(usedTimer);
            currentUsedTimerId = Guid.NewGuid();
        }

        public void MyShow()
        {
            Center();
            this.Show();
            this.Activate();
            reminderList.Focus();
        }

        #endregion

        #region 没用过的方法
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;
            //const int WM_NCPAINT = 0x85;
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    DisplayClipboardData();
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                    {
                        nextClipboardViewer = m.LParam;
                    }
                    else
                    {
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    }

                    break;
                //case WM_NCPAINT:
                //    IntPtr hdc = GetWindowDC(m.HWnd);
                //    if ((int)hdc != 0)
                //    {
                //        Graphics g = Graphics.FromHdc(hdc);
                //        g.FillRectangle(Brushes.Red, new Rectangle(0, 0, 4800, 23));
                //        g.Flush();
                //        ReleaseDC(m.HWnd, hdc);
                //    }
                //    break;
                default:
                    //HookManager_KeyDown_saveKeyBoard(m.WParam);
                    hotKeys.ProcessHotKey(m);//快捷键消息处理
                    base.WndProc(ref m);
                    break;
            }
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);
        void KillProcess(string processname)
        {
            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {
                if (p.ProcessName.ToLower() + ".exe" == processname.ToLower())
                {
                    for (int i = 0; i < p.Threads.Count; i++)
                    {
                        p.Threads[i].Dispose();
                    }

                    p.Kill();
                    break;
                }
            }
        }
        private bool closeProc(string ProcName)
        {
            bool result = false;
            System.Collections.ArrayList procList = new System.Collections.ArrayList();
            string tempName = "";
            foreach (System.Diagnostics.Process thisProc in System.Diagnostics.Process.GetProcesses())
            {
                tempName = thisProc.ProcessName;
                if (tempName.Contains("ocea"))
                {
                    MessageBox.Show(tempName);
                }
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                    {
                        thisProc.Kill(); //当发送关闭窗口命令无效时强行结束进程                    
                    }

                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 切换输入法
        /// </summary>
        /// <param name="cultureType">语言项，如zh-CN，en-US</param>
        private void SwitchToLanguageMode(string cultureType = "en-US")
        {
            //暂时去掉所有输入法设置
            //var installedInputLanguages = InputLanguage.InstalledInputLanguages;
            //if (installedInputLanguages.Cast<InputLanguage>().Any(i => i.Culture.Name == cultureType))
            //{
            //    foreach (InputLanguage item in installedInputLanguages)
            //    {
            //        if (item.Culture.Name == cultureType)
            //        {
            //            InputLanguage.CurrentInputLanguage = item;
            //        }
            //    }
            //}
        }
        #endregion

        #region allnode,allicon,allfile等数据加载
        public void GetAllNode(DirectoryInfo path)
        {
            foreach (FileInfo file in path.GetFiles("*.mm"))
            {
                if (!noFiles.Contains(file.Name) && file.Name[0] != '~')
                {
                    try
                    {
                        string str1 = "node";
                        string str2 = "TEXT";
                        System.Xml.XmlDocument x = new XmlDocument();
                        x.Load(file.FullName);
                        string fileName = file.Name.Substring(0, file.Name.Length - 3);
                        List<string> contents = new List<string>();
                        foreach (XmlNode node in x.GetElementsByTagName(str1))
                        {
                            try
                            {
                                if (node.Attributes[str2].Value != "")
                                {
                                    if (!contents.Contains(node.Attributes[str2].Value))
                                    {
                                        nodes.Add(new node
                                        {
                                            Text = node.Attributes[str2].Value,
                                            mindmapName = fileName,
                                            mindmapPath = file.FullName,
                                            editDateTime = DateTime.Now,
                                            Time = DateTime.Now,
                                            IDinXML = node.Attributes["ID"].Value,

                                        });
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        foreach (XmlNode node in x.GetElementsByTagName("richcontent"))
                        {
                            try
                            {
                                if (!contents.Contains(node.Attributes[str2].Value))
                                {
                                    nodes.Add(new node
                                    {
                                        Text = node.InnerText,
                                        mindmapName = fileName,
                                        mindmapPath = file.FullName,
                                        editDateTime = DateTime.Now,
                                        Time = DateTime.Now,
                                        IDinXML = node.Attributes["ID"].Value
                                    });
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (path.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo subPath in path.GetDirectories())
                {
                    //需要排除的文件夹
                    if (!".svn".Contains(subPath.Name) && subPath.Name[0] != '.')// && !noFolder.Contains(subPath.Name)
                    {
                        GetAllNode(subPath);
                    }
                }
            }
        }
        public void GetAllNodeIcon(DirectoryInfo path)
        {
            foreach (FileInfo file in path.GetFiles("*.mm"))
            {
                if (!noFiles.Contains(file.Name) && file.Name[0] != '~')
                {
                    try
                    {
                        string str1 = "icon";
                        string str2 = "BUILTIN";
                        System.Xml.XmlDocument x = new XmlDocument();
                        x.Load(file.FullName);
                        string fileName = file.Name.Substring(0, file.Name.Length - 3);
                        List<string> contents = new List<string>();
                        foreach (XmlNode node in x.GetElementsByTagName(str1))
                        {
                            try
                            {
                                if (node.Attributes[str2].Value != "")
                                {
                                    string filename = "";
                                    if (node.ParentNode.Attributes["TEXT"] == null)
                                    {
                                        foreach (XmlNode item in node.ParentNode.ChildNodes)
                                        {
                                            if (item.Attributes["TYPE"] != null && item.Attributes["TYPE"].Value == "NODE")
                                            {
                                                filename = new HtmlToString().StripHTML(((System.Xml.XmlElement)item).InnerXml).Replace("|", "").Replace("@", "").Replace("\r", "").Replace("\n", "");
                                                if (filename != "")
                                                {
                                                    nodeIconString += filename;
                                                    nodeIconString += "|";
                                                    nodeIconString += Tools.GetFirstSpell(filename);
                                                    nodeIconString += "|";
                                                    nodeIconString += Tools.ConvertToAllSpell(filename);
                                                    nodeIconString += "|";
                                                    nodeIconString += Tools.GetFirstSpell(filename);
                                                    nodeIconString += "|";
                                                    nodeIconString += "true";
                                                    nodeIconString += "|";
                                                    nodeIconString += node.ParentNode.Attributes["ID"].Value;
                                                    nodeIconString += "|";
                                                    nodeIconString += file.FullName;
                                                    nodeIconString += "@";
                                                }

                                                break;
                                            }
                                        }
                                    }
                                    else if (!contents.Contains(node.ParentNode.Attributes["TEXT"].Value))
                                    {
                                        if (node.ParentNode.Attributes["TEXT"].Value != "")
                                        {
                                            nodesicon.Add(new node
                                            {
                                                Text = node.ParentNode.Attributes["TEXT"].Value,
                                                mindmapName = fileName,
                                                mindmapPath = file.FullName,
                                                editDateTime = DateTime.Now,
                                                Time = DateTime.Now
                                            });
                                            filename = node.ParentNode.Attributes["TEXT"].Value.Replace("|", "").Replace("@", "").Replace("\r", "").Replace("\n", "");
                                            nodeIconString += filename;
                                            nodeIconString += "|";
                                            nodeIconString += Tools.GetFirstSpell(filename);
                                            nodeIconString += "|";
                                            nodeIconString += Tools.ConvertToAllSpell(filename);
                                            nodeIconString += "|";
                                            nodeIconString += Tools.GetFirstSpell(filename);
                                            nodeIconString += "|";
                                            nodeIconString += "true";
                                            nodeIconString += "|";
                                            nodeIconString += node.ParentNode.Attributes["ID"].Value;
                                            nodeIconString += "|";
                                            nodeIconString += file.FullName;
                                            nodeIconString += "@";
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (path.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo subPath in path.GetDirectories())
                {
                    //需要排除的文件夹
                    if (!".svn".Contains(subPath.Name) && subPath.Name[0] != '.')//&& !noFolder.Contains(subPath.Name)
                    {
                        GetAllNodeIcon(subPath);
                    }
                }
            }
        }
        public void GetAllNodeIconout()
        {
            nodesicon.Clear();
            nodeIconString = "";
            GetAllNodeIcon(rootrootpath);
            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"allnodeicon.json"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"allnodeicon.json", "");
            }
            else
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"allnodeicon.json", "");
            }
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"allnodeicon.json");
            JavaScriptSerializer js = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string json = js.Serialize(nodesicon);
            using (StreamWriter sw = fi.AppendText())
            {
                sw.Write(json);
            }
            RecordLogallnodesicon(nodeIconString);
        }
        public void GetAllNodeout()
        {
            nodes.Clear();
            GetAllNode(rootrootpath);
            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"allnode.json"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"allnode.json", "");
            }
            else
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"allnode.json", "");
            }
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"allnode.json");
            JavaScriptSerializer js = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string json = js.Serialize(nodes);
            using (StreamWriter sw = fi.AppendText())
            {
                sw.Write(json);
            }
        }
        public void GetAllNodeJsonFile()
        {
            Thread th = new Thread(() => GetAllNodeout())
            {
                IsBackground = true
            };
            th.Start();

        }
        public void GetAllFilesJsonIconFile()
        {
            Thread th = new Thread(() => GetAllNodeIconout())
            {
                IsBackground = true
            };
            th.Start();
        }
        public void GetAllFiles(DirectoryInfo path)
        {
            foreach (FileInfo file in path.GetFiles("*.*"))
            {
                allfiles.Add(new node
                {
                    Text = file.FullName,
                    mindmapName = file.Name,
                    mindmapPath = file.FullName,
                    editDateTime = DateTime.Now,
                    Time = DateTime.Now
                });
            }
            if (path.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo subPath in path.GetDirectories())
                {
                    allfiles.Add(new node
                    {
                        Text = subPath.FullName,
                        mindmapName = subPath.Name,
                        mindmapPath = subPath.FullName,
                        editDateTime = DateTime.Now,
                        Time = DateTime.Now
                    });
                    GetAllFiles(subPath);
                }
            }
        }
        public void GetAllFilesout()
        {
            allfiles.Clear();
            GetAllFiles(rootrootpath);
            //File.WriteAllText(@"allfiles.json", "");
            ClearTxt(System.AppDomain.CurrentDomain.BaseDirectory + @"allfiles.json");
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"allfiles.json");
            JavaScriptSerializer js = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            string json = js.Serialize(allfiles);
            using (StreamWriter sw = fi.AppendText())
            {
                sw.Write(json);
            }
        }
        public void GetAllFilesJsonFile()
        {
            Thread thfiles = new Thread(() => GetAllFilesout());
            thfiles.Start();
        }

        #endregion
        //将所有节点放到内存里，方便查询，不知道要用多大阿
        #region 未整理

        public static void RecordLogallnodesicon(string Content)
        {
            string logSite = AppDomain.CurrentDomain.BaseDirectory + "\\allnodesicon.txt";//本地文件
            StreamWriter sw = new StreamWriter(logSite, false, Encoding.GetEncoding("GB2312"));
            sw.WriteLine(Content);
            sw.Close();
            sw.Dispose();
        }

        public void ClearTxt(String txtPath)
        {
            FileStream stream = File.Open(txtPath, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();
        }

        //获取包含任务的导图
        public void LoadFile(DirectoryInfo path)
        {
            if (true)
            {
                foreach (FileInfo file in path.GetFiles("*.mm"))
                {
                    if (!noFiles.Contains(file.Name) && file.Name[0] != '~')
                    {
                        try
                        {
                            if (mindmapfiles.FirstOrDefault(m => m.filePath == file.FullName) == null)
                            {
                                mindmapfiles.Add(new mindmapfile { name = file.Name.Substring(0, file.Name.Length - 3), filePath = file.FullName });
                            }
                            if (!remindmapsList.Contains(file.FullName))
                            {
                                //暂时取消remindmapsList的作用，因为避免总是需要考虑这个问题。
                                //continue;
                            }
                            string str1 = "hook";
                            string str2 = "NAME";
                            string str3 = "plugins/TimeManagementReminder.xml";
                            System.Xml.XmlDocument x = new XmlDocument();
                            x.Load(file.FullName);
                            int number = 0;
                            foreach (XmlNode node in x.GetElementsByTagName(str1))
                            {
                                try
                                {
                                    if (node.Attributes != null && node.Attributes[str2] != null && node.Attributes[str2].Value == str3)
                                    {
                                        if (node.ParentNode.Attributes["TEXT"] != null && node.ParentNode.Attributes["TEXT"].Value != "bin")
                                        {
                                            number++;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            if (number > 0)
                            {
                                mindmaplist.Items.Insert(0, new MyListBoxItem { Text = lenghtString(number.ToString(), 2) + " " + file.Name.Substring(0, file.Name.Length - 3), Value = file.FullName });

                                taskcount.Text = (Convert.ToInt16(taskcount.Text) + number).ToString();
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(file.FullName);
                        }
                    }
                }
                mindmaplist.Sorted = false;
                mindmaplist.Sorted = true;
            }
            if (path.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo subPath in path.GetDirectories())
                {
                    //需要排除的文件夹
                    if (!".svn".Contains(subPath.Name) && (allFloder || (!allFloder && !noFolder.Contains(subPath.Name))) && subPath.Name[0] != '.')
                    {
                        LoadFile(subPath);
                    }
                    else
                    {
                        if (noFolder.Contains(subPath.Name))
                        {
                            AddmindmapfilesOnly(subPath);
                        }
                    }
                }
            }
        }
        public void AddmindmapfilesOnly(DirectoryInfo path)
        {
            if (true)
            {
                foreach (FileInfo file in path.GetFiles("*.mm"))
                {
                    if (!noFiles.Contains(file.Name) && file.Name[0] != '~')
                    {
                        try
                        {
                            if (mindmapfiles.FirstOrDefault(m => m.filePath == file.FullName) == null)
                            {
                                mindmapfiles.Add(new mindmapfile { name = file.Name.Substring(0, file.Name.Length - 3), filePath = file.FullName });
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            else
            {
                //foreach (FileInfo file in path.GetFiles())
                //{
                //    mindmaplist.Items.Insert(0, new MyListBoxItem { Text = GetTopString(file) + file.Name, Value = file.FullName });
                //}
            }
            if (path.GetDirectories().Length > 0)
            {
                foreach (DirectoryInfo subPath in path.GetDirectories())
                {
                    //需要排除的文件夹
                    if (!".svn".Contains(subPath.Name) && (allFloder || (!allFloder && !noFolder.Contains(subPath.Name))) && subPath.Name[0] != '.')
                    {
                        AddmindmapfilesOnly(subPath);
                    }
                    else
                    {
                        if (noFolder.Contains(subPath.Name))
                        {
                            AddmindmapfilesOnly(subPath);
                        }
                    }
                }
            }
        }

        public void RRReminderlist()
        {
            if (mindmapSearch.Text!="")//清空一下这里的值，不然总是显示，很难受
            {
                mindmapSearch.Text = "";
            }
            reminderSelectIndex = -1;
            int task = 0;
            int ctask = 0;//周期任务个数
            int vtask = 0;//不重要任务数量
            int ebtask = 0;//Eb类型任务
            int passtask = 0;
            int isviewtask = 0;
            bool hasMorning = false;
            bool hasNoon = false;
            bool hasAfter = false;
            bool hasNight = false;
            List<MyListBoxItemRemind> reminderlistItems = new List<MyListBoxItemRemind>();
            Reminder reminderObject = new Reminder();
            if (!System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json", new JavaScriptSerializer().Serialize(reminderObject));
            }
            FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json");
            IsEncryptBool = false;
            using (StreamReader sw = fi.OpenText())
            {
                string s = sw.ReadToEnd();
                var serializer = new JavaScriptSerializer();
                reminderObject = serializer.Deserialize<Reminder>(s);
                foreach (ReminderItem item in reminderObject.reminders.Where(m => !m.isCompleted))
                {
                    if (mindmaplist.CheckedItems.Cast<MyListBoxItem>().Any(m => m.Value.IndexOf(item.mindmap) > 0))
                    {
                        item.isCurrect = false;
                        item.isNew = false;
                        item.isview = false;
                        item.isEBType = false;
                    }
                }
            }
            if (!IsViewModel.Checked && mindmapornode.Text == "")
            {
                reminderList.Items.Clear();
                //如果SS的时候只能当前类型的。
                List<MyListBoxItem> showMindmaps = new List<MyListBoxItem>();
                if (searchword.Text.StartsWith("ss"))
                {
                    foreach (MyListBoxItem item in mindmaplist_backup)
                    {
                        showMindmaps.Add(item);
                    }
                }
                else
                {
                    CustomCheckedListBox.CheckedItemCollection sourceMindmap = mindmaplist.CheckedItems;
                    foreach (MyListBoxItem item in sourceMindmap)
                    {
                        showMindmaps.Add(item);
                    }
                }
                foreach (MyListBoxItem path in showMindmaps)
                {
                    if (!path.Value.EndsWith("mm"))
                    {
                        return;
                    }
                    string str1 = "hook";
                    string str2 = "NAME";
                    string str3 = "plugins/TimeManagementReminder.xml";
                    System.Xml.XmlDocument x = new XmlDocument();
                    x.Load(path.Value);
                    if (x.GetElementsByTagName(str1).Count == 0)
                    {
                        continue;
                    }
                    if (path.Text == "bin")
                    {
                        //continue;
                    }
                    foreach (XmlNode node in x.GetElementsByTagName(str1))
                    {
                        try
                        {
                            if (node.Attributes[str2].Value == str3)
                            {
                                string reminder = "";
                                DateTime dt = DateTime.Now;
                                DateTime jinianDt = DateTime.Now;
                                if (node.InnerXml != "")
                                {
                                    reminder = node.InnerXml.Split('\"')[1];
                                    long unixTimeStamp = Convert.ToInt64(reminder);
                                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                    dt = startTime.AddMilliseconds(unixTimeStamp);
                                }
                                else
                                {
                                    reminder = GetAttribute(node.ParentNode, "RememberTime");
                                    if (reminder == "")
                                    {
                                    }
                                    else
                                    {
                                        long unixTimeStamp = Convert.ToInt64(reminder);
                                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                        dt = startTime.AddMilliseconds(unixTimeStamp);
                                    }
                                }
                                int editTime = 0;
                                string taskid = GetAttribute(node.ParentNode, "ID");
                                IEnumerable<ReminderItem> reminderthis = reminderObject.reminders.Where(m => m.ID == taskid && m.mindmap == path.Value.Split('\\')[path.Value.Split('\\').Length - 1].Split('.')[0]);
                                if (reminderthis.Count() > 0)
                                {
                                    foreach (ReminderItem item in reminderthis)
                                    {
                                        if (item.ID == taskid && item.mindmap == path.Value.Split('\\')[path.Value.Split('\\').Length - 1].Split('.')[0])
                                        {
                                            item.isCurrect = true;
                                            item.isNew = false;
                                            item.remindertype = GetAttribute(node.ParentNode, "REMINDERTYPE");
                                            item.rdays = MyToInt16(GetAttribute(node.ParentNode, "RDAYS"));
                                            item.rMonth = MyToInt16(GetAttribute(node.ParentNode, "RMONTH"));
                                            item.rWeek = MyToInt16(GetAttribute(node.ParentNode, "RWEEK"));
                                            item.name = GetAttribute(node.ParentNode, "TEXT");
                                            item.rweeks = GetAttribute(node.ParentNode, "RWEEKS").ToCharArray();
                                            item.ryear = MyToInt16(GetAttribute(node.ParentNode, "RYEAR"));
                                            item.tasktime = MyToInt16(GetAttribute(node.ParentNode, "TASKTIME"));
                                            item.tasklevel = MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL"));
                                            item.ebstring = MyToInt16(GetAttribute(node.ParentNode, "EBSTRING"));
                                            item.mindmapPath = path.Value;
                                            item.isCompleted = false;
                                            if (item.time.AddHours(8).ToString("yyyy/MM/dd HH:mm") != dt.ToString("yyyy/MM/dd HH:mm"))
                                            {
                                                item.time = dt;
                                                item.editCount += 1;
                                                reminderObject.editCount += 1;
                                                if (item.editTime == null)
                                                {
                                                    item.editTime = new List<DateTime>();
                                                }
                                                item.editTime.Add(DateTime.Now);
                                                break;
                                            }
                                            editTime = item.editCount;
                                            item.ID = GetAttribute(node.ParentNode, "ID");
                                            item.isview = GetAttribute(node.ParentNode, "ISVIEW") == "true" || MyToBoolean(GetAttribute(node.ParentNode, "ISReminderOnly"));
                                            item.isEBType=GetAttribute(node.ParentNode, "REMINDERTYPE") == "eb";
                                        }
                                    }
                                }
                                else
                                {
                                    ReminderItem newitem = new ReminderItem
                                    {
                                        name = GetAttribute(node.ParentNode, "TEXT"),
                                        mindmap = path.Value.Split('\\')[path.Value.Split('\\').Length - 1].Split('.')[0],
                                        time = dt,
                                        isNew = true,
                                        remindertype = GetAttribute(node.ParentNode, "REMINDERTYPE"),
                                        rhours = MyToInt16(GetAttribute(node.ParentNode, "RHOURS")),
                                        rdays = MyToInt16(GetAttribute(node.ParentNode, "RDAYS")),
                                        rMonth = MyToInt16(GetAttribute(node.ParentNode, "RMONTH")),
                                        rWeek = MyToInt16(GetAttribute(node.ParentNode, "RWEEK")),
                                        rweeks = GetAttribute(node.ParentNode, "RWEEKS").ToCharArray(),
                                        ryear = MyToInt16(GetAttribute(node.ParentNode, "RYEAR")),
                                        tasktime = MyToInt16(GetAttribute(node.ParentNode, "TASKTIME")),
                                        tasklevel = MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL")),
                                        ebstring = MyToInt16(GetAttribute(node.ParentNode, "EBSTRING")),
                                        mindmapPath = path.Value,
                                        ID = GetAttribute(node.ParentNode, "ID"),
                                        isview = GetAttribute(node.ParentNode, "ISVIEW") == "true" || MyToBoolean(GetAttribute(node.ParentNode, "ISReminderOnly")),
                                        isEBType=GetAttribute(node.ParentNode, "REMINDERTYPE") == "eb"
                                    };
                                reminderObject.reminders.Add(newitem);
                                    reminderObject.reminderCount += 1;
                                }
                                //添加提醒到提醒清单
                                string dakainfo = "";
                                if (GetAttribute(node.ParentNode, "ISDAKA") == "true")
                                {
                                    dakainfo = " | " + GetAttribute(node.ParentNode, "DAKADAY");
                                }
                                //纪念日
                                string jinianriInfo = "";
                                if (GetAttribute(node.ParentNode, "IsJinian") == "true")
                                {
                                    try
                                    {
                                        string JinianBeginTime = GetAttribute(node.ParentNode, "JinianBeginTime");
                                        long unixTimeStamp = Convert.ToInt64(JinianBeginTime);
                                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                        jinianDt = startTime.AddMilliseconds(unixTimeStamp);
                                        if (IsJinianCheckBox.Checked)
                                        {
                                            jinianriInfo = " | " + GetTimeSpanStr(Convert.ToInt16((DateTime.Now - jinianDt).TotalDays));
                                        }
                                        else
                                        {
                                            if (GetAttribute(node.ParentNode, "EndDate") != "")
                                            {
                                                string EndDate = GetAttribute(node.ParentNode, "EndDate");
                                                long unixTimeEndDate = Convert.ToInt64(EndDate);
                                                DateTime EndDateDt = startTime.AddMilliseconds(unixTimeEndDate);
                                                jinianriInfo = " | " + GetTimeSpanStr(Convert.ToInt16((EndDateDt - DateTime.Now).TotalDays));
                                            }
                                            //jinianriInfo = " | " + GetTimeSpanStr(Convert.ToInt16((dt-DateTime.Now).TotalDays));
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                //结束日
                                string EndDateInfo = "";
                                if (GetAttribute(node.ParentNode, "EndDate") != "")
                                {
                                    string JinianBeginTime = GetAttribute(node.ParentNode, "EndDate");
                                    long unixTimeStamp = Convert.ToInt64(JinianBeginTime);
                                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                    jinianDt = startTime.AddMilliseconds(unixTimeStamp);
                                    EndDateInfo = " | [" + jinianDt.ToString("yy-MM-dd") + "]";
                                }
                                //剩余打开次数
                                string LeftDakaDays = "";
                                if (GetAttribute(node.ParentNode, "LeftDakaDays") != "")
                                {
                                    LeftDakaDays = " | [" + GetAttribute(node.ParentNode, "LeftDakaDays") + "次]";
                                }
                                bool IsShow = true;
                                if (ISLevel.Checked)
                                {
                                    if (MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL")) != tasklevel.Value && reminderList.SelectedIndex < 0)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else
                                {
                                    //if (MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL")) < tasklevel.Value && reminderList.SelectedIndex < 0)
                                    //{
                                    //    IsShow = false;
                                    //}
                                }
                                //逆反，不需要了，按钮也删除掉
                                //if (nifan.Checked && reminderlist.SelectedIndex < 0)
                                //{
                                //    IsShow = !IsShow;
                                //}
                                //show if less than today not now
                                if (dt < DateTime.Today)
                                {
                                    IsShow = true;
                                }
                                else if ((dt.Day.Equals(DateTime.Now.Day) && dt.Month.Equals(DateTime.Now.Month) && dt.Year.Equals(DateTime.Now.Year)))//当天
                                {
                                    if (dt.Hour <= 8 && !morning.Checked)
                                    {
                                        IsShow = false;
                                    }
                                    if (dt.Hour <= 8)
                                    {
                                        hasMorning = true;
                                    }
                                    if (dt.Hour > 8 && dt.Hour < 12 && !day.Checked)
                                    {
                                        IsShow = false;
                                    }
                                    if (dt.Hour > 8 && dt.Hour < 12)
                                    {
                                        hasNoon = true;
                                    }
                                    if (dt.Hour >= 12 && dt.Hour < 18 && !afternoon.Checked)
                                    {
                                        IsShow = false;
                                    }
                                    if (dt.Hour >= 12 && dt.Hour < 18)
                                    {
                                        hasAfter = true;
                                    }
                                    if (dt.Hour >= 18)
                                    {
                                        hasNight = true;
                                    }
                                    if (dt.Hour >= 18 && dt.Hour < 24 && !night.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else if (dt < (DateTime.Now.AddDays(1).AddHours(24 - DateTime.Now.Hour)))//明天
                                {
                                    if (!showtomorrow.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else if ((true) && (dt) < DateTime.Now.AddDays(7).AddHours(24 - DateTime.Now.Hour))//一周
                                {
                                    if (!reminder_week.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else if ((true) && dt < DateTime.Now.AddDays(30).AddHours(24 - DateTime.Now.Hour))//一个月
                                {
                                    if (!reminder_month.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else if ((true) && (dt) < DateTime.Now.AddDays(365).AddHours(24 - DateTime.Now.Hour))//一年
                                {
                                    if (!reminder_year.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                else if ((true) && (dt) >= DateTime.Now.AddDays(365).AddHours(24 - DateTime.Now.Hour))//一年以后
                                {
                                    if (!reminder_yearafter.Checked)
                                    {
                                        IsShow = false;
                                    }
                                }
                                bool timebool = IsShow;
                                bool iSReminderOnly = MyToBoolean(GetAttribute(node.ParentNode, "ISReminderOnly"));

                                string tasktype = GetAttribute(node.ParentNode, "REMINDERTYPE");
                                //GetAttribute(node.ParentNode, "ISVIEW") == "true"
                                if (IsShow)
                                {
                                    //只要是查看任务，就加1
                                    if (iSReminderOnly)
                                    {
                                        vtask++;
                                    }
                                    else
                                    {
                                        if (tasktype != "")
                                        {
                                            if (tasktype == "eb")
                                            {
                                                ebtask++;
                                            }
                                            else
                                            {
                                                ctask++;
                                            }
                                        }
                                        else
                                        {
                                            task++;
                                        }
                                    }
                                }
                                // 判断是什么类型的标签
                                //不显示周期的时候，所有周期类型的都不显示
                                if (!showcyclereminder.Checked && (GetAttribute(node.ParentNode, "REMINDERTYPE") == "day" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "week" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "month" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "year" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "hour"))
                                {
                                    IsShow = false;
                                }
                                //选择周期时，且只选择周期时，所有非周期都不显示
                                else if (showcyclereminder.Checked && onlyZhouqi.Checked && !(GetAttribute(node.ParentNode, "REMINDERTYPE") == "day" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "week" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "month" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "year" || GetAttribute(node.ParentNode, "REMINDERTYPE") == "hour"))
                                {
                                    IsShow = false;
                                }
                                if (!ebcheckBox.Checked && GetAttribute(node.ParentNode, "REMINDERTYPE") == "eb")
                                {
                                    IsShow = false;
                                }
                                if (ebcheckBox.Checked && GetAttribute(node.ParentNode, "REMINDERTYPE") != "eb")
                                {
                                    IsShow = false;
                                }
                                if (searchword.Text != "")
                                {
                                    if (node.ParentNode.Attributes["TEXT"].Value.Contains(searchword.Text))
                                    {
                                        IsShow = true;
                                    }
                                    else
                                    {
                                        IsShow = false;
                                    }
                                }
                                if (!IsViewModel.Checked && GetAttribute(node.ParentNode, "ISVIEW") == "true")
                                {
                                    IsShow = false;
                                }
                                if (GetAttribute(node.ParentNode, "ISVIEW") == "true")
                                {
                                    isviewtask++;
                                    if (tasktype != "")
                                    {
                                        if (tasktype == "eb")
                                        {
                                            ebtask--;
                                        }
                                        else
                                        {
                                            ctask--;
                                        }
                                    }
                                    else
                                    {
                                        if (iSReminderOnly)
                                        {
                                            vtask--;
                                        }
                                        else
                                        {
                                            task--;
                                        }
                                    }
                                }
                                string taskName = "";
                                string taskNameDis = "";
                                bool isEncrypted = false;
                                taskName = GetAttribute(node.ParentNode, "TEXT");
                                if (taskName.Length > 6)
                                {
                                    if (taskName.Substring(0, 3) == "***")
                                    {
                                        passtask++;
                                        if (tasktype != "")
                                        {
                                            if (tasktype == "eb")
                                            {
                                                ebtask--;
                                            }
                                            else
                                            {
                                                ctask--;
                                            }
                                        }
                                        else
                                        {
                                            if (iSReminderOnly)
                                            {
                                                vtask--;
                                            }
                                            else
                                            {
                                                task--;
                                            }
                                        }
                                        if (PassWord == "")
                                        {
                                            IsShow = false;
                                        }
                                        else
                                        {
                                            taskName = encrypt.DecryptString(node.ParentNode.Attributes["TEXT"].Value);
                                            isEncrypted = true;
                                        }
                                    }
                                }
                                taskNameDis = taskName;
                                if (IsFileUrl(taskName))
                                {
                                    if (Path.GetExtension(taskName) != "")
                                    {
                                        taskNameDis = "#" + Path.GetFileName(taskName);
                                    }
                                    else
                                    {
                                        taskNameDis = "Path:" + Path.GetFullPath(taskName).Split('\\').Last(m => m != "");
                                    }
                                }
                                if (IsJinianCheckBox.Checked)
                                {
                                    if (jinianriInfo != "")
                                    {
                                        IsShow = true;
                                    }
                                    else
                                    {
                                        IsShow = false;
                                    }
                                }
                                //搜索任务模式
                                if (searchword.Text.StartsWith("ss"))
                                {
                                    string searchwordText = searchword.Text.Substring(2);
                                    if (taskName.Contains(searchwordText) || Tools.ConvertToAllSpell(taskName).Contains(searchwordText) || Tools.GetFirstSpell(taskName).Contains(searchwordText))
                                    {
                                        IsShow = true;
                                    }
                                    else
                                    {
                                        IsShow = false;
                                    }
                                }
                                //处理提醒 
                                if (timebool)
                                {
                                    if (IsReminderOnlyCheckBox.Checked)
                                    {
                                        if (iSReminderOnly)
                                        {
                                            IsShow = true;
                                        }
                                        else
                                        {
                                            IsShow = false;
                                        }

                                    }
                                    else
                                    {
                                        if (iSReminderOnly)
                                        {
                                            IsShow = false;
                                        }
                                        else
                                        {
                                        }
                                    }

                                }
                                string nodeid = GetAttribute(node.ParentNode, "ID");
                                if (reminderboxList.Where(m => m.IDinXML == nodeid).Count() > 0)
                                {
                                    IsShow = false;
                                }
                                if (IsShow)
                                {
                                    if (taskName.ToLower() != "bin")
                                    {
                                        if (isZhuangbi)
                                        {
                                            string patten = @"(\S)";
                                            Regex reg = new Regex(patten);
                                            taskNameDis = reg.Replace(taskNameDis, "*");
                                        }
                                        reminderList.Items.Add(new MyListBoxItemRemind
                                        {
                                            //Text = dt.ToString("yy-MM-dd-HH:mm") + " > " + dt.AddMinutes(MyToInt16(GetAttribute(node.ParentNode, "TASKTIME"))).ToString("HH:mm") + @"  " + (GetAttribute(node.ParentNode, "TASKLEVEL") != "" ? GetAttribute(node.ParentNode, "TASKLEVEL") : "0") + @"  " + taskNameDis + dakainfo,
                                            //Text = (IsJinianCheckBox.Checked ? jinianDt.ToString("dd HH:mm") : dt.ToString("dd HH:mm")) + @"  " + taskNameDis + dakainfo + LeftDakaDays + jinianriInfo + EndDateInfo,
                                            Text = (IsJinianCheckBox.Checked ? jinianDt.ToString("dd HH:mm") : dt.ToString("dd HH:mm")) + @"" + GetAttribute(node.ParentNode, "TASKTIME", 4) + @" " + taskNameDis + dakainfo + LeftDakaDays + jinianriInfo + EndDateInfo,
                                            Name = taskName,
                                            Time = dt,
                                            jinianDatetime = jinianDt,
                                            Value = path.Value,
                                            IsShow = false,
                                            remindertype = GetAttribute(node.ParentNode, "REMINDERTYPE"),
                                            rhours = MyToInt16(GetAttribute(node.ParentNode, "RHOUR")),
                                            rdays = MyToInt16(GetAttribute(node.ParentNode, "RDAYS")),
                                            rMonth = MyToInt16(GetAttribute(node.ParentNode, "RMONTH")),
                                            rWeek = MyToInt16(GetAttribute(node.ParentNode, "RWEEK")),
                                            rweeks = GetAttribute(node.ParentNode, "RWEEKS").ToCharArray(),
                                            ryear = MyToInt16(GetAttribute(node.ParentNode, "RYEAR")),
                                            rtaskTime = MyToInt16(GetAttribute(node.ParentNode, "TASKTIME")),
                                            IsDaka = GetAttribute(node.ParentNode, "ISDAKA"),
                                            IsView = GetAttribute(node.ParentNode, "ISVIEW"),
                                            DakaDay = MyToInt16(GetAttribute(node.ParentNode, "DAKADAY")),
                                            level = MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL")),
                                            ebstring = MyToInt16(GetAttribute(node.ParentNode, "EBSTRING")),
                                            DakaDays = StrToInt(GetAttribute(node.ParentNode, "DAKADAYS").Split(',')),
                                            editTime = editTime,
                                            isEncrypted = isEncrypted,
                                            IDinXML = GetAttribute(node.ParentNode, "ID"),
                                            link = GetAttribute(node.ParentNode, "LINK"),
                                            rootPath = rootpath.FullName,
                                            ISReminderOnly = iSReminderOnly
                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    // This text will always be added, making the file longer over time
                    // if it is not deleted.
                    //using (StreamReader sw = fi.OpenText())
                    //{
                    //    string s = sw.ReadToEnd();
                    //    var serializer = new JavaScriptSerializer();
                    //    Reminder result = serializer.Deserialize<Reminder>(s);
                    //}
                }
            }
            else
            {
                if (mindmaplist.SelectedItem == null && mindmapornode.Text == "")
                {
                    return;
                }
                reminderList.Items.Clear();
                System.Xml.XmlDocument x = new XmlDocument();
                string currentPath = "";
                if (mindmapornode.Text != "")
                {
                    if (mindmapornode.Text.Contains(">"))
                    {
                        currentPath = renameMindMapPath;
                    }
                    else
                    {
                        mindmapfile file = mindmapfiles.FirstOrDefault(m => m.name.ToLower() == mindmapornode.Text.ToLower());//不区分大小写 //是否需要优化下这个逻辑呢？？
                        if (file == null)
                        {
                            return;
                        }
                        currentPath = file.filePath;
                    }
                }
                else
                {
                    currentPath = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                }
                try
                {
                    x.Load(currentPath);
                }
                catch (Exception)
                {
                    return;
                }
                if (x.GetElementsByTagName("hook").Count == 0)
                {
                    return;
                }
                string str1 = "hook";
                string str2 = "NAME";
                string str3 = "plugins/TimeManagementReminder.xml";
                if (mindmapornode.Text.Contains(">"))
                {
                    foreach (XmlNode node in x.GetElementsByTagName("node"))
                    {
                        if (node.ParentNode != null && node.ParentNode.Attributes != null && node.ParentNode.Attributes["ID"] != null && node.ParentNode.Attributes["ID"].InnerText == renameMindMapFileID)
                        {
                            try
                            {
                                DateTime dt = DateTime.Now;
                                string reminder = GetAttribute(node, "CREATED");
                                long unixTimeStamp = Convert.ToInt64(reminder);
                                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                dt = startTime.AddMilliseconds(unixTimeStamp);
                                //todo要显示任务的时间，暂时不改
                                //try
                                //{
                                //    foreach (XmlNode item in node.ChildNodes)
                                //    {
                                //        if (item.Name == "nook")
                                //        {

                                //        }
                                //    }
                                //}
                                //catch (Exception)
                                //{
                                //}
                                //解决富文本的问题。
                                string TextString = "";
                                if (node.Attributes["TEXT"] == null)
                                {
                                    foreach (XmlNode item in node.ChildNodes)
                                    {
                                        if (item.Attributes["TYPE"] != null && item.Attributes["TYPE"].Value == "NODE")
                                        {
                                            TextString = new HtmlToString().StripHTML(((System.Xml.XmlElement)item).InnerXml).Replace("|", "").Replace("@", "").Replace("\r", "").Replace("\n", "");
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    TextString = GetAttribute(node, "TEXT");
                                }
                                reminderList.Items.Add(new MyListBoxItemRemind
                                {
                                    Text = dt.ToString("yyyy/MM/dd HH:mm  ") + TextString,
                                    Name = TextString,
                                    Time = dt,
                                    Value = currentPath,
                                    IsShow = false,
                                    remindertype = GetAttribute(node, "REMINDERTYPE"),
                                    rhours = MyToInt16(GetAttribute(node, "RHOUR")),
                                    rdays = MyToInt16(GetAttribute(node, "RDAYS")),
                                    rMonth = MyToInt16(GetAttribute(node, "RMONTH")),
                                    rWeek = MyToInt16(GetAttribute(node, "RWEEK")),
                                    rweeks = GetAttribute(node, "RWEEKS").ToCharArray(),
                                    ryear = MyToInt16(GetAttribute(node, "RYEAR")),
                                    rtaskTime = MyToInt16(GetAttribute(node, "TASKTIME")),
                                    IsDaka = GetAttribute(node, "ISDAKA"),
                                    IsView = GetAttribute(node, "ISVIEW"),
                                    DakaDay = MyToInt16(GetAttribute(node, "DAKADAY")),
                                    level = MyToInt16(GetAttribute(node, "TASKLEVEL")),
                                    ebstring = MyToInt16(GetAttribute(node, "EBSTRING")),
                                    DakaDays = StrToInt(GetAttribute(node, "DAKADAYS").Split(',')),
                                    editTime = 0,
                                    IDinXML = GetAttribute(node, "ID"),
                                    link = GetAttribute(node, "LINK")
                                });
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                else
                {
                    foreach (XmlNode node in x.GetElementsByTagName(str1))
                    {
                        try
                        {
                            if (node.Attributes[str2].Value == str3)
                            {
                                string reminder = "";
                                DateTime dt = DateTime.Now;
                                if (node.InnerXml != "")
                                {
                                    reminder = node.InnerXml.Split('\"')[1];
                                    long unixTimeStamp = Convert.ToInt64(reminder);
                                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                    dt = startTime.AddMilliseconds(unixTimeStamp);
                                }
                                else
                                {
                                    reminder = GetAttribute(node.ParentNode, "RememberTime");
                                    if (reminder == "")
                                    {
                                    }
                                    else
                                    {
                                        long unixTimeStamp = Convert.ToInt64(reminder);
                                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                        dt = startTime.AddMilliseconds(unixTimeStamp);
                                    }
                                }
                                //添加提醒到提醒清单
                                string dakainfo = "";
                                if (GetAttribute(node.ParentNode, "ISDAKA") == "true")
                                {
                                    dakainfo = " | " + GetAttribute(node.ParentNode, "DAKADAY");
                                }
                                string taskName = "";
                                string taskNameDis = "";
                                bool isEncrypted = false;
                                taskName = node.ParentNode.Attributes["TEXT"].Value;
                                if (taskName.Length > 6)
                                {
                                    if (taskName.Substring(0, 3) == "***")
                                    {
                                        if (PassWord != "")
                                        {
                                            taskName = encrypt.DecryptString(node.ParentNode.Attributes["TEXT"].Value);
                                            isEncrypted = true;
                                        }
                                    }
                                }
                                taskNameDis = taskName;
                                if (IsFileUrl(taskName))
                                {
                                    if (Path.GetExtension(taskName) != "")
                                    {
                                        taskNameDis = "#" + Path.GetFileName(taskName);
                                    }
                                    else
                                    {
                                        taskNameDis = "Path:" + Path.GetFullPath(taskName).Split('\\').Last(m => m != "");
                                    }
                                }
                                if (GetAttribute(node.ParentNode, "ISVIEW") == "true")
                                {
                                    taskNameDis = "待：" + taskNameDis;
                                }
                                if (taskName.ToLower() != "bin")
                                {
                                    reminderlistItems.Add(new MyListBoxItemRemind
                                    {
                                        Text = dt.ToString("yy-MM-dd-HH:mm") + @"  " + taskNameDis + dakainfo,
                                        Name = taskName,
                                        Time = dt,
                                        Value = currentPath,
                                        IsShow = false,
                                        remindertype = GetAttribute(node.ParentNode, "REMINDERTYPE"),
                                        rhours = MyToInt16(GetAttribute(node.ParentNode, "RHOUR")),
                                        rdays = MyToInt16(GetAttribute(node.ParentNode, "RDAYS")),
                                        rMonth = MyToInt16(GetAttribute(node.ParentNode, "RMONTH")),
                                        rWeek = MyToInt16(GetAttribute(node.ParentNode, "RWEEK")),
                                        rweeks = GetAttribute(node.ParentNode, "RWEEKS").ToCharArray(),
                                        ryear = MyToInt16(GetAttribute(node.ParentNode, "RYEAR")),
                                        rtaskTime = MyToInt16(GetAttribute(node.ParentNode, "TASKTIME")),
                                        IsDaka = GetAttribute(node.ParentNode, "ISDAKA"),
                                        IsView = GetAttribute(node.ParentNode, "ISVIEW"),
                                        DakaDay = MyToInt16(GetAttribute(node.ParentNode, "DAKADAY")),
                                        level = MyToInt16(GetAttribute(node.ParentNode, "TASKLEVEL")),
                                        ebstring = MyToInt16(GetAttribute(node.ParentNode, "EBSTRING")),
                                        DakaDays = StrToInt(GetAttribute(node.ParentNode, "DAKADAYS").Split(',')),
                                        editTime = 0,
                                        isEncrypted = isEncrypted,
                                        link = GetAttribute(node.ParentNode, "LINK"),
                                        IDinXML = GetAttribute(node.ParentNode, "ID")
                                    });
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            if (!searchword.Text.StartsWith("ss") && showfenge)
            {
                //添加一个当期时间条目
                if (!IsJinianCheckBox.Checked)
                {
                    reminderList.Items.Add(new MyListBoxItemRemind
                    {
                        Text = "此时",
                        Name = "当前时间",
                        Time = DateTime.Now,
                        Value = System.AppDomain.CurrentDomain.BaseDirectory,
                        IsShow = true,
                        remindertype = "",
                        rhours = 0,
                        rdays = 0,
                        rMonth = 0,
                        rWeek = 0,
                        rweeks = new char[0],
                        ryear = 0,
                        rtaskTime = 0,
                        IsDaka = "",
                        IsView = "",
                        DakaDay = 0,
                        level = 0,
                        DakaDays = new int[0],
                        editTime = 0,
                        isEncrypted = false,
                        isTask = false
                    });
                }
                if (hasMorning && hasNoon && morning.Checked && !IsJinianCheckBox.Checked)
                {
                    //添加一个当期时间条目
                    reminderList.Items.Add(new MyListBoxItemRemind
                    {
                        Text = "上午",
                        Name = "当前时间",
                        Time = DateTime.Today.AddHours(8.5),
                        Value = System.AppDomain.CurrentDomain.BaseDirectory,
                        IsShow = true,
                        remindertype = "",
                        rhours = 0,
                        rdays = 0,
                        rMonth = 0,
                        rWeek = 0,
                        rweeks = new char[0],
                        ryear = 0,
                        rtaskTime = 0,
                        IsDaka = "",
                        IsView = "",
                        DakaDay = 0,
                        level = 0,
                        DakaDays = new int[0],
                        editTime = 0,
                        isEncrypted = false,
                        isTask = false
                    });
                }
                if (hasNoon && hasAfter && day.Checked && !IsJinianCheckBox.Checked)
                {
                    //添加一个当期时间条目
                    reminderList.Items.Add(new MyListBoxItemRemind
                    {
                        Text = "下午",
                        Name = "当前时间",
                        Time = DateTime.Today.AddHours(12),
                        Value = System.AppDomain.CurrentDomain.BaseDirectory,
                        IsShow = true,
                        remindertype = "",
                        rhours = 0,
                        rdays = 0,
                        rMonth = 0,
                        rWeek = 0,
                        rweeks = new char[0],
                        ryear = 0,
                        rtaskTime = 0,
                        IsDaka = "",
                        IsView = "",
                        DakaDay = 0,
                        level = 0,
                        DakaDays = new int[0],
                        editTime = 0,
                        isEncrypted = false,
                        isTask = false

                    });
                }
                if (hasAfter && hasNight && afternoon.Checked && night.Checked && !IsJinianCheckBox.Checked)
                {
                    //添加一个当期时间条目
                    reminderList.Items.Add(new MyListBoxItemRemind
                    {
                        Text = "晚上",
                        Name = "当前时间",
                        Time = DateTime.Today.AddHours(18),
                        Value = System.AppDomain.CurrentDomain.BaseDirectory,
                        IsShow = true,
                        remindertype = "",
                        rhours = 0,
                        rdays = 0,
                        rMonth = 0,
                        rWeek = 0,
                        rweeks = new char[0],
                        ryear = 0,
                        rtaskTime = 0,
                        IsDaka = "",
                        IsView = "",
                        DakaDay = 0,
                        level = 0,
                        DakaDays = new int[0],
                        editTime = 0,
                        isEncrypted = false,
                        isTask = false
                    });
                }
            }
            foreach (MyListBoxItemRemind item in reminderlistItems)
            {
                reminderList.Items.Add(item);
            }
            reminderList.Refresh();
            foreach (ReminderItem item in reminderObject.reminders.Where(m => !(m.isCurrect || m.isNew) && !m.isCompleted))
            {
                if (mindmaplist.CheckedItems.Cast<MyListBoxItem>().Any(m => m.Value.IndexOf(item.mindmap) > 0))
                {
                    item.isCompleted = true;
                    item.isCurrect = false;
                    item.isNew = false;
                    item.comleteTime = DateTime.Now;
                }
            };
            //删除重复项
            //List<ReminderItem> repeatItems = new List<ReminderItem>();
            //foreach (ReminderItem item in reminderObject.reminders.Where(m => !m.isCompleted))
            //{
            //    foreach (ReminderItem itemComplete in reminderObject.reminders.Where(m => m.isCompleted))
            //    {
            //        if (item.name == itemComplete.name && itemComplete.mindmap == item.mindmap)
            //        {
            //            if (itemComplete.editTime != null)
            //            {
            //                if (item.editTime == null)
            //                {
            //                    item.editTime = new List<DateTime>();
            //                }
            //                item.editTime.AddRange(itemComplete.editTime);
            //            }
            //            item.editCount += itemComplete.editCount;
            //            reminderObject.editCount += 1;
            //            repeatItems.Add(itemComplete);
            //        }
            //    }
            //}
            //foreach (ReminderItem item in repeatItems)
            //{
            //    reminderObject.reminders.Remove(item);
            //}
            string json = new JavaScriptSerializer().Serialize(reminderObject);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json", "");
            using (StreamWriter sw = fi.AppendText())
            {
                sw.Write(json);
            }
            hourLeft.Text = (24 - DateTime.Now.Hour - (float)DateTime.Now.Minute / 60).ToString("N2");
            pageinfo(task + "(" + isviewtask + ")|" + ctask + "(" + ebtask + ")|" + vtask + "|" + passtask);
            if (IsReminderOnlyCheckBox.Checked && reminderList.Items.Count == 0)
            {
                IsReminderOnlyCheckBox.Checked = false;
                RRReminderlist();
            }
            SortReminderList();
            ReminderListBox_SizeChanged(null, null);
        }
        public void SortReminderList()
        {
            reminderList.Sorted = false;
            reminderList.Sorted = true;
        }
        private void mindmaplist_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(((MyListBoxItem)mindmaplist.SelectedItem).Value);
                MyHide();
                searchword.Focus();
            }
            catch (Exception)
            {

            }
        }
        private void Reminderlist_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (searchword.Text.ToLower().StartsWith("`") || searchword.Text.ToLower().StartsWith("·"))
                {
                    System.Diagnostics.Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                    SaveLog("打开：    " + ((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                    MyHide();
                    return;
                }
                if (IsURL(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                {
                    System.Diagnostics.Process.Start(GetUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                    SaveLog("打开：    " + ((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                }
                else if (IsFileUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                {
                    System.Diagnostics.Process.Start(getFileUrlPath(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                    SaveLog("打开：    " + ((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                }
                else
                {
                    System.Diagnostics.Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                }
                MyHide();
                searchword.Focus();
            }
            catch (Exception)
            {

            }
        }
        private void encryptbutton_Click(object sender, EventArgs e)
        {
            DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", "")); //System.AppDomain.CurrentDomain.BaseDirectory);
            mindmaplist.Items.Clear();
            reminderList.Items.Clear();
            LoadEncryptFile(path);
            mindmaplist_count.Text = mindmaplist.Items.Count.ToString();
        }
        public void LoadEncryptFile(DirectoryInfo path)
        {
            foreach (FileInfo file in path.GetFiles("*.mm", SearchOption.AllDirectories))
            {
                if ((file.OpenText().ReadToEnd()).Contains("ENCRYPTED_CONTENT"))
                {
                    mindmaplist.Items.Insert(0, new MyListBoxItem { Text = GetTopString(file) + file.Name, Value = file.FullName });
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            mindmaplist.Items.Clear();
            reminderList.Items.Clear();
            DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", "")); //System.AppDomain.CurrentDomain.BaseDirectory);
            string keyword = yixiaozi.Model.DocearReminder.Helper.ConvertString(searchword.Text);
            string searchPattern = "*.mm";
            if (!IsEncryptBool)
            {
                if (false)
                {
                    //searchPattern = "*";
                }
                foreach (FileInfo file in path.GetFiles(searchPattern, SearchOption.AllDirectories))
                {
                    if (file.Name.Contains(searchword.Text))
                    {
                        mindmaplist.Items.Insert(0, new MyListBoxItem { Text = GetTopString(file) + file.Name, Value = file.FullName });
                    }
                    else
                    {
                        if (".mm.txt.html".IndexOf(file.Extension) >= 0)
                        {
                            if ((file.OpenText().ReadToEnd().ToLower()).Contains(keyword.ToLower()))
                            {
                                mindmaplist.Items.Insert(0, new MyListBoxItem { Text = GetTopString(file) + file.Name, Value = file.FullName });
                            }
                        }
                    }
                }
                if (false)
                {
                    //mindmaplist.Sorted = false;
                    //for (int i = 1; i < mindmaplist.Items.Count; i++)
                    //{
                    //    MoveItem(i);
                    //}
                    //for (int i = 0; i < mindmaplist.Items.Count; i++)
                    //{
                    //    ((MyListBoxItem)mindmaplist.Items[i]).Text = ((MyListBoxItem)mindmaplist.Items[i]).Text.Substring(((MyListBoxItem)mindmaplist.Items[i]).Text.Split(' ')[0].Length + 1);
                    //}
                }
            }
            else
            {
                foreach (FileInfo file in path.GetFiles(searchPattern, SearchOption.AllDirectories))
                {
                    if (Regex.Matches((file.OpenText().ReadToEnd()), @"\*\*\*.*\*\*\*").Count > 0)
                    {
                        foreach (Match item in Regex.Matches((file.OpenText().ReadToEnd()), @"\*\*\*.*\*\*\*"))
                        {
                            if (keyword == "")
                            {
                                try
                                {
                                    string str = item.ToString();
                                    while (str.Contains("****"))
                                    {
                                        str = str.Replace("****", "***");
                                    }
                                    if (encrypt.DecryptString(str) != "")
                                    {
                                        reminderList.Items.Add(new MyListBoxItemRemind
                                        {
                                            Text = encrypt.DecryptString(str),
                                            Name = encrypt.DecryptString(str),
                                            Time = DateTime.Now.AddHours(1),
                                            Value = file.FullName
                                        });
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    string str = item.ToString();
                                    while (str.Contains("****"))
                                    {
                                        str = str.Replace("****", "***");
                                    }
                                    if (encrypt.DecryptString(str).Contains(keyword))
                                    {
                                        reminderList.Items.Add(new MyListBoxItemRemind
                                        {
                                            Text = encrypt.DecryptString(str),
                                            Name = encrypt.DecryptString(str),
                                            Time = DateTime.Now.AddHours(1),
                                            Value = file.FullName
                                        });
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
            }
            mindmaplist_count.Text = mindmaplist.Items.Count.ToString();
        }
        //删除全选按钮
        //private void selectALL_Click(object sender, EventArgs e)
        //{
        //    if (selectALL.Text == @"全选")
        //    {
        //        for (int i = 0; i < mindmaplist.Items.Count; i++)
        //        {
        //            mindmaplist.SetItemChecked(i, true);
        //        }
        //        showtomorrow.Checked = false;
        //        selectALL.Text = @"取消全选";
        //    }
        //    else
        //    {
        //        for (int i = 0; i < mindmaplist.Items.Count; i++)
        //        {
        //            mindmaplist.SetItemChecked(i, false);
        //        }
        //        reminder_yearafter.Checked = true;
        //        selectALL.Text = @"全选";
        //    }
        //    if (true)
        //    {
        //        shaixuanfuwei();
        //        ChangeReminder();
        //    }
        //}
        private void Reminderlist_DrawItem(object sender, DrawItemEventArgs e)
        {

            if (e.Index >= 0)
            {
                int zhongyao = 0;
                string name = "";
                zhongyao = ((MyListBoxItemRemind)reminderList.Items[e.Index]).level;
                name = ((MyListBoxItemRemind)reminderList.Items[e.Index]).Name;
                System.Drawing.Brush mybsh = Brushes.Gray;
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);
                Rectangle rectleft = new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height);
                if (zhongyao == 0)
                {
                    SolidBrush zeroColor = new SolidBrush(Color.FromArgb(238, 238, 242));
                    if (searchword.Text.StartsWith("#") || searchword.Text.StartsWith("*"))
                    {
                        zeroColor = new SolidBrush(Color.White);
                    }
                    e.Graphics.FillRectangle(zeroColor, rect);
                    mybsh = new SolidBrush(Color.FromArgb(238, 238, 242));
                    if (name == "当前时间")
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(238, 238, 242)), e.Bounds);
                        mybsh = Brushes.Gray;
                    }
                }
                else if (zhongyao == 1)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Azure), rect);
                    mybsh = new SolidBrush(Color.Azure);
                }
                else if (zhongyao == 2)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.PowderBlue), rect);
                    mybsh = new SolidBrush(Color.PowderBlue);
                }
                else if (zhongyao == 3)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightSkyBlue), rect);
                    mybsh = new SolidBrush(Color.LightSkyBlue);
                }
                else if (zhongyao == 4)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.DeepSkyBlue), rect);
                    mybsh = new SolidBrush(Color.DeepSkyBlue);
                }
                else if (zhongyao == 5)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.CadetBlue), rect);
                    mybsh = new SolidBrush(Color.CadetBlue);
                }
                else if (zhongyao == 6)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Gold), rect);
                    mybsh = new SolidBrush(Color.Gold);
                }
                else if (zhongyao == 7)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Orange), rect);
                    mybsh = new SolidBrush(Color.Orange);
                }
                else if (zhongyao == 8)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.OrangeRed), rect);
                    mybsh = new SolidBrush(Color.OrangeRed);
                }
                else if (zhongyao == 9)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Crimson), rect);
                    mybsh = new SolidBrush(Color.Crimson);
                }
                else if (zhongyao >= 10)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), rect);
                    mybsh = new SolidBrush(Color.Red);
                }
                if (e.Index == reminderList.SelectedIndex)
                {
                    e.Graphics.FillRectangle(mybsh, rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rectleft); //Yellow
                }
                else
                {
                    e.Graphics.FillRectangle(mybsh, rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), rectleft); //Yellow
                }
                if (searchword.Text.StartsWith("#"))
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderList.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    ////e.DrawFocusRectangle();

                }
                else if (searchword.Text.StartsWith("*"))
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderList.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    ////e.DrawFocusRectangle();
                }
                else if (!((MyListBoxItemRemind)reminderList.Items[e.Index]).isTask)
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderList.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    ////e.DrawFocusRectangle();
                }
                else
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderList.Items[e.Index]).Text.Substring(0, 3), e.Font, mybsh, rect, StringFormat.GenericDefault);
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderList.Items[e.Index]).Text.Substring(3), e.Font, Brushes.Gray, rectleft, StringFormat.GenericDefault);
                    ((MyListBoxItemRemind)reminderList.Items[e.Index]).IsShow = true;
                    e = new DrawItemEventArgs(e.Graphics,
                                            e.Font,
                                            e.Bounds,
                                            e.Index,
                                            e.State,
                                            e.ForeColor,
                                            Color.White);
                    ////e.DrawFocusRectangle();
                }
            }
        }
        public void pageinfo(string info)
        {
            int remindCount = 0;
            int remindHours = 0;
            labeltaskinfo.Text = info;
            foreach (MyListBoxItemRemind item in reminderList.Items)
            {
                if (item.IsShow)
                {
                    remindCount += 1;
                    if (item.Time.Hour + (float)item.rtaskTime / 60 <= 24)
                    {
                        remindHours += item.rtaskTime;
                    }
                    else
                    {
                        remindHours += (24 - item.Time.Hour) * 60;
                        remindHours -= item.Time.Minute;
                    }
                }
            }
            foreach (MyListBoxItemRemind item in reminderListBox.Items)
            {
                if (item.IsShow)
                {
                    remindCount += 1;
                    if (item.Time.Hour + (float)item.rtaskTime / 60 <= 24)
                    {
                        remindHours += item.rtaskTime;
                    }
                    else
                    {
                        remindHours += (24 - item.Time.Hour) * 60;
                        remindHours -= item.Time.Minute;
                    }
                }
            }
            Hours.Text = ((float)remindHours / 60).ToString("N2");
            reminder_count.Text = remindCount.ToString();
        }
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (true)
            {
                shaixuanfuwei();
                RRReminderlist();
            }
        }
        private void showtomorrow_CheckedChanged(object sender, EventArgs e)
        {
            if (!showtomorrow.Checked)
            {
                reminder_week.Checked = false;
                reminder_month.Checked = false;
                reminder_year.Checked = false;
                reminder_yearafter.Checked = false;
            }
            if (!(reminder_week.Checked || reminder_month.Checked || reminder_year.Checked || reminder_yearafter.Checked))
            {
                if (true)
                {
                    //ChangeReminder();
                }
            }
        }
        private void Reminder_week_CheckedChanged(object sender, EventArgs e)
        {
            if (!reminder_week.Checked)
            {
                reminder_month.Checked = false;
                reminder_year.Checked = false;
                reminder_yearafter.Checked = false;
            }
            else
            {
                showtomorrow.Checked = true;
            }
            if (!(reminder_month.Checked || reminder_year.Checked || reminder_yearafter.Checked))
            {
                if (true)
                {
                    //ChangeReminder();
                }
            }
        }
        private void Reminder_month_CheckedChanged(object sender, EventArgs e)
        {
            if (!reminder_month.Checked)
            {
                reminder_year.Checked = false;
                reminder_yearafter.Checked = false;
            }
            else
            {
                showtomorrow.Checked = true;
                reminder_week.Checked = true;
            }
            if (!(reminder_year.Checked || reminder_yearafter.Checked))
            {
                if (true)
                {
                    //ChangeReminder();
                }
            }
        }
        private void Reminder_year_CheckedChanged(object sender, EventArgs e)
        {
            if (reminder_year.Checked)
            {
                showtomorrow.Checked = true;
                reminder_week.Checked = true;
                reminder_month.Checked = true;
            }
            else
            {
                reminder_yearafter.Checked = false;
            }
            if (!reminder_yearafter.Checked)
            {
                if (true)
                {
                    //ChangeReminder();
                }
            }
        }
        private void Reminder_yearafter_CheckedChanged(object sender, EventArgs e)
        {
            if (reminder_yearafter.Checked)
            {
                showtomorrow.Checked = true;
                reminder_week.Checked = true;
                reminder_month.Checked = true;
                reminder_year.Checked = true;
            }
            if (true)
            {
                //ChangeReminder();
            }
        }
        public string GetFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
        public string GetTopString(FileInfo file)
        {
            string topString = "";
            //if (false)
            //{
            //    topString = file.CreationTime.ToString("yyyy/MM/dd HH:mm") + "   ";
            //}
            //else if (false)
            //{
            //    topString = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm") + "   ";
            //}
            //else if (false)
            //{
            //    topString = file.Length.ToString() + " " + GetFileSize(file.Length) + "   ";
            //}
            return topString;
        }
        public void MoveItem(int n)
        {
            for (int i = n; i > 0; i--)
            {
                if (Convert.ToInt64(((MyListBoxItem)mindmaplist.Items[i - 1]).Text.Split(' ')[0]) > Convert.ToInt64(((MyListBoxItem)mindmaplist.Items[i]).Text.Split(' ')[0]))
                {
                    object item = mindmaplist.Items[i - 1];
                    mindmaplist.Items.RemoveAt(i - 1);
                    mindmaplist.Items.Insert(i, item);
                }
            }
        }
        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            //if (this.Location.Y < -60)//已隐藏
            //    Center();//= new System.Drawing.Point(this.Location.X, 2);
            if (InReminderBool)
            {
                reminderList.Refresh();
                InReminderBool = false;
            }
            //if (this.Location.X < -60)//已隐藏
            //    Center();//= new System.Drawing.Point(1, this.Location.Y);
        }
        private void Form1_MouseHover(object sender, EventArgs e)
        {
            //hoverTimer.Stop();//关闭计时器
            //hoverTimer.Start();//重新计时
        }
        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            //if (this.Location.Y < 30 && ((Cursor.Position.X < this.Location.X || Cursor.Position.Y < this.Location.Y) || (Cursor.Position.X > this.Location.X + 836 || Cursor.Position.Y > this.Location.Y + 544)))
            //Center();//= new Point(this.Location.X, -543);
        }
        private void taskComplete_btn_Click(object sender, EventArgs e)
        {
            CompleteSelectedTask();
        }
        public void CompleteSelectedTask()
        {
            try
            {
                int reminderIndex = reminderList.SelectedIndex;
                bool isreminderlist = true;
                if (reminderListBox.Focused)
                {
                    isreminderlist = false;
                    reminderIndex = reminderListBox.SelectedIndex;
                }
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                CompleteTask(selectedReminder);
                taskTime.Value = 0;
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                //Thread th1 = new Thread(() => AddTaskToFile("log.mm", "完成", selectedReminder.Name, false));
                //th1.Start();
                if (isreminderlist)
                {
                    reminderList.Items.RemoveAt(reminderIndex);
                    if (reminderIndex <= reminderList.Items.Count - 1)//1,0 0>=0
                    {
                        reminderList.SelectedIndex = reminderIndex;
                    }
                }
                else
                {
                    reminderListBox.Items.RemoveAt(reminderIndex);
                    Reminderlistboxchange();
                    if (reminderIndex <= reminderListBox.Items.Count - 1)//1,0 0>=0
                    {
                        reminderListBox.SelectedIndex = reminderIndex;
                    }
                }
                if (reminderList.Items.Count == 0 && ebcheckBox.Checked)
                {
                    ebcheckBox.Checked = false;
                }

                fenshuADD(selectedReminder.level > 0 ? selectedReminder.level : 1);
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        public static DateTime GetNextTime(DateTime dt, TimeSpan span)
        {
            do
            {
                dt += span;
            } while (dt < DateTime.Now);
            return dt;
        }
        public static TimeSpan getAddTime(int zhouqi, int zhouqinum)
        {
            switch (zhouqi)
            {
                case 1:
                    if (zhouqinum < 6)
                    {
                        return new TimeSpan(0, 5, 0);
                    }
                    else
                    {
                        return new TimeSpan(0, 30, 0);
                    }
                case 2:
                    if (zhouqinum < 24)
                    {
                        return new TimeSpan(0, 30, 0);
                    }
                    else
                    {
                        return new TimeSpan(12, 0, 0);
                    }
                case 3:
                    if (zhouqinum < 2)
                    {
                        return new TimeSpan(12, 0, 0);
                    }
                    else
                    {
                        return new TimeSpan(1, 0, 0, 0);
                    }
                case 4:
                    if (zhouqinum < 2)
                    {
                        return new TimeSpan(1, 0, 0, 0);
                    }
                    else
                    {
                        return new TimeSpan(2, 0, 0, 0);
                    }
                case 5:
                    if (zhouqinum < 2)
                    {
                        return new TimeSpan(2, 0, 0, 0);
                    }
                    else
                    {
                        return new TimeSpan(4, 0, 0, 0);
                    }
                case 6:
                    if (zhouqinum < 2)
                    {
                        return new TimeSpan(4, 0, 0, 0);
                    }
                    else
                    {
                        return new TimeSpan(7, 0, 0, 0);
                    }
                case 7:
                    if (zhouqinum < 2)
                    {
                        return new TimeSpan(7, 0, 0, 0);
                    }
                    else
                    {
                        return new TimeSpan(15, 0, 0, 0);
                    }
                case 8:
                    return new TimeSpan(15 + zhouqinum, 0, 0, 0);
                default:
                    return new TimeSpan(1, 0, 0);
            }
        }
        public static int GetNextZhouqi(int zhouqi, int zhouqinum)
        {
            switch (zhouqi)
            {
                case 0:
                    return 1;
                case 1:
                    if (zhouqinum < 6)
                    {
                        return zhouqi;
                    }
                    break;
                case 2:
                    if (zhouqinum < 24)
                    {
                        return zhouqi;
                    }
                    break;
                case 3:
                    if (zhouqinum < 2)
                    {
                        return zhouqi;
                    }
                    break;
                case 4:
                    if (zhouqinum < 2)
                    {
                        return zhouqi;
                    }
                    break;
                case 5:
                    if (zhouqinum < 2)
                    {
                        return zhouqi;
                    }
                    break;
                case 6:
                    if (zhouqinum < 2)
                    {
                        return zhouqi;
                    }
                    break;
                case 7:
                    if (zhouqinum < 2)
                    {
                        return zhouqi;
                    }
                    break;
                case 8:
                    return zhouqi;
                default:
                    break;
            }
            return zhouqi + 1;
        }
        public static int GetNextZhouqiNum(int zhouqi, int zhouqinum)
        {
            switch (zhouqi)
            {
                case 0:
                    return 1;
                case 1:
                    if (zhouqinum < 6)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 2:
                    if (zhouqinum < 24)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 3:
                    if (zhouqinum < 2)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 4:
                    if (zhouqinum < 2)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 5:
                    if (zhouqinum < 2)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 6:
                    if (zhouqinum < 2)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 7:
                    if (zhouqinum < 2)
                    {
                        return zhouqinum + 1;
                    }
                    break;
                case 8:
                    return zhouqinum + 1;
                default:
                    break;
            }
            return 1;
        }
        public void CompleteTask(MyListBoxItemRemind selectedReminder)
        {
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("node"))
            {
                if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == selectedReminder.IDinXML)
                {
                    //Thread th = new Thread(() => writeLog(selectedReminder.Text.Substring(29), selectedReminder.Value.Split('\\')[selectedReminder.Value.Split('\\').Length - 1], selectedReminder.Datetime, "button_ok"));
                    //th.Start();
                    SaveLog("完成任务：" + selectedReminder.Name + "    导图" + selectedReminder.Value.Split('\\')[selectedReminder.Value.Split('\\').Length - 1]);
                    if (selectedReminder.IsDaka == "true")
                    {
                        node.Attributes["DAKADAY"].Value = (selectedReminder.DakaDay += 1).ToString();
                    }
                    if (selectedReminder.remindertype == "" || selectedReminder.remindertype == "onetime")
                    {
                        XmlNode newElem = x.CreateElement("icon");
                        XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                        BUILTIN.Value = "button_ok";
                        newElem.Attributes.Append(BUILTIN);
                        node.AppendChild(newElem);
                        //添加子节点
                        if (searchword.Text != "")
                        {
                            XmlNode newNote = x.CreateElement("node");
                            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                            newNotetext.Value = searchword.Text;
                            if (IsURL(newNotetext.Value))
                            {
                                string title = GetWebTitle(newNotetext.Value);
                                if (title != "" && title != "忘记了，后面再改")
                                {
                                    //添加属性
                                    XmlAttribute TASKLink = x.CreateAttribute("LINK");
                                    TASKLink.Value = newNotetext.Value;
                                    newNote.Attributes.Append(TASKLink);
                                    newNotetext.Value = title;
                                }
                            }
                            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            XmlAttribute TASKID = x.CreateAttribute("ID");
                            newNote.Attributes.Append(TASKID);
                            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                            newNote.Attributes.Append(newNotetext);
                            newNote.Attributes.Append(newNoteCREATED);
                            newNote.Attributes.Append(newNoteMODIFIED);
                            node.AppendChild(newNote);
                            searchword.Text = "";
                        }
                        //删除提醒
                        //node.RemoveChild(node);
                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (item != null && item.Attributes != null && item.Attributes["NAME"] != null && item.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml")
                            {
                                node.RemoveChild(item);
                                break;
                            }
                        }
                    }
                    else
                    {
                        switch (selectedReminder.remindertype)
                        {
                            case "hour":
                                if (selectedReminder.rhours == 0)
                                {
                                    selectedReminder.rhours = 1;
                                }
                                do
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddHours(selectedReminder.rhours);
                                }
                                while (selectedReminder.Time < DateTime.Now);
                                break;
                            case "day":
                                if (selectedReminder.rdays == 0)
                                {
                                    selectedReminder.rdays = 1;
                                }
                                do
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddDays(selectedReminder.rdays);
                                }
                                while (selectedReminder.Time < DateTime.Now);
                                break;
                            case "week":
                                do
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddDays(1);
                                    if (selectedReminder.Time.DayOfWeek.ToString() == "Sunday")
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(selectedReminder.rWeek * 7);
                                    }
                                }
                                while ((selectedReminder.rweeks != new char[] { } && !selectedReminder.rweeks.Contains(GetWeekIndex(selectedReminder.Time))) || DateTime.Now > selectedReminder.Time);
                                break;
                            case "month":
                                if (selectedReminder.rMonth == 0)
                                {
                                    selectedReminder.rMonth = 1;
                                }
                                do
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddMonths(selectedReminder.rMonth);
                                }
                                while (selectedReminder.Time < DateTime.Now);

                                break;
                            case "year":
                                if (selectedReminder.ryear == 0)
                                {
                                    selectedReminder.ryear = 1;
                                }
                                do
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddYears(selectedReminder.ryear);
                                }
                                while (selectedReminder.Time < DateTime.Now);
                                break;
                            case "eb":
                                if (selectedReminder.ebstring == 0 || selectedReminder.ebstring + 10000000 < ebconfig)
                                {
                                    selectedReminder.ebstring += 10000000;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddMinutes(30);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 1000000 < ebconfig)
                                {
                                    selectedReminder.ebstring += 1000000;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddHours(1);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 100000 < ebconfig)
                                {
                                    selectedReminder.ebstring += 100000;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddHours(5);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 10000 < ebconfig)
                                {
                                    selectedReminder.ebstring += 10000;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(1);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 1000 < ebconfig)
                                {
                                    selectedReminder.ebstring += 1000;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(2);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 100 < ebconfig)
                                {
                                    selectedReminder.ebstring += 100;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(4);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 10 < ebconfig)
                                {
                                    selectedReminder.ebstring += 10;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(7);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring + 1 < ebconfig)
                                {
                                    selectedReminder.ebstring += 1;
                                    if (selectedReminder.Time > DateTime.Now)
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(15);
                                    }
                                    else
                                    {
                                        selectedReminder.Time = DateTime.Now.AddMinutes(30);

                                    }
                                }
                                else if (selectedReminder.ebstring >= ebconfig - 1)//这个逻辑更复杂稍后在写
                                {
                                    selectedReminder.ebstring += 1;
                                    selectedReminder.Time = selectedReminder.Time.AddDays(15 + selectedReminder.ebstring - ebconfig);
                                }
                                else
                                {
                                    selectedReminder.Time = selectedReminder.Time.AddDays(1);
                                }
                                if (node.Attributes["EBSTRING"] == null)
                                {
                                    XmlAttribute ebstringAttribute = x.CreateAttribute("EBSTRING");
                                    ebstringAttribute.Value = "0";
                                    node.Attributes.Append(ebstringAttribute);
                                }
                                node.Attributes["EBSTRING"].Value = selectedReminder.ebstring.ToString();
                                break;
                        }
                        //node.FirstChild.Attributes["REMINDUSERAT"].Value = (Convert.ToInt64((selectedReminder.Time - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (item != null && item.Attributes != null && item.Attributes["NAME"] != null && item.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml")
                            {
                                item.FirstChild.Attributes["REMINDUSERAT"].Value = (Convert.ToInt64((selectedReminder.Time - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                break;
                            }
                        }
                        //添加子节点
                        //if (searchword.Text != "")
                        //{
                        //    XmlNode newNote = x.CreateElement("node");
                        //    XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                        //    newNotetext.Value = searchword.Text;
                        //    XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                        //    newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        //    XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                        //    newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        //    newNote.Attributes.Append(newNotetext);
                        //    newNote.Attributes.Append(newNoteCREATED);
                        //    newNote.Attributes.Append(newNoteMODIFIED);
                        //    node.AppendChild(newNote);
                        //    searchword.Text = "";
                        //}
                    }
                    x.Save(selectedReminder.Value);
                    return;
                }
            }
        }


        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //if (dateTimePicker.Focused && taskTime.Value == 0)
            //{
            //    taskTime.Value = 10;
            //}
            //if (dateTimePicker.Focused && tasklevel.Value == 0 && mindmapornode.Text.Contains(">"))
            //{
            //    tasklevel.Value = 1;
            //}
            //if (dateTimePicker.Focused && dateTimePicker.Value < DateTime.Now && dateTimePicker.Value.Day == 1)
            //{
            //    dateTimePicker.Value = dateTimePicker.Value.AddMonths(1);
            //}
        }
        bool isInReminderList = false;
        private void reminderlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            reminderlistSelectedItem = reminderList.SelectedItem;
            if (reminderlistSelectedItem != null)
            {
                isInReminderList = true;
            }
            else
            {
                isInReminderList = false;
            }
            if (reminderListBox.Focused)
            {
                reminderlistSelectedItem = reminderListBox.SelectedItem;
                reminderList.SelectedIndex = -1;
            }
            else
            {
                reminderListBox.SelectedIndex = -1;
            }
            if (reminderListBox.Visible)
            {
                reminderListBox.Refresh();
            }
            reminderList.Refresh();
            if (reminderlistSelectedItem == null)
            {
                return;
            }
            if (searchword.Text.ToLower().StartsWith("`") || searchword.Text.ToLower().StartsWith("·"))
            {
                if (reminderlistSelectedItem != null)
                {
                    richTextSubNode.Text = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                }
                return;
            }
            PlaySimpleSound("next");
            isInReminderlistSelect = true;
            if (searchword.Text.StartsWith("#"))
            {
                richTextSubNode.Text = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                return;
            }
            if (searchword.Text.StartsWith("node"))
            {
                if (reminderlistSelectedItem != null)
                {
                    richTextSubNode.Text = ((MyListBoxItemRemind)reminderlistSelectedItem).Text;

                }
                try
                {
                    //
                    string mindmap = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                    for (int i = 0; i < mindmaplist.Items.Count; i++)
                    {
                        if (mindmap == ((MyListBoxItem)mindmaplist.Items[i]).Value)
                        {
                            IsSelectReminder = true;
                            mindmaplist.SetSelected(i, true);
                            return;
                        }
                    }
                }
                catch (Exception)
                {

                    return;
                }
                return;
            }
            reminderSelectIndex = reminderList.SelectedIndex;
            if (reminderListBox.Focused)
            {
                reminderSelectIndex = reminderListBox.SelectedIndex;
            }
            if (reminderlistSelectedItem == null)
            {
                return;
            }
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            if (this.Height > 550)
            {
                SelectTreeNode(nodetree.Nodes, selectedReminder.Name);
            }
            if (selectedReminder.isEncrypted)
            {
                IsEncryptBool = true;
            }
            else
            {
                IsEncryptBool = false;
            }

            if (true)
            {
                ShowSubNode();
            }
            else
            {
                //ShowHTML();
            }
            //暂时不显示这些信息了
            //if (selectedReminder.IsDaka == "true")
            //{
            //    DAKAINFO.Text = String.Format("平均: {0}  ,  最高: {1}  ,  总天数： {2}   ，总次数： {3}    ，延迟：{4}", GetAVAge(selectedReminder.DakaDays, selectedReminder.DakaDay).ToString("0.##"), GetMax(selectedReminder.DakaDays, selectedReminder.DakaDay), GetSUM(selectedReminder.DakaDays, selectedReminder.DakaDay), GetNUM(selectedReminder.DakaDays, selectedReminder.DakaDay), selectedReminder.editTime);
            //}
            //else
            //{
            //    DAKAINFO.Text = String.Format("延迟：{0}", selectedReminder.editTime);
            //}
            tasklevel.Value = selectedReminder.level;
            c_day.Checked = c_week.Checked = c_hour.Checked =
                c_month.Checked =
                c_year.Checked =
                c_Saturday.Checked =
                c_Monday.Checked =
                c_Tuesday.Checked =
                c_Wednesday.Checked =
                c_Thursday.Checked =
                c_Friday.Checked =
                c_Saturday.Checked =
                c_remember.Checked =
                c_Sunday.Checked = false;
            button_cycle.Text = "设置周期";
            n_days.Value = 0;
            taskTime.Value = selectedReminder.rtaskTime;
            dateTimePicker.Value = selectedReminder.Time;
            if (selectedReminder.remindertype != "")
            {
                switch (selectedReminder.remindertype)
                {
                    case "hour":
                        c_hour.Checked = true;
                        n_days.Value = Convert.ToInt16(selectedReminder.rhours);
                        break;
                    case "day":
                        c_day.Checked = true;
                        n_days.Value = Convert.ToInt16(selectedReminder.rdays);
                        break;
                    case "week":
                        c_week.Checked = true;
                        n_days.Value = Convert.ToInt16(selectedReminder.rWeek);
                        for (int i = 0; i < selectedReminder.rweeks.Length; i++)
                        {
                            switch (selectedReminder.rweeks[i])
                            {
                                case '1':
                                    c_Monday.Checked = true;
                                    break;
                                case '2':
                                    c_Tuesday.Checked = true;
                                    break;
                                case '3':
                                    c_Wednesday.Checked = true;
                                    break;
                                case '4':
                                    c_Thursday.Checked = true;
                                    break;
                                case '5':
                                    c_Friday.Checked = true;
                                    break;
                                case '6':
                                    c_Saturday.Checked = true;
                                    break;
                                case '7':
                                    c_Sunday.Checked = true;
                                    break;
                            }
                        }
                        break;
                    case "month":
                        c_month.Checked = true;
                        n_days.Value = Convert.ToInt16(selectedReminder.rMonth);
                        break;
                    case "year":
                        c_year.Checked = true;
                        n_days.Value = Convert.ToInt16(selectedReminder.ryear);
                        break;
                    case "eb":
                        c_remember.Checked = true;
                        button_cycle.Text = selectedReminder.ebstring.ToString();
                        break;

                }
            }
            for (int i = 0; i < mindmaplist.Items.Count; i++)
            {
                if (selectedReminder.Value == ((MyListBoxItem)mindmaplist.Items[i]).Value)
                {
                    IsSelectReminder = true;
                    mindmaplist.SetSelected(i, true);
                    return;
                }
            }
        }
        private void button_cycle_Click(object sender, EventArgs e)
        {
            try
            {
                SetCycleTask();
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                RRReminderlist();
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        public string GetAttribute(XmlNode node, string name, int resultLenght = 0)
        {
            string resultdefault = "";
            for (int i = 0; i < resultLenght; i++)
            {
                resultdefault += " ";
            }
            try
            {
                if (node == null || node.Attributes == null || (name != "TEXT" && node.Attributes[name] == null))
                {
                    return resultdefault;
                }
                else if (node == null || node.Attributes == null || (name == "TEXT" && node.Attributes[name] == null))
                {
                    try
                    {
                        if (node.FirstChild.Name == "richcontent")
                        {
                            return new HtmlToString().StripHTML(node.FirstChild.InnerText);
                        }
                    }
                    catch (Exception)
                    {
                        return "未找到richcontent";
                    }
                }
                string result = "";
                result = node.Attributes[name].Value;
                for (int i = result.Length; i < resultLenght; i++)
                {
                    result = " " + result;
                }
                if (resultLenght != 0 && result.Trim() == "0")
                {
                    result = result.Replace("0", " ");
                }
                return result;
            }
            catch (Exception)
            {
                return resultdefault;
            }
        }
        public string lenghtString(string result, int resultLenght = 0)
        {
            for (int i = result.Length; i < resultLenght; i++)
            {
                result = "0" + result;
            }
            if (resultLenght != 0 && result.Trim() == "0")
            {
                result = result.Replace("0", " ");
            }
            return result;
        }

        public int MyToInt16(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return 0;
            }
            return Int32.Parse(value, CultureInfo.CurrentCulture);
        }
        public bool MyToBoolean(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region

        public void SetCycleTask()
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        if (selectedReminder.remindertype == "")
                        {
                            XmlNode newElem = x.CreateElement("icon");
                            XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                            BUILTIN.Value = "revision";
                            newElem.Attributes.Append(BUILTIN);
                            node.ParentNode.AppendChild(newElem);
                            XmlAttribute REMINDERTYPE = x.CreateAttribute("REMINDERTYPE");
                            XmlAttribute RDAYS = x.CreateAttribute("RDAYS");
                            XmlAttribute RWEEK = x.CreateAttribute("RWEEK");
                            XmlAttribute RMONTH = x.CreateAttribute("RMONTH");
                            XmlAttribute RWEEKS = x.CreateAttribute("RWEEKS");
                            XmlAttribute RYEAR = x.CreateAttribute("RYEAR");
                            XmlAttribute RHOUR = x.CreateAttribute("RHOUR");
                            node.ParentNode.Attributes.Append(REMINDERTYPE);
                            node.ParentNode.Attributes.Append(RDAYS);
                            node.ParentNode.Attributes.Append(RWEEK);
                            node.ParentNode.Attributes.Append(RMONTH);
                            node.ParentNode.Attributes.Append(RWEEKS);
                            node.ParentNode.Attributes.Append(RYEAR);
                            node.ParentNode.Attributes.Append(RHOUR);
                        }
                        //避免周期设置成0的问题
                        if (n_days.Value == 0)
                        {
                            n_days.Value = 1;
                        }
                        if (c_day.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "day";
                            selectedReminder.remindertype = "day";
                            node.ParentNode.Attributes["RDAYS"].Value = n_days.Value.ToString();
                            selectedReminder.rdays = (int)n_days.Value;
                        }
                        else if (c_week.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "week";
                            selectedReminder.remindertype = "week";
                            node.ParentNode.Attributes["RWEEK"].Value = n_days.Value.ToString();
                            selectedReminder.rWeek = (int)n_days.Value;
                            node.ParentNode.Attributes["RWEEKS"].Value = GetWeekStr();
                            selectedReminder.rweeks = GetWeekStr().ToArray();
                        }
                        else if (c_month.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "month";
                            selectedReminder.remindertype = "month";
                            node.ParentNode.Attributes["RMONTH"].Value = n_days.Value.ToString();
                            selectedReminder.rMonth = (int)n_days.Value;
                        }
                        else if (c_year.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "year";
                            selectedReminder.remindertype = "year";
                            node.ParentNode.Attributes["RYEAR"].Value = n_days.Value.ToString();
                            selectedReminder.ryear = (int)n_days.Value;
                        }
                        else if (c_hour.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "hour";
                            selectedReminder.remindertype = "hour";
                            node.ParentNode.Attributes["RHOUR"].Value = n_days.Value.ToString();
                            selectedReminder.rhours = (int)n_days.Value;
                        }
                        else if (c_remember.Checked)
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "eb";
                            selectedReminder.remindertype = "eb";
                            if (node.ParentNode.Attributes["EBSTRING"] == null)
                            {
                                XmlAttribute ebstringAttribute = x.CreateAttribute("EBSTRING");
                                ebstringAttribute.Value = "0";
                                node.ParentNode.Attributes.Append(ebstringAttribute);
                            }
                        }
                        else
                        {
                            node.ParentNode.Attributes["REMINDERTYPE"].Value = "onetime";
                            selectedReminder.remindertype = "onetime";
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        private void c_day_CheckedChanged(object sender, EventArgs e)
        {
            if (c_day.Checked)
            {
                c_week.Checked = c_month.Checked = c_year.Checked = c_hour.Checked = c_remember.Checked = false;
                button_cycle.Text = "设置周期";
                if (n_days.Value==0)
                {
                    n_days.Value = 1;
                }
            }
        }
        private void c_week_CheckedChanged(object sender, EventArgs e)
        {
            if (c_week.Checked)
            {
                c_day.Checked = c_month.Checked = c_year.Checked = c_hour.Checked = c_remember.Checked = false;
                button_cycle.Text = "设置周期";
            }
            if (n_days.Value == 0)
            {
                n_days.Value = 1;
            }
        }
        private void c_month_CheckedChanged(object sender, EventArgs e)
        {
            if (c_month.Checked)
            {
                c_week.Checked = c_day.Checked = c_year.Checked = c_hour.Checked = c_remember.Checked = false;
                button_cycle.Text = "设置周期";
            }
            if (n_days.Value == 0)
            {
                n_days.Value = 1;
            }
        }
        private void c_year_CheckedChanged(object sender, EventArgs e)
        {
            if (c_year.Checked)
            {
                c_week.Checked = c_month.Checked = c_day.Checked = c_hour.Checked = c_remember.Checked = false;
                button_cycle.Text = "设置周期";
            }
            if (n_days.Value == 0)
            {
                n_days.Value = 1;
            }
        }
        private void c_hour_CheckedChanged(object sender, EventArgs e)
        {
            if (c_hour.Checked)
            {
                c_week.Checked = c_month.Checked = c_day.Checked = c_year.Checked = c_remember.Checked = false;
                button_cycle.Text = "设置周期";
            }
            if (n_days.Value == 0)
            {
                n_days.Value = 1;
            }
        }
        private void c_remember_CheckedChanged(object sender, EventArgs e)
        {
            if (c_remember.Checked)
            {
                c_week.Checked = c_month.Checked = c_day.Checked = c_year.Checked = c_hour.Checked = false;
            }
        }
        #endregion

        public char GetWeekIndex(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Friday:
                    return '5';
                case DayOfWeek.Monday:
                    return '1';
                case DayOfWeek.Saturday:
                    return '6';
                case DayOfWeek.Sunday:
                    return '7';
                case DayOfWeek.Thursday:
                    return '4';
                case DayOfWeek.Tuesday:
                    return '2';
                case DayOfWeek.Wednesday:
                    return '3';
                default:
                    return '1';//避免编译报错
            }
        }
        /// <summary>
        /// 获取星期字符串，比如周一，周三则返回13
        /// </summary>
        /// <returns></returns>
        public string GetWeekStr()
        {
            string result = "";
            if (c_Monday.Checked)
            {
                result += "1";
            }
            if (c_Tuesday.Checked)
            {
                result += "2";
            }
            if (c_Wednesday.Checked)
            {
                result += "3";
            }
            if (c_Thursday.Checked)
            {
                result += "4";
            }
            if (c_Friday.Checked)
            {
                result += "5";
            }
            if (c_Saturday.Checked)
            {
                result += "6";
            }
            if (c_Sunday.Checked)
            {
                result += "7";
            }
            return result;
        }
        private void EditTime_Clic(object sender, EventArgs e)
        {
            try
            {
                if (reminderList.SelectedIndex != -1)
                {
                    reminderSelectIndex = reminderList.SelectedIndex;
                }
                EditTask();
                //ChangeReminder();
                reminderList.Focus();
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).Time = dateTimePicker.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).level = (int)tasklevel.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).rtaskTime = (int)taskTime.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).Text = newName((MyListBoxItemRemind)(reminderlistSelectedItem)).Text;
                taskTime.Value = 0;
                tasklevel.Value = 0;
                reminderList.SelectedIndex = reminderSelectIndex;
                reminderlist_SelectedIndexChanged(null, null);
                fenshuADD(1);
            }
            catch (Exception)
            {

            }
        }
        private void edittime_EndDate()
        {
            try
            {
                if (reminderList.SelectedIndex != -1)
                {
                    reminderSelectIndex = reminderList.SelectedIndex;
                }
                EditTaskEndDate();
                reminderList.Focus();
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).Time = dateTimePicker.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).level = (int)tasklevel.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).rtaskTime = (int)taskTime.Value;
                ((MyListBoxItemRemind)(reminderlistSelectedItem)).Text = newName((MyListBoxItemRemind)(reminderlistSelectedItem)).Text;
                taskTime.Value = 0;
                tasklevel.Value = 0;
                reminderList.Sorted = false;
                SortReminderList();
                reminderList.SelectedIndex = reminderSelectIndex;
                reminderlist_SelectedIndexChanged(null, null);
                fenshuADD(-1);
            }
            catch (Exception)
            {

            }
        }
        public void EditTask()
        {
            if (reminderList.SelectedIndex >= 0)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                string taskName = selectedReminder.Name;
                DateTime dateBefore = selectedReminder.Time;
                int taskTimeBefore = selectedReminder.rtaskTime;
                int tasklevelBefore = selectedReminder.level;
                if (selectedReminder.isEncrypted)
                {
                    taskName = encrypt.EncryptString(taskName);
                }
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == selectedReminder.IDinXML)
                    {
                        try
                        {
                            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            bool isHashook = false;
                            foreach (XmlNode item in node.ChildNodes)
                            {
                                if (item.Name == "hook" && !isHashook)
                                {
                                    isHashook = true;
                                    item.FirstChild.Attributes["REMINDUSERAT"].Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                }
                            }
                            if (!isHashook)
                            {
                                XmlNode remindernode = x.CreateElement("hook");
                                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                                remindernode.Attributes.Append(remindernodeName);
                                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                                remindernodeTime.Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                remindernodeParameters.Attributes.Append(remindernodeTime);
                                remindernode.AppendChild(remindernodeParameters);
                                node.AppendChild(remindernode);
                            }
                            XmlAttribute TASKTIME = x.CreateAttribute("TASKTIME");
                            node.Attributes.Append(TASKTIME);
                            node.Attributes["TASKTIME"].Value = taskTime.Value.ToString();
                            XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                            node.Attributes.Append(TASKLEVEL);
                            node.Attributes["TASKLEVEL"].Value = tasklevel.Value.ToString();
                            x.Save(selectedReminder.Value);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th.Start();
                            SaveLog("修改了任务：" + taskName + "    时间：" + dateBefore.ToString() + ">" + dateTimePicker.Value.ToString() + "    时长：" + taskTimeBefore.ToString() + ">" + taskTime.Value.ToString() + "    等级：" + tasklevelBefore.ToString() + ">" + tasklevel.Value.ToString());
                            return;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }
        public void EditTaskEndDate()
        {
            if (reminderList.SelectedIndex >= 0)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                string taskName = selectedReminder.Name;
                DateTime dateBefore = selectedReminder.EndDate;
                if (selectedReminder.isEncrypted)
                {
                    taskName = encrypt.EncryptString(taskName);
                }
                foreach (XmlNode node in x.GetElementsByTagName("hook"))
                {
                    try
                    {
                        if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                        {
                            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            if (GetAttribute(node.FirstChild, "EndDate") != "")
                            {
                                node.FirstChild.Attributes["EndDate"].Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            }
                            else
                            {
                                //添加属性
                                XmlAttribute TASKLEVEL = x.CreateAttribute("EndDate");
                                node.ParentNode.Attributes.Append(TASKLEVEL);
                                node.ParentNode.Attributes["EndDate"].Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            }
                            x.Save(selectedReminder.Value);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th.Start();
                            SaveLog("修改了任务：" + taskName + "    截止时间：" + dateBefore.ToString() + ">" + dateTimePicker.Value.ToString());
                            return;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        public void SetTaskIsView()
        {
            if (reminderList.SelectedIndex == -1)
            {
                return;
            }
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        if (selectedReminder.IsView == "true")
                        {
                            node.ParentNode.Attributes["ISVIEW"].Value = "false";
                        }
                        else
                        {
                            XmlAttribute IsView = x.CreateAttribute("ISVIEW");
                            IsView.Value = "true";
                            node.ParentNode.Attributes.Append(IsView);
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        public void OpenFanQie(int time, string name, string mindmap, int fanqieCount, bool isnotDefault = false)
        {
            Tomato fanqie = new Tomato(new DateTime().AddMinutes(time), name, mindmap, GetPosition(), isnotDefault);
            fanqie.ShowDialog();
        }
        public void OpenMenu()
        {
            Tools menu = new Tools();
            menu.ShowDialog();
        }
        private void Reminder_count_Click(object sender, EventArgs e)
        {
            if (reminderList.SelectedIndex >= 0)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                if (selectedReminder.rtaskTime > 0)
                {
                    Thread th = new Thread(() => OpenFanQie(selectedReminder.rtaskTime, selectedReminder.Name, selectedReminder.Value, GetPosition()));
                    tomatoCount += 1;
                    th.Start();
                }
            }
        }
        public void OpenFanqie(bool isnotdefault = false)
        {
            if (reminderList.SelectedIndex >= 0 || reminderListBox.SelectedIndex >= 0)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                if (selectedReminder.rtaskTime >= 0 || isnotdefault)
                {
                    Thread th = new Thread(() => OpenFanQie(selectedReminder.rtaskTime, selectedReminder.Name, selectedReminder.Value, GetPosition(), isnotdefault));
                    tomatoCount += 1;
                    th.Start();
                    if (IsURL(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                    {
                        System.Diagnostics.Process.Start(GetUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                        SaveLog("打开：    " + GetUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                    }
                    else if (IsFileUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                    {
                        System.Diagnostics.Process.Start(getFileUrlPath(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                        SaveLog("打开：    " + getFileUrlPath(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                    }
                    //如果是小时循环的，则自动完成
                    if (selectedReminder.remindertype == "hour")
                    {
                        try
                        {
                            CompleteTask(selectedReminder);
                            Thread th1 = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th1.Start();
                        }
                        catch (Exception)
                        {
                            if (reminderList.Items.Count > 0)
                            {
                                reminderList.SetSelected(0, true);
                            }

                        }
                    }
                }
            }
        }
        private void delay_Click(object sender, EventArgs e)
        {
            DelaySelectedTask();
        }
        public void DelaySelectedTask()
        {
            try
            {
                int reminderIndex = reminderList.SelectedIndex;
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                selectedReminder = newName(DelayTask(selectedReminder));
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                reminderList.Refresh();
                reminderList.Items.RemoveAt(reminderIndex);
                reminderList.SelectedIndex = reminderIndex;
                fenshuADD(-selectedReminder.level);
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }

        public MyListBoxItemRemind DelayTask(MyListBoxItemRemind selectedReminder)
        {
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            DateTime DateBefore = selectedReminder.Time;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        //if (selectedReminder.IsDaka == "true")
                        //{
                        //    node.ParentNode.Attributes["DAKADAYS"].Value = node.ParentNode.Attributes["DAKADAYS"].Value + "," + node.ParentNode.Attributes["DAKADAY"].Value;
                        //    node.ParentNode.Attributes["DAKADAY"].Value = "0";
                        //}
                        if (selectedReminder.remindertype == "" || selectedReminder.remindertype == "onetime")
                        {
                            //while (selectedReminder.Datetime < DateTime.Now && !DateTime.Equals(selectedReminder.Datetime, DateTime.Now.Date))
                            //{//如果时间小,且不是同一天,则加一天
                            //    selectedReminder.Datetime = selectedReminder.Datetime.AddDays(1);
                            //}
                            do
                            {
                                selectedReminder.Time = selectedReminder.Time.AddDays(1);
                            } while (selectedReminder.Time < DateTime.Today);
                        }
                        else
                        {
                            switch (selectedReminder.remindertype)
                            {
                                case "hour":
                                    if (selectedReminder.rhours == 0)
                                    {
                                        selectedReminder.rhours = 1;
                                    }
                                    do
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddHours(selectedReminder.rhours);
                                    }
                                    while (selectedReminder.Time < DateTime.Now);
                                    break;
                                case "day":
                                    if (selectedReminder.rdays == 0)
                                    {
                                        selectedReminder.rdays = 1;
                                    }
                                    do
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(selectedReminder.rdays);
                                    }
                                    while (selectedReminder.Time < DateTime.Now);
                                    break;
                                case "week":
                                    do
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddDays(1);
                                        if (selectedReminder.Time.DayOfWeek.ToString() == "Sunday")
                                        {
                                            selectedReminder.Time = selectedReminder.Time.AddDays(selectedReminder.rWeek * 7);
                                        }
                                    }
                                    while (!selectedReminder.rweeks.Contains(GetWeekIndex(selectedReminder.Time)) || DateTime.Now > selectedReminder.Time);
                                    break;
                                case "month":
                                    if (selectedReminder.rMonth == 0)
                                    {
                                        selectedReminder.rMonth = 1;
                                    }
                                    do
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddMonths(selectedReminder.rMonth);
                                    }
                                    while (selectedReminder.Time < DateTime.Now);

                                    break;
                                case "year":
                                    if (selectedReminder.ryear == 0)
                                    {
                                        selectedReminder.ryear = 1;
                                    }
                                    do
                                    {
                                        selectedReminder.Time = selectedReminder.Time.AddYears(selectedReminder.ryear);
                                    }
                                    while (selectedReminder.Time < DateTime.Now);
                                    break;
                            }
                        }
                        node.FirstChild.Attributes["REMINDUSERAT"].Value = (Convert.ToInt64((selectedReminder.Time - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        //取消增加等级
                        //try//避免没有TaskLevel的情况，懒得判断了
                        //{
                        //    if (Convert.ToInt64(node.ParentNode.Attributes["TASKLEVEL"].Value) < 9)
                        //    {
                        //        node.ParentNode.Attributes["TASKLEVEL"].Value = (Convert.ToInt64(node.ParentNode.Attributes["TASKLEVEL"].Value) + 1).ToString();
                        //        selectedReminder.level = Convert.ToInt16(node.ParentNode.Attributes["TASKLEVEL"].Value);
                        //    }
                        //}
                        //catch (Exception)
                        //{
                        //    XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                        //    node.ParentNode.Attributes.Append(TASKLEVEL);
                        //    node.ParentNode.Attributes["TASKLEVEL"].Value = "0";
                        //}
                        x.Save(selectedReminder.Value);
                        fenshuADD(-1);
                        SaveLog("推迟了任务:" + taskName + "从" + DateBefore.ToString() + "到" + selectedReminder.Time.ToString());
                        //AddTaskToFile("log.mm", "推迟", taskName + "从" + DateBefore.ToString() + "到" + selectedReminder.Datetime.ToString(), false);
                        return selectedReminder;
                    }
                }
                catch (Exception)
                {

                    return selectedReminder;
                }
            }
            return selectedReminder;
        }
        private void showcyclereminder_CheckedChanged(object sender, EventArgs e)
        {
            RRReminderlist();
        }
        public string GetTimeSpanStr(int day)
        {
            string result = "";
            int year = 0;
            int month = 0;
            if (day > 365)
            {
                year = day / 365;
                result += year + "年";
                day %= 365;
            }
            if (day > 30)
            {
                month = day / 30;
                result += month + "月";
                day %= 30;
            }
            result += day + "天";
            return result;
        }
        private void AddTask_Click(object sender, EventArgs e)
        {
            AddTask(false);
        }

        public void SearchNode()
        {
            isSearchFileOrNode = true;
            if (searchword.Text != "" && searchword.Text.StartsWith("*"))
            {
                string keywords = searchword.Text.Substring(1);
                if (keywords == "")
                {
                    return;
                }
                string[] keywordsArr = keywords.Split(' ');
                reminderList.Items.Clear();
                List<string> files = new List<string>();
                foreach (node item in nodes.Where(m => StringHasArrALL(m.Text, keywordsArr)).OrderByDescending(m => m.editDateTime).Take(200))
                {
                    if (!files.Contains(item.mindmapPath))
                    {
                        files.Add(item.mindmapPath);
                    }
                    reminderList.Items.Add(new MyListBoxItemRemind() { Text = item.Text, Value = item.mindmapPath, Time = item.Time, IDinXML = item.IDinXML });
                }
                mindmaplist.Items.Clear();
                foreach (string item in files)
                {
                    string filename = Path.GetFileName(item);
                    filename = filename.Substring(0, filename.Length - 3);
                    mindmaplist.Items.Insert(0, new MyListBoxItem { Text = filename, Value = item });
                }
                mindmapnumdesc = mindmaplist.Items.Count;
                for (int i = 0; i < mindmaplist.Items.Count; i++)
                {
                    mindmaplist.SetItemChecked(i, true);
                }
                return;
            }
        }
        public void AddTask(bool istask)
        {
            fenshuADD(3);
            PlaySimpleSound("add");
            if (searchword.Text != "" && searchword.Text.Contains("clip"))
            {
                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();
                string log = (string)iData.GetData(DataFormats.Text);
                if (log == null || log == "" || mindmaplist.SelectedItem == null)
                {
                    return;
                }
                if (IsURL(log.Trim()))
                {
                    log = GetWebTitle(log.Trim()) + " | " + log;
                }
                searchword.Text = searchword.Text.Replace("clip", log);
            }
            //搜索任务
            if (searchword.Text != "" && searchword.Text.StartsWith("t:"))
            {
            }
            if (searchword.Text != "" && searchword.Text.Contains("@@"))
            {
                string taskName = searchword.Text.Split('@')[0];
                string nodeName = searchword.Text.Split('@')[2];
                if (taskName == "")
                {
                    RenameNodeByID(nodeName);
                    SaveLog("修改节点名称：" + renameTaskName + "  To  " + searchword.Text);
                    searchword.Text = "";
                    //ChangeReminder();
                    return;
                }
                else
                {
                    AddNodeByID(istask, taskName);
                    SaveLog("Add节点名称：" + taskName + "  Map:  " + renameMindMapPath + "    节点：" + nodeName);
                    searchword.Text = "";
                    mindmapornode.Text = "";
                    if (istask)
                    {
                        RRReminderlist();
                    }
                    return;
                }
            }
            else if (searchword.Text != "" && searchword.Text.Contains("@"))
            {
                string filename = searchword.Text.Split('@')[1];
                string taskName = searchword.Text.Split('@')[0];
                if (filename == "gc")
                {
                    string gitCommand = "git";
                    //string gitAddArgument = @"add -A";
                    string gitCommitArgument = @"commit -a -m" + taskName + "@" + DateTime.Now.ToLongDateString();
                    //string gitPushArgument = @"push our_remote";
                    //System.Diagnostics.Process.Start(gitCommand, gitAddArgument);
                    //Thread.Sleep(2000);
                    System.Diagnostics.Process.Start(gitCommand, gitCommitArgument);
                    //System.Diagnostics.Process.Start(gitCommand, gitPushArgument);
                    SaveLog("git commit:" + searchword.Text);
                    searchword.Text = "";
                    return;
                }
                if (filename == "password")
                {
                    DocearReminderForm.PassWord = taskName;
                    searchword.Text = "";
                    RRReminderlist();
                    return;
                }
                mindmapfile file = mindmapfiles.FirstOrDefault(m => m.name.ToLower() == filename.ToLower());//不区分大小写 //是否需要优化下这个逻辑呢？？
                if (file == null)
                {
                    return;
                }
                if (taskName == "")
                {
                    System.Diagnostics.Process.Start(file.filePath);
                    searchword.Text = "";
                    MyHide();
                    return;
                }
                else
                {
                    System.Xml.XmlDocument x = new XmlDocument();
                    x.Load(file.filePath);
                    XmlNode root = x.GetElementsByTagName("node")[0];
                    //if (root.ChildNodes.Cast<XmlNode>().Any(m => m.Attributes[0].Name != "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString()))
                    if (!haschildNode(root, DateTime.Now.Year.ToString()))
                    {
                        XmlNode yearNode = x.CreateElement("node");
                        XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                        yearNodeValue.Value = DateTime.Now.Year.ToString();
                        yearNode.Attributes.Append(yearNodeValue);
                        root.AppendChild(yearNode);
                    }
                    XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString());
                    if (!haschildNode(year, DateTime.Now.Month.ToString()))
                    {
                        XmlNode monthNode = x.CreateElement("node");
                        XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                        monthNodeValue.Value = DateTime.Now.Month.ToString();
                        monthNode.Attributes.Append(monthNodeValue);
                        year.AppendChild(monthNode);
                    }
                    XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Month.ToString());
                    if (!haschildNode(month, DateTime.Now.Day.ToString()))
                    {
                        XmlNode dayNode = x.CreateElement("node");
                        XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                        dayNodeValue.Value = DateTime.Now.Day.ToString();
                        dayNode.Attributes.Append(dayNodeValue);
                        month.AppendChild(dayNode);
                    }
                    XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Day.ToString());
                    XmlNode newNote = x.CreateElement("node");
                    XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                    DateTime taskTime = DateTime.Now;
                    //任务时间
                    if (taskName.Contains("明天"))
                    {
                        taskTime = taskTime.AddDays(1);
                        taskName = taskName.Replace("明天", "");
                    }
                    if (taskName.Contains("后天"))
                    {
                        taskTime = taskTime.AddDays(2);
                        taskName = taskName.Replace("后天", "");
                    }
                    bool isHasHour = false;
                    if (taskName.Contains("10点"))
                    {
                        int hourDiff = 10 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("10点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("11点"))
                    {
                        int hourDiff = 11 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("11点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("12点"))
                    {
                        int hourDiff = 12 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("12点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("13点"))
                    {
                        int hourDiff = 13 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("13点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("14点"))
                    {
                        int hourDiff = 14 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("14点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("15点"))
                    {
                        int hourDiff = 15 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("15点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("16点"))
                    {
                        int hourDiff = 16 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("16点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("17点"))
                    {
                        int hourDiff = 17 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("17点", "");
                    }
                    if (taskName.Contains("18点"))
                    {
                        int hourDiff = 18 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("18点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("19点"))
                    {
                        int hourDiff = 19 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("19点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("20点"))
                    {
                        int hourDiff = 20 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("20点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("21点"))
                    {
                        int hourDiff = 21 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("21点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("22点"))
                    {
                        int hourDiff = 22 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("22点", "");
                    }
                    if (taskName.Contains("4点"))
                    {
                        int hourDiff = 4 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("4点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("5点"))
                    {
                        int hourDiff = 5 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("5点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("6点"))
                    {
                        int hourDiff = 6 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("6点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("7点"))
                    {
                        int hourDiff = 7 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("7点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("7点"))
                    {
                        int hourDiff = 8 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("8点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("9点"))
                    {
                        int hourDiff = 9 - taskTime.Hour;
                        taskTime = taskTime.AddHours(hourDiff);
                        taskName = taskName.Replace("9点", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("15分"))
                    {
                        int hourDiff = 15 - taskTime.Minute;
                        taskTime = taskTime.AddMinutes(hourDiff);
                        taskName = taskName.Replace("15分", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("30分"))
                    {
                        int hourDiff = 30 - taskTime.Minute;
                        taskTime = taskTime.AddMinutes(hourDiff);
                        taskName = taskName.Replace("30分", "");
                        isHasHour = true;
                    }
                    if (taskName.Contains("45分"))
                    {
                        int hourDiff = 45 - taskTime.Minute;
                        taskTime = taskTime.AddMinutes(hourDiff);
                        taskName = taskName.Replace("45分", "");
                        isHasHour = true;
                    }
                    if (isHasHour && taskName.Contains("整"))
                    {
                        int hourDiff = 0 - taskTime.Minute;
                        taskTime = taskTime.AddMinutes(hourDiff);
                        taskName = taskName.Replace("整", "");
                    }
                    if (isHasHour && taskName.Contains("半"))
                    {
                        int hourDiff = 30 - taskTime.Minute;
                        taskTime = taskTime.AddMinutes(hourDiff);
                        taskName = taskName.Replace("半", "");
                    }
                    string taskLevel1 = "1";
                    MatchCollection jc = Regex.Matches(taskName, @"[1-9]\d*j");
                    foreach (Match m in jc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskLevel1 = m.Value.Substring(0, m.Value.Length - 1);
                        break;
                    }
                    MatchCollection Mc = Regex.Matches(taskName, @"[1-9]\d*month");
                    foreach (Match m in Mc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskTime = taskTime.AddMonths(Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 5)));
                        break;
                    }
                    MatchCollection mc = Regex.Matches(taskName, @"[1-9]\d*m");
                    string minutes = "0";
                    foreach (Match m in mc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        minutes = m.Value.Substring(0, m.Value.Length - 1);
                        break;
                    }
                    //几年以后
                    MatchCollection Yc = Regex.Matches(taskName, @"[1-9]\d*Y");
                    foreach (Match m in Yc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskTime = taskTime.AddYears(Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 1)));
                        break;
                    }

                    MatchCollection Dc = Regex.Matches(taskName, @"[1-9]\d*D");
                    foreach (Match m in Dc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskTime = taskTime.AddDays(Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 1)));
                        break;
                    }
                    MatchCollection Hc = Regex.Matches(taskName, @"[1-9]\d*H");
                    foreach (Match m in Hc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskTime = taskTime.AddHours(Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 1)));
                        break;
                    }
                    Mc = Regex.Matches(taskName, @"[1-9]\d*M");
                    foreach (Match m in Mc)
                    {
                        taskName = taskName.Replace(m.Value, "");
                        taskTime = taskTime.AddMinutes(Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 1)));
                        break;
                    }
                    newNotetext.Value = taskName;
                    XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                    newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                    XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                    newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                    newNote.Attributes.Append(newNotetext);
                    newNote.Attributes.Append(newNoteCREATED);
                    newNote.Attributes.Append(newNoteMODIFIED);
                    XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                    newNote.Attributes.Append(TASKLEVEL);
                    newNote.Attributes["TASKLEVEL"].Value = taskLevel1;
                    XmlAttribute TASKTIME = x.CreateAttribute("TASKTIME");
                    newNote.Attributes.Append(TASKTIME);
                    newNote.Attributes["TASKTIME"].Value = minutes;
                    XmlAttribute TASKID = x.CreateAttribute("ID");
                    newNote.Attributes.Append(TASKID);
                    newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                    //如果已.开始，不设置任务，只添加文本
                    if (!(taskName.StartsWith(".") || taskName.StartsWith(" ")))
                    {
                        newNotetext.Value = taskName;
                        XmlNode remindernode = x.CreateElement("hook");
                        XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                        remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                        remindernode.Attributes.Append(remindernodeName);
                        XmlNode remindernodeParameters = x.CreateElement("Parameters");
                        XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                        remindernodeTime.Value = (Convert.ToInt64((taskTime - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        remindernodeParameters.Attributes.Append(remindernodeTime);
                        remindernode.AppendChild(remindernodeParameters);
                        newNote.AppendChild(remindernode);
                    }
                    else
                    {
                        newNotetext.Value = taskName.Substring(1);
                    }
                    if (IsURL(newNotetext.Value))
                    {
                        string title = GetWebTitle(newNotetext.Value);
                        if (title!=""&& title != "忘记了，后面再改")
                        {
                            //添加属性
                            XmlAttribute TASKLink = x.CreateAttribute("LINK");
                            TASKLink.Value = newNotetext.Value;
                            newNote.Attributes.Append(TASKLink);
                            newNotetext.Value = title;
                        }
                    }
                    day.AppendChild(newNote);
                    searchword.Text = "";
                    if (ebdefault.Contains(new FileInfo(file.filePath).Name))
                    {
                        XmlAttribute REMINDERTYPE = x.CreateAttribute("REMINDERTYPE");
                        REMINDERTYPE.Value = "eb";
                        XmlAttribute RDAYS = x.CreateAttribute("RDAYS");
                        XmlAttribute RWEEK = x.CreateAttribute("RWEEK");
                        XmlAttribute RMONTH = x.CreateAttribute("RMONTH");
                        XmlAttribute RWEEKS = x.CreateAttribute("RWEEKS");
                        XmlAttribute RYEAR = x.CreateAttribute("RYEAR");
                        XmlAttribute RHOUR = x.CreateAttribute("RHOUR");
                        newNote.Attributes.Append(REMINDERTYPE);
                        newNote.Attributes.Append(RDAYS);
                        newNote.Attributes.Append(RWEEK);
                        newNote.Attributes.Append(RMONTH);
                        newNote.Attributes.Append(RWEEKS);
                        newNote.Attributes.Append(RYEAR);
                        newNote.Attributes.Append(RHOUR);
                    }
                    x.Save(file.filePath);
                    Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(file.filePath));
                    th.Start();
                    SaveLog("添加任务@：" + taskName + "    导图" + filename);
                    shaixuanfuwei();
                    searchword.Text = "";
                    RRReminderlist();
                    return;
                }
            }
            if (mindmaplist.SelectedIndex >= 0 && searchword.Text.EndsWith(".") && searchword.Text != "")
            {
                string path = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(path);
                XmlNode root = x.GetElementsByTagName("node")[0];
                if (!haschildNode(root, DateTime.Now.Year.ToString()))
                {
                    XmlNode yearNode = x.CreateElement("node");
                    XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                    yearNodeValue.Value = DateTime.Now.Year.ToString();
                    yearNode.Attributes.Append(yearNodeValue);
                    root.AppendChild(yearNode);
                }
                XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString());
                if (!haschildNode(year, DateTime.Now.Month.ToString()))
                {
                    XmlNode monthNode = x.CreateElement("node");
                    XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                    monthNodeValue.Value = DateTime.Now.Month.ToString();
                    monthNode.Attributes.Append(monthNodeValue);
                    year.AppendChild(monthNode);
                }
                XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Month.ToString());
                if (!haschildNode(month, DateTime.Now.Day.ToString()))
                {
                    XmlNode dayNode = x.CreateElement("node");
                    XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                    dayNodeValue.Value = DateTime.Now.Day.ToString();
                    dayNode.Attributes.Append(dayNodeValue);
                    month.AppendChild(dayNode);
                }
                XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Day.ToString());
                XmlNode newNote = x.CreateElement("node");
                string changedtaskname = "";
                XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                string taskname = searchword.Text.Substring(0, searchword.Text.Length - 1);
                if (IsEncryptBool)
                {
                    if (PassWord == "")
                    {
                        return;
                    }
                    changedtaskname = encrypt.EncryptString(taskname);
                    IsEncryptBool = false;
                }
                else
                {
                    if (IsURL(taskname.Trim()))
                    {
                        changedtaskname = GetWebTitle(taskname.Trim()) + " | " + taskname;
                    }
                    else
                    {
                        changedtaskname = taskname;
                    }
                }
                SaveLog("添加任务：" + changedtaskname + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                newNotetext.Value = changedtaskname;
                if (IsURL(newNotetext.Value))
                {
                    string title = GetWebTitle(newNotetext.Value);
                    if (title != "" && title != "忘记了，后面再改")
                    {
                        //添加属性
                        XmlAttribute TASKLink = x.CreateAttribute("LINK");
                        TASKLink.Value = newNotetext.Value;
                        newNote.Attributes.Append(TASKLink);
                        newNotetext.Value = title;
                    }
                }
                XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                newNote.Attributes.Append(newNotetext);
                newNote.Attributes.Append(newNoteCREATED);
                newNote.Attributes.Append(newNoteMODIFIED);
                XmlAttribute TASKID = x.CreateAttribute("ID");
                newNote.Attributes.Append(TASKID);
                newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                newNote.Attributes.Append(TASKLEVEL);
                newNote.Attributes["TASKLEVEL"].Value = "1";
                XmlNode remindernode = x.CreateElement("hook");
                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                remindernode.Attributes.Append(remindernodeName);
                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                remindernodeParameters.Attributes.Append(remindernodeTime);
                remindernode.AppendChild(remindernodeParameters);
                newNote.AppendChild(remindernode);
                day.AppendChild(newNote);
                x.Save(path);
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                shaixuanfuwei();
                taskname = "";
                searchword.Text = "";
                RRReminderlist();
            }
            //给任务添加节点
            if ((reminderList.SelectedIndex >= 0 || reminderListBox.SelectedIndex >= 0) && searchword.Text != "" && IsSelectReminder)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    try
                    {
                        if (node.ParentNode.Attributes["ID"].Value == selectedReminder.IDinXML)
                        {
                            XmlNode newNote = x.CreateElement("node");
                            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                            newNotetext.Value = searchword.Text;
                            if (IsURL(newNotetext.Value))
                            {
                                string title = GetWebTitle(newNotetext.Value);
                                if (title != "" && title != "忘记了，后面再改")
                                {
                                    //添加属性
                                    XmlAttribute TASKLink = x.CreateAttribute("LINK");
                                    TASKLink.Value = newNotetext.Value;
                                    newNote.Attributes.Append(TASKLink);
                                    newNotetext.Value = title;
                                }
                            }
                            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            newNote.Attributes.Append(newNotetext);
                            newNote.Attributes.Append(newNoteCREATED);
                            newNote.Attributes.Append(newNoteMODIFIED);
                            XmlAttribute TASKID = x.CreateAttribute("ID");
                            newNote.Attributes.Append(TASKID);
                            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                            //XmlNode newElem = x.CreateElement("icon");
                            //XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                            //BUILTIN.Value = "flag-orange";
                            //newElem.Attributes.Append(BUILTIN);
                            //newNote.AppendChild(newElem);
                            node.ParentNode.AppendChild(newNote);
                            SaveLog("添加子节点：" + searchword.Text + "      @节点：" + selectedReminder.Name + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                            searchword.Text = "";
                            x.Save(selectedReminder.Value);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th.Start();
                            searchword.Text = "";
                            RRReminderlist();
                            return;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            //给导图添加节点
            if (mindmaplist.SelectedIndex >= 0 && searchword.Text != "" && !IsSelectReminder)
            {
                string path = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(path);
                XmlNode root = x.GetElementsByTagName("node")[0];
                if (!haschildNode(root, DateTime.Now.Year.ToString()))
                {
                    XmlNode yearNode = x.CreateElement("node");
                    XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                    yearNodeValue.Value = DateTime.Now.Year.ToString();
                    yearNode.Attributes.Append(yearNodeValue);
                    root.AppendChild(yearNode);
                }
                XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString());
                if (!haschildNode(year, DateTime.Now.Month.ToString()))
                {
                    XmlNode monthNode = x.CreateElement("node");
                    XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                    monthNodeValue.Value = DateTime.Now.Month.ToString();
                    monthNode.Attributes.Append(monthNodeValue);
                    year.AppendChild(monthNode);
                }
                XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Month.ToString());
                if (!haschildNode(month, DateTime.Now.Day.ToString()))
                {
                    XmlNode dayNode = x.CreateElement("node");
                    XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                    dayNodeValue.Value = DateTime.Now.Day.ToString();
                    dayNode.Attributes.Append(dayNodeValue);
                    month.AppendChild(dayNode);
                }
                XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Day.ToString());

                XmlNode newNote = x.CreateElement("node");
                string changedtaskname = searchword.Text;
                XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                newNotetext.Value = changedtaskname;
                if (IsURL(newNotetext.Value))
                {
                    string title = GetWebTitle(newNotetext.Value);
                    if (title != "" && title != "忘记了，后面再改")
                    {
                        //添加属性
                        XmlAttribute TASKLink = x.CreateAttribute("LINK");
                        TASKLink.Value = newNotetext.Value;
                        newNote.Attributes.Append(TASKLink);
                        newNotetext.Value = title;
                    }
                }
                XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                newNote.Attributes.Append(newNotetext);
                newNote.Attributes.Append(newNoteCREATED);
                newNote.Attributes.Append(newNoteMODIFIED);
                XmlAttribute TASKID = x.CreateAttribute("ID");
                newNote.Attributes.Append(TASKID);
                newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                newNote.Attributes.Append(TASKLEVEL);
                newNote.Attributes["TASKLEVEL"].Value = "1";
                XmlNode newElem = x.CreateElement("icon");
                XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                BUILTIN.Value = "flag-orange";
                newElem.Attributes.Append(BUILTIN);
                newNote.AppendChild(newElem);
                day.AppendChild(newNote);
                x.Save(path);
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                SaveLog("添加任务：" + changedtaskname + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                searchword.Text = "";
                RRReminderlist();
            }
            //这个会在什么时候进入呢？
            if (mindmaplist.SelectedIndex >= 0 && searchword.Text != "" && !IsSelectReminder)
            {
                string path = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(path);
                XmlNode root = x.GetElementsByTagName("node")[0];
                XmlNode newNote = x.CreateElement("node");
                string changedtaskname = "";
                XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                if (IsEncryptBool)
                {
                    if (PassWord == "")
                    {
                        return;
                    }
                    changedtaskname = encrypt.EncryptString(searchword.Text);
                    IsEncryptBool = false;
                }
                else
                {
                    if (IsURL(searchword.Text.Trim()))
                    {
                        changedtaskname = GetWebTitle(searchword.Text.Trim()) + " | " + searchword.Text;
                    }
                    else
                    {
                        changedtaskname = searchword.Text;
                    }
                }
                SaveLog("添加任务：" + changedtaskname + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                newNotetext.Value = changedtaskname;
                if (IsURL(newNotetext.Value))
                {
                    string title = GetWebTitle(newNotetext.Value);
                    if (title != "" && title != "忘记了，后面再改")
                    {
                        //添加属性
                        XmlAttribute TASKLink = x.CreateAttribute("LINK");
                        TASKLink.Value = newNotetext.Value;
                        newNote.Attributes.Append(TASKLink);
                        newNotetext.Value = title;
                    }
                }
                XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                newNote.Attributes.Append(newNotetext);
                newNote.Attributes.Append(newNoteCREATED);
                newNote.Attributes.Append(newNoteMODIFIED);
                XmlAttribute TASKID = x.CreateAttribute("ID");
                newNote.Attributes.Append(TASKID);
                newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                newNote.Attributes.Append(TASKLEVEL);
                newNote.Attributes["TASKLEVEL"].Value = "1";
                XmlNode remindernode = x.CreateElement("hook");
                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                remindernode.Attributes.Append(remindernodeName);
                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                remindernodeParameters.Attributes.Append(remindernodeTime);
                remindernode.AppendChild(remindernodeParameters);
                newNote.AppendChild(remindernode);
                root.AppendChild(newNote);
                x.Save(path);
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                shaixuanfuwei();
                searchword.Text = "";
                RRReminderlist();
            }
            if ((reminderList.SelectedIndex >= 0 || reminderListBox.SelectedIndex >= 0) && searchword.Text != "" && IsSelectReminder)
            {
                string currentPath = "";
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                if (mindmapornode.Text != "")
                {
                    if (mindmapornode.Text.Contains(">"))
                    {
                        currentPath = renameMindMapPath;
                    }
                    else
                    {
                        mindmapfile file = mindmapfiles.FirstOrDefault(m => m.name.ToLower() == mindmapornode.Text.ToLower());//不区分大小写 //是否需要优化下这个逻辑呢？？
                        if (file == null)
                        {
                            return;
                        }
                        currentPath = file.filePath;
                    }
                }
                else
                {
                    currentPath = selectedReminder.Value;
                }
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(currentPath);
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    try
                    {
                        if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == selectedReminder.IDinXML)
                        {
                            XmlNode newNote = x.CreateElement("node");
                            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                            newNotetext.Value = searchword.Text;
                            if (IsURL(newNotetext.Value))
                            {
                                string title = GetWebTitle(newNotetext.Value);
                                if (title != "" && title != "忘记了，后面再改")
                                {
                                    //添加属性
                                    XmlAttribute TASKLink = x.CreateAttribute("LINK");
                                    TASKLink.Value = newNotetext.Value;
                                    newNote.Attributes.Append(TASKLink);
                                    newNotetext.Value = title;
                                }
                            }
                            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            newNote.Attributes.Append(newNotetext);
                            newNote.Attributes.Append(newNoteCREATED);
                            newNote.Attributes.Append(newNoteMODIFIED);
                            XmlAttribute TASKID = x.CreateAttribute("ID");
                            newNote.Attributes.Append(TASKID);
                            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                            XmlAttribute TASKLEVEL = x.CreateAttribute("TASKLEVEL");
                            newNote.Attributes.Append(TASKLEVEL);
                            newNote.Attributes["TASKLEVEL"].Value = "1";
                            if (istask)
                            {
                                XmlNode remindernode = x.CreateElement("hook");
                                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                                remindernode.Attributes.Append(remindernodeName);
                                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                                //如果是子节点，时间和子节点一样
                                if (dateTimePicker.Value > DateTime.Now)
                                {
                                    remindernodeTime.Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                }
                                else
                                {
                                    remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                }
                                remindernodeParameters.Attributes.Append(remindernodeTime);
                                remindernode.AppendChild(remindernodeParameters);
                                newNote.AppendChild(remindernode);
                            }
                            node.AppendChild(newNote);
                            SaveLog("添加子节点：" + searchword.Text + "      @节点：" + selectedReminder.Name + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                            searchword.Text = "";
                            x.Save(currentPath);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(currentPath));
                            th.Start();
                            if (istask)
                            {
                                shaixuanfuwei();
                                RRReminderlist();
                            }
                            else
                            {
                                ShowSubNode();
                            }
                            return;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            else if (searchword.Text != "" && !searchword.Text.EndsWith("."))
            {
                AddTaskToFile(ini.ReadString("path", "binmm", ""), "Tasks", searchword.Text, true);
                tasklevel.Value = 0;
                taskTime.Value = 0;
                searchword.Text = "";
                RRReminderlist();
            }
        }
        public void AddNodeByID(bool istask, string taskName)
        {
            try
            {
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(renameMindMapPath);
                //x.GetElementById(id).RemoveAll(); ;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == renameMindMapFileID)
                    {
                        XmlNode newNote = x.CreateElement("node");
                        XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                        newNotetext.Value = taskName;
                        if (IsURL(newNotetext.Value))
                        {
                            string title = GetWebTitle(newNotetext.Value);
                            if (title != "" && title != "忘记了，后面再改")
                            {
                                //添加属性
                                XmlAttribute TASKLink = x.CreateAttribute("LINK");
                                TASKLink.Value = newNotetext.Value;
                                newNote.Attributes.Append(TASKLink);
                                newNotetext.Value = title;
                            }
                        }
                        XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                        newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                        newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        newNote.Attributes.Append(newNotetext);
                        newNote.Attributes.Append(newNoteCREATED);
                        newNote.Attributes.Append(newNoteMODIFIED);
                        XmlAttribute TASKID = x.CreateAttribute("ID");
                        newNote.Attributes.Append(TASKID);
                        newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                        //XmlNode newElem = x.CreateElement("icon");
                        //XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                        //BUILTIN.Value = "flag-orange";
                        //newElem.Attributes.Append(BUILTIN);
                        //newNote.AppendChild(newElem);
                        if (istask)
                        {
                            XmlNode remindernode = x.CreateElement("hook");
                            XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                            remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                            remindernode.Attributes.Append(remindernodeName);
                            XmlNode remindernodeParameters = x.CreateElement("Parameters");
                            XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                            remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            remindernodeParameters.Attributes.Append(remindernodeTime);
                            remindernode.AppendChild(remindernodeParameters);
                            newNote.AppendChild(remindernode);
                            fenshuADD(3);
                        }
                        node.AppendChild(newNote);
                        searchword.Text = "";
                        x.Save(renameMindMapPath);
                        Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(renameMindMapPath));
                        th.Start();
                        ShowSubNode();
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void mindmaplist_MouseUp(object sender, MouseEventArgs e)
        {
            IsSelectReminder = false;
        }
        private void feeling_Click(object sender, EventArgs e)
        {
            AddTaskToFile("home.mm", "feeling", searchword.Text, false);
            searchword.Text = "";
        }
        public bool haschildNode(XmlNode node, string child)
        {
            foreach (XmlNode item in node.ChildNodes.Cast<XmlNode>().Where(m => m.Name == "node"))
            {
                if (item.Attributes.Cast<XmlAttribute>().Any(m => m.Name == "TEXT"))
                {
                    if (item.Attributes["TEXT"].Value == child)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void WriteLog(string task, string mindmap, DateTime taskTime, String Btnstring)
        {
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load("Home.mm");
            XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == "feeling");
            //if (root.ChildNodes.Cast<XmlNode>().Any(m => m.Attributes[0].Name != "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString()))
            if (!haschildNode(root, taskTime.Year.ToString()))
            {
                XmlNode yearNode = x.CreateElement("node");
                XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                yearNodeValue.Value = taskTime.Year.ToString();
                yearNode.Attributes.Append(yearNodeValue);
                root.AppendChild(yearNode);
            }
            XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes["TEXT"].Value == taskTime.Year.ToString());
            if (!haschildNode(year, taskTime.Month.ToString()))
            {
                XmlNode monthNode = x.CreateElement("node");
                XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                monthNodeValue.Value = taskTime.Month.ToString();
                monthNode.Attributes.Append(monthNodeValue);
                year.AppendChild(monthNode);
            }
            XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes["TEXT"].Value == taskTime.Month.ToString());
            if (!haschildNode(month, taskTime.Day.ToString()))
            {
                XmlNode dayNode = x.CreateElement("node");
                XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                dayNodeValue.Value = taskTime.Day.ToString();
                dayNode.Attributes.Append(dayNodeValue);
                month.AppendChild(dayNode);
            }
            XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes["TEXT"].Value == taskTime.Day.ToString());
            if (!haschildNode(day, "Task"))
            {
                XmlNode taskNodeAdd = x.CreateElement("node");
                XmlAttribute taskNodeValue = x.CreateAttribute("TEXT");
                taskNodeValue.Value = "Task";
                taskNodeAdd.Attributes.Append(taskNodeValue);
                day.AppendChild(taskNodeAdd);
            }
            XmlNode taskNode = day.ChildNodes.Cast<XmlNode>().First(m => m.Attributes["TEXT"].Value == "Task");
            if (!haschildNode(taskNode, mindmap))
            {
                XmlNode mindmapNodeAdd = x.CreateElement("node");
                XmlAttribute mindmapNodeValue = x.CreateAttribute("TEXT");
                mindmapNodeValue.Value = mindmap;
                mindmapNodeAdd.Attributes.Append(mindmapNodeValue);
                taskNode.AppendChild(mindmapNodeAdd);
            }
            XmlNode mindmapNode = taskNode.ChildNodes.Cast<XmlNode>().First(m => m.Attributes["TEXT"].Value == mindmap);
            XmlNode newNote = x.CreateElement("node");
            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
            newNotetext.Value = task;
            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            newNote.Attributes.Append(newNotetext);
            newNote.Attributes.Append(newNoteCREATED);
            newNote.Attributes.Append(newNoteMODIFIED);
            XmlAttribute TASKID = x.CreateAttribute("ID");
            newNote.Attributes.Append(TASKID);
            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
            XmlNode newElem = x.CreateElement("icon");
            XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
            BUILTIN.Value = Btnstring;
            switch (Btnstring)
            {
                case "button_ok":
                    SaveLog("完成任务：" + task + "    导图" + mindmap);
                    break;
                case "button_cancel":
                    SaveLog("取消任务：" + task + "    导图" + mindmap);
                    break;
                default:
                    break;
            }
            newElem.Attributes.Append(BUILTIN);
            newNote.AppendChild(newElem);
            mindmapNode.AppendChild(newNote);
            x.Save("Home.mm");
            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile("Home.mm"));
            th.Start();
        }
        private void denyAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (MyListBoxItemRemind item in reminderList.Items)
                {
                    if (item.Time <= DateTime.Today)
                    {
                        DelayTask(item);
                        string path = item.Value;
                        Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                        th.Start();
                    }
                }
                searchword.Text = "";
                shaixuanfuwei();
                RRReminderlist();
                PlaySimpleSound("deny");
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        private void cancel_btn_Click(object sender, EventArgs e)
        {
            CanceSelectedlTask();
        }
        public void CanceSelectedlTask(bool IsAddIcon = true)
        {
            try
            {
                int reminderIndex = reminderList.SelectedIndex;
                CancelTask(IsAddIcon);
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                //shaixuanfuwei();
                //ChangeReminder();
                PlaySimpleSound("deny");
                reminderList.Items.RemoveAt(reminderIndex);
                reminderList.SelectedIndex = reminderIndex;
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        public void RemoveMyListBoxItemRemind()
        {
            try
            {
                int reminderIndex = reminderList.SelectedIndex;
                RemoveRemember();
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                reminderList.Items.RemoveAt(reminderIndex);
                reminderList.SelectedIndex = reminderIndex;
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        public void RemoveRemember()
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            foreach (XmlNode node in x.GetElementsByTagName("icon"))
            {
                try
                {
                    if (node.Attributes["BUILTIN"].Value == "flag-orange" && node.ParentNode.Attributes["TEXT"].Value == selectedReminder.Name)
                    {
                        node.Attributes["BUILTIN"].Value = "flag-yellow";
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        public void deleteNodeByID(string id)
        {
            try
            {
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(showMindmapName);
                fenshuADD(1);
                //x.GetElementById(id).RemoveAll(); ;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == id)
                    {
                        node.ParentNode.RemoveChild(node);
                        x.Save(showMindmapName);
                        PlaySimpleSound("delete");
                        nodetree.SelectedNode.Remove();
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SetTaskNodeByID(string id)
        {
            try
            {
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(showMindmapName);
                fenshuADD(1);
                //x.GetElementById(id).RemoveAll(); ;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == id)
                    {
                        XmlNode remindernode = x.CreateElement("hook");
                        XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                        remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                        remindernode.Attributes.Append(remindernodeName);
                        XmlNode remindernodeParameters = x.CreateElement("Parameters");
                        XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                        remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        remindernodeParameters.Attributes.Append(remindernodeTime);
                        remindernode.AppendChild(remindernodeParameters);
                        node.AppendChild(remindernode);
                        x.Save(showMindmapName);
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void RenameNodeByID(string taskName)
        {
            try
            {
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(renameMindMapPath);
                fenshuADD(2);
                //x.GetElementById(id).RemoveAll(); ;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == renameMindMapFileID)
                    {
                        node.Attributes["TEXT"].InnerText = taskName;
                        x.Save(renameMindMapPath);
                        PlaySimpleSound("edittask");

                        Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(renameMindMapPath));
                        th.Start();
                        if (renameMindMapFileIDParent != "")
                        {
                            renameMindMapFileID = renameMindMapFileIDParent;
                            renameMindMapFileIDParent = "";
                        }
                        //((MyListBoxItemRemind)reminderlistSelectedItem).Text = ();
                        return;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void CancelTask(bool IsAddIcon = true)
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("node"))
            {
                if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == selectedReminder.IDinXML)
                {
                    try
                    {
                        if (mindmapornode.Text == "" || (mindmapornode.Text != "" && !mindmapornode.Text.Contains(">")))
                        {
                            foreach (XmlNode item in node.ChildNodes)
                            {
                                if (item.Name == "hook")
                                {
                                    item.ParentNode.RemoveChild(item);
                                }
                            }
                            if (IsAddIcon)
                            {
                                XmlNode newElem = x.CreateElement("icon");
                                XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                                BUILTIN.Value = "button_cancel";
                                newElem.Attributes.Append(BUILTIN);
                                node.AppendChild(newElem);
                                SaveLog("取消任务：" + taskName + "    导图" + selectedReminder.Value);
                            }
                            else
                            {
                                SaveLog("结束周期任务：" + taskName + "    导图" + selectedReminder.Value);
                            }
                        }
                        else
                        {
                            if (mindmapornode.Text.Contains(">"))
                            {
                                node.ParentNode.RemoveChild(node);
                                SaveLog("删除节点：" + taskName + "    导图" + selectedReminder.Value);
                            }
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        private void DaKa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SetDaka();
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                shaixuanfuwei();
                RRReminderlist();
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        private void Jinian_btn_Click(object sender, EventArgs e)
        {
            try
            {
                SetJinian();
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                shaixuanfuwei();
                RRReminderlist();
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        #endregion

        #region 打卡
        public void SetDaka()
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        if (selectedReminder.IsDaka == "true")
                        {
                            node.ParentNode.Attributes["ISDAKA"].Value = "false";
                        }
                        else
                        {
                            XmlNode newElem = x.CreateElement("icon");
                            XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                            BUILTIN.Value = "addition";
                            newElem.Attributes.Append(BUILTIN);
                            node.ParentNode.AppendChild(newElem);
                            XmlAttribute ISDAKA = x.CreateAttribute("ISDAKA");
                            ISDAKA.Value = "true";
                            node.ParentNode.Attributes.Append(ISDAKA);
                            if (node.ParentNode.Attributes["DAKADAY"] == null)
                            {
                                XmlAttribute DAKADAY = x.CreateAttribute("DAKADAY");
                                DAKADAY.Value = "0";
                                node.ParentNode.Attributes.Append(DAKADAY);
                            }
                            if (node.ParentNode.Attributes["LeftDakaDays"] == null)
                            {
                                XmlAttribute DAKADAY = x.CreateAttribute("LeftDakaDays");
                                DAKADAY.Value = "3";
                                node.ParentNode.Attributes.Append(DAKADAY);
                            }
                            if (node.ParentNode.Attributes["DAKADAYSr"] == null)
                            {
                                XmlAttribute DAKADAYS = x.CreateAttribute("DAKADAYS");
                                DAKADAYS.Value = "";
                                node.ParentNode.Attributes.Append(DAKADAYS);
                            }
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        #endregion
        public void SetJinian()
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        if (node.ParentNode.Attributes["IsJinian"] != null && node.ParentNode.Attributes["IsJinian"].Value == "true")
                        {
                            node.ParentNode.Attributes["IsJinian"].Value = "false";
                        }
                        else
                        {
                            XmlNode newElem = x.CreateElement("icon");
                            XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                            BUILTIN.Value = "addition";
                            newElem.Attributes.Append(BUILTIN);
                            node.ParentNode.AppendChild(newElem);
                            XmlAttribute IsJinian = x.CreateAttribute("IsJinian");
                            IsJinian.Value = "true";
                            node.ParentNode.Attributes.Append(IsJinian);
                            if (node.ParentNode.Attributes["JinianBeginTime"] == null)
                            {
                                XmlAttribute JinianBeginTime = x.CreateAttribute("JinianBeginTime");
                                JinianBeginTime.Value = node.FirstChild.Attributes["REMINDUSERAT"].Value;
                                node.ParentNode.Attributes.Append(JinianBeginTime);
                            }
                            else
                            {
                                node.ParentNode.Attributes["JinianBeginTime"].Value = node.FirstChild.Attributes["REMINDUSERAT"].Value;
                            }
                            //if (node.ParentNode.Attributes["DAKADAYSr"] == null)
                            //{
                            //    XmlAttribute DAKADAYS = x.CreateAttribute("DAKADAYS");
                            //    DAKADAYS.Value = "";
                            //    node.ParentNode.Attributes.Append(DAKADAYS);
                            //}
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        public int[] StrToInt(string[] str)
        {
            int[] result = new int[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == "")
                {
                    result[i] = 0;
                }
                else
                {
                    result[i] = int.Parse(str[i]);
                }
            }
            return result;
        }
        public static string intostringwithlenght(int num, int lenght)
        {
            string resut = num.ToString();
            for (int i = resut.Length; i < lenght; i++)
            {
                resut = " " + resut;
            }
            return resut;
        }
        public double GetAVAge(int[] arr, int dakaday)
        {
            int num = 0;
            int sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0)
                {
                    num += 1;
                    sum += arr[i];
                }
            }
            if (dakaday != 0)
            {
                num += 1;
                sum += dakaday;
            }
            return (double)sum / num;
        }
        public int GetNUM(int[] arr, int dakaday)
        {
            int num = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0)
                {
                    num += 1;
                }
            }
            if (dakaday != 0)
            {
                num += 1;
            }
            return num;
        }
        public int GetSUM(int[] arr, int dakaday)
        {
            int sum = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != 0)
                {
                    sum += arr[i];
                }
            }
            if (dakaday != 0)
            {
                sum += dakaday;
            }
            return sum;
        }
        public int GetMax(int[] arr, int dakaday)
        {
            if (arr.Max() > dakaday)
            {
                return arr.Max();
            }
            else
            {
                return dakaday;
            }
        }
        private void Reminderlist_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && reminderList.SelectedIndex >= 0)
            {
                Clipboard.SetDataObject(((MyListBoxItemRemind)reminderlistSelectedItem).Name);
            }
        }
        private void reminderlist_MouseHover(object sender, EventArgs e)
        {
            InReminderBool = true;
            if (!searchword.Focused)
            {
                reminderList.Focus();
            }
            SwitchToLanguageMode();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // if it is a hotkey, return true; otherwise, return false
            switch (keyData)
            {
                //case Keys.NumPad0:
                //    return true;
                default:
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void tasklevel_ValueChanged(object sender, EventArgs e)
        {
            // if (reminderList.SelectedIndex<0||reminderList.SelectedIndex<0)
            // {
            //     RRReminderlist();
            // }
        }
        private void taskTime_ValueChanged(object sender, EventArgs e)
        {
            // if (reminderListFocused())
            // {
            //     RRReminderlist();
            // }
        }
        private void nifan_CheckedChanged(object sender, EventArgs e)
        {
            RRReminderlist();
        }
        public void shaixuanfuwei()
        {
            taskTime.Value = 0;
            tasklevel.Value = 0;
        }
        public void AddTaskToFile(string mindmap, string rootNode, string taskName, bool hasTime)
        {
            if (taskName == "")
            {
                return;
            }
            if (IsEncryptBool)
            {
                if (PassWord == "")
                {
                    MessageBox.Show("请设置密码！");
                    return;
                }
                taskName = encrypt.EncryptString(taskName);
                IsEncryptBool = false;
            }
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(mindmap);
            XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == rootNode);
            //if (root.ChildNodes.Cast<XmlNode>().Any(m => m.Attributes[0].Name != "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString()))
            if (!haschildNode(root, DateTime.Now.Year.ToString()))
            {
                XmlNode yearNode = x.CreateElement("node");
                XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                yearNodeValue.Value = DateTime.Now.Year.ToString();
                yearNode.Attributes.Append(yearNodeValue);
                root.AppendChild(yearNode);
            }
            XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Year.ToString());
            if (!haschildNode(year, DateTime.Now.Month.ToString()))
            {
                XmlNode monthNode = x.CreateElement("node");
                XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                monthNodeValue.Value = DateTime.Now.Month.ToString();
                monthNode.Attributes.Append(monthNodeValue);
                year.AppendChild(monthNode);
            }
            XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Month.ToString());
            if (!haschildNode(month, DateTime.Now.Day.ToString()))
            {
                XmlNode dayNode = x.CreateElement("node");
                XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                dayNodeValue.Value = DateTime.Now.Day.ToString();
                dayNode.Attributes.Append(dayNodeValue);
                month.AppendChild(dayNode);
            }
            XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == DateTime.Now.Day.ToString());
            XmlNode newNote = x.CreateElement("node");
            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
            string pstr = "";
            if (!hasTime)
            {
                pstr = DateTime.Now.ToString("HH:mm") + "    ";
            }
            newNotetext.Value = pstr + taskName;
            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            if (ebdefault.Contains(new FileInfo(mindmap).Name))
            {
                XmlAttribute REMINDERTYPE = x.CreateAttribute("REMINDERTYPE");
                REMINDERTYPE.Value = "eb";
                XmlAttribute RDAYS = x.CreateAttribute("RDAYS");
                XmlAttribute RWEEK = x.CreateAttribute("RWEEK");
                XmlAttribute RMONTH = x.CreateAttribute("RMONTH");
                XmlAttribute RWEEKS = x.CreateAttribute("RWEEKS");
                XmlAttribute RYEAR = x.CreateAttribute("RYEAR");
                XmlAttribute RHOUR = x.CreateAttribute("RHOUR");
                newNote.Attributes.Append(REMINDERTYPE);
                newNote.Attributes.Append(RDAYS);
                newNote.Attributes.Append(RWEEK);
                newNote.Attributes.Append(RMONTH);
                newNote.Attributes.Append(RWEEKS);
                newNote.Attributes.Append(RYEAR);
                newNote.Attributes.Append(RHOUR);
                hasTime = true;
            }
            if (hasTime)
            {
                XmlNode remindernode = x.CreateElement("hook");
                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                remindernode.Attributes.Append(remindernodeName);
                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                remindernodeParameters.Attributes.Append(remindernodeTime);
                remindernode.AppendChild(remindernodeParameters);
                newNote.AppendChild(remindernode);
            }
            newNote.Attributes.Append(newNotetext);
            newNote.Attributes.Append(newNoteCREATED);
            newNote.Attributes.Append(newNoteMODIFIED);
            XmlAttribute TASKID = x.CreateAttribute("ID");
            newNote.Attributes.Append(TASKID);
            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
            day.AppendChild(newNote);
            x.Save(mindmap);
            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(mindmap));
            th.Start();
        }
        public void AddClipToTask(bool istask=false)
        {
            IDataObject iData = new DataObject();
            iData = Clipboard.GetDataObject();
            string log = (string)iData.GetData(DataFormats.Text);
            if (log == null || log == "")
            {
                return;
            }
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            foreach (XmlNode node in x.GetElementsByTagName("node"))
            {
                try
                {
                    if (node.Attributes["ID"].Value == selectedReminder.IDinXML)
                    {
                        XmlNode newNote = x.CreateElement("node");
                        XmlAttribute newNotetext = x.CreateAttribute("TEXT");
                        newNotetext.Value = log;
                        if (IsURL(newNotetext.Value))
                        {
                            string title = GetWebTitle(newNotetext.Value);
                            if (title != "" && title != "忘记了，后面再改")
                            {
                                //添加属性
                                XmlAttribute TASKLink = x.CreateAttribute("LINK");
                                TASKLink.Value = newNotetext.Value;
                                newNote.Attributes.Append(TASKLink);
                                newNotetext.Value = title;
                            }
                        }
                        XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
                        newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
                        newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                        newNote.Attributes.Append(newNotetext);
                        newNote.Attributes.Append(newNoteCREATED);
                        newNote.Attributes.Append(newNoteMODIFIED);
                        XmlAttribute TASKID = x.CreateAttribute("ID");
                        newNote.Attributes.Append(TASKID);
                        newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
                        //XmlNode newElem = x.CreateElement("icon");
                        //XmlAttribute BUILTIN = x.CreateAttribute("BUILTIN");
                        //BUILTIN.Value = "flag-orange";
                        //newElem.Attributes.Append(BUILTIN);
                        //newNote.AppendChild(newElem);
                        if (istask)
                        {
                            XmlNode remindernode = x.CreateElement("hook");
                            XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                            remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                            remindernode.Attributes.Append(remindernodeName);
                            XmlNode remindernodeParameters = x.CreateElement("Parameters");
                            XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                            remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                            remindernodeParameters.Attributes.Append(remindernodeTime);
                            remindernode.AppendChild(remindernodeParameters);
                            newNote.AppendChild(remindernode);
                        }
                        node.AppendChild(newNote);
                        SaveLog("添加子节点：" + searchword.Text + "      @节点：" + selectedReminder.Name + "    导图：" + ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                        searchword.Text = "";
                        x.Save(selectedReminder.Value);
                        Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                        th.Start();
                        searchword.Text = "";
                        RRReminderlist();
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
            tasklevel.Value = 0;
            taskTime.Value = 0;
            RRReminderlist();
            Clipboard.Clear();
        }
        private void mindmaplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            Updateunchkeckmindmap();
            InMindMapBool = true;
            if (isInReminderList)
            {
                isInReminderList = false;
                return;
            }
            if (searchword.Text.StartsWith("#"))
            {
                return;
            }
            if (searchword.Text.StartsWith("*"))
            {
                if (mindmapnumdesc > 0)
                {
                    mindmapnumdesc--;
                    return;
                }
                if (searchword.Text.StartsWith("*"))
                {
                    for (int i = 0; i < mindmaplist.Items.Count; i++)
                    {
                        if (mindmaplist.CheckedItems.IndexOf(mindmaplist.Items[i]) == -1)
                        {
                            for (int k = reminderList.Items.Count - 1; k > 0; k--)
                            {
                                if (((MyListBoxItemRemind)reminderList.Items[k]).Value == ((MyListBoxItem)mindmaplist.Items[i]).Value)
                                {
                                    reminderList.Items.RemoveAt(k);
                                }
                            }
                        }
                    }
                }
                return;
            }
            if (IsViewModel.Checked)
            {
                RRReminderlist();
            }
        }
        public bool IsURL(string url)
        {
            string matchStr = @"http(s)?://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
            return Regex.IsMatch(url, matchStr);
        }
        public string GetUrl(string str)
        {
            string matchStr = @"http(s)?://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
            return Regex.Match(str, matchStr).Value;
        }
        private String GetWebTitle(String url)
        {
            System.Net.WebRequest wb;
            //请求资源
            try
            {
                wb = System.Net.WebRequest.Create(url.Trim());
            }
            catch (Exception)
            {

                return "";
            }
            //响应请求
            WebResponse webRes = null;
            //将返回的数据放入流中
            Stream webStream = null;
            try
            {
                webRes = wb.GetResponse();
                webStream = webRes.GetResponseStream();
            }
            catch (Exception)
            {
                return "";
            }
            //从流中读出数据
            StreamReader sr = new StreamReader(webStream, System.Text.Encoding.UTF8);
            //创建可变字符对象，用于保存网页数据
            StringBuilder sb = new StringBuilder();
            //读出数据存入可变字符中
            String str = "";
            while ((str = sr.ReadLine()) != null)
            {
                sb.Append(str);
            }
            //建立获取网页标题正则表达式
            String regex = @"(?<=<title>).+(?=</title>)";
            //返回网页标题
            String title = Regex.Match(sb.ToString(), regex).ToString();
            title = Regex.Replace(title, " ", "");
            //if (title.Length > 40)
            //{
            //    title = title.Substring(0, 30);
            //}
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + "\\html\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + "\\html\\");
                }
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + "\\html\\" + ReplaceSpecialCharacterV2(DateTime.Now.ToString()+ title)+ ".html", sb.ToString());
            }
            catch (Exception)
            {
                return title;
            }
            return title;
        }
        public string ReplaceSpecialCharacterV2(string str)
        {
            List<string> charArr = new List<string>() { "\\", "/", "*", "?", "<", ">", "|", ":", "\"" };
            return charArr.Aggregate(str, (current, c) => current.Replace(c, ""));
        }
        private void hiddenmenu_DoubleClick(object sender, EventArgs e)
        {
            Thread th = new Thread(() => OpenMenu());
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            //Center();//= new Point(this.Location.X, -1569);
            MyHide();
        }
        private void Home_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Home.mm");
        }
        private void bin_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ini.ReadString("path", "binmm", ""));
        }

        public static int GetPosition()
        {
            for (int i = 0; i < fanqiePosition.Length; i++)
            {
                if (!fanqiePosition[i])
                {
                    return i;
                }
            }
            return 100;
        }
        public void AddClip()
        {
            IDataObject iData = new DataObject();
            iData = Clipboard.GetDataObject();
            string log = (string)iData.GetData(DataFormats.Text);
            if (log == null || log == "" || mindmaplist.SelectedItem == null)
            {
                return;
            }
            if (IsURL(log.Trim()))
            {
                log = GetWebTitle(log.Trim()) + " | " + log;
            }
            string path = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(path);
            DateTime dt = DateTime.Now;
            //XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == rootNode);
            XmlNode root = x.GetElementsByTagName("node")[0];
            //if (root.ChildNodes.Cast<XmlNode>().Any(m => m.Attributes[0].Name != "TEXT" && m.Attributes["TEXT"].Value == dt.Year.ToString()))
            if (!haschildNode(root, dt.Year.ToString()))
            {
                XmlNode yearNode = x.CreateElement("node");
                XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                yearNodeValue.Value = dt.Year.ToString();
                yearNode.Attributes.Append(yearNodeValue);
                root.AppendChild(yearNode);
            }
            XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Year.ToString());
            if (!haschildNode(year, dt.Month.ToString()))
            {
                XmlNode monthNode = x.CreateElement("node");
                XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                monthNodeValue.Value = dt.Month.ToString();
                monthNode.Attributes.Append(monthNodeValue);
                year.AppendChild(monthNode);
            }
            XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Month.ToString());
            if (!haschildNode(month, dt.Day.ToString()))
            {
                XmlNode dayNode = x.CreateElement("node");
                XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                dayNodeValue.Value = dt.Day.ToString();
                dayNode.Attributes.Append(dayNodeValue);
                month.AppendChild(dayNode);
            }
            XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Day.ToString());
            XmlNode newNote = x.CreateElement("node");
            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
            newNotetext.Value = log;
            if (IsURL(newNotetext.Value))
            {
                string title = GetWebTitle(newNotetext.Value);
                if (title != "" && title != "忘记了，后面再改")
                {
                    //添加属性
                    XmlAttribute TASKLink = x.CreateAttribute("LINK");
                    TASKLink.Value = newNotetext.Value;
                    newNote.Attributes.Append(TASKLink);
                    newNotetext.Value = title;
                }
            }
            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
            newNoteCREATED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
            newNoteMODIFIED.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            newNote.Attributes.Append(newNotetext);
            newNote.Attributes.Append(newNoteCREATED);
            newNote.Attributes.Append(newNoteMODIFIED);
            XmlAttribute TASKID = x.CreateAttribute("ID");
            newNote.Attributes.Append(TASKID);
            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
            XmlNode remindernode = x.CreateElement("hook");
            XmlAttribute remindernodeName = x.CreateAttribute("NAME");
            remindernodeName.Value = "plugins/TimeManagementReminder.xml";
            remindernode.Attributes.Append(remindernodeName);
            XmlNode remindernodeParameters = x.CreateElement("Parameters");
            XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
            remindernodeTime.Value = (Convert.ToInt64((DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            remindernodeParameters.Attributes.Append(remindernodeTime);
            remindernode.AppendChild(remindernodeParameters);
            newNote.AppendChild(remindernode);
            day.AppendChild(newNote);
            x.Save(path);
            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
            th.Start();
            tasklevel.Value = 0;
            taskTime.Value = 0;
            RRReminderlist();
            Clipboard.Clear();
        }


        private void IsEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (reminderlistSelectedItem == null || PassWord == "")
                {
                    return;
                }
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                string taskName = selectedReminder.Name;
                if (selectedReminder.isEncrypted)
                {
                    taskName = encrypt.EncryptString(taskName);
                }
                foreach (XmlNode node in x.GetElementsByTagName("hook"))
                {
                    try
                    {
                        if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                        {
                            if (selectedReminder.isEncrypted)
                            {
                                node.ParentNode.Attributes["TEXT"].Value = encrypt.DecryptString(taskName);
                            }
                            else
                            {
                                node.ParentNode.Attributes["TEXT"].Value = encrypt.EncryptString(taskName);
                            }
                            x.Save(selectedReminder.Value);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                string path = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(path));
                th.Start();
                shaixuanfuwei();
                RRReminderlist();
            }
            catch (Exception)
            {
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SetSelected(0, true);
                }

            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ReminderListFocused() || reminderListBox.Focused || mindmaplist.Focused)
            {
                if (e.KeyCode != Keys.Space && e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.D1 && e.KeyCode != Keys.D2 && e.KeyCode != Keys.D3 && e.KeyCode != Keys.D4 && e.KeyCode != Keys.D5 && e.KeyCode != Keys.D6 && e.KeyCode != Keys.D7 && e.KeyCode != Keys.D8 && e.KeyCode != Keys.D9 && e.KeyCode != Keys.D0)
                {
                    e.SuppressKeyPress = true;
                }
            }
        }
        public bool keyNotWork()
        {
            return !(searchword.Focused || richTextSubNode.Focused || mindmapSearch.Focused || noterichTextBox.Focused);
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!keyNotWork() && e.KeyCode != Keys.Enter && e.KeyCode != Keys.Escape && e.KeyCode != Keys.Down && e.KeyCode != Keys.F1 && e.KeyCode != Keys.F2 && e.KeyCode != Keys.F4 && e.KeyCode != Keys.F3 && e.KeyCode != Keys.F5 && e.KeyCode != Keys.F6 && e.KeyCode != Keys.D7 && e.KeyCode != Keys.F8 && e.KeyCode != Keys.D9 && e.KeyCode != Keys.F11 && e.KeyCode != Keys.F10 && e.KeyCode != Keys.F12)
            {
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                    {
                        allFloder = !allFloder;

                    }
                    else
                    {
                        if (IsReminderOnlyCheckBox.Checked)
                        {
                            IsReminderOnlyCheckBox.Checked = false;
                            RRReminderlist();
                        }
                        else
                        {
                            showcyclereminder.Checked = !showcyclereminder.Checked;
                            shaixuanfuwei();
                            RRReminderlist();
                        }
                    }
                    break;
                case Keys.Add:
                    break;
                case Keys.Alt:
                    break;
                case Keys.Apps:
                    break;
                case Keys.Attn:
                    break;
                case Keys.B:
                    PlaySimpleSound("treeview");
                    if (ReminderListFocused() || reminderListBox.Focused || nodetree.Focused)
                    {
                        if (this.Height != 860)
                        {
                            ShowMindmap(true);
                            ShowMindmapFile();
                            this.Height = 860;
                            nodetree.Visible = FileTreeView.Visible = true;
                            nodetree.Focus();
                        }
                        else
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                ShowMindmap(true);
                                ShowMindmapFile();
                                nodetree.Focus();
                                nodetree.Visible = FileTreeView.Visible = true;
                            }
                            else
                            {
                                this.Height = 540;
                                nodetree.Top = 501;
                                FileTreeView.Top = 501;
                                nodetree.Height = 307;
                                FileTreeView.Height = 307;
                                nodetree.Visible = FileTreeView.Visible = false;
                                reminderList.Focus();
                            }
                        }
                        Center();
                    }
                    break;
                case Keys.Back:
                    needSuggest = true;
                    break;
                case Keys.BrowserBack:
                    break;
                case Keys.BrowserFavorites:
                    break;
                case Keys.BrowserForward:
                    break;
                case Keys.BrowserHome:
                    break;
                case Keys.BrowserRefresh:
                    break;
                case Keys.BrowserSearch:
                    break;
                case Keys.BrowserStop:
                    break;
                case Keys.C:
                    if (ReminderListFocused())
                    {
                        Clipboard.SetDataObject(((MyListBoxItemRemind)reminderlistSelectedItem).Name);
                        MyHide();
                    }
                    break;
                case Keys.Cancel:
                    break;
                case Keys.CapsLock:

                    break;
                case Keys.Clear:
                    break;
                case Keys.Control:
                    break;
                case Keys.ControlKey:
                    break;
                case Keys.Crsel:
                    break;
                case Keys.D:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            int reminderIndex = reminderList.SelectedIndex;
                            SetTaskIsView();
                            try
                            {
                                reminderList.Items.RemoveAt(reminderIndex);
                                RRReminderlist();
                                reminderList.SelectedIndex = reminderIndex;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            int reminderIndex = reminderList.SelectedIndex;
                            try
                            {
                                for (int i = 0; i < mindmaplist.Items.Count; i++)
                                {
                                    if (((MyListBoxItem)mindmaplist.Items[i]).Value == ((MyListBoxItemRemind)reminderlistSelectedItem).Value)
                                    {
                                        if (mindmaplist.GetItemCheckState(i) == CheckState.Checked)
                                        {
                                            mindmaplist.SetItemChecked(i, false);
                                            mindmaplist.Refresh();
                                            break;
                                        }
                                    }
                                }
                                RRReminderlist();
                                reminderList.SelectedIndex = reminderIndex;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            cancel_btn_Click(null, null);
                        }
                    }
                    break;
                case Keys.D0:
                    if (!(searchword.Focused || taskTime.Focused || tasklevel.Focused || dateTimePicker.Focused))
                    {
                        night.Checked = !night.Checked;
                    }
                    break;
                case Keys.D1:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            showtomorrow.Checked = !showtomorrow.Checked;
                            RRReminderlist();
                        }
                        else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            IsViewModel.Checked = !IsViewModel.Checked;
                        }
                        else
                        {
                            IsSelectReminder = false;
                            mindmaplist.Focus();
                        }
                    }
                    break;
                case Keys.D2:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            reminder_week.Checked = !reminder_week.Checked;
                            RRReminderlist();
                        }
                        else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            //moshiview.Checked = !moshiview.Checked;
                        }
                        else
                        {
                            IsSelectReminder = true;
                            reminderList.SelectedIndex = reminderSelectIndex;
                            reminderList.Focus();
                            if (reminderList.SelectedIndex < 0 || reminderList.SelectedIndex > reminderList.Items.Count - 1)
                            {
                                reminderList.SelectedIndex = 0;
                            }
                        }
                    }
                    break;
                case Keys.D3:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            reminder_month.Checked = !reminder_month.Checked;
                            RRReminderlist();
                        }
                        else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            //quanxuan.Checked = !quanxuan.Checked;
                        }
                        else
                        {
                            IsSelectReminder = false;
                            searchword.Focus();
                        }
                    }
                    break;
                case Keys.D4:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            reminder_year.Checked = !reminder_year.Checked;
                            RRReminderlist();
                        }
                        else
                        {
                            if (this.Height > 550)
                            {
                                nodetree.Focus();
                            }
                            else
                            {
                                richTextSubNode.Focus();
                            }
                            //FormX = this.Location.Y;
                            //Center();//= new Point(this.Location.X, -1569);
                            //System.Diagnostics.Process.Start(new DirectoryInfo(((MyListBoxItemRemind)reminderlistSelectedItem).Value).FullName.Substring(0, ((MyListBoxItemRemind)reminderlistSelectedItem).Value.Length - Path.GetFileName(((MyListBoxItemRemind)reminderlistSelectedItem).Value).Length));
                        }
                    }
                    break;
                case Keys.D5:
                    if (keyNotWork())
                    {
                        if (this.Height > 550)
                        {
                            FileTreeView.Focus();
                        }
                        else
                        {
                            if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                reminder_yearafter.Checked = !reminder_yearafter.Checked;
                            }
                            else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                onlyZhouqi.Checked = !onlyZhouqi.Checked;
                                shaixuanfuwei();
                                RRReminderlist();
                            }
                            else
                            {
                                //DirectoryInfo path = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
                                //foreach (FileInfo file in path.GetFiles("~*.mm", SearchOption.AllDirectories))
                                //{
                                //    file.Delete();
                                //}
                                if (IsReminderOnlyCheckBox.Checked)
                                {
                                    IsReminderOnlyCheckBox.Checked = false;
                                    RRReminderlist();
                                }
                                else
                                {
                                    showcyclereminder.Checked = !showcyclereminder.Checked;
                                    shaixuanfuwei();
                                    RRReminderlist();
                                }
                            }
                        }
                    }
                    break;
                case Keys.D6:
                    //if (!(searchword.Focused || taskTime.Focused || tasklevel.Focused || dateTimePicker.Focused))
                    //{
                    //    Load_Click(null, null);
                    //}
                    IsReminderOnlyCheckBox.Checked = !IsReminderOnlyCheckBox.Checked;
                    RRReminderlist();
                    break;
                case Keys.D7:
                    if (!(searchword.Focused || taskTime.Focused || tasklevel.Focused || dateTimePicker.Focused))
                    {
                        morning.Checked = !morning.Checked;
                    }
                    break;
                case Keys.D8:
                    if (!(searchword.Focused || taskTime.Focused || tasklevel.Focused || dateTimePicker.Focused))
                    {
                        day.Checked = !day.Checked;
                    }
                    break;
                case Keys.D9:
                    if (!(searchword.Focused || taskTime.Focused || tasklevel.Focused || dateTimePicker.Focused))
                    {
                        afternoon.Checked = !afternoon.Checked;
                    }
                    break;
                case Keys.Decimal:
                    break;
                case Keys.Delete:
                    if (nodetree.Focused)
                    {
                        if (nodetree.SelectedNode.Tag != null)
                        {
                            try
                            {
                                deleteNodeByID(((XmlAttribute)(nodetree.SelectedNode.Tag)).Value);
                                SaveLog("删除节点：" + nodetree.SelectedNode.Text + "    导图" + showMindmapName.Split('\\')[showMindmapName.Split('\\').Length - 1]);
                                fenshuADD(1);
                                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(showMindmapName));
                                th.Start();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        try
                        {
                            if (System.IO.File.Exists(FileTreeView.SelectedNode.Name))
                            {
                                System.IO.File.Delete(FileTreeView.SelectedNode.Name);
                                SaveLog("删除文件：" + FileTreeView.SelectedNode.Name);
                                fenshuADD(1);

                            }
                            else if (System.IO.Directory.Exists(FileTreeView.SelectedNode.Name))
                            {
                                System.IO.Directory.Delete(FileTreeView.SelectedNode.Name);
                                SaveLog("删除文件：" + FileTreeView.SelectedNode.Name);
                                fenshuADD(1);
                            }
                            FileTreeView.SelectedNode.Remove();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (ReminderListFocused())
                    {
                        if (searchword.Text.ToLower().StartsWith("!") || searchword.Text.ToLower().StartsWith("！"))
                        {
                            usedSuggest3.Remove(((MyListBoxItemRemind)reminderlistSelectedItem).Name + "|" + ((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                            new TextListConverter().WriteListToTextFile(usedSuggest3, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest3.txt");
                            reminderList.Items.RemoveAt(reminderList.SelectedIndex);
                        }
                        else if (searchword.Text.ToLower().StartsWith("`") || searchword.Text.ToLower().StartsWith("·"))
                        {
                            RecentlyFileHelper.DeleteRecentlyFiles(((MyListBoxItemRemind)reminderlistSelectedItem).Name);
                            reminderList.Items.RemoveAt(reminderList.SelectedIndex);
                        }
                    }
                    break;
                case Keys.Divide:
                    break;
                case Keys.Down:
                    if (searchword.Focused && !SearchText_suggest.Visible)
                    {
                        reminderList.Focus();
                        reminderList.Refresh();
                    }
                    break;
                case Keys.E:
                    if (keyNotWork())
                    {
                        ebcheckBox.Checked = !ebcheckBox.Checked;
                    }
                    break;
                case Keys.End:
                    break;
                case Keys.Enter:

                    if (ReminderListFocused())
                    {
                        try
                        {
                            if (searchword.Text.ToLower().StartsWith("`") || searchword.Text.ToLower().StartsWith("·"))
                            {
                                isSearchFileOrNode = true;
                                System.Diagnostics.Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                                try
                                {
                                    FileInfo file = new FileInfo(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                                    string name = file.Name + "|" + file.FullName;
                                    if (!usedSuggest3.Contains(name))//放到这里也可以放到最终也可以暂时放这里
                                    {
                                        usedSuggest3.Add(name);
                                    }
                                    else
                                    {
                                        usedSuggest3.Remove(name);
                                        usedSuggest3.Add(name);
                                    }
                                    new TextListConverter().WriteListToTextFile(usedSuggest3, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest3.txt");
                                }
                                catch (Exception)
                                {
                                }
                                MyHide();
                                return;
                            }
                            if (IsURL(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                            {
                                System.Diagnostics.Process.Start(GetUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                                SaveLog("打开：    " + GetUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                            }
                            else if (IsFileUrl(((MyListBoxItemRemind)reminderlistSelectedItem).Name.Trim()))
                            {
                                System.Diagnostics.Process.Start(getFileUrlPath(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                                SaveLog("打开：    " + getFileUrlPath(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                            }
                            else if (((MyListBoxItemRemind)reminderlistSelectedItem).link != "" && ((MyListBoxItemRemind)reminderlistSelectedItem).link != null)
                            {
                                System.Diagnostics.Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).link);
                                SaveLog("打开：    " + ((MyListBoxItemRemind)reminderlistSelectedItem).link);
                            }
                            else
                            {
                                System.Diagnostics.Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                            }
                            try
                            {
                                FileInfo file = new FileInfo(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                                string name = file.Name + "|" + file.FullName;
                                if (!usedSuggest3.Contains(name))//放到这里也可以放到最终也可以暂时放这里
                                {
                                    usedSuggest3.Add(name);
                                }
                                else
                                {
                                    usedSuggest3.Remove(name);
                                    usedSuggest3.Add(name);
                                }
                                new TextListConverter().WriteListToTextFile(usedSuggest3, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest3.txt");
                            }
                            catch (Exception)
                            {
                            }
                        }
                        catch (Exception)
                        {

                        }
                        MyHide();
                    }
                    else if (mindmaplist.Focused)
                    {
                        if (mindmaplist.SelectedIndex > -1)
                        {
                            System.Diagnostics.Process.Start(((MyListBoxItem)mindmaplist.SelectedItem).Value);
                            MyHide();
                        }
                    }
                    else if (searchword.Focused)
                    {
                        if (SearchText_suggest.Visible)
                        {
                            return;
                        }
                        if (isRename)
                        {
                            RenameNodeByID(searchword.Text);
                            SaveLog("修改节点名称：" + renameTaskName + "  To  " + searchword.Text);
                            searchword.Text = "";
                            RRReminderlist();
                            return;
                        }
                        if (searchword.Text.StartsWith("path:"))
                        {
                            try
                            {
                                if (searchword.Text.Length < 7)
                                {
                                    new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                                    //rootpath = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
                                }
                                else
                                {
                                    string changePath = searchword.Text.Substring(5);
                                    for (int i = 0; i < PathcomboBox.Items.Count; i++)
                                    {
                                        if (PathcomboBox.Items[i].ToString() == changePath)
                                        {
                                            selectedpath = false;
                                            PathcomboBox.SelectedIndex = i;
                                        }
                                    }
                                    if (changePath.ToLower() == "rss")
                                    {
                                    }
                                    else
                                    {
                                    }
                                    if (changePath.Contains('\\'))
                                    {
                                        rootpath = new DirectoryInfo(searchword.Text.Substring(5));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            rootpath = new DirectoryInfo(ini.ReadString("path", changePath, ""));
                                        }
                                        catch (Exception)
                                        {
                                            rootpath = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                                        }
                                    }
                                }
                                if (!pathArr.Contains(rootpath.FullName))
                                {
                                    pathArr.Add(rootpath.FullName);
                                }
                                mindmapPath = rootpath.FullName;
                                searchword.Text = "";

                                Load_Click(null, null);
                                reminderList.Focus();
                                return;
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else if (searchword.Text.ToLower().StartsWith("lock"))
                        {
                            lockForm = true;
                            searchword.Text = "";
                        }
                        else if (searchword.Text.ToLower().StartsWith("unlock"))
                        {
                            lockForm = false;
                            searchword.Text = "";
                        }
                        else if (searchword.Text.ToLower().StartsWith("o="))
                        {
                            try
                            {
                                string num = searchword.Text.Substring(2);
                                this.Opacity = Convert.ToDouble(num);
                                searchword.Text = "";
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.StartsWith("#"))
                        {
                            if (searchword.Text.Length >= 2)
                            {
                                SearchFiles();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (searchword.Text.StartsWith("*"))
                        {
                            if (searchword.Text.Length >= 2)
                            {
                                SearchNode();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (searchword.Text.ToLower().StartsWith("ss"))
                        {
                            shaixuanfuwei();
                            RRReminderlist();
                        }
                        else if (searchword.Text.ToLower().StartsWith("p"))
                        {
                            GetPassWord();
                            searchword.Text = "";
                        }
                        else if (searchword.Text.ToLower().StartsWith("link:"))
                        {
                            SetLink(searchword.Text.Substring(5));
                            searchword.Text = "";
                        }
                        else if (searchword.Text.ToLower().StartsWith("g"))
                        {
                            try
                            {
                                if (searchword.Text.Length == 2)
                                {
                                    searchword.Text = "";
                                    WriteTagFile();
                                }
                                else
                                {
                                    tagCloudControl.AddItem(searchword.Text.Substring(1));
                                    SaveLog("添加Tag:    " + searchword.Text.Substring(1).Trim());
                                    searchword.Text = "";
                                    WriteTagFile();
                                    fenshuADD(1);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.ToLower().StartsWith("`") || searchword.Text.ToLower().StartsWith("·"))
                        {
                            try
                            {
                                reminderList.Items.Clear();
                                //reminderlist.Items.AddRange(RecentlyFileHelper.GetRecentlyFiles());
                                foreach (var file in RecentlyFileHelper.GetRecentlyFiles(searchword.Text.Substring(1)))
                                {
                                    try
                                    {
                                        reminderList.Items.Add(file);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                reminderList.Sorted = false;
                                reminderList.Sorted = true;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.ToLower().StartsWith("pass="))
                        {
                            try
                            {
                                string password = searchword.Text.Substring(5);
                                if (password != "")
                                {
                                    PassWord = password;
                                    encrypt = new Encrypt(PassWord);
                                    IsEncryptBool = true;
                                }
                                else
                                {
                                    PassWord = "";
                                    IsEncryptBool = false;
                                }
                                searchword.Text = "";
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.Contains(ini.ReadString("money", "cost", "")) || searchword.Text.Contains(ini.ReadString("money", "income", "")) || searchword.Text.Contains(ini.ReadString("money", "save", "")) || searchword.Text.Contains(ini.ReadString("money", "waste", "")))
                        {
                            bool ishasSound = false;
                            if (searchword.Text.Contains(ini.ReadString("money", "save", "")))
                            {
                                PlaySimpleSound("save");
                                ishasSound = true;
                                searchword.Text = searchword.Text.Replace(ini.ReadString("money", "save", ""), ini.ReadString("money", "save", "") + ini.ReadString("money", "income", ""));
                            }
                            if (searchword.Text.Contains(ini.ReadString("money", "waste", "")))
                            {
                                PlaySimpleSound("waste");
                                ishasSound = true;
                                searchword.Text = searchword.Text.Replace(ini.ReadString("money", "waste", ""), ini.ReadString("money", "waste", "") + ini.ReadString("money", "cost", ""));
                            }
                            string taskName = searchword.Text;
                            string account = ini.ReadString("money", "account", "");
                            string currentAccount = "";
                            string money = "0";
                            bool isIncome = false;
                            MatchCollection jc = Regex.Matches(taskName, ini.ReadString("money", "cost", "") + @"[1-9]\d*\.?\d*");
                            foreach (Match m in jc)
                            {
                                taskName = taskName.Replace(m.Value, "");
                                money = m.Value.Substring(2);
                                break;
                            }
                            MatchCollection sr = Regex.Matches(taskName, ini.ReadString("money", "income", "") + @"[1-9]\d*\.?\d*");
                            foreach (Match m in sr)
                            {
                                taskName = taskName.Replace(m.Value, "");
                                money = m.Value.Substring(2);
                                isIncome = true;
                                break;
                            }
                            foreach (string item in account.Split(';'))
                            {
                                if (item == "")
                                {
                                    continue;
                                }
                                if (taskName.Contains(item))
                                {
                                    currentAccount = item;
                                    taskName = taskName.Replace(currentAccount, "");
                                }
                            }
                            if (!ishasSound)
                            {
                                if (isIncome)
                                {
                                    PlaySimpleSound("income");
                                }
                                else
                                {
                                    PlaySimpleSound("cost");
                                }
                            }

                            AddMoney(ini.ReadString("money", "money", ""), currentAccount, money, isIncome, taskName);
                            try
                            {
                                if (searchword.Text.Contains(ini.ReadString("money", "save", "")) || searchword.Text.Contains(ini.ReadString("money", "waste", "")))
                                {
                                    showMoneyLeft(ini.ReadString("money", "money", ""), "saveAccount");
                                }
                                else
                                {
                                    showMoneyLeft(ini.ReadString("money", "money", ""), "balanceAccount");
                                }
                            }
                            catch (Exception)
                            {
                            }
                            searchword.Text = "";
                        }
                        else if (searchword.Text.StartsWith("#"))
                        {
                            SearchFiles();
                            return;
                        }
                        else if (searchword.Text.StartsWith("remind"))
                        {
                            DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                            string content = "";
                            foreach (FileInfo file in path.GetFiles("*.mm", SearchOption.AllDirectories))
                            {
                                string filename = Path.GetFileNameWithoutExtension(file.FullName);
                                content += filename;
                                content += "|";
                                content += Tools.GetFirstSpell(filename);
                                content += "|";
                                content += Tools.ConvertToAllSpell(filename);
                                content += "|";
                                content += Tools.GetFirstSpell(filename);
                                content += "@";
                            }
                            Tools.RecordLog(content);
                            searchword.Text = "";
                            return;
                        }
                        else if (searchword.Text.StartsWith("rss"))
                        {
                            try
                            {
                                string rss = searchword.Text.Substring(3);
                                if (rss == "" || !IsUri(rss))
                                {
                                    return;
                                }

                                string path = ini.ReadString("path", "rss", "");
                                string domin = GetTopDomin(rss);
                                if (!System.IO.Directory.Exists(path + "\\" + domin))
                                {
                                    System.IO.Directory.CreateDirectory(path + "\\" + domin);
                                }
                                WebClient webClient = new WebClient();
                                webClient.Headers.Add("user-agent", "MyRSSReader/1.0");
                                XmlReader readers = XmlReader.Create(webClient.OpenRead(rss));
                                XmlDocument doc = new XmlDocument(); // 创建文档对象
                                try
                                {
                                    doc.Load(readers);//加载XML 包括HTTP：// 和本地
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);//异常处理
                                }
                                string titleStr = doc.GetElementsByTagName("title")[0].InnerText;
                                titleStr = Regex.Replace(titleStr, @"[\""]+", "");
                                titleStr = titleStr.Replace(" ", "");
                                titleStr = Regex.Replace(titleStr, @"((?=[\x21-\x7e]+)[^A-Za-z0-9])", "");
                                string demoPath = ini.ReadString("path", "rssdemo", "");
                                string fileName = path + "\\" + domin + "\\" + titleStr + ".mm";// getTitle(rss);
                                if (!File.Exists(fileName))//&&//将所有RSS记录一下，如果包含就不操作了。
                                {
                                    System.IO.File.Copy(demoPath, fileName);
                                    TextContentReplace(fileName, "####URL####", rss);
                                }
                                XmlNodeList list = doc.GetElementsByTagName("item");  // 获得项           
                                System.Xml.XmlDocument x = new XmlDocument();
                                x.Load(fileName);
                                foreach (XmlNode node in list)  // 循环每一项
                                {
                                    XmlElement ele = (XmlElement)node;
                                    string title = ele.GetElementsByTagName("title")[0].InnerText;//获得标题
                                    string link = ele.GetElementsByTagName("link")[0].InnerText;//获得联接
                                    string description = ele.GetElementsByTagName("description")[0].InnerText;//获得联接
                                    string guidurl = ele.GetElementsByTagName("guid").Count == 0 ? "" : ele.GetElementsByTagName("guid")[0].InnerText;//获得联接
                                    DateTime dt = DateTime.Now;
                                    try
                                    {
                                        dt = Convert.ToDateTime(((System.Xml.XmlElement)ele.PreviousSibling).InnerText);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    //添加到列表内
                                    ListViewItem item = new ListViewItem
                                    {
                                        Text = title,
                                        Tag = link
                                    };
                                    AddTaskToFile(x, "文章", title, link, description, guidurl, dt);
                                }
                                x.Save(fileName);
                                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(fileName));
                                th.Start();
                                searchword.Text = "";
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.StartsWith("t"))
                        {
                            try
                            {
                                string taskname = searchword.Text.Substring(1);
                                MatchCollection mc = Regex.Matches(taskname, @"[1-9]\d*m");
                                int tasktime = 5;
                                foreach (Match m in mc)
                                {
                                    taskname = taskname.Replace(m.Value, "");
                                    tasktime = Convert.ToInt16(m.Value.Substring(0, m.Value.Length - 1));
                                    break;
                                }
                                Thread th = new Thread(() => OpenFanQie(tasktime, taskname, System.AppDomain.CurrentDomain.BaseDirectory, GetPosition()));
                                tomatoCount += 1;
                                th.Start();
                                searchword.Text = "";
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.StartsWith("T"))
                        {
                            try
                            {
                                string taskname = searchword.Text.Substring(1);
                                Thread th = new Thread(() => OpenFanQie(0, taskname, System.AppDomain.CurrentDomain.BaseDirectory, GetPosition(), true));
                                tomatoCount += 1;
                                th.Start();
                                searchword.Text = "";
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (searchword.Text.StartsWith("@@"))
                        {
                            searchword.Text = "";
                            //显示当前导图的所有任务
                            RRReminderlist();
                            reminderList.Focus();
                            if (reminderList.Items.Count > 0)
                            {
                                reminderList.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                shaixuanfuwei();
                                RRReminderlist();
                            }
                            else if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                AddTask(true);
                            }
                            else
                            {
                                AddTask(false);
                            }
                        }
                    }
                    else if (nodetree.Focused)
                    {
                        if (IsMindMapNodeEdit)
                        {
                            IsMindMapNodeEdit = false;
                            return;
                        }
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            //将树节点设置成任务
                            if (nodetree.SelectedNode.Tag != null)
                            {
                                try
                                {
                                    SetTaskNodeByID(((XmlAttribute)(nodetree.SelectedNode.Tag)).Value);
                                    SaveLog("设置节点为任务：" + nodetree.SelectedNode.Text + "    导图" + showMindmapName.Split('\\')[showMindmapName.Split('\\').Length - 1]);
                                    Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(showMindmapName));
                                    th.Start();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        else
                        {
                            if (nodetree.Top != 9)
                            {
                                nodetree.Top = FileTreeView.Top = 9;
                                nodetree.Height = FileTreeView.Height = 799;
                            }
                            else
                            {
                                nodetree.Top = FileTreeView.Top = 501;
                                nodetree.Height = FileTreeView.Height = 307;
                            }
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            if (nodetree.Top != 9)
                            {
                                nodetree.Top = FileTreeView.Top = 9;
                                nodetree.Height = FileTreeView.Height = 799;
                            }
                            else
                            {
                                nodetree.Top = FileTreeView.Top = 501;
                                nodetree.Height = FileTreeView.Height = 307;
                            }
                            FileTreeView.Focus();
                        }
                        else
                        {
                            if (IsFileNodeEdit)
                            {
                                IsFileNodeEdit = false;
                                return;
                            }
                            try
                            {
                                System.Diagnostics.Process.Start(FileTreeView.SelectedNode.Name);
                                SaveLog("打开：    " + FileTreeView.SelectedNode.Name);
                                MyHide();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else if (e.Modifiers.CompareTo(Keys.Shift) == 0 && dateTimePicker.Focused)
                    {
                        edittime_EndDate();
                        //修改EndDate
                    }
                    else if (dateTimePicker.Focused || taskTime.Focused || tasklevel.Focused)
                    {
                        EditTime_Clic(null, null);
                    }
                    else if (n_days.Focused)
                    {
                        button_cycle_Click(null, null);
                    }
                    break;
                case Keys.EraseEof:
                    break;
                case Keys.Escape:
                    if (!keyNotWork())
                    {
                        if (searchword.Focused && IsViewModel.Checked)
                        {
                            mindmaplist.Focus();
                        }
                        else if (dateTimePicker.Focused)
                        {
                            reminderList.Focus();
                        }
                        else
                        {
                            if (searchword.Text.Contains("@"))//如果包含@
                            {
                                SearchText_suggest.Visible = false;
                                if (searchword.Text.Contains("@@"))
                                {

                                }
                                else
                                {
                                    mindmapornode.Text = searchword.Text.Split('@')[1];
                                }
                                searchword.Text = "";
                                //显示当前导图的所有任务
                                RRReminderlist();
                                reminderList.Focus();
                                if (reminderList.Items.Count > 0)
                                {
                                    reminderList.SelectedIndex = 0;
                                }
                            }
                            else if (searchword.Text.StartsWith("#"))
                            {
                                searchword.Text = "";
                                reminderList.Focus();
                                RRReminderlist();
                                reminderList.SelectedIndex = reminderSelectIndex;
                            }
                            else
                            {
                                searchword.Text = "";
                                reminderList.Focus();
                                RRReminderlist();
                                reminderList.SelectedIndex = reminderSelectIndex;
                            }
                        }
                    }
                    break;
                case Keys.Execute:
                    break;
                case Keys.Exsel:
                    break;
                case Keys.F:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            OpenFanqie();
                            MyHide();
                        }
                    }
                    break;
                case Keys.F1:
                    if (keyNotWork())
                    {
                        try
                        {
                            MyHide();
                            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + @"\README.docx");
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                case Keys.F10:
                    Jietu();
                    break;
                case Keys.F11:
                    //以后退出程序只有输入Exit
                    Application.Exit();

                    break;
                case Keys.F12:
                    if (keyNotWork())
                    {
                        //System.Diagnostics.Process.Start(@"log.txt");
                        MyHide();
                        Log log = new Log();
                        log.ShowDialog();
                    }
                    break;
                case Keys.F13:
                    break;
                case Keys.F14:
                    break;
                case Keys.F15:
                    break;
                case Keys.F16:
                    break;
                case Keys.F17:
                    break;
                case Keys.F18:
                    break;
                case Keys.F19:
                    break;
                case Keys.F2:
                    if (keyNotWork() && !FileTreeView.Focused && !nodetree.Focused)
                    {
                        isRename = true;
                        reminderSelectIndex = reminderList.SelectedIndex;
                        searchword.Text = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                        renameTaskName = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                        renameMindMapPath = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                        if (mindmapornode.Text.Contains(">"))
                        {
                            renameMindMapFileIDParent = renameMindMapFileID;
                        }
                        renameMindMapFileID = ((MyListBoxItemRemind)reminderlistSelectedItem).IDinXML;
                        searchword.Focus();
                    }
                    break;
                case Keys.F20:
                    break;
                case Keys.F21:
                    break;
                case Keys.F22:
                    break;
                case Keys.F23:
                    break;
                case Keys.F24:
                    break;
                case Keys.F3:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_pc.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_pc.Checked = !fenlei_pc.Checked;
                        //}
                    }
                    break;
                case Keys.F4:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_learn.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_learn.Checked = !fenlei_learn.Checked;
                        //}
                    }
                    break;
                case Keys.F5:
                    SearchText_suggest.Visible = false;
                    RRReminderlist();
                    break;
                case Keys.F6:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_todo.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_todo.Checked = !fenlei_todo.Checked;
                        //}
                        MyHide();
                        OpenSearch();
                    }
                    break;
                case Keys.F7:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_keepme.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_keepme.Checked = !fenlei_keepme.Checked;
                        //}
                        MyHide();
                        Btn_OpenFolder_Click();
                    }
                    break;
                case Keys.F8:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_rest.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_rest.Checked = !fenlei_rest.Checked;
                        //}
                        MyHide();
                        btn_OpenFile_MouseClick();
                    }
                    break;
                case Keys.F9:
                    if (keyNotWork())
                    {
                        //if (moshiview.Checked)
                        //{
                        //    setUnCheck();
                        //    fenlei_wonderfull.Checked = true;
                        //}
                        //else
                        //{
                        //    fenlei_wonderfull.Checked = !fenlei_wonderfull.Checked;
                        //}
                        MyHide();
                        Tools menu = new Tools();
                        menu.ShowDialog();
                    }
                    break;
                case Keys.FinalMode:
                    break;
                case Keys.G:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            OpenFanqie(true);
                            MyHide();
                        }
                    }
                    break;
                case Keys.H:
                    if (nodetree.Focused)
                    {
                        try
                        {
                            if (nodetree.SelectedNode.Parent != null)
                            {
                                nodetree.SelectedNode.Parent.Collapse();
                                //treeView1.SelectedNode = treeView1.SelectedNode.Parent;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        try
                        {
                            if (FileTreeView.SelectedNode.Parent != null)
                            {
                                FileTreeView.SelectedNode.Parent.Collapse();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (ReminderListFocused() || mindmaplist.Focused || this.Focused)
                    {
                        if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            if (PathcomboBox.SelectedIndex > 0)
                            {
                                PathcomboBox.SelectedIndex--;
                            }
                            else
                            {
                                PathcomboBox.SelectedIndex = PathcomboBox.Items.Count - 1;
                            }
                        }
                        else if (ReminderListFocused())
                        {
                            try
                            {
                                Thread th = new Thread(() => yixiaozi.Media.Audio.Audio.SpeakText(((MyListBoxItemRemind)reminderlistSelectedItem).Name));
                                th.Start();
                            }
                            catch (Exception)
                            {
                            }
                        }

                    }
                    break;
                case Keys.HanguelMode:
                    break;
                case Keys.HanjaMode:
                    break;
                case Keys.Help:
                    break;
                case Keys.Home:
                    break;
                case Keys.I:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            isRename = true;
                            reminderSelectIndex = reminderList.SelectedIndex;
                            searchword.Text = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                            renameTaskName = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                            renameMindMapPath = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                            if (mindmapornode.Text.Contains(">"))
                            {
                                renameMindMapFileIDParent = renameMindMapFileID;
                            }
                            renameMindMapFileID = ((MyListBoxItemRemind)reminderlistSelectedItem).IDinXML;
                            searchword.Focus();
                        }
                        else
                        {
                            isRename = false;
                            reminderSelectIndex = reminderList.SelectedIndex;
                            searchword.Focus();
                        }
                    }
                    break;
                case Keys.IMEAccept:
                    break;
                case Keys.IMEConvert:
                    break;
                case Keys.IMEModeChange:
                    break;
                case Keys.IMENonconvert:
                    break;
                case Keys.Insert:
                    break;
                case Keys.J:
                    if (keyNotWork())
                    {
                        if ((ReminderListFocused() && (reminderList.Items.Count != 0) || reminderListBox.Items.Count != 0))
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                if (tasklevel.Value < 100)
                                {
                                    tasklevel.Value += 1;
                                    EditTime_Clic(null, null);
                                }
                            }
                            else if (e.Modifiers.CompareTo(Keys.Alt) == 0)
                            {
                                if (taskTime.Value < 700)
                                {
                                    taskTime.Value += 10;
                                    EditTime_Clic(null, null);
                                }
                            }
                            else if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                if (dateTimePicker.Value != null)
                                {
                                    dateTimePicker.Value = dateTimePicker.Value.AddHours(1);
                                    EditTime_Clic(null, null);
                                }
                            }
                            else
                            {
                                if (reminderList.Focused)
                                {
                                    if (reminderList.SelectedIndex + 1 < reminderList.Items.Count)
                                    {
                                        reminderList.SelectedIndex++;
                                    }
                                    else
                                    {
                                        if (reminderListBox.Items.Count > 0)
                                        {
                                            reminderListBox.Focus();
                                            reminderListBox.SelectedIndex = 0;
                                            reminderList.SelectedIndex = -1;
                                        }
                                        else
                                        {
                                            reminderList.Focus();
                                            reminderList.SelectedIndex = 0;
                                            reminderListBox.SelectedIndex = -1;
                                            reminderListBox.Refresh();
                                        }
                                    }
                                    reminderList.Refresh();
                                }
                                else if (reminderListBox.Focused)
                                {
                                    if (reminderListBox.SelectedIndex + 1 < reminderListBox.Items.Count)
                                    {
                                        reminderListBox.SelectedIndex++;
                                    }
                                    else
                                    {
                                        if (reminderList.Items.Count > 0)
                                        {
                                            reminderList.Focus();
                                            reminderList.SelectedIndex = 0;
                                            reminderListBox.SelectedIndex = -1;
                                            reminderList.Refresh();
                                        }
                                        else
                                        {
                                            reminderListBox.Focus();
                                            reminderListBox.SelectedIndex = 0;
                                            reminderList.SelectedIndex = -1;
                                        }
                                    }
                                    reminderListBox.Refresh();
                                }
                            }
                        }
                        else if (mindmaplist.Focused && mindmaplist.Items.Count != 0)
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                for (int i = mindmaplist.SelectedIndex + 1; i < mindmaplist.Items.Count; i++)
                                {
                                    if (mindmaplist.GetItemCheckState(i) == CheckState.Checked)
                                    {
                                        mindmaplist.SelectedIndex = i;
                                        mindmaplist.Refresh();
                                        return;
                                    }
                                }
                                for (int i = 0; i < mindmaplist.SelectedIndex; i++)
                                {
                                    if (mindmaplist.GetItemCheckState(i) == CheckState.Checked)
                                    {
                                        mindmaplist.SelectedIndex = i;
                                        mindmaplist.Refresh();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (mindmaplist.SelectedIndex + 1 < mindmaplist.Items.Count)
                                {
                                    mindmaplist.SelectedIndex++;
                                }
                                else
                                {
                                    mindmaplist.SelectedIndex = 0;
                                }
                                mindmaplist.Refresh();
                            }
                        }
                    }
                    if (taskTime.Focused)
                    {
                        if (taskTime.Value <= 718)
                        {
                            taskTime.Value += 5;
                        }
                    }
                    else if (tasklevel.Focused)
                    {
                        tasklevel.Value += 1;
                    }
                    else if (n_days.Focused)
                    {
                        n_days.Value += 1;
                    }
                    else if (dateTimePicker.Focused)
                    {
                        dateTimePicker.Value= dateTimePicker.Value.AddHours(1);
                    }
                    else if (nodetree.Focused)
                    {
                        try
                        {
                            if (nodetree.SelectedNode.NextNode != null)
                            {
                                nodetree.SelectedNode = nodetree.SelectedNode.NextNode;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        try
                        {
                            if (FileTreeView.SelectedNode.NextNode != null)
                            {
                                FileTreeView.SelectedNode = FileTreeView.SelectedNode.NextNode;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    break;
                case Keys.JunjaMode:
                    break;
                case Keys.K:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                if (tasklevel.Value >= 1)
                                {
                                    tasklevel.Value -= 1;
                                    EditTime_Clic(null, null);
                                }
                            }
                            else if (e.Modifiers.CompareTo(Keys.Alt) == 0)
                            {
                                if (taskTime.Value >= 10)
                                {
                                    taskTime.Value -= 10;
                                    EditTime_Clic(null, null);
                                }
                            }
                            else if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                if (dateTimePicker.Value != null)
                                {
                                    dateTimePicker.Value = dateTimePicker.Value.AddHours(-1);
                                    EditTime_Clic(null, null);
                                }
                            }
                            else
                            {
                                if (reminderList.Focused)
                                {
                                    if (reminderList.SelectedIndex > 0)
                                    {
                                        reminderList.SelectedIndex--;
                                    }
                                    else
                                    {
                                        if (reminderListBox.Items.Count > 0)
                                        {
                                            reminderListBox.Focus();
                                            reminderListBox.SelectedIndex = reminderListBox.Items.Count - 1;
                                            reminderList.SelectedIndex = -1;
                                            reminderListBox.Refresh();
                                        }
                                        else
                                        {
                                            reminderList.Focus();
                                            reminderList.SelectedIndex = reminderList.Items.Count - 1;
                                            reminderListBox.SelectedIndex = -1;
                                        }
                                    }
                                    reminderList.Refresh();
                                }
                                else if (reminderListBox.Focused)
                                {
                                    if (reminderListBox.SelectedIndex > 0)
                                    {
                                        reminderListBox.SelectedIndex--;
                                    }
                                    else
                                    {
                                        if (reminderList.Items.Count > 0)
                                        {
                                            reminderList.Focus();
                                            reminderList.SelectedIndex = reminderList.Items.Count - 1;
                                            reminderListBox.SelectedIndex = -1;
                                            reminderList.Refresh();
                                        }
                                        else
                                        {
                                            reminderListBox.Focus();
                                            reminderListBox.SelectedIndex = reminderListBox.Items.Count - 1;
                                            reminderList.SelectedIndex = -1;
                                        }
                                    }
                                    reminderListBox.Refresh();
                                }
                            }
                        }
                        else if (mindmaplist.Focused)
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                for (int i = mindmaplist.SelectedIndex - 1; i >= 0; i--)
                                {
                                    if (mindmaplist.GetItemCheckState(i) == CheckState.Checked)
                                    {
                                        mindmaplist.SelectedIndex = i;
                                        mindmaplist.Refresh();
                                        return;
                                    }
                                }
                                for (int i = mindmaplist.Items.Count - 1; i > mindmaplist.SelectedIndex; i--)
                                {
                                    if (mindmaplist.GetItemCheckState(i) == CheckState.Checked)
                                    {
                                        mindmaplist.SelectedIndex = i;
                                        mindmaplist.Refresh();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (mindmaplist.SelectedIndex > 0)
                                {
                                    mindmaplist.SelectedIndex--;
                                }
                                else
                                {
                                    mindmaplist.SelectedIndex = mindmaplist.Items.Count - 1;
                                }
                            }
                        }
                    }
                    if (taskTime.Focused)
                    {
                        if (taskTime.Value >= 5)
                        {
                            taskTime.Value -= 5;
                        }
                    }
                    else if (tasklevel.Focused)
                    {
                        if (tasklevel.Value >= 1)
                        {
                            tasklevel.Value -= 1;
                        }
                    }
                    else if (n_days.Focused)
                    {
                        if (n_days.Value >= 1)
                        {
                            n_days.Value -= 1;
                        }
                    }
                    
                    else if (dateTimePicker.Focused)
                    {
                        dateTimePicker.Value = dateTimePicker.Value.AddHours(-1);
                    }
                    else if (nodetree.Focused)
                    {
                        if (nodetree.SelectedNode.PrevNode != null)
                        {
                            nodetree.SelectedNode = nodetree.SelectedNode.PrevNode;
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        if (FileTreeView.SelectedNode.PrevNode != null)
                        {
                            FileTreeView.SelectedNode = FileTreeView.SelectedNode.PrevNode;
                        }
                    }
                    break;
                case Keys.KeyCode:
                    break;
                case Keys.L:
                    if (keyNotWork())
                    {
                        if (nodetree.Focused)
                        {
                            try
                            {
                                if (nodetree.SelectedNode.Nodes != null && nodetree.SelectedNode.Nodes.Count != 0)
                                {
                                    nodetree.SelectedNode = nodetree.SelectedNode.Nodes[0];
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (FileTreeView.Focused)
                        {
                            try
                            {
                                if (FileTreeView.SelectedNode.Nodes != null && FileTreeView.SelectedNode.Nodes.Count != 0)
                                {
                                    FileTreeView.SelectedNode = FileTreeView.SelectedNode.Nodes[0];
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (ReminderListFocused() || reminderListBox.Focused || mindmaplist.Focused || this.Focused)
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                if (PathcomboBox.SelectedIndex < PathcomboBox.Items.Count - 1)
                                {
                                    PathcomboBox.SelectedIndex++;
                                }
                                else
                                {
                                    PathcomboBox.SelectedIndex = 0;
                                }
                            }
                            else if (e.Modifiers.CompareTo(Keys.Alt) == 0)
                            {
                                if (this.Width == 1180)
                                {
                                    this.Width = 888;
                                }
                                else
                                {
                                    this.Width = 1180;
                                }
                                Center();
                            }
                            else
                            {
                                tasklevel.Focus();
                            }
                        }
                    }
                    break;
                //case Keys.LButton:
                //break;
                case Keys.LControlKey:
                    break;
                case Keys.LMenu:
                    break;
                case Keys.LShiftKey:
                    break;
                case Keys.LWin:
                    break;
                case Keys.LaunchApplication1:
                    break;
                case Keys.LaunchApplication2:
                    break;
                case Keys.LaunchMail:
                    break;
                case Keys.Left:
                    break;
                case Keys.LineFeed:
                    break;
                case Keys.M:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused() || reminderListBox.Focused || dateTimePicker.Focused || tasklevel.Focused)
                        {
                            taskTime.Focus();
                        }
                    }
                    break;
                case Keys.MButton:
                    break;
                case Keys.MediaNextTrack:
                    break;
                case Keys.MediaPlayPause:
                    break;
                case Keys.MediaPreviousTrack:
                    break;
                case Keys.MediaStop:
                    break;
                case Keys.Menu:
                    break;
                case Keys.Modifiers:
                    break;
                case Keys.Multiply:
                    break;
                case Keys.N:
                    if (keyNotWork())
                    {
                        PlaySimpleSound("treeview");
                        if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            //下面窗口设置一下
                            nodetree.Top = FileTreeView.Top = 501;
                            nodetree.Height = FileTreeView.Height = 307;
                            pictureBox1.Visible = false;
                            this.Height = 540;
                            reminderList.Focus();
                            //Center(); 
                        }
                        else if (e.Modifiers.CompareTo(Keys.Control) == 0)
                        {
                            string mindmapPath = "";
                            if (ReminderListFocused())
                            {
                                if (reminderList.SelectedIndex < 0)
                                {
                                    return;
                                }
                                if (((MyListBoxItemRemind)reminderlistSelectedItem).Name == "当前时间")
                                {
                                    return;
                                }
                                mindmapPath = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                            }
                            else if (mindmaplist.Focused)
                            {
                                mindmapPath = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                            }
                            if (mindmapPath == "")
                            {
                                return;
                            }
                            System.Diagnostics.Process.Start(new System.IO.FileInfo(mindmapPath).Directory.FullName);
                        }
                        else
                        {
                            if (ReminderListFocused())
                            {
                                ShowMindmap();
                                ShowMindmapFile();
                                nodetree.Visible = FileTreeView.Visible = true;
                                this.Height = 860;
                                nodetree.Focus();
                            }
                            else if (mindmaplist.Focused)
                            {
                                ShowMindmap();
                                ShowMindmapFile();
                                nodetree.Visible = FileTreeView.Visible = true;
                                this.Height = 860;
                                nodetree.Focus();
                            }
                            else if (nodetree.Focused || FileTreeView.Focused)
                            {
                                //下面窗口设置一下
                                nodetree.Top = FileTreeView.Top = 501;
                                nodetree.Height = FileTreeView.Height = 307;
                                pictureBox1.Visible = false;
                                nodetree.Visible = FileTreeView.Visible = false;
                                this.Height = 540;
                                reminderList.Focus();
                            }
                        }
                        Center();
                    }
                    break;
                case Keys.Next:
                    break;
                case Keys.NoName:
                    break;
                case Keys.None:
                    break;
                case Keys.NumLock:
                    break;
                case Keys.NumPad0:
                    break;
                case Keys.NumPad1:
                    break;
                case Keys.NumPad2:
                    break;
                case Keys.NumPad3:
                    break;
                case Keys.NumPad4:
                    break;
                case Keys.NumPad5:
                    break;
                case Keys.NumPad6:
                    break;
                case Keys.NumPad7:
                    break;
                case Keys.NumPad8:
                    break;
                case Keys.NumPad9:
                    break;
                case Keys.O:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                do
                                {
                                    dateTimePicker.Value = dateTimePicker.Value.AddHours(1);
                                }
                                while (dateTimePicker.Value < DateTime.Now);
                                EditTime_Clic(null, null);
                            }
                            else if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                CanceSelectedlTask(false);
                            }
                            else
                            {
                                taskComplete_btn_Click(null, null);
                                PlaySimpleSound("Done");
                            }
                        }
                        else if (dateTimePicker.Focused)
                        {
                            dateTimePicker.Value = dateTimePicker.Value.AddDays(1);
                        }
                    }
                    break;
                case Keys.Oem1:
                    break;
                case Keys.Oem102:
                    break;
                case Keys.Oem2:
                    break;
                case Keys.Oem3:
                    if (keyNotWork())
                    {
                        showcyclereminder.Checked = !showcyclereminder.Checked;
                    }
                    break;
                case Keys.Oem4:
                    break;
                case Keys.Oem5:
                    break;
                case Keys.Oem6:
                    break;
                case Keys.Oem7:
                    break;
                case Keys.Oem8:
                    break;
                case Keys.OemClear:
                    break;
                case Keys.OemMinus:
                    break;
                case Keys.OemPeriod:
                    break;
                case Keys.P:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                            {
                                dateTimePicker.Value = dateTimePicker.Value.AddHours(-1);
                                EditTime_Clic(null, null);

                            }
                            else
                            {
                                DelaySelectedTask();
                            }
                        }
                        else if (dateTimePicker.Focused)
                        {
                            dateTimePicker.Value = dateTimePicker.Value.AddDays(-1);
                        }
                    }
                    break;
                case Keys.Pa1:
                    break;
                case Keys.Packet:
                    break;
                case Keys.PageUp:
                    int n = pathArr.IndexOf(rootpath.FullName);
                    if (e.Modifiers.CompareTo(Keys.Shift) == 0)//pagedown被占用
                    {
                        if (n == 0)
                        {
                            rootpath = new DirectoryInfo(pathArr[pathArr.Count - 1]);
                        }
                        else
                        {
                            rootpath = new DirectoryInfo(pathArr[n - 1]);
                        }
                    }
                    else
                    {
                        if (n + 1 >= pathArr.Count)
                        {
                            rootpath = new DirectoryInfo(pathArr[0]);
                        }
                        else
                        {
                            rootpath = new DirectoryInfo(pathArr[n + 1]);
                        }
                    }
                    mindmapPath = rootpath.FullName;
                    searchword.Text = "";
                    Load_Click(null, null);
                    break;
                case Keys.Pause:
                    break;
                case Keys.Play:
                    break;
                case Keys.Print:
                    break;
                case Keys.PrintScreen:
                    break;
                case Keys.ProcessKey:
                    break;
                case Keys.Q:
                    if (keyNotWork())
                    {
                        if (mindmaplist.Focused)
                        {
                            IsSelectReminder = false;
                        }
                        Thread thCalendarForm = new Thread(() => Application.Run(new Calendar.CalendarForm(mindmapPath)));
                        thCalendarForm.Start();
                        MyHide();
                        return;
                    }
                    break;
                case Keys.R:
                    if (keyNotWork())
                    {
                        if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            Jinian_btn_Click(null, null);
                        }
                        else
                        {
                            if (searchword.Text.StartsWith("*"))
                            {
                                SearchNode();
                                //AddTask(false);
                            }
                            else
                            {
                                if (nodetree.Focused)
                                {
                                    reminderList.Focus();
                                    ShowMindmap();
                                    ShowMindmapFile();
                                    nodetree.Visible = FileTreeView.Visible = true;
                                    this.Height = 860;
                                    nodetree.Focus();
                                }
                                else
                                {
                                    shaixuanfuwei();
                                    int reminderIndex = reminderList.SelectedIndex;
                                    RRReminderlist();
                                    try
                                    {
                                        if (reminderIndex > reminderList.Items.Count - 1)
                                        {
                                            reminderList.SelectedIndex = 0;
                                        }
                                        else
                                        {
                                            reminderList.SelectedIndex = reminderIndex;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Keys.RControlKey:
                    break;
                case Keys.RMenu:
                    break;
                case Keys.RShiftKey:

                //case Keys.RWin:
                //break;
                case Keys.Right:
                    break;
                case Keys.S:
                    if (keyNotWork())
                    {
                        IsViewModel.Checked = !IsViewModel.Checked;
                        if (!IsViewModel.Checked)
                        {
                            shaixuanfuwei();
                            RRReminderlist();
                            reminderList.Focus();
                        }
                        else
                        {
                            reminderList.SelectedIndex = -1;
                            reminderSelectIndex = -1;
                            IsSelectReminder = false;
                            mindmaplist.Focus();
                        }
                    }
                    break;
                case Keys.Scroll:
                    break;
                case Keys.Select:
                    break;
                case Keys.SelectMedia:
                    break;
                case Keys.Separator:
                    break;
                case Keys.Shift:

                    break;
                case Keys.ShiftKey:
                    break;
                case Keys.Sleep:
                    break;
                case Keys.Space:

                    if (ReminderListFocused())
                    {
                        if (isInReminderlistSelect)
                        {
                            MyHide();
                        }
                    }
                    break;
                case Keys.Subtract:
                    break;
                case Keys.T:
                    if (ReminderListFocused() || reminderListBox.Focused || taskTime.Focused || tasklevel.Focused)
                    {
                        dateTimePicker.Focus();
                    }
                    break;
                case Keys.Tab:
                    if (searchword.Focused)
                    {
                        reminderList.Focus();
                        reminderList.SelectedIndex = 0;
                    }
                    break;
                case Keys.U:
                    if (ReminderListFocused() || reminderListBox.Focused || taskTime.Focused || tasklevel.Focused)
                    {
                        if (e.Modifiers.CompareTo(Keys.Shift) == 0)
                        {
                            SetLeftDakaDays(-1);
                        }
                        else
                        {
                            //设置任务剩余次数
                            SetLeftDakaDays(1);
                        }
                    }
                    else if (FileTreeView.Focused)
                    {
                        ShowMindmapFileUp();
                    }
                    break;
                case Keys.Up:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            reminderList.Refresh();
                        }
                    }
                    break;
                case Keys.V:
                    if (keyNotWork())
                    {
                        if (ReminderListFocused())
                        {
                            if (e.Modifiers.CompareTo(Keys.Control) == 0)
                            {
                                AddClipToTask(true);
                            }
                            else
                            {
                                AddClipToTask();
                            }
                        }
                        else if (mindmaplist.Focused)
                        {
                            AddClip();
                        }
                    }
                    break;
                case Keys.VolumeDown:
                    break;
                case Keys.VolumeMute:
                    break;
                case Keys.VolumeUp:
                    break;
                case Keys.W:
                    mindmapornode.Text = "";
                    tasklevel.Value = 0;
                    taskTime.Value = 0;
                    RRReminderlist();
                    //why ,what is it?
                    //if (keyNotWork())
                    //{
                    //    if (reminderListFocused())
                    //    {
                    //        if (reminderlist.SelectedIndex != -1)
                    //        {
                    //            reminderSelectIndex = reminderlist.SelectedIndex;
                    //            reminderlist.SelectedIndex = -1;
                    //            IsSelectReminder = false;
                    //        }
                    //        else
                    //        {
                    //            reminderlist.SelectedIndex = reminderSelectIndex;
                    //        }
                    //    }
                    //    else if (mindmaplist.Focused)
                    //    {
                    //        if (mindmaplist.SelectedIndex != -1)
                    //        {
                    //            mindmapSelectIndex = mindmaplist.SelectedIndex;
                    //            mindmaplist.SelectedIndex = -1;
                    //        }
                    //        else
                    //        {
                    //            mindmaplist.SelectedIndex = mindmapSelectIndex;
                    //        }
                    //    }
                    //}
                    break;
                case Keys.X:
                    if (keyNotWork())
                    {
                        //if (mindmaplist.Focused)
                        //{
                        //    IsSelectReminder = false;
                        //}
                        //Thread thFileTreeForm = new Thread(() => Application.Run(new FileTreeForm(rootpath))); 
                        //thFileTreeForm.SetApartmentState(ApartmentState.STA);
                        //thFileTreeForm.Name = "FileTreeForm";
                        //thFileTreeForm.Start();
                        //MyHide();
                        if (reminderList.Focused)
                        {
                            reminderListBox.Items.Add((MyListBoxItemRemind)reminderlistSelectedItem);
                            reminderboxList.Add((MyListBoxItemRemind)reminderlistSelectedItem);
                            Reminderlistboxchange();
                            reminderList.Items.RemoveAt(reminderList.SelectedIndex);
                        }
                        else if (reminderListBox.Focused)
                        {
                            reminderboxList.Remove((MyListBoxItemRemind)reminderListBox.SelectedItem);
                            reminderListBox.Items.RemoveAt(reminderListBox.SelectedIndex);
                            Reminderlistboxchange();
                            if (reminderListBox.Items.Count == 0)
                            {
                                reminderList.Focus();
                                reminderList.SelectedIndex = 0;
                            }
                        }
                        return;
                    }
                    break;
                case Keys.XButton1:
                    break;
                case Keys.XButton2:
                    break;
                case Keys.Y:
                    ShowSubNode();
                    break;
                case Keys.Z:
                    if (e.Modifiers.CompareTo(Keys.Control) == 0)
                    {
                        n_days.Focus();
                    }
                    else
                    {
                        //提醒任务
                        int selectindex = reminderList.SelectedIndex;
                        SetReminderOnly((MyListBoxItemRemind)reminderlistSelectedItem);
                        RRReminderlist();
                        if (selectindex < reminderList.Items.Count - 1)
                        {
                            reminderList.SelectedIndex = selectindex;
                        }
                    }

                    break;
                case Keys.Zoom:
                    break;
                default:
                    break;
            }
        }

        private void SetLink(string link)
        {
            if (reminderList.SelectedIndex >= 0)
            {
                MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                string taskName = selectedReminder.Name;
                DateTime dateBefore = selectedReminder.EndDate;
                if (selectedReminder.isEncrypted)
                {
                    taskName = encrypt.EncryptString(taskName);
                }
                foreach (XmlNode node in x.GetElementsByTagName("hook"))
                {
                    try
                    {
                        if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                        {
                            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            if (GetAttribute(node.FirstChild, "LINK") != "")
                            {
                                node.FirstChild.Attributes["LINK"].Value = link;
                            }
                            else
                            {
                                //添加属性
                                XmlAttribute TASKLEVEL = x.CreateAttribute("LINK");
                                node.ParentNode.Attributes.Append(TASKLEVEL);
                                node.ParentNode.Attributes["LINK"].Value = link;
                            }
                            x.Save(selectedReminder.Value);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th.Start();
                            SaveLog("修改了任务：" + taskName + "    截止时间：" + dateBefore.ToString() + ">" + dateTimePicker.Value.ToString());
                            return;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private void GetPassWord()
        {
            Encrypt e = new Encrypt("pass");
            string password = e.EncryptString(searchword.Text.Substring(1), false);
            password = password.Replace("a", "@");
            password = password.Replace("b", "+");
            password = password.Replace("c", "-");
            password = password.Replace("e", "%");
            password = password.Replace("=", "#");
            if (password.Length > 13)
            {
                password = password.Substring(0, 13);
            }
            password += ".";
            Clipboard.SetDataObject(password, true);
        }

        public bool ReminderListFocused()
        {
            return reminderList.Focused || reminderListBox.Focused;
        }
        public void SetReminderOnly(MyListBoxItemRemind selectedReminder)//selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
        {
            if (reminderList.SelectedIndex >= 0)
            {
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(selectedReminder.Value);
                string taskName = selectedReminder.Name;
                if (selectedReminder.isEncrypted)
                {
                    taskName = encrypt.EncryptString(taskName);
                }
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    if (node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == selectedReminder.IDinXML)
                    {
                        try
                        {
                            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                            bool isHashook = false;
                            foreach (XmlNode item in node.ChildNodes)
                            {
                                if (item.Name == "hook" && !isHashook)
                                {
                                    isHashook = true;
                                    item.FirstChild.Attributes["REMINDUSERAT"].Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                }
                            }
                            if (!isHashook)
                            {
                                XmlNode remindernode = x.CreateElement("hook");
                                XmlAttribute remindernodeName = x.CreateAttribute("NAME");
                                remindernodeName.Value = "plugins/TimeManagementReminder.xml";
                                remindernode.Attributes.Append(remindernodeName);
                                XmlNode remindernodeParameters = x.CreateElement("Parameters");
                                XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
                                remindernodeTime.Value = (Convert.ToInt64((dateTimePicker.Value - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
                                remindernodeParameters.Attributes.Append(remindernodeTime);
                                remindernode.AppendChild(remindernodeParameters);
                                node.AppendChild(remindernode);
                            }
                            if (node.Attributes["ISReminderOnly"] != null)
                            {
                                node.Attributes["ISReminderOnly"].Value = (!selectedReminder.ISReminderOnly).ToString();
                            }
                            else
                            {
                                XmlAttribute ISReminderOnly = x.CreateAttribute("ISReminderOnly");
                                node.Attributes.Append(ISReminderOnly);
                                node.Attributes["ISReminderOnly"].Value = (!selectedReminder.ISReminderOnly).ToString();
                            }
                            x.Save(selectedReminder.Value);
                            Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(selectedReminder.Value));
                            th.Start();
                            SaveLog("修改了任务(是否是任务)：" + taskName + "    ：" + selectedReminder.ISReminderOnly.ToString());
                            return;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }


        }
        //获取顶级域名
        public string GetTopDomin(string url)
        {
            try
            {
                return url.Split('.')[url.Split('.').Length - 2];
            }
            catch (Exception)
            {
                return url;
            }
        }
        public void AddTaskToFile(XmlDocument x, string rootNode, string taskName, string link, string taskContent, string guid, DateTime dt)
        {
            if (taskName == "")
            {
                return;
            }
            //System.Xml.XmlDocument x = new XmlDocument();
            //x.Load(mindmap);
            XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == rootNode);
            //if (root.ChildNodes.Cast<XmlNode>().Any(m => m.Attributes[0].Name != "TEXT" && m.Attributes["TEXT"].Value == dt.Year.ToString()))
            if (!haschildNode(root, dt.Year.ToString()))
            {
                XmlNode yearNode = x.CreateElement("node");
                XmlAttribute yearNodeValue = x.CreateAttribute("TEXT");
                yearNodeValue.Value = dt.Year.ToString();
                yearNode.Attributes.Append(yearNodeValue);
                root.AppendChild(yearNode);
            }
            XmlNode year = root.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Year.ToString());
            if (!haschildNode(year, dt.Month.ToString()))
            {
                XmlNode monthNode = x.CreateElement("node");
                XmlAttribute monthNodeValue = x.CreateAttribute("TEXT");
                monthNodeValue.Value = dt.Month.ToString();
                monthNode.Attributes.Append(monthNodeValue);
                year.AppendChild(monthNode);
            }
            XmlNode month = year.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Month.ToString());
            if (!haschildNode(month, dt.Day.ToString()))
            {
                XmlNode dayNode = x.CreateElement("node");
                XmlAttribute dayNodeValue = x.CreateAttribute("TEXT");
                dayNodeValue.Value = dt.Day.ToString();
                dayNode.Attributes.Append(dayNodeValue);
                month.AppendChild(dayNode);
            }
            XmlNode day = month.ChildNodes.Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value == dt.Day.ToString());
            XmlNode newNote = x.CreateElement("node");
            XmlAttribute newNotetext = x.CreateAttribute("TEXT");
            string pstr = "";
            newNotetext.Value = pstr + taskName;
            XmlAttribute newNoteCREATED = x.CreateAttribute("CREATED");
            newNoteCREATED.Value = (Convert.ToInt64((dt - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            XmlAttribute newNoteMODIFIED = x.CreateAttribute("MODIFIED");
            newNoteMODIFIED.Value = (Convert.ToInt64((dt - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds)).ToString();
            XmlAttribute TASKID = x.CreateAttribute("ID");
            newNote.Attributes.Append(TASKID);
            newNote.Attributes["ID"].Value = Guid.NewGuid().ToString();
            XmlNode remindernode = x.CreateElement("hook");
            XmlAttribute remindernodeName = x.CreateAttribute("NAME");
            remindernodeName.Value = "plugins/TimeManagementReminder.xml";
            remindernode.Attributes.Append(remindernodeName);
            XmlNode remindernodeParameters = x.CreateElement("Parameters");
            XmlAttribute remindernodeTime = x.CreateAttribute("REMINDUSERAT");
            remindernodeTime.Value = Convert.ToInt64((dt - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds).ToString();
            remindernodeParameters.Attributes.Append(remindernodeTime);
            remindernode.AppendChild(remindernodeParameters);
            newNote.AppendChild(remindernode);
            XmlAttribute guidurlAtt = x.CreateAttribute("guidurl");
            guidurlAtt.Value = guid;
            XmlAttribute urlAtt = x.CreateAttribute("Link");
            urlAtt.Value = link;
            XmlNode taskContentnode = x.CreateElement("node");
            XmlAttribute taskContentValue = x.CreateAttribute("TEXT");
            taskContentValue.Value = taskContent;
            taskContentnode.Attributes.Append(taskContentValue);
            newNote.AppendChild(taskContentnode);
            newNote.Attributes.Append(newNotetext);
            newNote.Attributes.Append(newNoteCREATED);
            newNote.Attributes.Append(newNoteMODIFIED);
            newNote.Attributes.Append(guidurlAtt);
            newNote.Attributes.Append(urlAtt);
            day.AppendChild(newNote);
        }
        public static void TextContentReplace(string file, string oldvalue, string newvalue)
        {
            String strFile = File.ReadAllText(file);
            strFile = strFile.Replace(oldvalue, newvalue);
            File.WriteAllText(file, strFile);
        }
        /// <summary>
        /// 是否为Uri
        /// </summary>
        /// <param name="s">判断字符串</param>
        /// <returns></returns>
        public static bool IsUri(string s)
        {
            return Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out Uri u);
        }
        public void SetLeftDakaDays(int num)
        {
            MyListBoxItemRemind selectedReminder = (MyListBoxItemRemind)reminderlistSelectedItem;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(selectedReminder.Value);
            string taskName = selectedReminder.Name;
            if (selectedReminder.isEncrypted)
            {
                taskName = encrypt.EncryptString(taskName);
            }
            foreach (XmlNode node in x.GetElementsByTagName("hook"))
            {
                try
                {
                    if (node.Attributes["NAME"].Value == "plugins/TimeManagementReminder.xml" && node.ParentNode.Attributes["TEXT"].Value == taskName)
                    {
                        if (node.ParentNode.Attributes["LeftDakaDays"] == null)
                        {
                            XmlAttribute DAKADAY = x.CreateAttribute("LeftDakaDays");
                            DAKADAY.Value = num.ToString();
                            node.ParentNode.Attributes.Append(DAKADAY);
                        }
                        else
                        {
                            node.ParentNode.Attributes["LeftDakaDays"].Value = (Convert.ToInt16(node.ParentNode.Attributes["LeftDakaDays"].Value) + num).ToString();
                        }
                        x.Save(selectedReminder.Value);
                        return;
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        private void SearchFiles()
        {
            isSearchFileOrNode = true;
            string keywords = searchword.Text.Substring(1);
            if (keywords == "")
            {
                return;
            }
            string[] keywordsArr = keywords.Split(' ');
            reminderList.Items.Clear();
            List<string> files = new List<string>();
            foreach (node item in allfiles.Where(m => StringHasArrALL(m.mindmapPath, keywordsArr)).OrderByDescending(m => m.editDateTime).Take(200))
            {
                if (!files.Contains(item.mindmapPath))
                {
                    files.Add(item.mindmapPath);
                }
                string filename = item.Text.Replace(rootpath.ToString(), "");
                filename = filename.Replace(".files", "");
                filename = filename.Replace(".images", "");
                filename = filename.Replace("\\\\", "\\");
                reminderList.Items.Add(new MyListBoxItemRemind() { Text = filename, Value = item.mindmapPath, Name = item.Text });
            }
            mindmaplist.Items.Clear();
            foreach (string item in files)
            {
                if (System.IO.Directory.Exists(item))
                {
                    mindmaplist.Items.Insert(0, new MyListBoxItem { Text = item.Split('\\')[item.Split('\\').Length - 1], Value = item });
                }
            }
            mindmapnumdesc = mindmaplist.Items.Count;
            for (int i = 0; i < mindmaplist.Items.Count; i++)
            {
                mindmaplist.SetItemChecked(i, true);
            }
        }
        public static MyListBoxItemRemind newName(MyListBoxItemRemind reminder)
        {
            //reminder.Text = reminder.Datetime.ToString("yy-MM-dd-HH:mm") + " > " + reminder.Datetime.AddMinutes(reminder.rtaskTime).ToString("HH:mm") + @"  " + (reminder.level != null ? reminder.level.ToString() : "0") + @"  " + reminder.Name;
            reminder.Text = reminder.Time.ToString("dd HH:mm") + intostringwithlenght(reminder.rtaskTime, 4) + @" " + reminder.Name;
            return reminder;
        }
        public static bool IsFileUrl(string str)
        {
            if (Regex.IsMatch(str, @"(\w:\\)?([\w|.|:]*\\)?\w*\\{1}"))
            {
                return true;
            }
            return false;
        }
        public static string getFileUrlPath(string str)
        {
            string path = str;
            //我自己都看不懂。。。
            //if (Regex.IsMatch(str, @"(\w:\\)?([\w|.|:]*\\)?\w*\\{1}.*\.\w*"))
            //{
            //    path = Regex.Match(str, @"(\w:\\)?([\w|.|:]*\\)?\w*\\{1}.*\.\w*").ToString();
            //}
            //else
            //{
            //    path = Regex.Match(str, @"(\w:\\)?([\w|.|:]*\\)?\w*\\{1}").ToString();
            //}
            if (path[0] == '\\')
            {
                path = "." + path;
            }
            path = Path.GetFullPath(path);
            return path;
        }
        private void AddBin_btn_Paint(object sender, PaintEventArgs e)
        {
        }
        public void SaveLog(string log)
        {
            log = log.Replace("\r", " ").Replace("\n", " ");
            log = (DateTime.Now + "    " + log);
            log = encryptlog.EncryptString(log);
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + @"\log.txt", true))
            {
                if (log != "")
                {
                    //file.Write(DateTime.Now + "        ");
                    file.WriteLine(log);
                    //file.Write("\r");
                }
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //是否取消close操作
                e.Cancel = true;
                MyHide();
            }
        }
        private void fenlei_CheckedChanged(object sender, EventArgs e)
        {
            if (((IsViewModel.Checked && !InMindMapBool) || isHasNoFenleiModel) && mindmaplist.SelectedIndex != -1)
            {
                //设置分类
                Reminder reminderObject = new Reminder();
                FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json");
                using (StreamReader sw = fi.OpenText())
                {
                    string s = sw.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    reminderObject = serializer.Deserialize<Reminder>(s);
                    jsonHasMindmaps = reminderObject.mindmaps;
                    foreach (Control item in this.Controls)
                    {
                        if (item.Name.Contains("fenlei_") && !reminderObject.Fenleis.Any(m => m.Name == item.Text))
                        {
                            reminderObject.Fenleis.Add(new Fenlei { Name = item.Text, MindMaps = new List<string>() });
                        }
                    }
                    if (mindmaplist.SelectedItem != null)
                    {
                        foreach (Control item in this.Controls)
                        {
                            if (item.Name.Contains("fenlei_"))
                            {
                                if (((CheckBox)item).Checked && !reminderObject.Fenleis.First(m => m.Name == item.Text).MindMaps.Contains(((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3)))
                                {
                                    reminderObject.Fenleis.First(m => m.Name == item.Text).MindMaps.Add(((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                                }
                                if (((MyListBoxItem)mindmaplist.SelectedItem != null))
                                {
                                    if (!((CheckBox)item).Checked && reminderObject.Fenleis.First(m => m.Name == item.Text).MindMaps.Contains(((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3)))
                                    {
                                        while (reminderObject.Fenleis.First(m => m.Name == item.Text).MindMaps.Contains(((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3)))
                                        {
                                            reminderObject.Fenleis.First(m => m.Name == item.Text).MindMaps.Remove(((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                string json = new JavaScriptSerializer().Serialize(reminderObject);
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json", "");
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.Write(json);
                }
                if (isHasNoFenleiModel)
                {
                    isHasNoFenleiModel = false;
                    foreach (Control item in this.Controls)
                    {
                        if (item.Name.Contains("fenlei_") && ((CheckBox)item).Checked)
                        {
                            ((CheckBox)item).Checked = false;
                        }
                    }
                    fenlei_CheckedChanged(null, null);
                }
            }
            else
            {
                FileInfo fi = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + @"reminder.json");
                List<string> mindmaps = new List<string>();
                using (StreamReader sw = fi.OpenText())
                {
                    string s = sw.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    reminderObjectOut = serializer.Deserialize<Reminder>(s);
                    jsonHasMindmaps = reminderObjectOut.mindmaps;
                    if (false) //fenleidanxuan.Checked
                    {
                        //foreach (Control item in this.Controls)
                        //{
                        //    if (item.Name.Contains("fenlei_") && ((CheckBox)item).Checked)
                        //    {
                        //        isCodeFenlei = true;
                        //        ((CheckBox)item).Checked = false;
                        //        isCodeFenlei = false;
                        //    }
                        //}
                        //CheckBox currentCheckBox = sender as CheckBox;
                        //isCodeFenlei = true;
                        //currentCheckBox.Checked = true;
                        //isCodeFenlei = false;
                        //mindmaps.AddRange(reminderObjectOut.Fenleis.First(m => m.Name == currentCheckBox.Text).MindMaps);
                    }
                    else
                    {
                        int selectedfenleicount = 0;
                        //在这里将某个分类的添加进去。
                        foreach (Control item in this.Controls)
                        {
                            if (item.Name.Contains("fenlei_") && ((CheckBox)item).Checked)
                            {
                                selectedfenleicount++;
                                mindmaps.AddRange(reminderObjectOut.Fenleis.First(m => m.Name == item.Text).MindMaps);
                            }
                        }
                        //如果一个都没选择，就把没有分类的显示出来，方便分类。
                        isHasNoFenleiModel = false;
                        if (selectedfenleicount == 0 && reminderObjectOut.NoFenleiMindmaps.Count != 0)
                        {
                            isHasNoFenleiModel = true;
                            mindmaps.AddRange(reminderObjectOut.NoFenleiMindmaps);
                        }
                    }
                    //mindmaplist = mindmaplist_backup;
                    mindmaplist.Items.Clear();
                    mindmaplist.Items.AddRange(mindmaplist_backup);
                    //for (int i = 0; i < mindmaplist.Items.Count; i++)
                    //{
                    //    if (mindmaps.Contains(((MyListBoxItem)mindmaplist.Items[i]).Text))
                    //    {
                    //        mindmaplist.SetItemCheckState(i, CheckState.Checked);
                    //    }
                    //    else
                    //    {
                    //        mindmaplist.Items.RemoveAt(i);
                    //        //mindmaplist.SetItemCheckState(i, CheckState.Unchecked);
                    //    }
                    //}
                    for (int i = mindmaplist.Items.Count - 1; i >= 0; i--)
                    {
                        if (mindmaps.Contains(((MyListBoxItem)mindmaplist.Items[i]).Text))
                        {
                            mindmaplist.SetItemCheckState(i, CheckState.Checked);
                        }
                        else
                        {
                            if (IsViewModel.Checked)
                            {
                                if (!jsonHasMindmaps.Contains(((MyListBoxItem)mindmaplist.Items[i]).Text))
                                {
                                    MyListBoxItem newitem = new MyListBoxItem
                                    {
                                        Text = ((MyListBoxItem)mindmaplist.Items[i]).Text,
                                        Value = ((MyListBoxItem)mindmaplist.Items[i]).Value,
                                        IsSpecial = true
                                    };
                                    mindmaplist.Items.RemoveAt(i);
                                    mindmaplist.Items.Insert(i, newitem);
                                }
                                mindmaplist.SetItemCheckState(i, CheckState.Unchecked);
                            }
                            else
                            {
                                mindmaplist.Items.RemoveAt(i);
                            }
                        }
                    }
                }
                tasklevel.Value = 0;
                taskTime.Value = 0;
                //将没有分过类的导图设置颜色
                RRReminderlist();
            }
        }
        private void mindmaplist_MouseHover(object sender, EventArgs e)
        {
            InMindMapBool = true;
            SwitchToLanguageMode();
            if (!searchword.Focused)
            {
                mindmaplist.Focus();
            }
        }
        private void mindmaplist_MouseLeave(object sender, EventArgs e)
        {
            InMindMapBool = false;
        }

        public void SetUnCheck()
        {
            isCodeFenlei = false;
            foreach (Control item in this.Controls)
            {
                if (item.Name.Contains("fenlei_") && ((CheckBox)item).Checked)
                {
                    ((CheckBox)item).Checked = false;
                }
            }
            isCodeFenlei = true;
        }
        private void Quanxuan_CheckedChanged(object sender, EventArgs e)
        {
            return;
            //isHasNoFenleiModel = false;
            //if (true)
            //{
            //    IsViewModel.Checked = false;
            //    //moshiview.Checked = false;
            //    //fenleidanxuan.Checked = false;
            //    isCodeFenlei = false;
            //    foreach (Control item in this.Controls)
            //    {
            //        if (item.Name.Contains("fenlei_") && !((CheckBox)item).Checked)
            //        {
            //            ((CheckBox)item).Checked = true;
            //        }
            //    }
            //    isCodeFenlei = true;
            //    fenlei_CheckedChanged(null, null);
            //}
            //else
            //{
            //    isCodeFenlei = false;
            //    foreach (Control item in this.Controls)
            //    {
            //        if (item.Name.Contains("fenlei_") && ((CheckBox)item).Checked)
            //        {
            //            ((CheckBox)item).Checked = false;
            //        }
            //    }
            //    isCodeFenlei = true;
            //    fenlei_CheckedChanged(null, null);
            //}
        }
        private void IsViewModel_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void morning_CheckedChanged(object sender, EventArgs e)
        {
            if (true)
            {
                taskTime.Value = 0;
                tasklevel.Value = 0;
                RRReminderlist();
            }
        }
        public void fenshuADD(int n)
        {
            try
            {
                fenshu.Text = (Convert.ToInt16(fenshu.Text) + n).ToString();
                ini.WriteInt("info", "score", Convert.ToInt16(fenshu.Text) + n);
            }
            catch (Exception)
            {
            }
        }
        private void IsRememberModel_CheckedChanged(object sender, EventArgs e)
        {
            IsViewModel.Checked = false;
            Load_Click(null, null);
        }
        private void IsShowSub_CheckedChanged(object sender, EventArgs e)
        {
            //ShowSubNode();
        }
        public void ShowMindmapFile(bool isShowSub = false)
        {
            if (reminderlistSelectedItem == null && mindmaplist.SelectedItem == null)
            {
                return;
            }
            FileTreeView.Nodes.Clear();
            string mindmapPath = "";
            string Name = "";
            string id = "";
            if (ReminderListFocused())
            {
                if (reminderlistSelectedItem == null || ((MyListBoxItemRemind)reminderlistSelectedItem).Name == "当前时间")
                {
                    return;
                }
                Name = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                mindmapPath = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                id = ((MyListBoxItemRemind)reminderlistSelectedItem).IDinXML;
            }
            else if (mindmaplist.Focused)
            {
                if (mindmaplist.SelectedItem == null)
                {
                    return;
                }
                mindmapPath = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                Name = ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3);
            }
            if (mindmapPath == "")
            {
                return;
            }
            fileTreePath = new FileInfo(mindmapPath).Directory;
            BuildTree(fileTreePath, FileTreeView.Nodes, true);
            FileTreeView.Sort();
        }

        public void ShowMindmapFileUp()
        {
            FileTreeView.Nodes.Clear();
            fileTreePath = fileTreePath.Parent;
            BuildTree(fileTreePath, FileTreeView.Nodes, true);
            FileTreeView.Sort();
        }

        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe, bool isRoot)
        {
            TreeNode curNode = new TreeNode();
            if (isRoot)
            {
                foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
                {
                    if ((subdir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        BuildTree(subdir, FileTreeView.Nodes, false);
                    }
                }
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    if (file.Name.StartsWith("~") || (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        continue;
                    }
                    FileTreeView.Nodes.Add(file.FullName, file.Name);
                }
            }
            else
            {
                curNode = addInMe.Add(directoryInfo.FullName, " " + directoryInfo.Name);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    if (file.Name.StartsWith("~") && (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        continue;
                    }
                    curNode.Nodes.Add(file.FullName, file.Name);
                }
                foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
                {
                    if ((subdir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        BuildTree(subdir, curNode.Nodes, false);
                    }
                }
            }
        }


        private void FileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pictureBox1.Visible = false;//richTextBox.Visible = treeView1.Visible = webBrowser.Visible = axAcroPDF1.Visible = axFramerControl1.Visible = false;
            nodetree.Visible = true;
            if (e.Node.Name.ToLower().EndsWith("txt"))
            {
                //richTextBox.Visible = true;
                //StreamReader reader = new StreamReader(e.Node.Name, System.Text.Encoding.Default);
                //richTextBox.Text = reader.ReadToEnd();
                //reader.Close();
            }
            //*.jpg,*.jpeg,*.bmp,*.gif,*.ico,*.png,*.tif,*.wmf
            else if (e.Node.Name.ToLower().EndsWith("jpg") || e.Node.Name.ToLower().EndsWith("gif") || e.Node.Name.ToLower().ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("png") || e.Node.Name.ToLower().EndsWith("jpeg") || e.Node.Name.ToLower().EndsWith("ico") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp") || e.Node.Name.ToLower().EndsWith("bmp"))
            {
                nodetree.Visible = false;
                pictureBox1.Image = Image.FromFile(e.Node.Name);
                pictureBox1.Left = 260;
                pictureBox1.Top = 9;
                pictureBox1.Height = 785;
                pictureBox1.Width = 827;
                pictureBox1.Visible = true;
            }
            else if (e.Node.Name.ToLower().EndsWith("mm"))
            {
                string mindmapPath = e.Node.Name;
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(mindmapPath);
                nodetree.Nodes.Clear();
                TreeNode tNode = new TreeNode
                {
                    Text = "Root"
                };
                if (showMindmapName == mindmapPath)
                {
                    //如果为空则继续显示
                    if (nodetree.Nodes.Count != 0)
                    {
                        return;
                    }
                }
                else
                {
                    showMindmapName = mindmapPath;
                }
                AddNode(x.DocumentElement, tNode, true);
                nodetree.Visible = true;
            }
            else if (e.Node.Name.ToLower().EndsWith("pdf"))
            {
            }
            else if (e.Node.Name.ToLower().EndsWith("docx") || e.Node.Name.ToLower().EndsWith("doc") || e.Node.Name.ToLower().EndsWith("ppt") || e.Node.Name.ToLower().EndsWith("pptx") || e.Node.Name.ToLower().EndsWith("xlsx") || e.Node.Name.ToLower().EndsWith("xls"))
            {
            }
        }



        public void ShowMindmap(bool isShowSub = false)
        {
            if (searchword.Text.StartsWith("#") || (reminderlistSelectedItem == null && mindmaplist.SelectedItem == null))
            {
                return;
            }
            string Name = "";
            string id = "";
            string rootname = "Root";
            if (ReminderListFocused())
            {
                if (reminderlistSelectedItem == null || ((MyListBoxItemRemind)reminderlistSelectedItem).Name == "当前时间")
                {
                    return;
                }
                Name = ((MyListBoxItemRemind)reminderlistSelectedItem).Name;
                showMindmapName = renameMindMapPath = ((MyListBoxItemRemind)reminderlistSelectedItem).Value;
                id = ((MyListBoxItemRemind)reminderlistSelectedItem).IDinXML;
                if (isShowSub)
                {
                    rootname = "RootWithTime";
                }
            }
            else if (mindmaplist.Focused)
            {
                if (mindmaplist.SelectedItem == null)
                {
                    return;
                }
                showMindmapName = renameMindMapPath = ((MyListBoxItem)mindmaplist.SelectedItem).Value;
                Name = ((MyListBoxItem)mindmaplist.SelectedItem).Text.Substring(3);
            }
            if (renameMindMapPath == "")
            {
                return;
            }

            richTextSubNode.Clear();
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(renameMindMapPath);
            nodetree.Nodes.Clear();
            TreeNode tNode = new TreeNode
            {
                Text = rootname// "RootWithTime"
            };
            if (isShowSub)
            {
                XmlNode parentNode = x.FirstChild;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    try
                    {
                        if (node != null && node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == id)
                        {
                            parentNode = node;
                            AddNode(parentNode, tNode, true);
                            SelectTreeNode(nodetree.Nodes, Name);
                        }
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                AddNode(x.DocumentElement, tNode, true);
                SelectTreeNode(nodetree.Nodes, Name);
            }

        }
        /// <summary>
        /// Renders a node of XML into a TreeNode. Recursive if inside the node there are more child nodes.
        /// </summary>
        /// <param name="inXmlNode"></param>
        /// <param name="inTreeNode"></param>
        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode, bool isSubNode = false)
        {
            XmlNode xNode;
            TreeNode tNode = new TreeNode();
            XmlNodeList nodeList;
            int i;
            if (!inXmlNode.HasChildNodes)
            {
                return;
            }
            nodeList = inXmlNode.ChildNodes;
            for (i = 0; i <= nodeList.Count - 1; i++)
            {
                xNode = inXmlNode.ChildNodes[i];
                if (xNode.Name == "hook" || xNode.Name == "icon" || xNode.Name == "edge")
                {
                    continue;
                }
                if (isSubNode && xNode.Name == "node" && xNode.Attributes != null && xNode.Attributes["TEXT"] != null)
                {
                    if (showMindmapName.Contains(xNode.Attributes["TEXT"].InnerText))
                    {
                        AddNode(xNode, inTreeNode, true);
                    }
                    else if (xNode.Attributes["TEXT"].InnerText == "bin")//不显示bin节点
                    {
                        continue;
                    }
                    else
                    {
                        int id1 = 0;
                        TreeNode inTreeNodeAdd;
                        if (inTreeNode.Text.StartsWith("Root"))
                        {
                            //CREATED
                            DateTime dt = DateTime.Now;
                            string reminder = GetAttribute(xNode, "CREATED");
                            if (reminder != "")
                            {
                                long unixTimeStamp = Convert.ToInt64(reminder);
                                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                                dt = startTime.AddMilliseconds(unixTimeStamp);
                            }
                            id1 = nodetree.Nodes.Add(new TreeNode((reminder != "" && inTreeNode.Text == "RootWithTime" ? dt.ToString("MMddHH ") : "") + xNode.Attributes["TEXT"].InnerText));
                            inTreeNodeAdd = nodetree.Nodes[id1];
                        }
                        else
                        {
                            id1 = inTreeNode.Nodes.Add(new TreeNode(xNode.Attributes["TEXT"].InnerText));
                            inTreeNodeAdd = inTreeNode.Nodes[id1];
                        }
                        object objectTag = null;
                        if (xNode.Attributes != null && xNode.Attributes["ID"] != null)
                        {
                            objectTag = xNode.Attributes["ID"];
                        }
                        inTreeNodeAdd.Tag = objectTag;
                        if (xNode.HasChildNodes)
                        {
                            AddNode(xNode, inTreeNodeAdd, true);
                        }
                    }
                }
            }
        }

        public void SelectTreeNode(TreeNodeCollection node, string name)
        {
            foreach (TreeNode item in node)
            {
                if (item.Text == name)
                {
                    nodetree.SelectedNode = item;
                    return;
                }
                SelectTreeNode(item.Nodes, name);
            }
        }
        public void ShowSubNode()
        {
            if (searchword.Text.StartsWith("#") || searchword.Text.StartsWith("！") || searchword.Text.StartsWith("·") || searchword.Text.StartsWith("~"))
            {
                return;
            }
            if (((MyListBoxItemRemind)reminderlistSelectedItem).Name == "当前时间" || !((MyListBoxItemRemind)reminderlistSelectedItem).Value.EndsWith("mm"))
            {
                return;
            }
            richTextSubNode.Clear();
            //当任务长度大于某个长度时，将其显示在子节点框
            if (((MyListBoxItemRemind)reminderlistSelectedItem).Text.Length > 45)
            {
                richTextSubNode.AppendText((richTextSubNode.Text == "" ? "" : Environment.NewLine) + ((MyListBoxItemRemind)reminderlistSelectedItem).Name);
            }
            if (((MyListBoxItemRemind)reminderlistSelectedItem).link != "")
            {
                richTextSubNode.AppendText((richTextSubNode.Text == "" ? "" : Environment.NewLine) + ((MyListBoxItemRemind)reminderlistSelectedItem).link);
            }
            System.Xml.XmlDocument x = new XmlDocument();
            string id = "";
            try//解决文件被占用时报错
            {
                x.Load(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
            }
            catch (Exception)
            {
                return;
            }
            id = ((MyListBoxItemRemind)reminderlistSelectedItem).IDinXML;
            if (x.GetElementsByTagName("node").Count == 0)
            {
                return;
            }
            //XmlNode tasknode = x.GetElementById(id);
            //foreach (XmlNode node in tasknode.ChildNodes)
            //{
            //    try
            //    {
            //        if (node.Name== "node")
            //        {
            //            richTextSubNode.AppendText((richTextSubNode.Text == "" ? "" : Environment.NewLine) + node.Attributes["TEXT"].Value);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            foreach (XmlNode node in x.GetElementsByTagName("node"))
            {
                try
                {
                    if (node != null && node.Attributes != null && node.Attributes["ID"] != null && node.Attributes["ID"].InnerText == id)
                    {
                        try
                        {
                            //显示父节点
                            fathernode.Text = GetFatherNodeName(node);
                            foreach (XmlNode subNode in node.ChildNodes)
                            {
                                if (subNode.Attributes != null && subNode.Attributes["TEXT"] != null && subNode.Attributes["TEXT"].Value.ToLower() != "ok")
                                {
                                    richTextSubNode.AppendText((richTextSubNode.Text == "" ? "" : Environment.NewLine) + subNode.Attributes["TEXT"].Value);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        public string GetFatherNodeName(XmlNode node)
        {
            try
            {
                string s = "";
                while (node.ParentNode != null)
                {
                    try
                    {
                        //去掉根节点
                        if (node.ParentNode.ParentNode.Name == "map")
                        {
                            break;
                        }
                        s = node.ParentNode.Attributes["TEXT"].Value + (s != "" ? ">" : "") + s;
                        node = node.ParentNode;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                return s;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void ShowHTML()
        {
            return;
            //if (searchword.Text.StartsWith("#"))
            //{
            //    return;
            //}
            //if (reminderList.SelectedIndex < 0)
            //{
            //    return;
            //}
            //if (((MyListBoxItemRemind)reminderlistSelectedItem).Name == "当前时间")
            //{
            //    return;
            //}
            //richTextSubNode.Clear();
            //string str1 = "hook";
            //string str2 = "NAME";
            //string str3 = "plugins/TimeManagementReminder.xml";
            //System.Xml.XmlDocument x = new XmlDocument();
            //string Name = "";
            //if (x.GetElementsByTagName(str1).Count == 0)
            //{
            //    return;
            //}
            //foreach (XmlNode node in x.GetElementsByTagName(str1))
            //{
            //    try
            //    {
            //        if (node.Attributes[str2].Value == str3 && node.ParentNode.Attributes["TEXT"].Value == Name)
            //        {
            //            try
            //            {
            //                //rsslinktextBox.Text = node.ParentNode.Attributes["Link"].Value;
            //            }
            //            catch (Exception)
            //            {
            //            }
            //            foreach (XmlNode subnode in node.ParentNode.ChildNodes)
            //            {
            //                if (subnode.Name == "node")
            //                {
            //                    try
            //                    {
            //                        using (WebBrowser webBrowser1 = new WebBrowser())
            //                        {
            //                            webBrowser1.Visible = false;
            //                            webBrowser1.DocumentText = subnode.Attributes["TEXT"].Value;
            //                            webBrowser1.Document.Write(subnode.Attributes["TEXT"].Value);
            //                            if (webBrowser1.Document.Body.InnerText.StartsWith("\r\n"))
            //                            {
            //                                //richTextBox1.Text = webBrowser1.Document.Body.InnerText.Substring(4).Replace("\r\n\r\n", "\r\n"); ;
            //                            }
            //                            else
            //                            {
            //                                //richTextBox1.Text = webBrowser1.Document.Body.InnerText.Replace("\r\n\r\n", "\r\n");
            //                            }
            //                        }
            //                    }
            //                    catch (Exception)
            //                    {
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception) { }
            //}
        }
        public void GetNowIndex()
        {
            //{
            //    if (((MyListBoxItemRemind)item).Text.Contains("当前时间")) {

            //        return;
            //    }
            //}
            for (int i = 0; i < reminderList.Items.Count; i++)
            {
                MyListBoxItemRemind item = (MyListBoxItemRemind)reminderList.Items[i];
                if (item.Text.Contains("此时") && item.Name == "当前时间")
                {
                    if (reminderList.Items.Count >= i + 1)
                    {
                        reminderList.SelectedIndex = i;
                        return;
                    }
                    //会自动加1的
                    //reminderlist.SelectedIndex = i;
                    //return;
                }
            }
        }
        private void Searchword_TextChanged(object sender, EventArgs e)
        {
            if (searchword.Text.ToLower().StartsWith("ss") && !searchword.Text.ToLower().EndsWith("jj"))
            {
                return;
            }
            try
            {
                //单子打字练习
                if (mindmaplist.SelectedItem != null)
                {
                    if (((MyListBoxItem)mindmaplist.SelectedItem).Text.Contains("单词"))
                    {
                        if (searchword.Text == ((MyListBoxItemRemind)reminderlistSelectedItem).Name)
                        {
                            taskComplete_btn_Click(null, null);
                            searchword.Text = "";
                            reminderList.Focus();
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            if (searchword.Text.ToLower().Contains("exit"))
            {
                Application.Exit();
            }
            else if (searchword.Text.StartsWith("rr"))
            {
                searchword.Text = "";
                mindmapornode.Text = "";
                Load_Click(null, null);
            }
            else if (searchword.Text.ToLower().StartsWith("clipse"))
            {
                searchword.Text = "";
                MyHide();
                OpenSearch();
            }
            else if (searchword.Text.StartsWith("clipF"))
            {
                searchword.Text = "";
                MyHide();
                Btn_OpenFolder_Click();
            }
            else if (searchword.Text.ToLower().StartsWith("clipf"))
            {
                searchword.Text = "";
                MyHide();
                btn_OpenFile_MouseClick();
            }
            else if (searchword.Text.ToLower().StartsWith("gread"))
            {
                searchword.Text = "";
                ReadTagFile();
            }
            else if (searchword.Text.ToLower().StartsWith("gwrite"))
            {
                searchword.Text = "";
                WriteTagFile();
            }
            else if (searchword.Text.ToLower() == "`" || searchword.Text.ToLower() == "·")
            {
                isSearchFileOrNode = true;
                reminderList.Items.Clear();
                foreach (var file in RecentlyFileHelper.GetRecentlyFiles(""))
                {
                    try
                    {
                        reminderList.Items.Add(file);
                    }
                    catch (Exception)
                    {
                    }
                }
                reminderList.Sorted = false;
                reminderList.Sorted = true;
            }
            else if (searchword.Text.ToLower().StartsWith("playsound"))
            {
                searchword.Text = "";
                isPlaySound = !isPlaySound;
            }
            else if (searchword.Text.ToLower().StartsWith("playback"))
            {
                searchword.Text = "";
                playBackGround = !playBackGround;
            }
            else if (searchword.Text.StartsWith("ga"))
            {
                this.Close();
                string gitCommand = "git";
                string gitAddArgument = @"add -A";
                System.Diagnostics.Process.Start(gitCommand, gitAddArgument);
                searchword.Text = "";
                return;
            }
            else if (searchword.Text.ToLower().StartsWith("help"))
            {
                try
                {
                    searchword.Text = "";
                    MyHide();
                    //DRHelper menu = new DRHelper();
                    //menu.ShowDialog();
                }
                catch (Exception)
                {
                }
                return;
            }
            else if (searchword.Text.StartsWith("deltemp"))
            {
                try
                {
                    ////首先删除临时文件
                    DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", "")); //System.AppDomain.CurrentDomain.BaseDirectory);
                    foreach (FileInfo file in path.GetFiles("~*.mm", SearchOption.AllDirectories))
                    {
                        file.Delete();
                    }
                    foreach (FileInfo file in path.GetFiles("*.MM", SearchOption.AllDirectories))
                    {
                        System.IO.File.Move(file.FullName, file.FullName.Substring(0, file.FullName.Length - 2) + "mm");
                    }
                    searchword.Text = "";
                }
                catch (Exception)
                {
                }
            }
            else if (searchword.Text.ToLower().StartsWith("usedsug"))
            {
                searchword.Text = "";
                usedSuggest = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest.txt");
            }
            else if (searchword.Text.ToLower().StartsWith("usedsu2"))
            {
                usedSuggest2 = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest2.txt");
                searchword.Text = "";
            }
            else if (searchword.Text.ToLower().StartsWith("usedsu3"))
            {
                usedSuggest3 = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest3.txt");
                searchword.Text = "";
            }
            else if (searchword.Text.ToLower().StartsWith("jj") || searchword.Text.ToLower().Contains("jjj"))
            {
                if (searchword.Text.StartsWith("#"))
                {
                    searchword.Text = searchword.Text.Replace("jjj", "");
                    reminderList.Focus();
                    if (reminderList.Items.Count > 0)
                    {
                        reminderList.SelectedIndex = 0;
                    }
                }
                else
                {
                    searchword.Text = "";
                    reminderList.Focus();
                    GetNowIndex();
                }
            }
            else if (searchword.Text.StartsWith("allfile"))
            {
                searchword.Text = "";
                GetAllFilesJsonFile();
                yixiaozi.Model.DocearReminder.StationInfo.StationData = null;
            }
            else if (searchword.Text.StartsWith("allicon"))
            {
                searchword.Text = "";
                GetAllFilesJsonIconFile();
                yixiaozi.Model.DocearReminder.StationInfo.NodeData = null;
            }
            else if (searchword.Text.StartsWith("showlog"))
            {
                searchword.Text = "";
                MyHide();
                Log log = new Log();
                log.ShowDialog();
            }
            else if (searchword.Text.StartsWith("fenge"))
            {
                searchword.Text = "";
                showfenge = !showfenge;
            }
            else if (searchword.Text.StartsWith("allnode"))
            {
                searchword.Text = "";
                GetAllNodeJsonFile();
            }
            else if (searchword.Text.StartsWith("links"))
            {
                searchword.Text = "";
                GetIniFile();
            }
            else if (searchword.Text.StartsWith("denyall"))
            {
                searchword.Text = "";
                denyAll_Click(null, null);
                reminderList.Focus();
                if (reminderList.Items.Count > 0)
                {
                    reminderList.SelectedIndex = 0;
                }
            }
            else if (searchword.Text.ToLower().StartsWith("mindmaps"))
            {
                searchword.Text = "";
                Tools.createsuggest_fun();
                DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                foreach (FileInfo file in path.GetFiles("*.mm", SearchOption.AllDirectories))
                {
                    if (mindmapfiles.FirstOrDefault(m => m.filePath == file.FullName) == null)
                    {
                        mindmapfiles.Add(new mindmapfile { name = file.Name.Substring(0, file.Name.Length - 3), filePath = file.FullName });
                    }
                }
            }
            else if (searchword.Text.ToLower().StartsWith("remindmaps"))
            {
                searchword.Text = "";
                remindmapsList.Clear();
                DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
                foreach (FileInfo file in path.GetFiles("*.mm", SearchOption.AllDirectories))
                {
                    string text = System.IO.File.ReadAllText(file.FullName);
                    if (text.Contains(@"TimeManagementReminder.xml"))
                    {
                        remindmapsList.Add(file.FullName);
                    }
                }
                new TextListConverter().WriteListToTextFile(remindmapsList, System.AppDomain.CurrentDomain.BaseDirectory + @"\remindmaps.txt");
            }
            else if (searchword.Text.ToLower().StartsWith("tool"))
            {
                searchword.Text = "";
                int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
                int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
                this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
                MyHide();         //窗体的起始位置为(x,y)
                Tools menu = new Tools();
                menu.ShowDialog();
            }
            else if (searchword.Text.ToLower().StartsWith("ok"))
            {
                taskComplete_btn_Click(null, null);
                searchword.Text = "";
                reminderList.Focus();
            }
            else if (searchword.Text.ToLower().StartsWith("!") || searchword.Text.ToLower().StartsWith("！"))
            {
                isSearchFileOrNode = true;
                reminderList.Items.Clear();
                DateTime time = DateTime.Now;
                for (int i = usedSuggest3.Count - 1; i > -1; i--)
                {
                    string item = usedSuggest3[i];
                    time = time.AddDays(1);
                    try
                    {
                        reminderList.Items.Add(new MyListBoxItemRemind
                        {
                            Text = item.Split('|')[0],
                            Name = item.Split('|')[0],
                            Value = item.Split('|')[1],
                            Time = time,
                            isTask = false
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
                reminderList.Sorted = false;
                reminderList.Sorted = true;
            }
            else
            {
                if (isSearchFileOrNode)
                {
                    isSearchFileOrNode = false;
                    //重新进入导图模式
                    searchword.Text = "";
                    Load_Click(null, null);
                    reminderList.Focus();
                }
                else
                {

                }
            }
        }
        private void panel3_DoubleClick(object sender, EventArgs e)
        {
            encryptbutton_Click(null, null);
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }
        private void Hiddenmenu_Paint(object sender, PaintEventArgs e)
        {
        }

        private void Searchword_Enter(object sender, EventArgs e)
        {
            PlaySimpleSound("input");
            SwitchToLanguageMode("zh-CN");
        }
        private void Searchword_Leave(object sender, EventArgs e)
        {
            SwitchToLanguageMode();
        }

        private void mindmaplist_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (mindmapnumdesc > 0)
            {
                mindmapnumdesc--;
                return;
            }
            if (searchword.Text.Contains("*"))
            {
                for (int i = 0; i < mindmaplist.Items.Count; i++)
                {
                    if (mindmaplist.CheckedItems.IndexOf(mindmaplist.Items[i]) == -1)
                    {
                        for (int k = reminderList.Items.Count - 1; k > 0; k--)
                        {
                            if (((MyListBoxItemRemind)reminderList.Items[k]).Value == ((MyListBoxItem)mindmaplist.Items[i]).Value)
                            {
                                reminderList.Items.RemoveAt(k);
                            }
                        }
                    }
                }
            }
            else
            {
                Updateunchkeckmindmap();
            }
        }
        public void Updateunchkeckmindmap()
        {
            if (isRefreshMindmap)
            {
                return;
            }
            for (int i = 0; i < mindmaplist.Items.Count; i++)
            {
                if (((MyListBoxItem)mindmaplist.Items[i]).Value == "")
                {
                    continue;
                }
                if (mindmaplist.CheckedItems.IndexOf(mindmaplist.Items[i]) == -1)
                {
                    if (!unchkeckmindmap.Contains(((MyListBoxItem)mindmaplist.Items[i]).Value))
                    {
                        unchkeckmindmap.Add(((MyListBoxItem)mindmaplist.Items[i]).Value);
                        new TextListConverter().WriteListToTextFile(unchkeckmindmap, System.AppDomain.CurrentDomain.BaseDirectory + @"\unchkeckmindmap.txt");
                    }
                }
                else
                {
                    while (unchkeckmindmap.Contains(((MyListBoxItem)mindmaplist.Items[i]).Value))
                    {
                        unchkeckmindmap.Remove(((MyListBoxItem)mindmaplist.Items[i]).Value);
                        new TextListConverter().WriteListToTextFile(unchkeckmindmap, System.AppDomain.CurrentDomain.BaseDirectory + @"\unchkeckmindmap.txt");
                    }
                }
            }
        }
        private void RichSubTest_Enter(object sender, EventArgs e)
        {
            //if (richTextSubNode.Lines.Length > 12)
            //{
            //    
            //}
        }
        private void RichSubTest_Leave(object sender, EventArgs e)
        {
            //
        }
        private void RichSubTest_MouseHover(object sender, EventArgs e)
        {
            //if (richTextSubNode.Lines.Length > 12)
            //{
            //    
            //}
        }
        private void RichSubTest_MouseLeave(object sender, EventArgs e)
        {
            //
        }
        private void Panel4_Paint(object sender, PaintEventArgs e)
        {
        }
        private void Panel4_DoubleClick(object sender, EventArgs e)
        {
            Thread th = new Thread(() => Application.Run(new Calendar.CalendarForm(mindmapPath)));
            th.Start();
        }
        public void Jietu()
        {
            //截图
            Bitmap bit = new Bitmap(this.Width, this.Height);//实例化一个和窗体一样大的bitmap
            Graphics g = Graphics.FromImage(bit);
            g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
            g.CopyFromScreen(this.Left, this.Top, 0, 0, new Size(this.Width, this.Height));//保存整个窗体为图片
                                                                                           //g.CopyFromScreen(panel游戏区 .PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//只保存某个控件（这里是panel游戏区）
            try
            {
                bit.Save(CalendarImagePath + DateTime.Now.ToString("yyyy年MM月dd日HH时mm分ss秒") + ".png");//默认保存格式为PNG，保存成jpg格式质量不是很好
            }
            catch (Exception)
            {
            }
        }
        bool needSuggest = true;
        private void Searchword_KeyUp(object sender, KeyEventArgs e)
        {
            if (searchword.Text.ToLower().StartsWith("ss"))
            {
                return;
            }
            if (searchword.Text.Length < 2&&!searchword.Text.StartsWith("@"))
            {
                needSuggest = true;
                SearchText_suggest.Visible = false;
                return;
            }
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Space)
            {
                if (e.KeyCode == Keys.Back && e.Modifiers.CompareTo(Keys.Control) == 0)
                {
                    searchword.Text = "";
                    return;
                }
                else
                {
                    needSuggest = true;
                }
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
            {
                if (SearchText_suggest.SelectedIndex > 0)
                {
                    SearchText_suggest.SelectedIndex--;
                }
                else if (SearchText_suggest.SelectedIndex == 0)
                {
                    SearchText_suggest.SelectedIndex = SearchText_suggest.Items.Count - 1;
                }
                else
                {
                    SearchText_suggest.SelectedIndex = 0;
                }

                return;
            }
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
            {
                if (SearchText_suggest.SelectedIndex < SearchText_suggest.Items.Count - 1)
                {
                    SearchText_suggest.SelectedIndex++;
                }
                else
                {
                    SearchText_suggest.SelectedIndex = 0;
                }

                return;
            }
            if (searchword.Text != "" && searchword.Text.Contains("@@"))
            {
                string taskname = searchword.Text.Split('@')[0];
                if (searchword.SelectionStart < taskname.Length)
                {
                    return;
                }
                string filename = searchword.Text.Split('@')[2];
                searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                if (SearchText_suggest.SelectedItem != null && filename == (SearchText_suggest.SelectedItem as StationInfo).StationName_CN)
                {
                    SearchText_suggest.Visible = false;
                    return;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                    searchword.Text = taskname + "@@" + info.StationName_CN;
                    mindmapornode.Text = info.mindmapurl.Split('\\')[info.mindmapurl.Split('\\').Length - 1] + ">" + info.StationName_CN;
                    renameMindMapPath = info.mindmapurl;
                    renameMindMapFileID = info.nodeID;
                    SearchText_suggest.Visible = false;
                    searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                    if (!usedSuggest2.Contains(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl))//放到这里也可以放到最终也可以暂时放这里
                    {
                        usedSuggest2.Add(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl);
                    }
                    else
                    {
                        usedSuggest2.Remove(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl);
                        usedSuggest2.Add(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl);
                    }
                    new TextListConverter().WriteListToTextFile(usedSuggest2, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest2.txt");
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (filename == "")
                    {
                        StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                        if (!usedSuggest2.Contains(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl))
                        {
                        }
                        else
                        {
                            usedSuggest2.Remove(info.StationName_CN + "|" + info.StationName_EN + "|" + info.StationName_JX + "|" + info.nodeID + "|" + info.mindmapurl);
                        }
                        new TextListConverter().WriteListToTextFile(usedSuggest2, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest2.txt");
                    }
                    else
                    {
                        StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                        ignoreSuggest.Add(info.StationName_CN);
                        new TextListConverter().WriteListToTextFile(ignoreSuggest, System.AppDomain.CurrentDomain.BaseDirectory + @"\ignoreSuggest.txt");
                    }
                }
                else
                {
                    if (filename != "")
                    {
                        IList<StationInfo> dataSource = StationInfo.GetNodes(filename.Trim());
                        //处理建议，去掉重复（重复的没有意义），曾经选择过的排列在上面
                        for (int i = usedSuggest2.Count - 1; i > -1; i--)
                        {
                            if (dataSource.Count(m => m.StationName_CN == usedSuggest2[i].Split('|')[0]) > 0)
                            {
                                int index = dataSource.IndexOf(dataSource.FirstOrDefault(m => m.StationName_CN == usedSuggest2[i].Split('|')[0]));
                                dataSource = Swap(dataSource, index);
                            }
                        }
                        foreach (StationInfo item in dataSource.Where(m => m.StationName_CN.Length > 50))
                        {
                            item.StationName_CN = item.StationName_CN.Substring(0, 50);
                        }
                        if (dataSource.Count > 0)
                        {
                            SearchText_suggest.DataSource = dataSource;
                            SearchText_suggest.DisplayMember = "StationName_CN";
                            SearchText_suggest.ValueMember = "StationValue";
                            SearchText_suggest.Visible = true;
                        }
                        else
                        {
                            SearchText_suggest.Visible = false;
                        }
                    }
                    else
                    {
                        List<string> dd = usedSuggest2;
                        //显示之前选过的
                        List<StationInfo> ddd = new List<StationInfo>();
                        for (int i = usedSuggest2.Count - 1; i > -1; i--)
                        {
                            try
                            {
                                ddd.Add(new StationInfo() { StationName_CN = usedSuggest2[i].Split('|')[0], StationName_EN = usedSuggest2[i].Split('|')[1], StationName_JX = usedSuggest2[i].Split('|')[2], nodeID = usedSuggest2[i].Split('|')[3], mindmapurl = usedSuggest2[i].Split('|')[4] });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        SearchText_suggest.DataSource = ddd;
                        SearchText_suggest.DisplayMember = "StationName_CN";
                        SearchText_suggest.ValueMember = "StationValue";
                        SearchText_suggest.Visible = true;
                    }
                }
                if (SearchText_suggest.Visible)
                {
                    try
                    {
                        mindmapSearch.Text = Path.GetFileName(((StationInfo)SearchText_suggest.SelectedItem).mindmapurl);
                    }
                    catch (Exception)
                    {
                        mindmapSearch.Text = "";
                    }
                }
                SearchText_suggest.Height = SearchText_suggest.PreferredHeight; //12 * SearchText_suggest.Items.Count + 10;
            }
            else if (searchword.Text != "" && searchword.Text.Contains("@"))
            {
                string taskname = searchword.Text.Split('@')[0];
                if (searchword.SelectionStart < taskname.Length)
                {
                    return;
                }
                string filename = searchword.Text.Split('@')[1];
                if (filename != "" && command.Contains(filename))
                {
                    SearchText_suggest.Visible = false;
                    return;
                }
                searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                                                              //if (SearchText_suggest.SelectedItem != null && filename == (SearchText_suggest.SelectedItem as StationInfo).StationName_CN)会导致如果一样就没办法选别的
                                                              //{
                                                              //    SearchText_suggest.Visible = false;
                                                              //    return;
                                                              //}

                if (e.KeyCode == Keys.Enter)
                {
                    StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                    if (command.Contains(info.StationName_CN))
                    {
                        searchword.Text = info.StationName_CN;
                    }
                    else
                    {
                        searchword.Text = taskname + "@" + info.StationName_CN;
                    }
                    SearchText_suggest.Visible = false;
                    searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                    if (!usedSuggest.Contains(info.StationName_CN))//放到这里也可以放到最终也可以暂时放这里
                    {
                        usedSuggest.Add(info.StationName_CN);
                    }
                    else
                    {
                        usedSuggest.Remove(info.StationName_CN);
                        usedSuggest.Add(info.StationName_CN);
                    }
                    new TextListConverter().WriteListToTextFile(usedSuggest, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest.txt");
                    if (command.Contains(info.StationName_CN))
                    {
                        SendKeys.Send("{ENTER}");
                    }
                    return;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (filename == "")
                    {
                        StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                        if (!usedSuggest.Contains(info.StationName_CN))
                        {
                        }
                        else
                        {
                            usedSuggest.Remove(info.StationName_CN);
                        }
                        new TextListConverter().WriteListToTextFile(usedSuggest, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest.txt");
                    }
                    else
                    {
                        StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                        ignoreSuggest.Add(info.StationName_CN);
                        new TextListConverter().WriteListToTextFile(ignoreSuggest, System.AppDomain.CurrentDomain.BaseDirectory + @"\ignoreSuggest.txt");
                    }
                }
                else
                {
                    if (filename != "")
                    {
                        IList<StationInfo> dataSource = StationInfo.GetStations(filename.Trim());
                        //处理建议，去掉重复（重复的没有意义），曾经选择过的排列在上面
                        for (int i = usedSuggest.Count - 1; i > -1; i--)
                        {
                            if (dataSource.Count(m => m.StationName_CN == usedSuggest[i]) > 0)
                            {
                                int index = dataSource.IndexOf(dataSource.FirstOrDefault(m => m.StationName_CN == usedSuggest[i]));
                                dataSource = Swap(dataSource, index);
                            }
                        }
                        if (dataSource.Count > 0)
                        {
                            SearchText_suggest.DataSource = dataSource;
                            SearchText_suggest.DisplayMember = "StationName_CN";
                            SearchText_suggest.ValueMember = "StationValue";
                            SearchText_suggest.Visible = true;
                        }
                        else
                        {
                            SearchText_suggest.Visible = false;
                        }
                    }
                    else
                    {
                        List<string> dd = usedSuggest;
                        //显示之前选过的
                        List<StationInfo> ddd = new List<StationInfo>();
                        for (int i = usedSuggest.Count - 1; i > -1; i--)
                        {
                            ddd.Add(new StationInfo() { StationName_CN = usedSuggest[i] });
                        }
                        SearchText_suggest.DataSource = ddd;
                        SearchText_suggest.DisplayMember = "StationName_CN";
                        SearchText_suggest.ValueMember = "StationValue";
                        SearchText_suggest.Visible = true;
                    }
                }
                SearchText_suggest.Height = SearchText_suggest.PreferredHeight; //12 * SearchText_suggest.Items.Count + 10;
            }
            else if (needSuggest)
            {
                List<StationInfo> result = suggestListData.FindAll(m => m.StationName_CN.ToLower().Contains(searchword.Text.ToLower()) || (m.isNode == "" && StringHasArrALL(m.mindmapurl.ToLower(), searchword.Text.ToLower().Split(' '))));
                if (result.Count() > 0)
                {
                    string taskname = searchword.Text;
                    searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                    if (e.KeyCode == Keys.Enter)
                    {
                        StationInfo info = SearchText_suggest.SelectedItem as StationInfo;
                        SearchText_suggest.Visible = false;
                        searchword.Select(searchword.Text.Length, 1); //光标定位到文本框最后
                        richTextSubNode.Clear();
                        richTextSubNode.Height = 0;
                        try
                        {
                            Process.Start(info.mindmapurl);
                            SaveLog("打开：    " + info.mindmapurl);
                        }
                        catch (Exception)
                        {
                        }
                        if (!usedSuggest4.Contains(info.StationName_CN))//放到这里也可以放到最终也可以暂时放这里
                        {
                            usedSuggest4.Add(info.StationName_CN);
                        }
                        else
                        {
                            usedSuggest4.Remove(info.StationName_CN);
                            usedSuggest4.Add(info.StationName_CN);
                        }
                        new TextListConverter().WriteListToTextFile(usedSuggest4, System.AppDomain.CurrentDomain.BaseDirectory + @"\usedSuggest4.txt");
                        if (command.Contains(info.StationName_CN))
                        {
                            SendKeys.Send("{ENTER}");
                        }
                        searchword.Text = "";
                        SearchText_suggest.Visible = false;
                        MyHide();
                        return;
                    }
                    else
                    {
                        SearchText_suggest.DataSource = result;
                        SearchText_suggest.DisplayMember = "StationName_CN";
                        SearchText_suggest.ValueMember = "mindmapurl";
                        SearchText_suggest.Visible = true;
                        SearchText_suggest.Height = SearchText_suggest.PreferredHeight;// 12 * SearchText_suggest.Items.Count + 10;
                    }
                }
                else
                {
                    needSuggest = false;
                    SearchText_suggest.Visible = false;
                }
            }
            else
            {
                SearchText_suggest.Visible = false;
            }
        }
        private void mindmapSearch_TextChanged(object sender, EventArgs e)
        {
            if (SearchText_suggest.Visible)
            {
                return;
            }
            if (mindmapSearch.Text == "")
            {
                isRefreshMindmap = true;
                mindmaplist.Items.Clear();
                foreach (var item in mindmaplist_backup)
                {
                    mindmaplist.Items.Add(item);
                }
                mindmaplist.Sorted = false;
                mindmaplist.Sorted = true;
                for (int i = 0; i < mindmaplist.Items.Count; i++)
                {
                    mindmaplist.SetItemChecked(i, true);
                    string file = ((MyListBoxItem)mindmaplist.Items[i]).Value;
                    if (unchkeckmindmap.Contains(file))
                    {
                        mindmaplist.SetItemChecked(i, false);
                    }
                }
                isRefreshMindmap = false;
                return;
            }
            for (int i = mindmaplist.Items.Count - 1; i >= 0; i--)
            {
                if (!((MyListBoxItem)mindmaplist.Items[i]).Text.Contains(mindmapSearch.Text))
                {
                    mindmaplist.Items.RemoveAt(i);
                }
            }
        }
        private static IList<StationInfo> Swap<StationInfo>(IList<StationInfo> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
            return list;
        }
        private static IList<StationInfo> Swap<StationInfo>(IList<StationInfo> list, int index1)
        {
            var temp = list[index1];
            list.RemoveAt(index1);
            list.Insert(0, temp);
            return list;
        }
        private void JinainPanel_Paint(object sender, PaintEventArgs e)
        {
            Jinian_btn_Click(null, null);
        }
        private void onlyZhouqi_CheckedChanged(object sender, EventArgs e)
        {
            RRReminderlist();
        }
        private void AddClip_panel_Paint(object sender, PaintEventArgs e)
        {
        }
        private void TreeView1_ParentChanged(object sender, EventArgs e)
        {
        }
        private void TreeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, e.Node.Bounds);
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null)
                {
                    nodeFont = ((TreeView)sender).Font;
                }

                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.Gray, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }
        }
        private void TreeView1_MouseHover(object sender, EventArgs e)
        {
            nodetree.Focus();
        }
        private void TreeView1_MouseLeave(object sender, EventArgs e)
        {
            //reminderlist.Focus();
        }
        // Create a node sorter that implements the IComparer interface.
        public class NodeSorter : IComparer
        {
            // Compare the length of the strings, or the strings
            // themselves, if they are the same length.
            public int Compare(object x, object y)
            {
                TreeNode tx = x as TreeNode;
                TreeNode ty = y as TreeNode;
                // Compare the length of the strings, returning the difference.
                if (tx.Text.Length != ty.Text.Length)
                {
                    return tx.Text.Length - ty.Text.Length;
                }
                // If they are the same length, call Compare.
                return string.Compare(tx.Text, ty.Text);
            }
        }
        private void panel5_Click(object sender, EventArgs e)
        {
            //下面窗口设置一下
            nodetree.Top = FileTreeView.Top = 501;
            nodetree.Height = FileTreeView.Height = 307;
            pictureBox1.Visible = false;
            this.Height = 540;
            reminderList.Focus();
            //Center();
        }
        private void allFloderChanged(object sender, EventArgs e)
        {
            rootpath = new DirectoryInfo(ini.ReadString("path", "rootpath", ""));
            mindmapPath = rootpath.FullName;
            searchword.Text = "";
            Load_Click(null, null);
        }
        private void RsscheckBox_CheckedChanged(object sender, EventArgs e)
        {
        }
        private void Rsstimer_Tick(object sender, EventArgs e)
        {
            //if (rssrenewend)
            //{
            //    rssrenewend = false;
            //    Thread th = new Thread(() => RSSRenew());
            //    th.Start();
            //}
        }
        public string GetRSSURL(string url)
        {
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(url);
            foreach (XmlNode item in x.GetElementsByTagName("node"))
            {
                if (item.Attributes["ISURL"] != null)
                {
                    return item.Attributes["TEXT"].InnerText;
                }
            }
            return "";
        }
        #region 字符串，XML，List帮助方法
        public bool ISHasInDoc(string file, string str)
        {
            String strFile = File.ReadAllText(file);
            return strFile.Contains(yixiaozi.Model.DocearReminder.Helper.ConvertString(str));

        }
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        private void PlaySimpleSound(string type)
        {
            try
            {
                //SystemSounds.Exclamation.Play();
                //return;
                if (!isPlaySound || type == "")
                {
                    return;
                }
                string path = ini.ReadString("sound", "path", "");
                path += ini.ReadString("sound", type, "");
                //simpleSound.SoundLocation = path;
                ////MediaPlayer simpleSound = new MediaPlayer()
                ////{
                ////    Volume = 100.0f
                ////};
                ////simpleSound.Open(new Uri(new FileInfo(path).FullName));
                ////simpleSound.Play();
                //simpleSound.Load();

                //simpleSound.Play();

                player.SoundLocation = path;
                player.Play();
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        public bool StringHasArrALL(string str, string[] arr)
        {
            if (str == null || str == "")
            {
                return false;
            }
            str = str.ToLower();
            foreach (string item in arr)
            {
                if (item.ToLower().StartsWith("e"))
                {
                    if (item.ToLower().Trim() == "e")
                    {
                        return true;
                    }
                    if (str.Contains(item.ToLower().Trim().Substring(1)))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!str.ToLower().Contains(item.ToLower().Trim()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
        #region RSS
        public void RSSRenew()
        {
            DirectoryInfo path = new DirectoryInfo(ini.ReadString("path", "rss", ""));
            foreach (FileInfo file in path.GetFiles("*.mm", SearchOption.AllDirectories))
            {
                try
                {
                    string rss = GetRSSURL(file.FullName);
                    if (rss == "" || !IsUri(rss))
                    {
                        continue;
                    }
                    XmlDocument doc = new XmlDocument(); // 创建文档对象
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("user-agent", "MyRSSReader/1.0");
                    XmlReader readers = XmlReader.Create(webClient.OpenRead(rss));
                    try
                    {
                        doc.Load(readers);//加载XML 包括HTTP：// 和本地
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);//异常处理
                    }
                    string fileName = file.FullName;
                    XmlNodeList list = doc.GetElementsByTagName("item");  // 获得项           
                    System.Xml.XmlDocument x = new XmlDocument();
                    x.Load(fileName);
                    foreach (XmlNode node in list)  // 循环每一项
                    {
                        XmlElement ele = (XmlElement)node;
                        string title = ele.GetElementsByTagName("title")[0].InnerText;//获得标题
                        string link = ele.GetElementsByTagName("link")[0].InnerText;//获得联接
                        string description = ele.GetElementsByTagName("description")[0].InnerText;//获得联接
                        string guidurl = ele.GetElementsByTagName("guid").Count == 0 ? "" : ele.GetElementsByTagName("guid")[0].InnerText;//获得联接
                        if (guidurl != "" && ISHasInDoc(fileName, guidurl))
                        {
                            continue;
                        }
                        if (title != "" && ISHasInDoc(fileName, title))
                        {
                            continue;
                        }
                        if (link != "" && ISHasInDoc(fileName, link))
                        {
                            continue;
                        }
                        DateTime dt = DateTime.Now;
                        try
                        {
                            dt = Convert.ToDateTime(((System.Xml.XmlElement)ele.PreviousSibling).InnerText);
                        }
                        catch (Exception)
                        {
                        }
                        AddTaskToFile(x, "文章", title, link, description, guidurl, dt);
                    }
                    x.Save(fileName);
                    Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(fileName));
                    th.Start();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region 理财
        public void AddMoney(string mindmapPath, string account, string money, bool addOrDel = false, string content = "")
        {
            if (mindmapPath == "" || account == "" || money == "")
            {
                return;
            }
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(mindmapPath);
            XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value.Contains(account + "："));
            double lastMoney = Convert.ToDouble(root.Attributes["TEXT"].Value.Split('：')[1]);
            if (addOrDel)
            {
                lastMoney += Convert.ToDouble(money);
            }
            else
            {
                lastMoney -= Convert.ToDouble(money);
            }
            root.Attributes["TEXT"].Value = account + "：" + lastMoney;
            x.Save(mindmapPath);
            AddTaskToFile(mindmapPath, "记录", content + "|" + money + "|" + account, false);
            richTextSubNode.Text += account + "：" + lastMoney;
            richTextSubNode.Text += "\r\n";
        }
        public void showMoneyLeft(string mindmapPath, string name)
        {
            double money = 0;
            System.Xml.XmlDocument x = new XmlDocument();
            x.Load(mindmapPath);
            string nameaccount = ini.ReadString("money", name, "");
            foreach (string account in nameaccount.Split(';'))
            {
                XmlNode root = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value.Contains(account + "："));
                double lastMoney = Convert.ToDouble(root.Attributes["TEXT"].Value.Split('：')[1]);
                money += lastMoney;
            }
            XmlNode root1 = x.GetElementsByTagName("node").Cast<XmlNode>().First(m => m.Attributes[0].Name == "TEXT" && m.Attributes["TEXT"].Value.Contains(ini.ReadString("money", name + "Name", "") + "："));
            root1.Attributes["TEXT"].Value = name + "：" + money;
            x.Save(mindmapPath);
            richTextSubNode.Text += name + "：" + money;
            richTextSubNode.Text += "\r\n";
        }
        #endregion
        #region 鼠标控制
        #endregion
        void SuggestText_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                //if the item state is selected them change the back color
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e = new DrawItemEventArgs(e.Graphics,
                                              e.Font,
                                              e.Bounds,
                                              e.Index,
                                              e.State ^ DrawItemState.Selected,
                                              e.ForeColor,
                                              Color.LightGray);//Choose the colorYellow
                }

                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Draw the current item text
                e.Graphics.DrawString(((StationInfo)SearchText_suggest.Items[e.Index]).StationName_CN, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            //e.DrawFocusRectangle();
        }
        void Mindmaplist_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            //if the item state is selected them change the back color
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.LightGray);//Choose the colorYellow
            }

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(((StationInfo)SearchText_suggest.Items[e.Index]).StationName_CN, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            //e.DrawFocusRectangle();
        }
        private void tree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                if (nodetree.Focused)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Gray), e.Bounds);
                }
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.TreeView.Font, e.Node.Bounds, e.Node.ForeColor);
        }
        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
        private void SearchText_suggest_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextSubNode.Clear();
                if (SearchText_suggest.SelectedItem != null && ((StationInfo)SearchText_suggest.SelectedItem).mindmapurl != null)
                {
                    richTextSubNode.AppendText(((StationInfo)SearchText_suggest.SelectedItem).mindmapurl);
                }
            }
            catch (Exception)
            {
            }
        }

        private void ebcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ebcheckBox.Checked)
            {
                showcyclereminder.Checked = onlyZhouqi.Checked = IsReminderOnlyCheckBox.Checked = false;
            }
            else
            {
                showcyclereminder.Checked = false;
                onlyZhouqi.Checked = true;
                IsReminderOnlyCheckBox.Checked = false;
            }
        }

        #region 文件树，导图树
        private void FileTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Node.Name);
            SaveLog("打开：    " + e.Node.Name);
        }
        bool IsFileNodeEdit = false;

        private void FileTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string newTxt = e.Label;//获取新文本
            string oldTxt = e.Node.Text;//获取原来的文本 
            if (newTxt != null && newTxt != oldTxt)
            {
                FileInfo fi = new FileInfo(FileTreeView.SelectedNode.Name);
                //新路径这样判断可能出错
                //fi.MoveTo(FileTreeView.SelectedNode.Name.Replace(id,newTxt));
                fi.MoveTo(fi.Directory.FullName + "\\" + newTxt);
                FileTreeView.SelectedNode.Name = fi.Directory.FullName + "\\" + newTxt;
                //todo需改导图里的地址
            }
        }
        bool IsMindMapNodeEdit = false;
        private void TreeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string newTxt = e.Label;//获取新文本
            string oldTxt = e.Node.Text;//获取原来的文本 
            if (newTxt != null && newTxt != oldTxt)
            {
                RenameNodeByID(newTxt);
                SaveLog("修改节点名称：" + oldTxt + "  To  " + newTxt);
                return;
            }
        }

        private void TreeView1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    IsMindMapNodeEdit = true;
                    isRename = true;
                    renameTaskName = nodetree.SelectedNode.Text;
                    renameMindMapFileID = ((XmlAttribute)(nodetree.SelectedNode.Tag)).Value;
                    {
                        nodetree.SelectedNode.BeginEdit();
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PathcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!selectedpath)
            {
                selectedpath = true;
                return;
            }
            try
            {
                if (PathcomboBox.SelectedItem.ToString() == "all")
                {
                    allFloder = true;
                    rootpath = new DirectoryInfo(ini.ReadString("path", "rootPath", ""));
                }
                else
                {
                    allFloder = false;
                    rootpath = new DirectoryInfo(ini.ReadString("path", PathcomboBox.SelectedItem.ToString(), ""));
                }
                mindmapPath = rootpath.FullName;
                searchword.Text = "";
                PlaySimpleSound("changepath");
                Load_Click(null, null);
                reminderList.Focus();
                Center();
            }
            catch (Exception)
            {
            }
        }
        #region 剪切板
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        //public static string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string clipordFilePath = System.AppDomain.CurrentDomain.BaseDirectory;

        IntPtr nextClipboardViewer;
        private System.Threading.Timer reminder;
        int mouseDisplacement = 20;
        public string log;
        public int hour = 0;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        List<string> logHistory = new List<string>();
        void DisplayClipboardData()
        {
            try
            {
                IDataObject iData = new DataObject();
                iData = Clipboard.GetDataObject();
                if (iData.GetDataPresent(DataFormats.Rtf))
                {
                    log = (string)iData.GetData(DataFormats.Text);
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    log = (string)iData.GetData(DataFormats.Text);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    foreach (string item in ConvertStringCollection(Clipboard.GetFileDropList()))
                    {
                        log += (item + "\r");
                        if (System.IO.Directory.Exists(item))
                        {
                            log = LoadTree(item, log, 1);
                        }
                    }
                }
                else if (Clipboard.ContainsImage())
                {
                    System.IO.Directory.CreateDirectory(clipordFilePath + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + "images");
                    Clipboard.GetImage().Save((clipordFilePath + "\\" + DateTime.Now.Year + "\\" + DateTime.Now.Month + "\\" + "images" + "\\" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".jpg").Replace(@"\\", @"\"));
                }
                if (log.Length > 10)
                {
                    if (!logHistory.Contains(log))
                    {
                        SaveLogClip(log);
                        logHistory.Add(log);
                        if (logHistory.Count > 10)
                        {
                            logHistory.RemoveAt(0);
                        }
                    }
                }
                log = "";
            }
            catch (Exception)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        public static List<string> ConvertStringCollection(StringCollection collection)
        {
            List<string> list = new List<string>();
            foreach (string item in collection)
            {
                list.Add(item);
            }
            return list;
        }
        public void SaveLogClip(string log)
        {
            System.IO.Directory.CreateDirectory(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\");
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\" + DateTime.Now.Day.ToString() + ".txt", true))
            {
                if (log != "")
                {
                    file.Write(DateTime.Now + "   ");
                    file.WriteLine(log);
                    //file.Write("\r");
                }
            }
        }

        private void btn_OpenFile_MouseClick()
        {
            if (System.IO.File.Exists(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\" + DateTime.Now.Day.ToString() + ".txt"))
            {
                System.Diagnostics.Process.Start(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\" + DateTime.Now.Day.ToString() + ".txt");
            }
        }

        private void Btn_OpenFolder_Click()
        {
            System.Diagnostics.Process.Start(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AutoRun_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取程序执行路径..
            string starupPath = Application.ExecutablePath;
            //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
            RegistryKey loca = Registry.LocalMachine;
            try
            {
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                //SetValue:存储值的名称
                run.SetValue("DocearReminder", starupPath);
                MessageBox.Show("已启用开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loca.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("设置开机启动请使用管理员权限打开本软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisAutoRun_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string starupPath = Application.ExecutablePath;
            RegistryKey loca = Registry.LocalMachine;
            try
            {
                //SetValue:存储值的名称
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                run.DeleteValue("DocearReminder");
                MessageBox.Show("已停止开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loca.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("取消开机启动请使用管理员权限打开本软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string LoadTree(string path, string str, int f)
        {
            string __ = "";
            string str1 = "";
            for (int i = 0; i < f * 2; i++)
            {
                __ += "-";
            }

            str1 += (__ + path + "\r\n");
            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                str1 += (__.Replace('-', ' ').Replace('I', ' ') + file.Split('\\')[file.Split('\\').Length - 1] + "\r\n");
            }
            string[] dirs = Directory.GetDirectories(path);//获取子目录
            if (dirs.Length > 0)
            {
                f += 1;
                foreach (string dir in dirs)
                {
                    str1 = LoadTree(dir, str1, f);
                }
            }
            return str + str1 + "\r\n";
        }

        private void reminder_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Minute < 2)
            {
                Type shellType = Type.GetTypeFromProgID("Shell.Application");
                object shellObject = System.Activator.CreateInstance(shellType);
                shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod, null, shellObject, null);
            }
        }

        public void SearchbuttonClick()
        {
            System.Threading.Thread th = new System.Threading.Thread(() => OpenSearch());
            th.Start();
        }

        public void OpenSearch()
        {
            MyHide();
            Search searchform = new Search();
            searchform.ShowDialog();
        }

        public struct Point
        {
            public int p1;
            public int p2;

            public Point(int p1, int p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        private void HookManager_KeyDown_saveKeyBoard(IntPtr e)
        {
            //记录键盘键
            System.IO.Directory.CreateDirectory(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\");
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(clipordFilePath + "\\\\" + DateTime.Now.Year + "\\\\" + DateTime.Now.Month + "\\\\key.txt", true))
            {
                if (DateTime.Now.Hour != hour)
                {
                    hour = DateTime.Now.Hour;
                    file.Write("\r");
                    file.Write(DateTime.Now);
                }
                //char* pChar = reinterpret_cast<char*>(e.ToPointer());
                if (e != null)
                {
                    //file.Write(e.ToInt32().ToString()+"|");
                }
            }
        }
        /// <summary>   
        /// 设置鼠标的坐标   
        /// </summary>   
        /// <param name="x">横坐标</param>   
        /// <param name="y">纵坐标</param>   
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,
            byte bScan,
            int dwFlags,  //这里是整数类型  0 为按下，2为释放  
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0  
        );
        /// <summary>   
        /// 获取鼠标的坐标   
        /// </summary>   
        /// <param name="lpPoint">传址参数，坐标point类型</param>   
        /// <returns>获取成功返回真</returns>   
        [DllImport("User32")]
        public extern static bool GetCursorPos(ref Point lpPoint);
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; // press left mouse button
        const int MOUSEEVENTF_LEFTUP = 0x0004; // release left mouse button
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; // whole screen, not just application window
        const int MOUSEEVENTF_MOVE = 0x0001; // move mouse
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_WHEEL = 0x800;

        private void niazhi_CheckedChanged(object sender, EventArgs e)
        {
            if (true)
            {
                keybd_event(18, 0, 0, 0);

            }
            else
            {
                //keybd_event(18, 0, 0x2, 0);
            }
        }

        #endregion

        private void Reminderlist_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int posindex = reminderList.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));
                reminderList.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < reminderList.Items.Count)
                {
                    reminderList.SelectedIndex = posindex;
                    menu_reminderlist.Show(reminderList, new System.Drawing.Point(e.X, e.Y));
                }
                else
                {
                    IsReminderOnlyCheckBox.Checked = !IsReminderOnlyCheckBox.Checked;
                    RRReminderlist();
                    reminderList.Refresh();
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                int posindex = reminderList.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));
                reminderList.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < reminderList.Items.Count)
                {
                    //reminderList.SelectedIndex = posindex;
                    //menu_reminderlist.Show(reminderList, new System.Drawing.Point(e.X, e.Y));
                }
                else
                {
                    if (IsReminderOnlyCheckBox.Checked)
                    {
                        IsReminderOnlyCheckBox.Checked = !IsReminderOnlyCheckBox.Checked;
                        RRReminderlist();
                    }
                    else
                    {
                        showcyclereminder.Checked = !showcyclereminder.Checked;
                        RRReminderlist();
                    }
                    reminderList.Refresh();
                }
            }
        }
        private void mindmaplist_MouseDown(object sender, MouseEventArgs e)
        {
            IsSelectReminder = false;
            if (e.Button == MouseButtons.Right)
            {
                int posindex = mindmaplist.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));
                mindmaplist.ContextMenuStrip = null;
                if (posindex >= 0 && posindex < mindmaplist.Items.Count)
                {
                    mindmaplist.SelectedIndex = posindex;
                    menu_mindmaps.Show(mindmaplist, new System.Drawing.Point(e.X, e.Y));
                }
            }
            mindmaplist.Refresh();
        }

        private void nodetree_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void nodetree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                System.Drawing.Point ClickPoint = new System.Drawing.Point(e.X, e.Y);
                TreeNode CurrentNode = nodetree.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    CurrentNode.ContextMenuStrip = menu_nodetree;
                    //name = nodetree.SelectedNode.Text.ToString();//存储节点的文本
                    nodetree.SelectedNode = CurrentNode;//选中这个节点
                }
            }
        }

        private void menu_filetree_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void DocearReminderForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                int posindex = mindmaplist.IndexFromPoint(new System.Drawing.Point(e.X, e.Y));
                menu.Show(this, new System.Drawing.Point(e.X, e.Y));
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            taskComplete_btn_Click(null, null);
            PlaySimpleSound("Done");
        }
        public void GetIniFile()
        {
            suggestListData.Clear();
            #region 这样读取ini所有节点有问题
            //IniFile ini1 = new IniFile(ini.ReadStringDefault("path", Environment.MachineName, ""));
            //List<string> list = ini1.ReadSections();
            //foreach (string item in list)
            //{
            //    if (item.StartsWith("Btn")&&list2.FindAll(m=>m.StationName_CN == ini1.ReadStringDefault(item, "Name", "")).Count()==0)
            //    {
            //        string file = ini1.ReadStringDefault(item, "File", "");
            //        if (file != "")
            //        {
            //            string name = ini1.ReadStringDefault(item, "Name", "");
            //            string proString = "";
            //            if (System.IO.File.Exists(file))
            //            {
            //                proString = "f:";
            //            }
            //            else if (System.IO.Directory.Exists(file))
            //            {
            //                proString = "F:";
            //            }
            //            else if (isURL(file))
            //            {
            //                proString = "B:";
            //            }
            //            else
            //            {
            //            }
            //            list2.Add(new StationInfo { mindmapurl = file, StationName_CN = "f:"+name });
            //        }
            //    }
            //} 
            #endregion
            getinifilenode();
            //处理bookmarks
            GetBookmarksLinks();
            GetFolderToSuggest();
            GetAllLinksToSuggest();
        }
        public void GetAllLinksToSuggest()
        {
            try
            {
                List<string> links = new List<string>();
                links = new TextListConverter().ReadTextFileToList(System.AppDomain.CurrentDomain.BaseDirectory + @"\alllinks.txt");
                foreach (string item in links)
                {
                    if (!item.Contains("|"))
                    {
                        continue;
                    }
                    suggestListData.Add(new StationInfo { StationName_CN = "alllinks:" + item.Split('|')[0], mindmapurl = item.Split('|')[1], isNode = "file" });
                }
            }
            catch (Exception)
            {
            }
        }

        private void GetFolderToSuggest()
        {
            string pathArr = ini.ReadStringDefault("path", Environment.MachineName + "Folders", "");
            foreach (string item in pathArr.Split(';'))
            {
                string path = ini.ReadStringDefault("path", item, "");
                foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories))
                {
                    if (file.FullName.Contains(".svn") || file.FullName.Contains(".vs") || file.FullName.Contains(".git") || file.FullName.ToLower().Contains("backup"))
                    {
                        continue;
                    }
                    suggestListData.Add(new StationInfo { StationName_CN = item + ":" + file.Name, mindmapurl = file.FullName, isNode = "file" });
                }
            }
        }

        public void getinifilenode()
        {
            try
            {
                const Int32 BufferSize = 128;
                string path = ini.ReadStringDefault("path", Environment.MachineName + "CLaunchIni", "");
                using (var fileStream = File.OpenRead(path))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        string Name = "";
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            if (line.Trim() == "")
                            {
                                continue;
                            }
                            if (line.ToUpper().StartsWith("NAME"))
                            {
                                Name = line.Substring(5);
                            }
                            if (line.ToUpper().StartsWith("FILE") && Name != "")
                            {
                                if (line.Contains("\\") || line.Contains("/"))
                                {
                                    suggestListData.Add(new StationInfo { StationName_CN = "CLaunch:" + Name, mindmapurl = line.Substring(5) });
                                }
                                Name = "";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void GetBookmarksLinks()
        {
            try
            {
                string bookmarksmindmap = ini.ReadString("path", "bookmarks", "");
                System.Xml.XmlDocument x = new XmlDocument();
                x.Load(bookmarksmindmap);
                bool isNeedUpdate = false;
                foreach (XmlNode node in x.GetElementsByTagName("node"))
                {
                    try
                    {
                        if (node.Attributes != null && node.Attributes["TEXT"] != null && IsURL(node.Attributes["TEXT"].Value) && node.Attributes["LINK"] == null)
                        {
                            XmlAttribute LINK = x.CreateAttribute("LINK");
                            LINK.Value = node.Attributes["TEXT"].Value;
                            node.Attributes["TEXT"].Value = yixiaozi.Net.HttpHelp.Web.getTitle(node.Attributes["TEXT"].Value);
                            node.Attributes.Append(LINK);
                        }
                        if (node.Attributes != null && node.Attributes["TEXT"] != null && node.Attributes["LINK"] != null)
                        {
                            suggestListData.Add(new StationInfo { mindmapurl = node.Attributes["LINK"].Value, StationName_CN = "bookmarks:" + node.Attributes["TEXT"].Value });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (isNeedUpdate)
                {
                    x.Save(bookmarksmindmap);
                    yixiaozi.Model.DocearReminder.Helper.ConvertFile(bookmarksmindmap);
                }
            }
            catch (Exception)
            {
            }
        }

        private void FileTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//判断你点的是不是右键
            {
                System.Drawing.Point ClickPoint = new System.Drawing.Point(e.X, e.Y);
                TreeNode CurrentNode = FileTreeView.GetNodeAt(ClickPoint);
                if (CurrentNode != null)//判断你点的是不是一个节点
                {
                    CurrentNode.ContextMenuStrip = menu_filetree;
                    //name = nodetree.SelectedNode.Text.ToString();//存储节点的文本
                    FileTreeView.SelectedNode = CurrentNode;//选中这个节点
                }
            }
        }

        private void ReminderListBox_SizeChanged(object sender, EventArgs e)
        {
            if (reminderListBox.Items.Count > 0)
            {
                reminderList.Top = reminderListBox.Top + reminderListBox.Height + (reminderListBox.Height == 0 ? 0 : 10);
                reminderList.Height = mindmaplist.Height - reminderListBox.Height - (reminderListBox.Height == 0 ? 0 : 10);//- 51
            }
            else
            {
                reminderList.Top = mindmaplist.Top;
                reminderList.Height = mindmaplist.Height;
            }
        }

        private void ReminderListBox_DataSourceChanged(object sender, EventArgs e)
        {

        }
        public void Reminderlistboxchange()
        {
            if (reminderListBox.Items.Count > 0)
            {
                reminderListBox.Height = reminderListBox.PreferredHeight;
                reminderListBox.Visible = true;
            }
            else
            {
                reminderListBox.Height = 0;
                reminderListBox.Visible = false;
                reminderList.Top = mindmaplist.Top;
                reminderList.Height = mindmaplist.Height;
                reminderList.Focus();
            }
        }

        private void ReminderlistBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                int zhongyao = 0;
                string name = "";
                zhongyao = ((MyListBoxItemRemind)reminderListBox.Items[e.Index]).level;
                name = ((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Name;
                System.Drawing.Brush mybsh = Brushes.Gray;
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Height, e.Bounds.Height);
                Rectangle rectleft = new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height);
                if (zhongyao == 0)
                {
                    SolidBrush zeroColor = new SolidBrush(Color.FromArgb(238, 238, 242));
                    if (searchword.Text.StartsWith("#") || searchword.Text.StartsWith("*"))
                    {
                        zeroColor = new SolidBrush(Color.White);
                    }
                    e.Graphics.FillRectangle(zeroColor, rect);
                    mybsh = new SolidBrush(Color.FromArgb(238, 238, 242));
                    if (name == "当前时间")
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(238, 238, 242)), e.Bounds);
                        mybsh = Brushes.Gray;
                    }
                }
                else if (zhongyao == 1)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Azure), rect);
                    mybsh = new SolidBrush(Color.Azure);
                }
                else if (zhongyao == 2)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.PowderBlue), rect);
                    mybsh = new SolidBrush(Color.PowderBlue);
                }
                else if (zhongyao == 3)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightSkyBlue), rect);
                    mybsh = new SolidBrush(Color.LightSkyBlue);
                }
                else if (zhongyao == 4)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.DeepSkyBlue), rect);
                    mybsh = new SolidBrush(Color.DeepSkyBlue);
                }
                else if (zhongyao == 5)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.CadetBlue), rect);
                    mybsh = new SolidBrush(Color.CadetBlue);
                }
                else if (zhongyao == 6)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Gold), rect);
                    mybsh = new SolidBrush(Color.Gold);
                }
                else if (zhongyao == 7)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Orange), rect);
                    mybsh = new SolidBrush(Color.Orange);
                }
                else if (zhongyao == 8)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.OrangeRed), rect);
                    mybsh = new SolidBrush(Color.OrangeRed);
                }
                else if (zhongyao == 9)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Crimson), rect);
                    mybsh = new SolidBrush(Color.Crimson);
                }
                else if (zhongyao >= 10)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Red), rect);
                    mybsh = new SolidBrush(Color.Red);
                }
                if (e.Index == reminderListBox.SelectedIndex)
                {
                    e.Graphics.FillRectangle(mybsh, rect);
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), rectleft); //Yellow
                }
                if (searchword.Text.StartsWith("#"))
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    //e.DrawFocusRectangle();
                }
                else if (searchword.Text.StartsWith("*"))
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    //e.DrawFocusRectangle();
                }
                else if (!((MyListBoxItemRemind)reminderListBox.Items[e.Index]).isTask)
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Text, e.Font, Brushes.Gray, e.Bounds, StringFormat.GenericDefault);
                    //e.DrawFocusRectangle();
                }
                else
                {
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Text.Substring(0, 3), e.Font, mybsh, rect, StringFormat.GenericDefault);
                    e.Graphics.DrawString(((MyListBoxItemRemind)reminderListBox.Items[e.Index]).Text.Substring(3), e.Font, Brushes.Gray, rectleft, StringFormat.GenericDefault);
                    ((MyListBoxItemRemind)reminderListBox.Items[e.Index]).IsShow = true;
                    //e.DrawFocusRectangle();
                }
            }
        }

        private void richTextSubNode_SizeChanged(object sender, EventArgs e)
        {
            tagCloudControl.Top = richTextSubNode.Top + richTextSubNode.Height + (richTextSubNode.Height != 0 ? 14 : 0);
            tagCloudControl.Height = 475 - tagCloudControl.Top;
        }

        private void RichTextSubNode_TextChanged(object sender, EventArgs e)
        {
            if (richTextSubNode.Text.Trim() == "")
            {
                richTextSubNode.Height = 0;
                return;
            }
            //this.richTextSubNode.Text = Regex.Replace(this.richTextSubNode.Text, @"(?s)\n\s*\n", "\n");
            int EM_GETLINECOUNT = 0x00BA;//获取总行数的消息号 
            int lc = SendMessage(this.richTextSubNode.Handle, EM_GETLINECOUNT, IntPtr.Zero, IntPtr.Zero);
            int sf = this.richTextSubNode.Font.Height * (lc + 1);
            this.richTextSubNode.Height = sf;
            //richTextSubNode.Height = e.NewRectangle.Height + 10;
            //richTextSubNode.Height = richTextSubNode.Lines.Length * 13;
        }

        private void dateTimePicker_KeyUp(object sender, KeyEventArgs e)
        {

        }
        bool isnottagcloudonload = true;
        private void tagCloudControl_ControlAdded(object sender, ControlEventArgs e)
        {
            //if (isnottagcloudonload)
            //{
            //    saveTagFile();
            //}
        }

        private void TagCloudControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            //if (isnottagcloudonload)
            //{
            //    saveTagFile();
            //}
        }
        public void WriteTagFile()
        {
            //FileInfo fi = new FileInfo(@"tagcloud.json");
            //JavaScriptSerializer js = new JavaScriptSerializer
            //{
            //    MaxJsonLength = Int32.MaxValue
            //};
            //string json = js.Serialize(tagCloudControl.GetAllItems());
            //File.WriteAllText(@"tagcloud.json", "");
            //using (StreamWriter sw = fi.AppendText())
            //{
            //    sw.Write(json);
            //}
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + @"tagcloud.xml";
            bool b = tagCloudControl.WriteTagFile(filename);
        }
        public void ReadTagFile()
        {
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + @"tagcloud.xml";
            bool b = tagCloudControl.ReadTagFile(filename);
        }

        private void ReadBookmarks()
        {
            try
            {
                string chromeDistribution = ini.ReadString("path", "chromeDistribution", "");
                foreach (string item in chromeDistribution.Split(';'))
                {
                    try
                    {
                        string path = ini.ReadString("path", item, "");
                        string ChromeDatePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + path;
                        string ChromeBookMarksPath = ChromeDatePath + @"\Bookmarks";
                        if (File.Exists(ChromeBookMarksPath))
                        {
                            ConvertbookmarketToini(GetChromeBookmarksData(ChromeBookMarksPath).roots.bookmark_bar, item);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void ConvertbookmarketToini(datameta bookmarks, string preStr)
        {
            if (bookmarks.type == "url")
            {
                suggestListData.Add(new StationInfo { StationName_CN = preStr + (preStr != "" ? ":" : "") + bookmarks.name, mindmapurl = bookmarks.url });
            }
            if (bookmarks.children != null)
            {
                foreach (datameta item in bookmarks.children)
                {
                    ConvertbookmarketToini(item, preStr);
                }
            }
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static string ListToJson<T>(T data)
        {
            string str = string.Empty;
            try
            {
                if (null != data)
                {
                    str = JsonConvert.SerializeObject(data);
                }
            }
            catch (Exception)
            {

            }
            return str;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="jsonstr">数据</param>
        /// <returns></returns>
        public static Object JsonToList<T>(string jsonstr)
        {
            Object obj = null;
            try
            {
                if (null != jsonstr)
                {
                    obj = JsonConvert.DeserializeObject<T>(jsonstr);//反序列化
                }
            }
            catch (Exception)
            {

            }
            return obj;
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileRead(string filePath)
        {
            string rel = File.ReadAllText(filePath);
            return rel;
        }

        /// <summary>
        /// 获取Chrome浏览器书签对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ChromeBookmarks GetChromeBookmarksData(string filePath)
        {
            string str = FileRead(filePath);
            object chromeBookmarks = JsonToList<ChromeBookmarks>(str);
            if (chromeBookmarks != null)
            {
                return (ChromeBookmarks)chromeBookmarks;
            }
            return null;
        }

        private void panel_clearSearchWord_Click(object sender, EventArgs e)
        {

        }

        private void noterichTextBox_TextChanged(object sender, EventArgs e)
        {
            noterichTextBox.ForeColor = Color.Gray;
            try
            {
                noterichTextBox.SaveFile(ini.ReadString("path:", "note", System.AppDomain.CurrentDomain.BaseDirectory + @"note.txt"));
            }
            catch (Exception)
            {
            }
        }
        #region 右键菜单动作

        
        private void autoRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取程序执行路径..
            string starupPath = Application.ExecutablePath;
            //class Micosoft.Win32.RegistryKey. 表示Window注册表中项级节点,此类是注册表装.
            RegistryKey loca = Registry.LocalMachine;
            try
            {
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                //SetValue:存储值的名称
                run.SetValue("DocearReminder", starupPath);
                MessageBox.Show("已启用开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loca.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("设置开机启动请使用管理员权限打开本软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void disAutoRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string starupPath = Application.ExecutablePath;
            RegistryKey loca = Registry.LocalMachine;
            try
            {
                //SetValue:存储值的名称
                RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                run.DeleteValue("DocearReminder");
                MessageBox.Show("已停止开机运行!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loca.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("取消开机启动请使用管理员权限打开本软件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolStripMenuItem_deny_Click(object sender, EventArgs e)
        {
            DelaySelectedTask();
        }

        private void toolStripMenuItemCalcal_Click(object sender, EventArgs e)
        {
            cancel_btn_Click(null, null);
        }

        private void 打开所在目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new FileInfo(((MyListBoxItemRemind)reminderlistSelectedItem).Value).DirectoryName);
                MyHide();
            }
            catch (Exception)
            {
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            if (nodetree.SelectedNode.Tag != null)
            {
                try
                {
                    deleteNodeByID(((XmlAttribute)(nodetree.SelectedNode.Tag)).Value);
                    SaveLog("删除节点：" + nodetree.SelectedNode.Text + "    导图" + showMindmapName.Split('\\')[showMindmapName.Split('\\').Length - 1]);
                    fenshuADD(1);
                    Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(showMindmapName));
                    th.Start();
                    ShowMindmap();
                }
                catch (Exception)
                {
                }
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible == false)
            {
                this.Visible = true;
                //this.notifyIcon1.Visible = false;
                Center();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                this.Visible = true;
                //this.notifyIcon1.Visible = false;
                Center();
            }
        }

        private void 剪切板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchword.Text = "";
            MyHide();
            OpenSearch();
        }

        private void Searchword_MouseDown(object sender, MouseEventArgs e)
        {
            //searchworkmenu.Show();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new FileInfo(((MyListBoxItem)mindmaplist.SelectedItem).Value).DirectoryName);
                MyHide();
            }
            catch (Exception)
            {
            }
        }

        private void 打开程序目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private void 显示右侧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Width == 1180)
            {
                this.Width = 888;
            }
            else
            {
                this.Width = 1180;
            }
            Center();
        }

        private void 是否锁定窗口lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lockForm = !lockForm;
        }

        private void 操作记录F12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyHide();
            Log log = new Log();
            log.ShowDialog();
        }

        private void 剪切板文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchword.Text = "";
            MyHide();
            btn_OpenFile_MouseClick();
        }

        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchword.Text = "";
            MyHide();
            Btn_OpenFolder_Click();
        }

        private void 日历QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread thCalendarForm = new Thread(() => Application.Run(new Calendar.CalendarForm(mindmapPath)));
            thCalendarForm.Start();
            MyHide();
        }

        private void 工具箱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hiddenmenu_DoubleClick(null, null);
        }

        private void 查看模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsViewModel.Checked = !IsViewModel.Checked;
            if (!IsViewModel.Checked)
            {
                shaixuanfuwei();
                RRReminderlist();
                reminderList.Focus();
            }
            else
            {
                reminderList.SelectedIndex = -1;
                reminderSelectIndex = -1;
                IsSelectReminder = false;
                mindmaplist.Focus();
            }
        }

        private void 透明度ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void o05ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void o08ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.8;
        }

        private void o1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void 显示树视图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMindmap();
            ShowMindmapFile();
            nodetree.Visible = FileTreeView.Visible = true;
            this.Height = 860;
            nodetree.Focus();
        }

        private void 隐藏树视图SnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nodetree.Top = FileTreeView.Top = 501;
            nodetree.Height = FileTreeView.Height = 307;
            pictureBox1.Visible = false;
            nodetree.Visible = FileTreeView.Visible = false;
            this.Height = 540;
            reminderList.Focus();
        }

        private void 是否播放声音playsoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isPlaySound = !isPlaySound;
        }

        private void 推出F11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 仅查看CdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int reminderIndex = reminderList.SelectedIndex;
            SetTaskIsView();
            try
            {
                reminderList.Items.RemoveAt(reminderIndex);
                RRReminderlist();
                reminderList.SelectedIndex = reminderIndex;
            }
            catch (Exception)
            {

            }
        }

        private void 非重要ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //提醒任务
            int selectindex = reminderList.SelectedIndex;
            SetReminderOnly((MyListBoxItemRemind)reminderlistSelectedItem);
            RRReminderlist();
            if (selectindex < reminderList.Items.Count - 1)
            {
                reminderList.SelectedIndex = selectindex;
            }
        }

        private void 设置重要xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (reminderList.Focused)
            {
                reminderListBox.Items.Add((MyListBoxItemRemind)reminderlistSelectedItem);
                reminderboxList.Add((MyListBoxItemRemind)reminderlistSelectedItem);
                Reminderlistboxchange();
                reminderList.Items.RemoveAt(reminderList.SelectedIndex);
            }
            else if (reminderListBox.Focused)
            {
                reminderboxList.Remove((MyListBoxItemRemind)reminderListBox.SelectedItem);
                reminderListBox.Items.RemoveAt(reminderListBox.SelectedIndex);
                Reminderlistboxchange();
                if (reminderListBox.Items.Count == 0)
                {
                    reminderList.Focus();
                    reminderList.SelectedIndex = 0;
                }
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(((MyListBoxItemRemind)reminderlistSelectedItem).Value);
                MyHide();
            }
            catch (Exception)
            {
            }
        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon1_Click_1(object sender, EventArgs e)
        {
            //MyShow();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = !this.Visible;
                this.WindowState = FormWindowState.Normal;
                MyShow();
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void labeltaskinfo_Click(object sender, EventArgs e)
        {
            PlaySimpleSound("treeview");
            if (this.Height == 860)
            {
                nodetree.Top = FileTreeView.Top = 501;
                nodetree.Height = FileTreeView.Height = 307;
                pictureBox1.Visible = false;
                nodetree.Visible = FileTreeView.Visible = false;
                this.Height = 540;
                reminderList.Focus();
            }
            else
            {
                ShowMindmap();
                ShowMindmapFile();
                nodetree.Visible = FileTreeView.Visible = true;
                this.Height = 860;
                nodetree.Focus();
            }
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {

        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {

        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            try
            {
                SetTaskNodeByID(((XmlAttribute)(nodetree.SelectedNode.Tag)).Value);
                SaveLog("设置节点为任务：" + nodetree.SelectedNode.Text + "    导图" + showMindmapName.Split('\\')[showMindmapName.Split('\\').Length - 1]);
                Thread th = new Thread(() => yixiaozi.Model.DocearReminder.Helper.ConvertFile(showMindmapName));
                th.Start();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Click(object sender, EventArgs e)
        {
        }

        private void dateTimePicker_MouseLeave(object sender, EventArgs e)
        {
            //EditTime_Clic(null, null);
        }

        private void EditTaskTime_Click(object sender, EventArgs e)
        {
            EditTime_Clic(null, null);
        }
    }
}