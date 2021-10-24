# QuickStart
把常用的程序、命令集成在一起的快捷启动软件。


【开发环境】：

       Windows 10 Professional 21H1 + Visusl Studio 2019 Enterprise + .Net FrameWork 3.5


【运行要求】：
 
       需安装 .Net FrameWork 3.5 或者更高版本

       Download Url: https://www.microsoft.com/en-us/download/details.aspx?id=21

【使用说明】：
  
       1.可修改ini配置文件，随时修改可执行程序的启动信息。
       
       2.为避免频繁读取ini配置文件，程序只会在启动是读取一次。故ini文件修改后，需重新运行本程序。
       
       3.根据ini文件所配置的可视按钮数，自动显示按钮。每行最多显示 8 个按钮，多的会自动显示在下一行。
       
       4.按钮最多可显示 4 行，共 32 个按钮，多于 32 个的配置会被忽视。
  
       4.ini文件配置简例：
       #############################################
       #[Button_01] - [Button_32] 最大支持32个按钮，32个以上的忽视
       [Button_XX]
       #按钮显示名称
       Button_Name=
       #按钮可视否【1：可视；1 以外不可视；默认：空 不显示】
       Button_Visible=
       #启动程序的完整路径和程序名
       Program_Name=
       #程序启动的类型【exe：exe可执行程序；cmd：cmd命令或者*.cmd可执行程序；默认：exe】
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