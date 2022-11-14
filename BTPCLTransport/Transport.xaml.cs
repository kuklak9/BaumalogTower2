using System;
using System.Linq;
using System.Windows.Controls;
using BTPUtilities;
using System.Xml.Linq;
using System.Data;

namespace BTPCLTransport
{
    /// <summary>
    /// Interaction logic for Transport.xaml
    /// </summary>
    public partial class Transport : BTPControlLibrary.MainPanel
    {
        public Transport()
        {
            InitializeComponent();
            //InitializeComponents_ChangeBaseTransport();
        }

        //private void InitializeComponents_ChangeBaseTransport()
        //{
        //    var programedTransport = GetTransportFromCfg();

        //    programedTransport.BackColor = transportBase1.BackColor;
        //    programedTransport.Font = transportBase1.Font;
        //    programedTransport.IsControlMultipleTransalated = transportBase1.IsControlMultipleTransalated;
        //    programedTransport.Location = transportBase1.Location;
        //    programedTransport.Name = transportBase1.Name;
        //    programedTransport.Size = transportBase1.Size;
        //    programedTransport.TabIndex = transportBase1.TabIndex;

        //    transportBase1.Dispose();

        //    transportBase1 = programedTransport;
        //    this.panel1.Controls.Add(transportBase1);

        //    transportBase1.ShowArticleInfo += TransportBase1_ShowArticleInfo;
        //    transportBase1.HideArticelInfo += TransportBase1_HideArticelInfo;
        //    transportBase1.ShowTrayArticles += TransportBase1_ShowTrayArticles;
        //}

        //private Visualization.TransportBase GetTransportFromCfg()
        //{
        //    try
        //    {
        //        string transportNameFromCfg = GetTransportVisualizationNameFromConfigurationXml();

        //        var visualizationAssembly = System.Reflection.Assembly.GetAssembly(typeof(BTPCLTransport.Visualization.TransportBase));

        //        var transportVisualizationFromCfg = visualizationAssembly.GetTypes()
        //            .Where(r => r.Name.EndsWith(transportNameFromCfg));

        //        if (!transportVisualizationFromCfg.Any())
        //            throw new ArgumentNullException();

        //        return (Visualization.TransportBase)Activator.CreateInstance(transportVisualizationFromCfg.First());
        //    }
        //    catch
        //    {
        //        return new Visualization.StandardTransport();
        //    }
        //}

        //private string GetTransportVisualizationNameFromConfigurationXml()
        //{
        //    var cfgAssembly = System.Reflection.Assembly.GetAssembly(typeof(BTPConfig.Configuration));
        //    var cfgAssemblyLocation = cfgAssembly.Location;
        //    var cfgAssemblyPath = System.IO.Path.GetDirectoryName(cfgAssemblyLocation);

        //    string configurationFileName = "Tower.config";

        //    XDocument configurationData = XDocument.Load(cfgAssemblyPath + "\\" + configurationFileName);

        //    var transportElementFromCfq = configurationData.Descendants("Transport")
        //        .Descendants("VisualizationName")
        //        .FirstOrDefault();

        //    if (transportElementFromCfq.IsEmpty)
        //        throw new ArgumentNullException();

        //    return transportElementFromCfq.Value;
        //}

        //public void RefreshVisualization()
        //{
        //    transportBase1.UpdateTraysView();
        //    transportBase1.RefreshExtractorPosition();
        //}

        //protected override void InternalInit()
        //{
        //    towerPositionInfoPanel1.TowerNumber = _cfg.TowerNrControl;

        //    weightPanel1.Visible = _cfg.GetParamBool("TOWER_HasWeight");
        //    weightPanel1.TowerNumber = _cfg.TowerNrCfg;

        //    towerPositionInfoPanel1.Visible = !weightPanel1.Visible;
        //    towerPositionInfoPanel1.TowerNumber = _cfg.TowerNrCfg;

        //    stationAutoControl1.Visible = _cfg.GetParamBool("TOWER_Station") && !_cfg.GetParamBool("TOWER_StationManual");
        //    stationAutoControl1.TowerNumber = _cfg.TowerNrCfg;
        //    stationAutoControl1.WindowNumber = _cfg.WindowNr;

        //    bt_borrow.Visible = _cfg.GetParamBool("TOWER_AllowBorrowTrays");

        //    simpleBringTray1.WindowNumber = _cfg.WindowNr;

