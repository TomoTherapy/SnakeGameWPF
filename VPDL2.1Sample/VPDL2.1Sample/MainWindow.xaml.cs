using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViDi2;

namespace VPDL2._1Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViDi2.Runtime.Local.Control Control;
        ViDi2.Runtime.IWorkspace Workspace;
        ViDi2.Runtime.IStream Stream;

        public MainWindow()
        {
            InitializeComponent();

            Control = new ViDi2.Runtime.Local.Control(); // new Control
            Control.Workspaces.Add("Workspace", @"D:\PROJECT\LG ES\모듈팩\리드웰딩\LeadWelding_Vertical.vrws"); // load runtime workspace
            Workspace = Control.Workspaces["Workspace"];
            foreach (var stream in Workspace.Streams) // 처음 stream 
            {
                Stream = stream;
                break;
            }

            SetThresholdRecursive(Stream.Tools); // threshold 삽입 (0 ~ 1) 트리구조기 때문에 재귀함수
        }

        private void SetThresholdRecursive(IToolList<ViDi2.Runtime.ITool> tools)
        {
            foreach (var tool in tools) // threshold 삽입 (0 ~ 1)
            {
                if (tool is ViDi2.Runtime.IRedTool red)
                {
                    red.Parameters.Threshold = new ViDi2.Interval(0.6, 0.6);
                }
                if (tool is ViDi2.Runtime.IGreenTool green)
                {
                    green.Parameters.Threshold = 0.5;
                }
                if (tool is ViDi2.Runtime.IBlueTool blue)
                {
                    blue.Parameters.Threshold = 0.5;
                }

                if (tool.Children.Count > 0)
                {
                    SetThresholdRecursive(tool.Children);
                }
            }
        }

        private void RunVidi_Click(object sender, RoutedEventArgs e)
        {
            Bitmap bmp = new Bitmap(@"D:\PROJECT\LG ES\브이텐 모듈팩 리드웰딩\세로촬영 이미지\C727Image_0116\grab_1_1.bmp");
            ViDi2.ISample sample = Stream.Process(new ViDi2.FormsImage(bmp));

            foreach (var marking in sample.Markings)
            {
                if (marking.Value is ViDi2.IRedMarking red)
                {
                    var regions = red.Views[0].Regions; // 세그멘테이션 툴 리젼 콜렉션
                    var polygon = regions[0].Outer; // 리전의 폴리곤 좌표 콜렉션
                }
                else if (marking.Value is ViDi2.IGreenMarking green)
                {
                    string tagName = green.Views[0].BestTag.Name; // 이미지의 best tag 이름
                }
                else if (marking.Value is ViDi2.IBlueMarking blue)
                {
                    var features = blue.Views[0].Features; // 찾은 피쳐 콜렉션
                }
            }
        }
    }
}
