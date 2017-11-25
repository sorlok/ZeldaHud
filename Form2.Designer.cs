namespace zeldaGui
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.portListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // portListView
            // 
            this.portListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.portListView.FullRowSelect = true;
            this.portListView.Location = new System.Drawing.Point(1, 0);
            this.portListView.MultiSelect = false;
            this.portListView.Name = "portListView";
            this.portListView.Size = new System.Drawing.Size(265, 89);
            this.portListView.TabIndex = 0;
            this.portListView.UseCompatibleStateImageBehavior = false;
            this.portListView.View = System.Windows.Forms.View.Details;
            this.portListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.portListView_ItemSelectionChanged);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 88);
            this.Controls.Add(this.portListView);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView portListView;
    }
}