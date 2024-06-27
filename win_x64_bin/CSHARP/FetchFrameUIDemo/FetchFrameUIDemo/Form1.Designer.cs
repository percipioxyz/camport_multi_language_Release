namespace fetch_frame
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox_device_list = new System.Windows.Forms.ComboBox();

            this.button_flush = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.button_capture = new System.Windows.Forms.Button();

            this.checkBox_depth_enable = new System.Windows.Forms.CheckBox();
            this.checkBox_color_enable = new System.Windows.Forms.CheckBox();

            this.comboBox_depth_reso_list = new System.Windows.Forms.ComboBox();
            this.comboBox_color_reso_list = new System.Windows.Forms.ComboBox();

            this.pictureBox_depth = new System.Windows.Forms.PictureBox();
            this.pictureBox_color = new System.Windows.Forms.PictureBox();
            
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_depth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_color)).BeginInit();
            this.SuspendLayout();
            
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 37);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 28);
            this.textBox1.TabIndex = 0;
            this.textBox1.Tag = "";
            this.textBox1.Text = "SN：";
            // 
            // comboBox_device_list
            // 
            this.comboBox_device_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_device_list.FormattingEnabled = true;
            this.comboBox_device_list.Location = new System.Drawing.Point(135, 37);
            this.comboBox_device_list.Name = "comboBox1";
            this.comboBox_device_list.Size = new System.Drawing.Size(251, 26);
            this.comboBox_device_list.TabIndex = 1;
            // 
            // button_flush
            // 
            this.button_flush.Location = new System.Drawing.Point(392, 37);
            this.button_flush.Name = "button3";
            this.button_flush.Size = new System.Drawing.Size(119, 28);
            this.button_flush.TabIndex = 2;
            this.button_flush.Text = "Flush";
            this.button_flush.UseVisualStyleBackColor = true;
            this.button_flush.Click += new System.EventHandler(this.button_flush_device_list_Click);
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(517, 37);
            this.button_open.Name = "button1";
            this.button_open.Size = new System.Drawing.Size(119, 28);
            this.button_open.TabIndex = 3;
            this.button_open.Text = "Open";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_device_Click);
            // 
            // button_capture
            // 
            this.button_capture.Location = new System.Drawing.Point(643, 37);
            this.button_capture.Name = "button2";
            this.button_capture.Size = new System.Drawing.Size(119, 28);
            this.button_capture.TabIndex = 4;
            this.button_capture.Text = "Start";
            this.button_capture.UseVisualStyleBackColor = true;
            this.button_capture.Click += new System.EventHandler(this.button_stream_on_Click);
            // 
            // checkBox_depth_enable
            // 
            this.checkBox_depth_enable.AutoSize = true;
            this.checkBox_depth_enable.Location = new System.Drawing.Point(29, 96);
            this.checkBox_depth_enable.Name = "checkBox1";
            this.checkBox_depth_enable.Size = new System.Drawing.Size(142, 22);
            this.checkBox_depth_enable.TabIndex = 5;
            this.checkBox_depth_enable.Text = "Depth Enable";
            this.checkBox_depth_enable.UseVisualStyleBackColor = true;
            this.checkBox_depth_enable.CheckedChanged += new System.EventHandler(this.checkBox_depth_switch_CheckedChanged);
            // 
            // checkBox_color_enable
            // 
            this.checkBox_color_enable.AutoSize = true;
            this.checkBox_color_enable.Location = new System.Drawing.Point(825, 96);
            this.checkBox_color_enable.Name = "checkBox2";
            this.checkBox_color_enable.Size = new System.Drawing.Size(124, 22);
            this.checkBox_color_enable.TabIndex = 6;
            this.checkBox_color_enable.Text = "RGB Enable";
            this.checkBox_color_enable.UseVisualStyleBackColor = true;
            this.checkBox_color_enable.CheckedChanged += new System.EventHandler(this.checkBox_color_switch_CheckedChanged);
            // 
            // comboBox_depth_reso_list
            // 
            this.comboBox_depth_reso_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_depth_reso_list.FormattingEnabled = true;
            this.comboBox_depth_reso_list.Location = new System.Drawing.Point(262, 91);
            this.comboBox_depth_reso_list.Name = "comboBox2";
            this.comboBox_depth_reso_list.Size = new System.Drawing.Size(227, 26);
            this.comboBox_depth_reso_list.TabIndex = 7;
            // 
            // comboBox_color_reso_list
            // 
            this.comboBox_color_reso_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_color_reso_list.FormattingEnabled = true;
            this.comboBox_color_reso_list.Location = new System.Drawing.Point(1071, 92);
            this.comboBox_color_reso_list.Name = "comboBox3";
            this.comboBox_color_reso_list.Size = new System.Drawing.Size(227, 26);
            this.comboBox_color_reso_list.TabIndex = 8;
            // 
            // pictureBox_depth
            // 
            this.pictureBox_depth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_depth.Location = new System.Drawing.Point(29, 124);
            this.pictureBox_depth.Name = "pictureBox1";
            this.pictureBox_depth.Size = new System.Drawing.Size(761, 497);
            this.pictureBox_depth.TabIndex = 9;
            this.pictureBox_depth.TabStop = false;
            // 
            // pictureBox_color
            // 
            this.pictureBox_color.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_color.Location = new System.Drawing.Point(825, 124);
            this.pictureBox_color.Name = "pictureBox2";
            this.pictureBox_color.Size = new System.Drawing.Size(759, 497);
            this.pictureBox_color.TabIndex = 10;
            this.pictureBox_color.TabStop = false;
            
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1596, 668);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox_device_list);
            this.Controls.Add(this.button_flush);
            this.Controls.Add(this.button_open);
            this.Controls.Add(this.button_capture);
            this.Controls.Add(this.checkBox_depth_enable);
            this.Controls.Add(this.checkBox_color_enable);
            this.Controls.Add(this.comboBox_depth_reso_list);
            this.Controls.Add(this.comboBox_color_reso_list);
            this.Controls.Add(this.pictureBox_depth);
            this.Controls.Add(this.pictureBox_color);
            
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_depth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_color)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;

        private System.Windows.Forms.ComboBox comboBox_device_list;

        private System.Windows.Forms.Button button_flush;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Button button_capture;

        private System.Windows.Forms.CheckBox checkBox_depth_enable;
        private System.Windows.Forms.CheckBox checkBox_color_enable;

        private System.Windows.Forms.ComboBox comboBox_depth_reso_list;
        private System.Windows.Forms.ComboBox comboBox_color_reso_list;
        
        private System.Windows.Forms.PictureBox pictureBox_depth;
        private System.Windows.Forms.PictureBox pictureBox_color;
        
    }
}

