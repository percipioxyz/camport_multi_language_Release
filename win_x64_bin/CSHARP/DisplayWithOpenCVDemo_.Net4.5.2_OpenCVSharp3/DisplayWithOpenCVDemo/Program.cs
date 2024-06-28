using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pcammls;
using static pcammls.pcammls;

using OpenCvSharp;

namespace DisplayWithOpenCVDemo
{
    class Program
    {
        static PercipioSDK cl;
        static CSharpPercipioDeviceEvent _event;
        static System.IntPtr handle;

        static void PressAnyKeyExit()
        {
            Console.WriteLine("按任意键退出程序...");
            Console.ReadKey();
        }

        static void ShowDepthStream()
        {
            bool has = cl.DeviceHasStream(handle, PERCIPIO_STREAM_DEPTH);
            if (!has) {
                Console.WriteLine("This camera does not support depth stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_DEPTH);

            image_data depth = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++) 
                    { 
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            //depth raw data
                            Mat cv_depth = new Mat(frames[i].height, frames[i].width, MatType.CV_16U, frames[i].buffer.getCPtr());

                            Mat cv_depth_cvt_8_bit = new Mat();
                            Cv2.ConvertScaleAbs(cv_depth, cv_depth_cvt_8_bit);
                            Cv2.ImShow("Raw-Depth", cv_depth_cvt_8_bit);

                            cl.DeviceStreamDepthRender(frames[i], depth);

                            Mat cv_render_depth = new Mat(frames[i].height, frames[i].width, MatType.CV_8UC3, depth.buffer.getCPtr(), 3 * frames[i].width);
                            Cv2.ImShow("depth", cv_render_depth);

                            int key = Cv2.WaitKey(1);
                            if (key == 'q') {
                                fetch_exit = true;
                            }
                        }
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void ShowColorStream()
        {
            bool has = cl.DeviceHasStream(handle, PERCIPIO_STREAM_COLOR);
            if (!has)
            {
                Console.WriteLine("This camera does not support rgb stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_COLOR);

            image_data bgr = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_COLOR)
                        {
                            cl.DeviceStreamImageDecode(frames[i], bgr);

                            Mat cv_bgr = new Mat(frames[i].height, frames[i].width, MatType.CV_8UC3, bgr.buffer.getCPtr(), 3 * frames[i].width);
                            Cv2.ImShow("rgb", cv_bgr);

                            int key = Cv2.WaitKey(1);
                            if (key == 'q')
                            {
                                fetch_exit = true;
                            }
                        }
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void ShowIRStream()
        {
            bool has_leftIR = cl.DeviceHasStream(handle, PERCIPIO_STREAM_IR_LEFT);
            bool has_rightIR = cl.DeviceHasStream(handle, PERCIPIO_STREAM_IR_RIGHT);

            int stream_ir = 0;
            if (has_leftIR)
                stream_ir |= PERCIPIO_STREAM_IR_LEFT;
            if (has_rightIR)
                stream_ir |= PERCIPIO_STREAM_IR_RIGHT;
            if (0 == stream_ir)
            {
                Console.WriteLine("This camera does not support IR stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            //disable camera laser auto ctrl
            cl.DeviceControlLaserPowerAutoControlEnable(handle, false);
            //Set laser brightness, 0-100
            cl.DeviceControlLaserPowerConfig(handle, 80);

            cl.DeviceStreamEnable(handle, stream_ir);

            image_data leftIR = new image_data();
            image_data rightIR = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_IR_LEFT)
                        {
                            cl.DeviceStreamIRRender(frames[i], leftIR);

                            Mat cv_leftIR = new Mat(leftIR.height, leftIR.width, MatType.CV_8UC3, leftIR.buffer.getCPtr(), 3 * leftIR.width);
                            Cv2.ImShow("Left-IR", cv_leftIR);
                        }

                        if (frames[i].streamID == PERCIPIO_STREAM_IR_RIGHT)
                        {
                            cl.DeviceStreamIRRender(frames[i], rightIR);

                            Mat cv_rightIR = new Mat(rightIR.height, rightIR.width, MatType.CV_8UC3, rightIR.buffer.getCPtr(), 3 * rightIR.width);
                            Cv2.ImShow("Right-IR", cv_rightIR);
                        }

                        int key = Cv2.WaitKey(1);
                        if (key == 'q')
                        {
                            fetch_exit = true;
                        }
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void ShowRGBDRegistrationStream()
        {
            bool has_depth = cl.DeviceHasStream(handle, PERCIPIO_STREAM_DEPTH);
            if (!has_depth)
            {
                Console.WriteLine("This camera does not support depth stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            bool has_color = cl.DeviceHasStream(handle, PERCIPIO_STREAM_COLOR);
            if (!has_color)
            {
                Console.WriteLine("This camera does not support rgb stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            float depth_scale_unit = cl.DeviceReadCalibDepthScaleUnit(handle);
            PercipioCalibData color_calib_data = cl.DeviceReadCalibData(handle, PERCIPIO_STREAM_COLOR);
            PercipioCalibData depth_calib_data = cl.DeviceReadCalibData(handle, PERCIPIO_STREAM_DEPTH);

            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_DEPTH | PERCIPIO_STREAM_COLOR);

            image_data registration_depth = new image_data();
            image_data registration_depth_render = new image_data();
            image_data color = new image_data();
            image_data undsitortion_color = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            cl.DeviceStreamMapDepthImageToColorCoordinate(depth_calib_data, frames[i].width, frames[i].height, depth_scale_unit, frames[i],
                                color_calib_data, frames[i].width, frames[i].height, registration_depth);

                            
                            cl.DeviceStreamDepthRender(registration_depth, registration_depth_render);
                        }

                        if (frames[i].streamID == PERCIPIO_STREAM_COLOR)
                        {
                            cl.DeviceStreamImageDecode(frames[i], color);
                            cl.DeviceStreamDoUndistortion(color_calib_data, color, undsitortion_color);
                        }
                    }
                    Mat cv_depth = new Mat(registration_depth_render.height, registration_depth_render.width, MatType.CV_8UC3, registration_depth_render.buffer.getCPtr());

                    Mat cv_rsz_bgr = new Mat();
                    Mat cv_bgr = new Mat(undsitortion_color.height, undsitortion_color.width, MatType.CV_8UC3, undsitortion_color.buffer.getCPtr());
                    Cv2.Resize(cv_bgr, cv_rsz_bgr, cv_depth.Size());

                    Cv2.ImShow("Registration-Depth", cv_depth);
                    Cv2.ImShow("Undistortion-Color", cv_rsz_bgr);

                    int key = Cv2.WaitKey(1);
                    if (key == 'q')
                    {
                        fetch_exit = true;
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void DepthStreamToPointCloud()
        {
            bool has = cl.DeviceHasStream(handle, PERCIPIO_STREAM_DEPTH);
            if (!has)
            {
                Console.WriteLine("This camera does not support depth stream output!\n");
                cl.Close(handle);
                PressAnyKeyExit();
                return;
            }

            float depth_scale_unit = cl.DeviceReadCalibDepthScaleUnit(handle);
            PercipioCalibData depth_calib_data = cl.DeviceReadCalibData(handle, PERCIPIO_STREAM_DEPTH);

            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_DEPTH);
            
            pointcloud_data_list p3d_list = new pointcloud_data_list();
            
            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            //depth raw data
                            Mat cv_depth = new Mat(frames[i].height, frames[i].width, MatType.CV_16U, frames[i].buffer.getCPtr());

                            cl.DeviceStreamMapDepthImageToPoint3D(frames[i], depth_calib_data, depth_scale_unit, p3d_list);

                            Mat cv_p3d = new Mat(frames[i].height, frames[i].width, MatType.CV_32FC3, p3d_list.getPtr().getCPtr());

                            //Convert point cloud data into images that can be displayed
                            Mat cv_p3d_cvt_8bit = new Mat();
                            Cv2.ConvertScaleAbs(cv_p3d, cv_p3d_cvt_8bit);
                            Cv2.ImShow("PointCloud", cv_p3d_cvt_8bit);
                            
                            int key = Cv2.WaitKey(1);
                            if (key == 'q')
                            {
                                fetch_exit = true;
                            }
                        }
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void DeviceInTriggerMode()
        {
            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_DEPTH | PERCIPIO_STREAM_COLOR);

            //Enable trigger-mode
            cl.DeviceControlTriggerModeEnable(handle, Convert.ToInt32(true));

            image_data render_depth = new image_data();
            image_data color = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                //send soft trigger signal
                Console.WriteLine("Send soft trigger signal!\n");
                cl.DeviceControlTriggerModeSendTriggerSignal(handle);

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            cl.DeviceStreamDepthRender(frames[i], render_depth);
                            Mat cv_depth = new Mat(render_depth.height, render_depth.width, MatType.CV_8UC3, render_depth.buffer.getCPtr());
                            Cv2.ImShow("Depth", cv_depth);
                        }

                        if (frames[i].streamID == PERCIPIO_STREAM_COLOR)
                        {
                            cl.DeviceStreamImageDecode(frames[i], color);
                            Mat cv_bgr = new Mat(color.height, color.width, MatType.CV_8UC3, color.buffer.getCPtr());
                            Cv2.ImShow("Color", cv_bgr);
                        }
                    }
                    
                    int key = Cv2.WaitKey(1);
                    if (key == 'q')
                    {
                        fetch_exit = true;
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void DeviceLoadStorageParameters()
        {
            cl.DeviceStreamEnable(handle, PERCIPIO_STREAM_DEPTH | PERCIPIO_STREAM_COLOR);

            int ret = cl.DeviceLoadDefaultParameters(handle);
            if (ret != TY_STATUS_OK) {
                Console.WriteLine("Load storage data fail!\n");
            }

            image_data render_depth = new image_data();
            image_data color = new image_data();

            cl.DeviceStreamOn(handle);

            bool fetch_exit = false;
            while (!fetch_exit)
            {
                if (_event.isOffLine())
                {
                    break;
                }

                FrameVector frames = cl.DeviceStreamRead(handle, 2000);
                if (frames.Count() > 0)
                {
                    for (int i = 0; i < frames.Count(); i++)
                    {
                        if (frames[i].streamID == PERCIPIO_STREAM_DEPTH)
                        {
                            cl.DeviceStreamDepthRender(frames[i], render_depth);
                            Mat cv_depth = new Mat(render_depth.height, render_depth.width, MatType.CV_8UC3, render_depth.buffer.getCPtr());
                            Cv2.ImShow("Depth", cv_depth);
                        }

                        if (frames[i].streamID == PERCIPIO_STREAM_COLOR)
                        {
                            cl.DeviceStreamImageDecode(frames[i], color);
                            Mat cv_bgr = new Mat(color.height, color.width, MatType.CV_8UC3, color.buffer.getCPtr());
                            Cv2.ImShow("Color", cv_bgr);
                        }
                    }

                    int key = Cv2.WaitKey(1);
                    if (key == 'q')
                    {
                        fetch_exit = true;
                    }
                }
            }

            cl.DeviceStreamOff(handle);
            cl.Close(handle);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("test start\n");
            cl = new PercipioSDK();

            _event = new CSharpPercipioDeviceEvent();
            cl.DeviceRegiststerCallBackEvent(_event);

            DeviceInfoVector dev_list = cl.ListDevice();
            int sz = dev_list.Count();
            if (sz == 0)
            {
                Console.WriteLine(string.Format("no device found."));
                PressAnyKeyExit();
                return;
            }

            Console.WriteLine(string.Format("found follow devices:"));
            for (int idx = 0; idx < sz; idx++)
            {
                var item = dev_list[idx];
                Console.WriteLine("{0} -- {1} {2}", idx, item.id, item.modelName);
            }
            Console.WriteLine("select one:");
            int select = int.Parse(Console.ReadLine());

            handle = cl.Open(dev_list[select].id);
            if (!cl.isValidHandle(handle))
            {
                Console.WriteLine(string.Format("can not open device!"));
                PressAnyKeyExit();
                return;
            }

            Console.WriteLine(string.Format("Select a test item:"));
            Console.WriteLine(string.Format("\t0.output depth stream."));
            Console.WriteLine(string.Format("\t1.output color stream."));
            Console.WriteLine(string.Format("\t2.output ir stream."));
            Console.WriteLine(string.Format("\t3.RGBD registration."));
            Console.WriteLine(string.Format("\t4.output point cloud data."));
            Console.WriteLine(string.Format("\t5.The camera works in (soft) trigger mode."));
            Console.WriteLine(string.Format("\t6.The default configuration parameters are loaded when the camera starts."));
            Console.WriteLine("select one:");
            select = int.Parse(Console.ReadLine());
            Console.WriteLine("select = {0}", select);

            switch (select)
            {
                case 0:
                    ShowDepthStream();
                    break;
                case 1:
                    ShowColorStream();
                    break;
                case 2:
                    ShowIRStream();
                    break;
                case 3:
                    ShowRGBDRegistrationStream();
                    break;
                case 4:
                    DepthStreamToPointCloud();
                    break;
                case 5:
                    DeviceInTriggerMode();
                    break;
                case 6:
                    DeviceLoadStorageParameters();
                    break;
                default:
                    Console.WriteLine("Invalid index!");
                    cl.Close(handle);
                    PressAnyKeyExit();
                    return;
            }
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
