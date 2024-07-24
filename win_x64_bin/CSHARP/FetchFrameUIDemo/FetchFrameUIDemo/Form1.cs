using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Drawing.Imaging;

using pcammls;
using static pcammls.pcammls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace fetch_frame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            cl = new PercipioSDK();

            if (!ListDevice())
            {
                MessageBox.Show("No device found");
                Environment.Exit(0);
            }

            InitializeComponent();

            for (int i = 0; i < dev_list.Count(); i++)
            {
                comboBox_device_list.Items.Add(dev_list[i].id);

                uint type = dev_list[i].type();
                if (type == TY_INTERFACE_USB)
                {
                    Console.WriteLine("\tdevice type: usb-dpb.");
                    TY_DEVICE_USB_INFO  usb_info = dev_list[i].get_usbinfo();
                    Console.WriteLine("\tdevice addr: {0}.", usb_info.addr);
                    Console.WriteLine("\tdevice bus: {0}.", usb_info.bus);
                }
                else if (type == TY_INTERFACE_ETHERNET)
                {
                    Console.WriteLine("\tdevice type: net.");
                    TY_DEVICE_NET_INFO net_info = dev_list[i].get_netinfo();
                    Console.WriteLine("\tdevice ip: {0}.", net_info.ip());
                    Console.WriteLine("\tdevice gateway: {0}.", net_info.gateway());
                    Console.WriteLine("\tdevice netmask: {0}.", net_info.netmask());
                    Console.WriteLine("\tdevice mac: {0}.", net_info.mac());
                }
                else if (type == TY_INTERFACE_IEEE80211)
                {
                    Console.WriteLine("\tdevice type: wifi.");
                    TY_DEVICE_NET_INFO net_info = dev_list[i].get_netinfo();
                    Console.WriteLine("\tdevice ip: {0}.", net_info.ip());
                    Console.WriteLine("\tdevice gateway: {0}.", net_info.gateway());
                    Console.WriteLine("\tdevice netmask: {0}.", net_info.netmask());
                    Console.WriteLine("\tdevice mac: {0}.", net_info.mac());
                }
                else {
                    Console.Write("\tdevice type: unknown.");
                }
            }

            comboBox_device_list.SelectedIndex = 0;

            button_capture.Enabled = false;

            checkBox_depth_enable.Enabled = false;
            checkBox_color_enable.Enabled = false;

            comboBox_depth_reso_list.Enabled = false;
            comboBox_color_reso_list.Enabled = false;

            checkBox_depth_enable.Checked = isDepthEnabled;
            checkBox_color_enable.Checked = isColorEnabled;
        }
        
        private Thread camera;
        private PercipioSDK cl;
        private DeviceInfoVector dev_list;
        private System.IntPtr handle;
        private CSharpPercipioDeviceEvent _event;

        private bool has_depth = false;
        private bool has_color = false;

        private bool isCameraOpened = false;
        private bool isCameraRunning = false;
        private bool isDepthEnabled = true;
        private bool isColorEnabled = true;

        private EnumEntryVector depth_fmt_list;
        private EnumEntryVector color_fmt_list;

        private void dump_calib_data(CalibDataVector data, int col, int row)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int idx = i * col + j;
                    if (idx < data.Count())
                        Console.Write("\t{0}\t", data[idx]);
                    else
                    {
                        Console.Write("\n");
                        return;
                    }
                }
                Console.Write("\n");
            }
        }

        private bool ListDevice()
        {
            dev_list = cl.ListDevice();
            int sz = dev_list.Count();
            if (sz == 0) 
			{
                return false;
            }
            
            return true;
        }

        private bool DeviceInit(string id)
        {
            handle = cl.Open(id);
            if (!cl.isValidHandle(handle))
            {
                MessageBox.Show("Can not open device!");
                return false;
            }

            _event = new CSharpPercipioDeviceEvent();
            cl.DeviceRegiststerCallBackEvent(_event);

            //Attempts to read configuration parameters from the camera's storage area
            int err = cl.DeviceLoadDefaultParameters(handle);
            if (err != TY_STATUS_OK)
                Console.WriteLine(string.Format("Load default parameters fail: {0}!", err));
            else
                Console.WriteLine(string.Format("Load default parameters successful!"));
            return true;
        }

        private void CaptureCamera()
        {
            button_capture.Text = "Stop";

            comboBox_depth_reso_list.Enabled = false;
            comboBox_color_reso_list.Enabled = false;

            checkBox_depth_enable.Enabled = false;
            checkBox_color_enable.Enabled = false;

            isCameraRunning = true;
            camera = new Thread(CaptureCameraCallback);
            camera.Start();
        }

        private void StopCamera()
        {
            isCameraRunning = false;
            camera.Join();

            button_capture.Text = "Start";

            if (has_depth)
            {
                comboBox_depth_reso_list.Enabled = true;
                checkBox_depth_enable.Enabled = true;
            }
            else {
                comboBox_depth_reso_list.Enabled = false;
                checkBox_depth_enable.Enabled = false;
            }

            if(has_color)
            {
                comboBox_color_reso_list.Enabled = true;
                checkBox_color_enable.Enabled = true;
            }
            else
            {
                comboBox_color_reso_list.Enabled = false;
                checkBox_color_enable.Enabled = false;
            }

            return;
        }
		
        private void CloseDevice()
        {
            cl.Close(handle);

            button_capture.Enabled = false;

            checkBox_depth_enable.Enabled = false;
            checkBox_color_enable.Enabled = false;

            comboBox_depth_reso_list.Enabled = false;
            comboBox_color_reso_list.Enabled = false;
        }

        private IntPtr ArrToPtr(byte[] array)
        {
            return System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(array, 0);
        }
		
        private void CaptureCameraCallback()
        {
            cl.DeviceStreamOn(handle);
            while (isCameraRunning)
            {
                if (_event.isOffLine())
                {
                    isCameraRunning = false;
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            image_data depth = new image_data();
                            //The original depth image is a single channel 16bit wide matrix data
                            //After processing by the function DeviceStreamDepthRender, the output is rendered as a 3-channel RGB image
                            cl.DeviceStreamDepthRender(frames[i], depth);
                            IntPtr pt = depth.buffer.getCPtr();
                            Bitmap bmp_depth = new Bitmap(depth.width, depth.height, depth.width * 3, PixelFormat.Format24bppRgb, pt);
                            pictureBox_depth.Image = (Image)(new Bitmap(bmp_depth, new Size(640, 480))).Clone();
                        }
                        if (frames[i].streamID == PERCIPIO_STREAM_COLOR)
                        {
                            image_data bgr = new image_data();
                            cl.DeviceStreamImageDecode(frames[i], bgr);
                            IntPtr pt = bgr.buffer.getCPtr();
                            Bitmap bmp_color = new Bitmap(bgr.width, bgr.height, bgr.width * 3, PixelFormat.Format24bppRgb, pt);
                            pictureBox_color.Image = (Image)(new Bitmap(bmp_color, new Size(640, 480))).Clone();
                        }
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            if (pictureBox_depth.Image != null) {
                pictureBox_depth.Image.Dispose();
                pictureBox_depth.Image = null;
            }

            if (pictureBox_color.Image != null)
            {
                pictureBox_color.Image.Dispose();
                pictureBox_color.Image = null;
            }

            if (_event.isOffLine())
            {
                MessageBox.Show("Device offline");
                cl.Close(handle);
                Application.Exit();
            }
        }

        //open device
        private void button_open_device_Click(object sender, EventArgs e)
        {
            if (false == isCameraOpened)
            {
                string id = comboBox_device_list.SelectedItem.ToString();
                if (!DeviceInit(id))
                {
                    return;
                }

                button_flush.Enabled = false;
                button_capture.Enabled = true;
                button_capture.Text = "Start";

                comboBox_depth_reso_list.Items.Clear();
                comboBox_depth_reso_list.Text = "";

                comboBox_color_reso_list.Items.Clear();
                comboBox_color_reso_list.Text = "";

                has_depth = cl.DeviceHasStream(handle, PERCIPIO_STREAM_DEPTH);
                if (!has_depth)
                {
                    isDepthEnabled = false;
                    checkBox_depth_enable.Enabled = false;
                    comboBox_depth_reso_list.Enabled = false;
                }
                else 
                {
                    depth_fmt_list = cl.DeviceStreamFormatDump(handle, PERCIPIO_STREAM_DEPTH);
                    if (depth_fmt_list.Count() != 0)
                    {
                        comboBox_depth_reso_list.Enabled = true;
                        Console.WriteLine(string.Format("depth image format list:"));
                        for (int i = 0; i < depth_fmt_list.Count(); i++)
                        {
                            TY_ENUM_ENTRY fmt = depth_fmt_list[i];
                            Console.WriteLine(string.Format("\t{0} -size[{1}x{2}]\t-\t desc:{3}", i, cl.Width(fmt), cl.Height(fmt), fmt.getDesc()));
                            comboBox_depth_reso_list.Items.Add(fmt.getDesc());
                        }
                        comboBox_depth_reso_list.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBox_depth_reso_list.Enabled = false;
                    }

                    isDepthEnabled = true;
                    checkBox_depth_enable.Enabled = true;
                }

                has_color = cl.DeviceHasStream(handle, PERCIPIO_STREAM_COLOR);
                if (!has_color)
                {
                    isColorEnabled = false;
                    checkBox_color_enable.Enabled = false;
                    comboBox_color_reso_list.Enabled = false;
                }
                else
                {
                    color_fmt_list = cl.DeviceStreamFormatDump(handle, PERCIPIO_STREAM_COLOR);
                    if (color_fmt_list.Count() != 0)
                    {
                        comboBox_color_reso_list.Enabled = true;
                        Console.WriteLine(string.Format("color image format list:"));
                        for (int i = 0; i < color_fmt_list.Count(); i++)
                        {
                            TY_ENUM_ENTRY fmt = color_fmt_list[i];
                            Console.WriteLine(string.Format("\t{0} -size[{1}x{2}]\t-\t desc:{3}", i, cl.Width(fmt), cl.Height(fmt), fmt.getDesc()));
                            comboBox_color_reso_list.Items.Add(fmt.getDesc());
                        }
                        comboBox_color_reso_list.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBox_color_reso_list.Enabled = false;
                    }
                    isColorEnabled = true;
                    checkBox_color_enable.Enabled = true;
                }

                isCameraOpened = true;

                comboBox_device_list.Enabled = false;

                button_open.Text = "Close";
            }
            else 
			{
                //stop stream
                if (isCameraRunning)
                {
                    StopCamera();
                }

                CloseDevice();

                comboBox_depth_reso_list.Items.Clear();
                comboBox_depth_reso_list.Text = "";

                comboBox_color_reso_list.Items.Clear();
                comboBox_color_reso_list.Text = "";

                isCameraOpened = false;

                comboBox_device_list.Enabled = true;

                button_flush.Enabled = true;

                button_open.Text = "Open";
            }
        }

        //stream on
        private void button_stream_on_Click(object sender, EventArgs e)
        {
            if (!isCameraRunning)
            {
                if (!isDepthEnabled && !isColorEnabled)
                {
                    MessageBox.Show("At least one data stream needs to be opened!");
                    return;
                }

                int m_stream = 0;
                if (isDepthEnabled)
                {
                    m_stream |= PERCIPIO_STREAM_DEPTH;
                    Console.WriteLine(string.Format("depth stream enable!"));
                    
                    if (depth_fmt_list.Count() != 0)
                    {
                        for (int i = 0; i < depth_fmt_list.Count(); i++)
                        {
                            TY_ENUM_ENTRY fmt = depth_fmt_list[i];
                            if(fmt.getDesc() == comboBox_depth_reso_list.SelectedItem.ToString())
                            { 
                                cl.DeviceStreamFormatConfig(handle, PERCIPIO_STREAM_DEPTH, fmt);
                            }
                        }
                    }

                    PercipioCalibData depth_calib_data = cl.DeviceReadCalibData(handle, PERCIPIO_STREAM_DEPTH);
                    int depth_calib_width = depth_calib_data.Width();
                    int depth_calib_height = depth_calib_data.Height();
                    Console.WriteLine(string.Format("depth image calib size: {0}x{1}", depth_calib_width, depth_calib_height));
                    CalibDataVector depth_intr = depth_calib_data.Intrinsic();
                    Console.WriteLine(string.Format("depth image calib intrinsic:"));
                    dump_calib_data(depth_intr, 3, 3);
                    CalibDataVector depth_extr = depth_calib_data.Extrinsic();
                    Console.WriteLine(string.Format("depth image calib extrinsic:"));
                    dump_calib_data(depth_extr, 4, 4);
                    CalibDataVector depth_dist = depth_calib_data.Distortion();
                    Console.WriteLine(string.Format("depth image calib distortion:"));
                    dump_calib_data(depth_dist, depth_dist.Count(), 1);
                }

                if (isColorEnabled)
                {
                    m_stream |= PERCIPIO_STREAM_COLOR;
                    Console.WriteLine(string.Format("color stream enable!"));
                    if (color_fmt_list.Count() != 0)
                    {
                        for (int i = 0; i < color_fmt_list.Count(); i++)
                        {
                            TY_ENUM_ENTRY fmt = color_fmt_list[i];
                            if (fmt.getDesc() == comboBox_color_reso_list.SelectedItem.ToString())
                            {
                                cl.DeviceStreamFormatConfig(handle, PERCIPIO_STREAM_COLOR, fmt);
                            }
                        }
                    }

                    PercipioCalibData color_calib_data = cl.DeviceReadCalibData(handle, PERCIPIO_STREAM_COLOR);
                    int color_calib_width = color_calib_data.Width();
                    int color_calib_height = color_calib_data.Height();
                    Console.WriteLine(string.Format("color image calib size: {0}x{1}", color_calib_width, color_calib_height));
                    CalibDataVector color_intr = color_calib_data.Intrinsic();
                    Console.WriteLine(string.Format("color image calib intrinsic:"));
                    dump_calib_data(color_intr, 3, 3);
                    CalibDataVector color_extr = color_calib_data.Extrinsic();
                    Console.WriteLine(string.Format("color image calib extrinsic:"));
                    dump_calib_data(color_extr, 4, 4);
                    CalibDataVector color_dist = color_calib_data.Distortion();
                    Console.WriteLine(string.Format("color image calib distortion:"));
                    dump_calib_data(color_dist, color_dist.Count(), 1);
                }

                int ret = cl.DeviceStreamEnable(handle, m_stream);
                if(ret != 0) { 
                    MessageBox.Show("stream enable failed!");
                    return;
                }

                CaptureCamera();
            }
            else
            {
                StopCamera();
            }
        }

        private void button_flush_device_list_Click(object sender, EventArgs e)
        {
            comboBox_device_list.Items.Clear();
            comboBox_device_list.Text = "";

            button_open.Enabled = false;
            button_capture.Enabled = false;
            button_flush.Enabled = false;

            Thread t = new Thread(() =>
            {
                ListDevice();
                if (comboBox_device_list.InvokeRequired)
                {
                    if (dev_list.Count() > 0)
                    {
                        for (int i = 0; i < dev_list.Count(); i++)
                        {
                            comboBox_device_list.Invoke(new Action(() => comboBox_device_list.Items.Add(dev_list[i].id)));
                        }

                        comboBox_device_list.Invoke(new Action(() => comboBox_device_list.SelectedIndex = 0));
                    }
                    else
                    {
                        if (button_flush.InvokeRequired)
                        {
                            button_flush.Invoke(new Action(() => button_flush.Enabled = true));
                        }
                        return;
                    }
                }

                if (button_open.InvokeRequired)
                {
                    button_open.Invoke(new Action(() => button_open.Enabled = true));
                }

                //if (button_capture.InvokeRequired)
                //{
                //    button_capture.Invoke(new Action(() => button_capture.Enabled = true));
                //}

                if (button_flush.InvokeRequired)
                {
                    button_flush.Invoke(new Action(() => button_flush.Enabled = true));
                }
                });
            t.Start();
        }

        private void checkBox_depth_switch_CheckedChanged(object sender, EventArgs e)
        {
            isDepthEnabled = checkBox_depth_enable.Checked;
        }

        private void checkBox_color_switch_CheckedChanged(object sender, EventArgs e)
        {
            isColorEnabled = checkBox_color_enable.Checked;
        }
		
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isCameraRunning) 
			{
                StopCamera();
                
            }

            if(cl.isValidHandle(handle))
            { 
                CloseDevice();
            }
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }

    class CSharpPercipioDeviceEvent : DeviceEvent
    {
        bool Offline = false;
        public override int run(SWIGTYPE_p_void handle, int event_id)
        {
            IntPtr dev = handle.getCPtr();
            if (event_id == TY_EVENT_DEVICE_OFFLINE)
            {
                Offline = true;
                Console.WriteLine(string.Format("=== Event Callback: Device Offline"));
            }
            return 0;
        }

        public bool isOffLine()
        {
            return Offline;
        }
    }
}
