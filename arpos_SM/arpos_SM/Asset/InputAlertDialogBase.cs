using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace arpos_SM.Asset
{
    /// <summary>
    /// The awesome Transparent Popup Page
    /// sub-classed from Rg.Plugins.Popup
    /// Customized for our usecase with
    /// Generic data type support for the result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputAlertDialogBase<T> : PopupPage
    {
        // the awaitable task
        public Task<T> PageClosedTask { get { return PageClosedTaskCompletionSource.Task; } }

        // the task completion source
        public TaskCompletionSource<T> PageClosedTaskCompletionSource { get; set; }

        public InputAlertDialogBase(View contentBody)
        {
            Content = contentBody;

            // init the task completion source
            PageClosedTaskCompletionSource = new System.Threading.Tasks.TaskCompletionSource<T>();

            this.BackgroundColor = new Color(0, 0, 0, 0.4);
        }

        // Method for animation child in PopupPage
        // Invoced after custom animation end
        protected override void OnAppearingAnimationEnd()
        {
            Content.FadeTo(1);
        }

        // Method for animation child in PopupPage
        // Invoked before custom animation begin
        protected override void OnDisappearingAnimationBegin()
        {
            Content.FadeTo(1);
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back button pressed action on android
            //return base.OnBackButtonPressed();
            return true;
        }

        // Invoced when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Prevent background clicked action
            //return base.OnBackgroundClicked();
            return false;
        }
    }
}
