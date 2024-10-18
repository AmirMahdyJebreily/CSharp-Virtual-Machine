using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Virtual_Machine
{
    public partial class Form1 : Form
    {
        #region Component Def
        WebView2 webView;
        #endregion

        #region Constructor And inits
        public Form1()
        {
            InitializeComponent();
            InitWebView();
        }

        void InitWebView()
        {
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.webView.AllowExternalDrop = true;
            this.webView.BackColor = SystemColors.ControlDarkDark;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = Color.Black;
            this.webView.Dock = DockStyle.Fill;
            this.webView.Location = new Point(0, 24);
            this.webView.Name = "webView";
            this.webView.Size = new Size(494, 605);
            this.webView.TabIndex = 3;
            this.webView.ZoomFactor = 1D;
            this.webView.CoreWebView2InitializationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs>(this.webView_CoreWebView2InitializationCompleted);
            this.webView.NavigationStarting += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs>(this.webView_NavigationStarting);
            //this.webView.WebMessageReceived += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs>(this.webView_WebMessageReceived);
            this.Controls.Add(this.webView);
        }

        async Task InitWebViewAsyncs()
        {
            string cacheFolder = Path.GetTempPath();

            CoreWebView2EnvironmentOptions options = null;
            CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, cacheFolder, options);

            await webView.EnsureCoreWebView2Async(env);
        }
        #endregion

        #region Virtual Domain Host
        const string domainForInterface = "codeaghavm";
        const string interfaceIndex = "\\Interface";
        string BaseDomain => $"https://{domainForInterface}";
        string IndexPage => $"{BaseDomain}/index.html";
        #endregion

        #region Web View
        private void webView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                try
                {
                    webView.CoreWebView2.SetVirtualHostNameToFolderMapping(domainForInterface, $"{Application.StartupPath}{interfaceIndex}", CoreWebView2HostResourceAccessKind.Allow);
                }
                catch { }
                this.webView.CoreWebView2.ContextMenuRequested += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs>(this.webView_CoreWebView2ContextMenuRequested);

                webView.Source = new Uri(IndexPage);
            }
        }
        private void webView_CoreWebView2ContextMenuRequested(object sender, Microsoft.Web.WebView2.Core.CoreWebView2ContextMenuRequestedEventArgs e)
        {
            CoreWebView2ContextMenuItem Refresh = webView.CoreWebView2.Environment.CreateContextMenuItem("بارگذاری مجدد", null, CoreWebView2ContextMenuItemKind.Command);
            Refresh.CustomItemSelected += delegate (object send, Object ex)
            {
                webView.Source = webView.Source;
            };

            CoreWebView2ContextMenuItem Separator = webView.CoreWebView2.Environment.CreateContextMenuItem("-", null, CoreWebView2ContextMenuItemKind.Separator);

            CoreWebView2ContextMenuItem Exit = webView.CoreWebView2.Environment.CreateContextMenuItem("خروج از نرم افزار", null, CoreWebView2ContextMenuItemKind.Command);
            Exit.CustomItemSelected += delegate (object send, Object ex)
            {
                Application.Exit();
            };
            e.MenuItems.Clear();
            e.MenuItems.Add(Refresh);
            e.MenuItems.Add(Separator);
            e.MenuItems.Add(Exit);

        }
        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
#if !DEBUG
            webView.CoreWebView2.DefaultDownloadDialogCornerAlignment = CoreWebView2DefaultDownloadDialogCornerAlignment.BottomRight;
            webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
            webView.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
            webView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            webView.CoreWebView2.Settings.HiddenPdfToolbarItems = Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.None;
#endif
        }
        #endregion

        private async void Form1_Load(object sender, EventArgs e)
        {
            await InitWebViewAsyncs();
        }
    }
}
