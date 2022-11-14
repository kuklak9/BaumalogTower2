using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BTPDataBase;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BTPControlLibrary;
using BTPTwinCatADS;
using BTPUtilities;
using System.Windows.Media.Animation;

namespace BTPControlLibrary
{
    public class MainWindow : Window
    {
        public static BTPTwinCatADS.PLC _plc = null;
        public static BTPDataBase.DB_WMS_3 _dbWMS = null;
        public static BTPConfig.Configuration _cfg = null;
        public static BTPCLInput.Keyboard _keyboard;
        public static BTPCLInput.NumericKeyboard _numericKeyboard;
        public static BTPLogs.Logs _log = null;
        public static BTPLogs.EventErrorLogger _eventErrorLogger = null;
        public static BTPLogin.LoggedUser _loggedUser = null;

        #region translation
        public static BTPTranslation.Translation _translation = null;
        protected String _language = "pl";

        //public void ChangeLanguage(String language)
        //{
        //    _language = language;

        //    if (IsControlMultipleTransalated) //Jeżeli okno posiada kilka różnych tłumaczeń
        //        _translation.TranslateControl(language, this, ParentWindowControlPath, PrefixForSetWindowControls);
        //    else
        //        _translation.TranslateControl(language, this);

        //    try
        //    {
        //        InternalChangeLanguage(language);

        //        foreach (DataGrid dgv in _autoTransalateDGVHeaders)
        //        {
        //            AutoDGVHeaders_LanguageChanged(dgv);
        //        }

        //        AutoChangeLanguageForLocalMp(language);
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        protected String GetTranslatedText(String key, int value)
        {
            return _translation == null ? null : _translation.GetTraslatedText(_language, this, key, value);
        }
        protected String[] GetAlarmDescription(String source, int alarm, String language)
        {
            return _translation == null ? null : _translation.GetAlarmDescriptionFromDB(source, alarm, language);
        }
        protected virtual void InternalChangeLanguage(String language) { }

        //protected void TranslationCreateFiles()
        //{
        //    return;
        //    BTPTranslation.Translation.SaveTranslation(this, "lang\\default\\");
        //}

        #endregion translation

        public static void InitFirst(BTPCLInput.Keyboard kb,
            BTPCLInput.NumericKeyboard nkb,
            DB_WMS_3 db,
            BTPConfig.Configuration cfg,
            BTPLogs.Logs log,
            BTPLogin.LoggedUser lu,
            BTPTwinCatADS.PLC plc,
            BTPTranslation.Translation translation,
            BTPLogs.EventErrorLogger errorLogger)
        {
            _dbWMS = db;
            _cfg = cfg;
            _keyboard = kb;
            _numericKeyboard = nkb;
            _log = log;
            _loggedUser = lu;
            _plc = plc;
            _eventErrorLogger = errorLogger;
            _translation = translation;
        }

        public void Init()
        {
            InternalInit();
        }
        //protected void CheckControlPositionInsideParent(Control c)
        //{
        //    this.SuspendLayout();
        //    if (c.Top + c.Height > c.Parent.Height)
        //        c.Top = c.Parent.Height - c.Height;
        //    if (c.Top < 0)
        //        c.Top = 0;
        //    if (c.Left + c.Width > c.Parent.Width)
        //        c.Left = c.Parent.Width - c.Width;
        //    if (c.Left < 0)
        //        c.Left = 0;
        //    this.ResumeLayout();

        //}
        public void AutoInputInterface(string name, object value)
        {
            try
            {
                InputInterface(name, value);
            }
            catch (Exception ex)
            {
                _eventErrorLogger.AddLog(
                    BTPLogs.ErrorType.InputInterface,
                    new string[]
                    {
                        this.GetType().FullName.Replace("+", "."),
                        name,
                        value.ToString(),
                        ex.Message
                    });
            }
        }
        public string ActualLanguageSelected => _language;

        public bool AllowChangePanel
        {
            get
            {
                return _allowChangePanel;
            }
        }

        protected bool _wasInternalInit = false;
        protected bool _allowChangePanel = true;
        public bool AllowTrayMove = true;
        public int ActualTrayInWindow = 0; // table

        /// <summary>
        /// Z PLC na podstawie zmiennych + bez try..catch
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public virtual void InputInterface(String name, Object value) { }

        protected virtual void InternalInit() { }

        protected void HideKeyboards()
        {
            try
            {
                _numericKeyboard.Hide();
            }
            catch { }

            try
            {
                _keyboard.Hide();
            }
            catch { }
        }

