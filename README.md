# QuickStart
把常用的程序、命令集成在一起的快捷启动软件。

【开发环境】：

       Windows 10 Professional 21H1 + Visusl Studio 2019 Enterprise + .Net FrameWork 3.5

【运行要求】：
 
       需安装 .Net FrameWork 3.5 或者更高版本

       Download Url: https://www.microsoft.com/en-us/download/details.aspx?id=21

【使用说明】：
  
       1.可修改ini配置文件，随时修改可执行程序的启动信息。
       
       2.为避免频繁读取ini文件，程序只会在启动时读取一次。故ini文件修改后，需重新运行本程序。
       
       3.根据ini文件所配置的可视按钮个数，自动顺序显示按钮。每行最多显示 8 个，多的会自动显示在下一行。
       
       4.按钮最多可显示 8 行，共 40 个按钮，多于 40 个的配置会被忽视。
  
       5.ini文件配置简例：
        #############################################
        #[Button_01] - [Button_40] 最大支持40个按钮，40个以上的忽视
        [Button_XX]
        #按钮显示名称
        Button_Name=
        #按钮可视否【1：可视；1 以外不可视；默认：空 不显示】
        Button_Visible=
        #启动程序的完整路径和程序名
        Program_Name=
        #程序启动的类型【exe：exe可执行程序；cmd：cmd命令或者*.cmd、*.bat可执行程序；默认：exe】
        Program_Type=
        #程序的启动参数
        Program_Param=
        #############################################

        [Button_01]
        Button_Name=Eclipse_v4.21
        Button_Visible=1
        Program_Name=D:\Eclipse_4.21\eclipse.exe
        Program_Type=exe
        Program_Param=-clean

      6.默认显示的Reboot，Shutdown，不要随便点！！！！！！
      
【版本更新】：
      
      1.v21.10
        初版发布
      
      2.v21.11
        新增窗体透明度配置
        #主窗体的不透明度(0 ~ 1；默认1，1为100%不透明)
        MainForm_Opacity=1

      3.v21.12
        新增窗体靠屏幕上、左、右边停靠（已知双屏停靠有故障。停靠主次屏中间的屏幕左右侧时，会频繁自动左右跳动。暂无解）
        新增退出时消息框确认
        修改最大按钮可支持数，从 32个 变更到 40个      
        