        //    if (_cfg.GetParamBool("LoaderUnloader_Enable"))
        //    {
        //        p_loader3015.Visible = true;
        //        p_loader3015.BringToFront();
        //        loaderControl1.OutputInterface += LoaderControl1_OutputInterface;
        //    }

        //    transportBase1.OutputInterface += TransportBase1_OutputInterface;
        //}

        //private void TransportBase1_OutputInterface(object sender, UniversalEventArgs e)
        //{
        //    switch ((string)sender)
        //    {
        //        case "Close":
        //            loaderControl1.DisableSemiAutomaticMode();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private void LoaderControl1_OutputInterface(object sender, UniversalEventArgs e)
        //{
        //    if (sender is String name)
        //        transportBase1.InputInterface(name, e.Data);
        //}

        //private void CheckAndCorrectArticleInfoPopupPosition()
        //{
        //    CheckControlPositionInsideParent(l_article_info);
        //}

        //private void ArticleInfoShowPopup(String text, int top, int left)
        //{
        //    l_article_info.BringToFront();
        //    l_article_info.Text = text;

        //    this.SuspendLayout();
        //    l_article_info.Top = top;
        //    l_article_info.Left = left;
        //    CheckAndCorrectArticleInfoPopupPosition();
        //    l_article_info.Visible = text != "";
        //    this.ResumeLayout();
        //    tm_hidePopup.Enabled = false;
        //}

        //private void ArticleInfoHidePopup()
        //{
        //    tm_hidePopup.Enabled = true;
        //}

        //private String GetArticlesOnTrayFromDBForPopup(int tray)
        //{
        //    DataTable dt = null;

        //    try
        //    {
        //        dt = ((BTPDataBase.DB_WMS_3)_dbWMS).GetArticleOnTray(tray, -1);
        //    }
        //    catch
        //    {
        //        return "";
        //    }

        //    if (dt == null || dt.Rows.Count == 0)
        //        return "";

        //    var querry = from row in dt.AsEnumerable()
        //                 select (row.Field<string>(0)).Length;

        //    int len = querry.Max();

        //    String text = "";
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        if (text != "")
        //            text += Environment.NewLine;

        //        text += dr[0].ToString().PadRight(len, ' ') + " | " + dr[2].ToString();
        //    }

        //    return text;
        //}

        //private void TransportBase1_ShowTrayArticles(object sender, IntEventArgs e)
        //{
        //    FireOutputEvent("ShowTrayArticles", new BTPUtilities.UniversalEventArgs(e.number, typeof(Int32)));
        //}

        //private void TransportBase1_HideArticelInfo(object sender, EventArgs e)
        //{
        //    ArticleInfoHidePopup();
        //}

        //private void TransportBase1_ShowArticleInfo(object sender, Visualization.TransportBase.ArticleInfoLabelEventArgs e)
        //{
        //    simpleBringTray1.EnteredTray = e.TrayNr;

        //    if (e.TrayNr == 0 || _loggedUser == null )
        //        return;

        //    String text = GetArticlesOnTrayFromDBForPopup(e.TrayNr);
        //    ArticleInfoShowPopup(text, e.TopPosition, e.LeftPosition);
        //}

        //private void p_Click(object sender, EventArgs e)
        //{
        //    _keyboard.HideInput();
        //    _numericKeyboard.HideInput();
        //}

        //private void Transport_VisibleChanged(object sender, EventArgs e)
        //{
        //    RefreshVisualization();
        //    simpleBringTray1.EnteredTray = -1;
        //}

        //private void bt_showArtOn(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        int trayID = transportBase1.GetTrayNrInWindow();
        //        if (trayID > 0)
        //            FireOutputEvent("ShowTrayArticles", new BTPUtilities.UniversalEventArgs(trayID, typeof(Int32)));
        //    }
        //    catch { }
        //}

        //private void tm_hidePopup_Tick(object sender, EventArgs e)
        //{
        //    l_article_info.Visible = false;

        //}

        //private void bt_borrow_Click(object sender, EventArgs e)
        //{
        //    borrowTrays1.BringToFront();
        //    borrowTrays1.Visible = true;
        //}

        //private void Popup_VisibleChanged(object sender, EventArgs e)
        //{
        //    Panel panel = sender as Panel;


        //    if (panel == null && !(sender is Controls.BorrowTrays))
        //        return;

        //    p.Enabled = !borrowTrays1.Visible;

        //    if (panel != null && p.Visible)
        //        p.BringToFront();
        //}
    }
}
