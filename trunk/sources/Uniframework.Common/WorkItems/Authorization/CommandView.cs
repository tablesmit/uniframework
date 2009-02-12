using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.Security;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class CommandView : DevExpress.XtraEditors.XtraUserControl
    {
        public CommandView()
        {
            InitializeComponent();
        }

        #region Dependency Services

        private CommandPresenter presenter;
        [CreateNew]
        public CommandPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #endregion

        private void edtImage_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog() { 
                CheckFileExists = true,
                Filter = "所有文件(*.*)|*.*",
                Title = "选择"
            };
            if (diag.ShowDialog() == DialogResult.OK) {
                edtImage.Text = diag.FileName;
                string imagefile = Path.GetFileNameWithoutExtension(diag.FileName);
                image.ContentImage = Presenter.ImageService.GetBitmap("${" + imagefile + "}", new Size(32, 32));
            }    
        }

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            Presenter.AuthorizationStoreService.DeleteCommand(edtCommandUri.Text);
            AuthorizationCommand command = new AuthorizationCommand() { 
                Name = edtName.Text,
                CommandUri = edtCommandUri.Text,
                Category = edtCategory.Text,
                Image = "${" + Path.GetFileNameWithoutExtension(edtImage.Text) + "}"
            };

            try {
                Presenter.AuthorizationStoreService.SaveCommand(command);
            }
            catch (Exception ex) {
                XtraMessageBox.Show("保存命令 \"" + edtName.Text + "\" 时失败，"　+　ex.Message);
                btnOK.DialogResult = DialogResult.None;
            }
        }

        private void edtName_Leave(object sender, EventArgs e)
        {
            labPreview.Text = edtName.Text;
        }

        /// <summary>
        /// 订阅列表中当前命令变化事件
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.EventArgs&lt;Uniframework.Security.AuthorizationCommand&gt;"/> instance containing the event data.</param>
        [EventSubscription(EventNames.Authorization_CurrentCommandChanged, ThreadOption.UserInterface)]
        public void OnCurrentCommandChanged(object sender, EventArgs<AuthorizationCommand> e)
        {
            edtName.Text = e.Data.Name;
            edtCommandUri.Text = e.Data.CommandUri;
            edtCategory.Text = e.Data.Category;
            edtImage.Text = e.Data.Image;
        }
    }
}