        public void ChangeKeyboard(BTPCLInput.Keyboard kb, BTPCLInput.NumericKeyboard nkb)
        {
            _keyboard = kb;
            _numericKeyboard = nkb;

            try
            {
                InternalChangeKeyboard();
            }
            catch { }
        }

        protected virtual void InternalChangeKeyboard()
        { }

        //public void Init(BTPCLInput.Keyboard kb,
        //    BTPCLInput.NumericKeyboard nkb,
        //    BTPDataBase.DB_WMS_3 db,
        //    BTPConfig.Configuration cfg,
        //    BTPLogs.Logs log,
        //    BTPLogin.LoggedUser lu,
        //    BTPTwinCatADS.PLC plc,
        //    BTPTranslation.Translation translation,
        //    BTPLogs.EventErrorLogger errorLogger)
        //{
        //    if (_wasInternalInit)
        //        return;


        //    _dbWMS = db;
        //    _cfg = cfg;
        //    _keyboard = kb;
        //    _numericKeyboard = nkb;
        //    _log = log;
        //    _loggedUser = lu;
        //    _plc = plc;
        //    _eventErrorLogger = errorLogger;

        //    if (translation == null)
        //        _translation = new BTPTranslation.Translation(_cfg);
        //    else
        //        _translation = translation;

        //    try
        //    {
        //        //ControlDrillDownEventAdd(this);
        //        //Auto_InternalInit_Drill(this);
        //    }
        //    catch { }

        //    try
        //    {
        //        InternalInit();
        //    }
        //    catch { }
        //    _wasInternalInit = true;
        //}


        protected void tb_clear_on_Enter(object sender, EventArgs e)
        {
            try
            {
                if (_numericKeyboard.Visibility == Visibility.Hidden && _keyboard.Visibility == Visibility.Hidden)
                {
                    TextBox tb = (TextBox)sender;
                    tb.Text = "";
                }

            }
            catch
            { }

        }

        protected void tb_with_kb_Enter(object sender, EventArgs e)
        {
            try
            {
                //_keyboard.Show((Control)sender, false);
                _keyboard.Show();
            }
            catch { }
        }

        protected void tb_with_nkb_Enter(object sender, EventArgs e)
        {
            try
            {
                //_numericKeyboard.Show((Control)sender, false);
                _numericKeyboard.Show();
            }
            catch { }
        }

        protected void any_button_Clicked(object sender, EventArgs e)
        {
            HideKeyboards();
            FireEvent(AnyButtonClicked);
        }

        public event EventHandler<BTPUtilities.UniversalEventArgs> OutputInterface;

        protected void FireOutputEvent(String sender, BTPUtilities.UniversalEventArgs eh)
        {
            if (OutputInterface != null)
            {
                OutputInterface((object)sender, eh);
            }
        }

        public event EventHandler AnyButtonClicked;

        protected void FireEvent(EventHandler eh)
        {
            if (eh != null)
                eh(this, EventArgs.Empty);
        }

        public event EventHandler<BTPUtilities.IntEventArgs> ButtonClick;

        protected void FireButtonClick(BTPUtilities.IntEventArgs bcea)
        {

            HideKeyboards();

            if (ButtonClick != null)
            {
                ButtonClick(this, bcea);
            }

        }

        protected void FireButtonClick(String name, BTPUtilities.IntEventArgs bcea)
        {

            HideKeyboards();

            if (ButtonClick != null)
            {
                ButtonClick(name, bcea);
            }

        }

        //public void AutoInputInterface(string name, object value)
        //{
        //    try
        //    {
        //        InputInterface(name, value);

        //        //foreach (var mp in _localMpList)
        //        //    mp.AutoInputInterface(name, value);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventErrorLogger.AddLog(
        //            BTPLogs.ErrorType.InputInterface,
        //            new string[]
        //            {
        //                this.GetType().FullName.Replace("+", "."),
        //                name,
        //                value.ToString(),
        //                ex.Message
        //            });
        //    }
        //}

        protected void AddErrorLog(string name, string value, string message)
        {
            _eventErrorLogger.AddLog(
                    BTPLogs.ErrorType.InputInterface,
                    new string[]
                    {
                        this.GetType().FullName.Replace("+", "."),
                        name,
                        value,
                        message
                    });
        }

        #region Button click auto log
        /// <summary>
        /// Dodaje log po naciśnięciu przycisku -> "[MainPanel] | [bt.Text]"
        /// </summary>
        /// <param name="bt"></param>
        public void AddButtonClickLog(Button bt, string device)
        {
            bt.Click += delegate (object sender, RoutedEventArgs e)
            {
                _log.AddLog_Button(device, "[" + this.GetType().Name + "] | [" + bt.Content + "]");
            };
        }

