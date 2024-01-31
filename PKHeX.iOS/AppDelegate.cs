using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using UIKit;

namespace PKHeX.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init();

            Syncfusion.XForms.iOS.TabView.SfTabViewRenderer.Init();
            Syncfusion.XForms.iOS.Accordion.SfAccordionRenderer.Init();
            Syncfusion.XForms.iOS.ComboBox.SfComboBoxRenderer.Init();
            new SfNumericTextBoxRenderer();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            AppCenter.Start("6696eefb-f004-4303-963d-72a3fe9a8061", typeof(Analytics), typeof(Crashes));

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
