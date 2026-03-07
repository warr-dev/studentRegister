namespace StudentsInformationSystem
{
    partial class Form3
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
            this.Family_Background = new System.Windows.Forms.DataGridView();
            this.pb_savephoto = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Students_Information = new System.Windows.Forms.DataGridView();
            this.btn_update = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btn_back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Family_Background)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_savephoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Students_Information)).BeginInit();
            this.SuspendLayout();
            // 
            // Family_Background
            // 
            this.Family_Background.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Family_Background.Location = new System.Drawing.Point(27, 322);
            this.Family_Background.Name = "Family_Background";
            this.Family_Background.Size = new System.Drawing.Size(722, 142);
            this.Family_Background.TabIndex = 1;
            this.Family_Background.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Family_Background_CellContentClick);
            // 
            // pb_savephoto
            // 
            this.pb_savephoto.Location = new System.Drawing.Point(313, 12);
            this.pb_savephoto.Name = "pb_savephoto";
            this.pb_savephoto.Size = new System.Drawing.Size(168, 112);
            this.pb_savephoto.TabIndex = 2;
            this.pb_savephoto.TabStop = false;
            this.pb_savephoto.Click += new System.EventHandler(this.pb_savephoto_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(309, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Students Information";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(309, 311);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Family Background";
            // 
            // Students_Information
            // 
            this.Students_Information.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Students_Information.Location = new System.Drawing.Point(27, 148);
            this.Students_Information.Name = "Students_Information";
            this.Students_Information.Size = new System.Drawing.Size(722, 142);
            this.Students_Information.TabIndex = 0;
            this.Students_Information.AllowUserToResizeColumnsChanged += new System.EventHandler(this.Form3_Load);
            this.Students_Information.AllowUserToResizeRowsChanged += new System.EventHandler(this.Form3_Load);
            this.Students_Information.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Students_Information_CellContentClick);
            // 
            // btn_update
            // 
            this.btn_update.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btn_update.Location = new System.Drawing.Point(708, 487);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(75, 23);
            this.btn_update.TabIndex = 5;
            this.btn_update.Text = "Update";
            this.btn_update.UseVisualStyleBackColor = false;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.button2.Location = new System.Drawing.Point(610, 487);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btn_Delete);
            // 
            // btn_back
            // 
            this.btn_back.Location = new System.Drawing.Point(12, 487);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(75, 23);
            this.btn_back.TabIndex = 7;
            this.btn_back.Text = "Add";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(795, 512);
            this.Controls.Add(this.btn_back);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_update);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pb_savephoto);
            this.Controls.Add(this.Family_Background);
            this.Controls.Add(this.Students_Information);
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Profiling";
            this.Load += new System.EventHandler(this.Form3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Family_Background)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_savephoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Students_Information)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView Family_Background;
        private System.Windows.Forms.PictureBox pb_savephoto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView Students_Information;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_back;
    }
}