        /// <summary>
        /// Dodaje log po naciśnięciu przycisku -> "text"
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="text"></param>
        public void AddButtonClickLog(Button bt, string text, string device)
        {
            bt.Click += delegate (object sender, RoutedEventArgs e)
            {
                _log.AddLog_Button(device, text);
            };
        }

        /// <summary>
        /// Dodaje log po naciśnięciu przycisku  -> "textFormat {0}" {0} -> tb.Text
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="textFormat"></param>
        /// <param name="tb"></param>
        public void AddButtonClickLog(Button bt, string textFormat, TextBox tb, string device, bool clearTbValueAfterClick = false)
        {
            bt.Click += delegate (object sender, RoutedEventArgs e)
            {
                _log.AddLog_Button(device, String.Format(textFormat, tb.Text));

                if (clearTbValueAfterClick)
                    tb.Text = "";
            };
        }

        /// <summary>
        /// Dodaje log po naciśnięciu przycisku  -> "textFormat {0} {1}" {0} -> tb.Text | {1} -> cb.SelectedItem
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="textFormat"></param>
        /// <param name="tb"></param>
        public void AddButtonClickLog(Button bt, string textFormat, TextBox tb, ComboBox cb, string device, bool clearTbValueAfterClick = false)
        {
            bt.Click += delegate (object sender, RoutedEventArgs e)
            {
                _log.AddLog_Button(device, String.Format(textFormat, tb.Text, cb.SelectedItem));

                if (clearTbValueAfterClick)
                    tb.Text = "";
            };
        }


        /// <summary>
        /// Dodaje log po naciśnięciu przycisku -> w zależności od podśiwtlenia zmienia tekst
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="c_btOn"></param>
        /// <param name="c_btOff"></param>
        /// <param name="s_btOn"></param>
        /// <param name="s_btOff"></param>
        //public void AddButtonClickLog(Button bt, Color c_btOn, string s_btOn, string s_btOff, string device)
        //{
        //    bt.Click += delegate (object sender, RoutedEventArgs e)
        //    {
        //        //_log.AddLog_Button(device, bt.Background == c_btOn ? s_btOn : s_btOff);
        //    };
        //}
        #endregion

        //#region Auto transalate for DGV headers
        ///// <summary>
        ///// Lista DGV do tłumaczenia
        ///// </summary>
        //List<DataGrid> _autoTransalateDGVHeaders = new List<DataGrid>();
        ///// <summary>
        ///// Auto tłumaczenie nagłówków DGV | W DB: [Parent z .] [Nazwa grida] [Numer kolumny]
        ///// </summary>
        ///// <param name="dgv">DGV do auto tłumaczenia</param>
        //public void AddDGVToAutoTransalateHeaders(DataGrid dgv)
        //{
        //    _autoTransalateDGVHeaders.Add(dgv);
        //    dgv.Chan += Dgv_DataSourceChanged;
        //}

        ///// <summary>
        ///// Po zmianie źródła tłumaczy DGV
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Dgv_DataSourceChanged(object sender, RoutedEventArgs e)
        //{
        //    DataGrid dgv = sender as DataGrid;

        //    if (dgv != null)
        //        AutoDGVHeaders_LanguageChanged(dgv);
        //}

        ///// <summary>
        ///// Auto tłumaczenie nagłówków DGV na podstawie listy _autoTransalateDGVHeaders
        ///// </summary>
        //private void AutoDGVHeaders_LanguageChanged(DataGrid dgv)
        //{
        //    try
        //    {
        //        String parentName = this.IsControlMultipleTransalated ? ParentWindowControlPath : this.GetType().FullName.Replace("+", ".");
        //        String controlPath = this.IsControlMultipleTransalated ? this.PrefixForSetWindowControls + dgv.Name : dgv.Name;

        //        if (dgv.Columns.Count > 0)
        //        {
        //            foreach (DataGridColumn col in dgv.Columns)
        //            {
        //                string s = _translation.GetTraslatedTextForDgvColumnHeader(_language, parentName, controlPath, col.Index);

        //                if (!String.IsNullOrEmpty(s))
        //                    col.HeaderText = s;

        //                //col.HeaderText = _translation2.GetTranslationDataTable().AsEnumerable()
        //                //    .Where(r => r.Control == parentName && r.Path == dgv.Name
        //                //        && r.Nr == col.Index)
        //                //    .Select(r => (string)r[_language])
        //                //    .First();
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        //#endregion

