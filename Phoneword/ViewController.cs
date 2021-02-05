using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Phoneword
{
    public partial class ViewController : UIViewController
    {
        string translatedNumber = String.Empty;

        public List<string> PhoneNumbers { get; set; }

        protected ViewController(IntPtr handle) : base(handle)
        {
            //initialize list of phone numbers called for Call History screen
            PhoneNumbers = new List<string>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            TranslateButton.TouchUpInside += (object sender, EventArgs e) => {
                // Convert the phone number with text to a number
                // using PhoneTranslator.cs
                translatedNumber = PhoneTranslator.ToNumber(PhoneNumberText.Text);

                // Dismiss the keyboard if text field was tapped
                PhoneNumberText.ResignFirstResponder();

                if (translatedNumber == String.Empty)
                {
                    CallButton.SetTitle("Call ", UIControlState.Normal);
                    CallButton.Enabled = false;
                }
                else
                {
                    CallButton.SetTitle("Call " + translatedNumber, UIControlState.Normal);
                    CallButton.Enabled = true;
                }
            };

            CallButton.TouchUpInside += (object sender, EventArgs e) => {

                //Store the phone number that we're dialing in PhoneNumbers
                PhoneNumbers.Add(translatedNumber);

                // Use URL handler with tel: prefix to invoke Apple's Phone app...
                var url = new NSUrl("tel:" + translatedNumber);

                // otherwise show an alert dialog
                if (!UIApplication.SharedApplication.OpenUrl(url))
                {
                    var alert = UIAlertController.Create("Not supported", "Scheme 'tel:' is not supported on this device", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                }
            };
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            // set the view controller that’s powering the screen we’re
            // transitioning to

            var callHistoryController = segue.DestinationViewController as CallHistoryController;

            //set the table view controller’s list of phone numbers to the
            // list of dialed phone numbers

            if (callHistoryController != null)
            {
                callHistoryController.PhoneNumbers = PhoneNumbers;
            }
        }
    }
}