        //#region Auto InternalInit
        ///// <summary>
        ///// Lista wszystkich lokalnych MP
        ///// </summary>
        //List<MainPanel> _localMpList = new List<MainPanel>();

        ///// <summary>
        ///// Auto zainicjowanie wszystkich pochodnych od MainPanel
        ///// </summary>
        ///// <param name="parent"></param>
        //private void Auto_InternalInit_Drill(Control parent)
        //{
        //    try
        //    {

        //        foreach (Control ctr in parent.Controls)
        //        {
        //            MainPanel mp = ctr as MainPanel;

        //            if (mp == null)
        //            {
        //                if (ctr.HasChildren)
        //                {
        //                    Auto_InternalInit_Drill(ctr);
        //                }
        //            }
        //            else
        //            {
        //                if (mp.IsControlMultipleTransalated)
        //                {
        //                    if (this.IsControlMultipleTransalated)
        //                    {
        //                        mp.PrefixForSetWindowControls = this.PrefixForSetWindowControls + mp.Name + ".";
        //                        mp.ParentWindowControlPath = this.ParentWindowControlPath;
        //                    }
        //                    else
        //                    {
        //                        mp.PrefixForSetWindowControls = mp.Name + ".";
        //                        mp.ParentWindowControlPath = this.GetType().FullName.Replace("+", ".");
        //                    }
        //                }

        //                mp.Init(_keyboard, _numericKeyboard, _dbWMS, _cfg, _log, LoggedUser, _plc, _translation, _eventErrorLogger);
        //                _localMpList.Add(mp);

        //                mp.ShowTransalatePopup += Mp_ShowTransalatePopup;
        //            }
        //        }
        //    }
        //    catch { }
        //}


        ///// <summary>
        ///// Drill down event do MAIN, pokazuje popup od tłumaczeń, tylko DEBUG 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Mp_ShowTransalatePopup(object sender, BTPUtilities.StringEventArgs e)
        //{
        //    MainPanel mp = sender as MainPanel;

        //    if (mp == null)
        //        return;

        //    if (mp.IsControlMultipleTransalated)
        //    {
        //        e.Strings[0] = this.GetType().FullName.Replace("+", ".");
        //        e.Strings[1] = mp.Name + "." + e.Strings[1];
        //        ShowTransalatePopup?.Invoke(this, e);
        //    }
        //    else
        //        ShowTransalatePopup?.Invoke(sender, e);
        //}
        //#endregion
        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        //#region Auto/Safe InputInterface
        ///// <summary>
        ///// InputInterface z Drill we wszystkie kontrolki
        ///// </summary>
        ///// <param name="name">nazwa zmiennej PLC</param>
        ///// <param name="value">wartość zmiennej PLC</param>
        //public void AutoInputInterface(string name, object value)
        //{
        //    try
        //    {
        //        InputInterface(name, value);

        //        foreach (var mp in _localMpList)
        //            mp.AutoInputInterface(name, value);
        //    }
        //    catch (Exception ex)
        //    {
        //        _eventErrorLogger.AddLog(
        //            BTPLogs.ErrorType.InputInterface,
        //            new string[]
        //            {
        //                this.GetType().FullName.Replace("+", "."),
        //                name,
        //                value.ToString(),
        //                ex.Message
        //            });
        //    }
        //}
        //#endregion

        ///// <summary>
        ///// InputInterface wyłącznie do jednej kontrolki
        ///// </summary>
        ///// <param name="name">nazwa zmiennej</param>
        ///// <param name="value">wartość zmiennej</param>
        //public void InputInterface_Safe(string name, object value)
        //{
        //    try
        //    {
        //        InputInterface(name, value);
        //    }
        //    catch
        //    { }
        //}
        //#endregion

        //#region Transalate Right Click_ForDebug
        ///// <summary>
        ///// Wywala do MAIN event z wyświetleniem popupu do tłumaczenia
        ///// </summary>
        //public event EventHandler<BTPUtilities.StringEventArgs> ShowTransalatePopup;

        ///// <summary>
        ///// Dodanie prawego kliknięcia do wyświetlenia popup od tłumaczeń, tylko DEBUG
        ///// </summary>
        ///// <param name="ctr"></param>
        //public void EnableRightClickForTransalate(Control ctr)
        //{
        //    Button bt = ctr as Button;
        //    Label lb = ctr as Label;
        //    MainPanel mp = ctr as MainPanel;
        //    DataGrid dgv = ctr as DataGrid;

        //    if (mp != null && mp != this)
        //    {
        //        mp.EnableRightClickForTransalate(mp);
        //    }
        //    else if (bt != null)
        //    {
        //        bt.MouseDown += transalate_MouseClick;
        //        bt.MouseDown +=
        //    }
        //    else if (lb != null)
        //    {
        //        lb.MouseClick += transalate_MouseClick;
        //    }
        //    else if (dgv != null)
        //    {
        //        //dgv.MouseClick += transalate_MouseClick;
        //        dgv.ColumnHeaderMouseClick += Dgv_ColumnHeaderMouseClick;
        //    }
        //    else
        //    {
        //        if (ctr.HasChildren)
        //        {
        //            foreach (Control child in ctr.Controls)
        //            {
        //                EnableRightClickForTransalate(child);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Dgv_ColumnHeaderMouseClick(object sender, DataGridCellMouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        DataGrid dgv = sender as DataGrid;

        //        String parentName = this.GetType().FullName.Replace("+", ".");
        //        String path = "";

        //        if (dgv != null)
        //        {
        //            path = dgv.Name;
        //        }

        //        if (path != "")
        //        {
        //            string[] text_lang = _translation2.GetAllTraslatedText(
        //                        this.IsControlMultipleTransalated ? ParentWindowControlPath : parentName,
        //                        this.IsControlMultipleTransalated ? PrefixForSetWindowControls + path : path,
        //                        e.ColumnIndex);

        //            bool notInDB = false;

        //            if (text_lang == null)
        //            {
        //                text_lang = new string[] { "", "", "", "" };
        //                notInDB = true;
        //            }

        //            ShowTransalatePopup?.Invoke(this, new BTPUtilities.StringEventArgs(new string[]
        //                {
        //                    parentName,
        //                    path,
        //                    text_lang[0],
        //                    text_lang[1],
        //                    text_lang[2],
        //                    text_lang[3],
        //                    notInDB ? "1" : "0",
        //                    e.ColumnIndex.ToString()
        //                }));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Wyrzuca event z danymi do tłumaczenia popup
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void transalate_MouseClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        Button bt = sender as Button;
        //        Label lb = sender as Label;
        //        DataGrid dgv = sender as DataGrid;

        //        String parentName = this.GetType().FullName.Replace("+", ".");
        //        String path = "";
        //        String lang = "";

        //        if (bt != null)
        //        {
        //            path = bt.Name;
        //            lang = bt.Text;
        //        }
        //        else if (lb != null)
        //        {
        //            path = lb.Name;
        //            lang = lb.Text;
        //        }
        //        else if (dgv != null)
        //        {
        //            path = dgv.Name;
        //            lang = "";
        //        }

        //        if (path != "")
        //        {
        //            string[] text_lang = _translation2.GetAllTraslatedText(
        //                        this.IsControlMultipleTransalated ? ParentWindowControlPath : parentName,
        //                        this.IsControlMultipleTransalated ? PrefixForSetWindowControls + path : path,
        //                        0);

        //            bool notInDB = false;

        //            if (text_lang == null)
        //            {
        //                text_lang = new string[] { "", "", "", "" };
        //                notInDB = true;
        //            }

        //            ShowTransalatePopup?.Invoke(this, new BTPUtilities.StringEventArgs(new string[]
        //                {
        //                    parentName,
        //                    path,
        //                    text_lang[0],
        //                    text_lang[1],
        //                    text_lang[2],
        //                    text_lang[3],
        //                    notInDB  ? "1" : "0",
        //                    "0"
        //                }));
        //        }
        //    }
        //}
        //#endregion

        //#region Path for multiple transalated control
        ///// <summary>
        ///// Czy kontrolka posiada kilka różnych tłumaczeń
        ///// </summary>
        //public bool IsControlMultipleTransalated { get; set; }

        ///// <summary>
        ///// Ścieżka do okna w którym kontrolka jest osadzona
        ///// </summary>
        //protected string ParentWindowControlPath = "";

        ///// <summary>
        ///// Prefix do nazw kontrolek jeżeli okno osadzone w oknie osadzonym
        ///// </summary>
        //protected string PrefixForSetWindowControls = "";
        //#endregion

        //#region Auto ChangeLanguage
        //private void AutoChangeLanguageForLocalMp(string language)
        //{
        //    try
        //    {
        //        foreach (var mp in _localMpList)
        //            mp.ChangeLanguage(language);
        //    }
        //    catch { }
        //}
        //#endregion
    }
}
